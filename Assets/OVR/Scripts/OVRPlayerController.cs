/************************************************************************************

Filename    :   OVRPlayerController.cs
Content     :   Player controller interface. 
				This script drives OVR camera as well as controls the locomotion
				of the player, and handles physical contact in the world.	
Created     :   January 8, 2013
Authors     :   Peter Giokaris

Copyright   :   Copyright 2013 Oculus VR, Inc. All Rights reserved.

Licensed under the Oculus VR SDK License Version 2.0 (the "License"); 
you may not use the Oculus VR SDK except in compliance with the License, 
which is provided at the time of installation or download, or which 
otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at

http://www.oculusvr.com/licenses/LICENSE-2.0 

Unless required by applicable law or agreed to in writing, the Oculus VR SDK 
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

************************************************************************************/

using UnityEngine;
using System.Collections.Generic;
// Hugo
using System;

[RequireComponent(typeof(CharacterController))]

//-------------------------------------------------------------------------------------
// ***** OVRPlayerController
//
// OVRPlayerController implements a basic first person controller for the Rift. It is 
// attached to the OVRPlayerController prefab, which has an OVRCameraController attached
// to it. 
// 
// The controller will interact properly with a Unity scene, provided that the scene has
// collision assigned to it. 
//
// The OVRPlayerController prefab has an empty GameObject attached to it called 
// ForwardDirection. This game object contains the matrix which motor control bases it
// direction on. This game object should also house the body geometry which will be seen
// by the player.
//
public class OVRPlayerController : OVRComponent
{
	protected CharacterController 	Controller 		 = null;
	protected OVRCameraController 	CameraController = null;

	public float Acceleration 	   = 0.05f;
	public float Damping 		   = 0.2f;
	public float BackAndSideDampen = 0.5f;
	public float JumpForce 		   = 0.3f;
	public float RotationAmount    = 1.5f;
	public float GravityModifier   = 0.379f;
		
	private float   MoveScale 	   = 1.0f;
	private Vector3 MoveThrottle   = Vector3.zero;
	private float   FallSpeed 	   = 0.0f;
	
	// Initial direction of controller (passed down into CameraController)
	private Quaternion OrientationOffset = Quaternion.identity;			
	// Rotation amount from inputs (passed down into CameraController)
	private float 	YRotation 	 = 0.0f;

	// Transfom used to point player in a given direction; 
	// We should attach objects to this if we want them to rotate 
	// separately from the head (i.e. the body)
	protected Transform DirXform = null;

	// We can adjust these to influence speed and rotation of player controller
	private float MoveScaleMultiplier     = 1.0f; 
	private float RotationScaleMultiplier = 1.0f; 
	// Hugo - This is way too slow for the gamepad
	public float GamepadRotationScaleMultiplier = 3.0f; 
	// Hugo - HeadStick
	public float HeadStickRotationScaleMultiplier = 5.0f;

	private bool  AllowMouseRotation      = true;
	private bool  HaltUpdateMovement      = false;
	
	// TEST: Get Y from second sensor
	//private float YfromSensor2            = 0.0f;

	// Hugo - Control mode
	public enum ControlMode {OculusSDK, FreeHead, FreeHeadLS, HeadStick, FixedAngle, FixedAngleLS, FixedAngleHead, TurnButton, TECHNOLUST};
	public int CurrentControlMode = (int)ControlMode.OculusSDK;
	
	// Hugo - Blinking stuff
	public bool BlinkOnRotation = true;
	public const int BLINK_NONE = 0;
	public const int BLINK_IN = 1;
	public const int BLINK_OUT = 2;
	public int BlinkState = BLINK_NONE;
	public float BlinkDeadZone = 0.4f;
	
	// Hugo - Angle of fixed angle mode turns
	public float TurnAngle = 20.0f;

	// Hugo - limits
	public const float MIN_TURN_ANGLE = 10.0f;
	public const float MAX_TURN_ANGLE = 90.0f;
	public const float TURN_ANGLE_INC = 5.0f;

	// Hugo - progressive turning
	public bool SmoothTurn = false;
	// Hugo - Turning speed (°/s)
	public float TurnSpeed = 300.0f;
	// Hugo - 180° Turning speed (°/s)
	public float UTurnSpeed = 600.0f;
	// Hugo - limits
	public const float MIN_TURN_SPEED = 100.0f;
	public const float MAX_TURN_SPEED = 950.0f;
	public const float TURN_SPEED_INC = 50.0f;
	// Hugo - "Fixed Angle" modes Analog stick deadzone
	private float turnAxisDeadZone = 0.4f;
	// Hugo - Fixed Angle Head mode. Delay in seconds between turns when keeping head in turning position
	public float headTurnDelay = 0.4f;

	// * * * * * * * * * * * * *
	
	// Awake
	new public virtual void Awake()
	{
		base.Awake();
		// We use Controller to move player around
		Controller = gameObject.GetComponent<CharacterController>();
		
		if(Controller == null)
			Debug.LogWarning("OVRPlayerController: No CharacterController attached.");
					
		// We use OVRCameraController to set rotations to cameras, 
		// and to be influenced by rotation
		OVRCameraController[] CameraControllers;
		CameraControllers = gameObject.GetComponentsInChildren<OVRCameraController>();
		
		if(CameraControllers.Length == 0)
			Debug.LogWarning("OVRPlayerController: No OVRCameraController attached.");
		else if (CameraControllers.Length > 1)
			Debug.LogWarning("OVRPlayerController: More then 1 OVRCameraController attached.");
		else
			CameraController = CameraControllers[0];	
	
		// Instantiate a Transform from the main game object (will be used to 
		// direct the motion of the PlayerController, as well as used to rotate
		// a visible body attached to the controller)
		DirXform = null;
		Transform[] Xforms = gameObject.GetComponentsInChildren<Transform>();
		
		for(int i = 0; i < Xforms.Length; i++)
		{
			if(Xforms[i].name == "ForwardDirection")
			{
				DirXform = Xforms[i];
				break;
			}
		}
		
		if(DirXform == null)
			Debug.LogWarning("OVRPlayerController: ForwardDirection game object not found. Do not use.");
	}

	// Start
	new public virtual void Start()
	{
		base.Start();
		
		InitializeInputs();	
		SetCameras();
	}
		
	// Update 
	new public virtual void Update()
	{
		base.Update();
		
		// Test: get Y from sensor 2 
		/*if(OVRDevice.SensorCount == 2)
		{
			Quaternion q = Quaternion.identity;
			OVRDevice.GetPredictedOrientation(1, ref q);
			YfromSensor2 = q.eulerAngles.y;
		}*/
		
		UpdateMovement();

		Vector3 moveDirection = Vector3.zero;
		
		float motorDamp = (1.0f + (Damping * DeltaTime));
		MoveThrottle.x /= motorDamp;
		MoveThrottle.y = (MoveThrottle.y > 0.0f) ? (MoveThrottle.y / motorDamp) : MoveThrottle.y;
		MoveThrottle.z /= motorDamp;

		moveDirection += MoveThrottle * DeltaTime;
		
		// Gravity
		if (Controller.isGrounded && FallSpeed <= 0)
			FallSpeed = ((Physics.gravity.y * (GravityModifier * 0.002f)));	
		else
			FallSpeed += ((Physics.gravity.y * (GravityModifier * 0.002f)) * DeltaTime);	

		moveDirection.y += FallSpeed * DeltaTime;

		// Offset correction for uneven ground
		float bumpUpOffset = 0.0f;
		
		if (Controller.isGrounded && MoveThrottle.y <= 0.001f)
		{
			bumpUpOffset = Mathf.Max(Controller.stepOffset, 
									 new Vector3(moveDirection.x, 0, moveDirection.z).magnitude); 
			moveDirection -= bumpUpOffset * Vector3.up;
		}			
	 
		Vector3 predictedXZ = Vector3.Scale((Controller.transform.localPosition + moveDirection), 
											 new Vector3(1, 0, 1));	
		
		// Move contoller
		Controller.Move(moveDirection);
		
		Vector3 actualXZ = Vector3.Scale(Controller.transform.localPosition, new Vector3(1, 0, 1));
		
		if (predictedXZ != actualXZ)
			MoveThrottle += (actualXZ - predictedXZ) / DeltaTime; 
		
		// Update rotation using CameraController transform, possibly proving some rules for 
		// sliding the rotation for a more natural movement and body visual
		UpdatePlayerForwardDirTransform();

		// Hugo
		UpdateOptions();
	}
		
	// UpdateMovement
	//
	// COnsolidate all movement code here
	//
	static float sDeltaRotationOld = 0.0f;
	// Hugo
	float turnDegs = 0.0f;
	float currentTurnSpeed = 0.0f;
	bool triggerPressed = false;
	private bool isTurning = false;
	float lastTurnTime = 0.0f;
	private bool canTurn = true;
	public virtual void UpdateMovement()
	{
		// Do not apply input if we are showing a level selection display
		if(HaltUpdateMovement == true)
			return;
	
		bool moveForward = false;
		bool moveLeft  	 = false;
		bool moveRight   = false;
		bool moveBack    = false;
				
		MoveScale = 1.0f;
			
		// * * * * * * * * * * *
		// Keyboard input
			
		// Move
			
		// WASD
		if (Input.GetKey(KeyCode.W)) moveForward = true;
		if (Input.GetKey(KeyCode.A)) moveLeft	 = true;
		if (Input.GetKey(KeyCode.S)) moveBack 	 = true; 
		if (Input.GetKey(KeyCode.D)) moveRight 	 = true; 
		// Arrow keys
		if (Input.GetKey(KeyCode.UpArrow))    moveForward = true;
		if (Input.GetKey(KeyCode.LeftArrow))  moveLeft 	  = true;
		if (Input.GetKey(KeyCode.DownArrow))  moveBack 	  = true; 
		if (Input.GetKey(KeyCode.RightArrow)) moveRight   = true; 
			
		if ( (moveForward && moveLeft) || (moveForward && moveRight) ||
			 (moveBack && moveLeft)    || (moveBack && moveRight) )
			MoveScale = 0.70710678f;
			
		// No positional movement if we are in the air
		if (!Controller.isGrounded)	
			MoveScale = 0.0f;
			
		MoveScale *= DeltaTime;
			
		// Compute this for key movement
		float moveInfluence = Acceleration * 0.1f * MoveScale * MoveScaleMultiplier;
			
		// Run!
		if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			moveInfluence *= 2.0f;

		if(DirXform != null)
		{
			if (moveForward)
				MoveThrottle += DirXform.TransformDirection(Vector3.forward * moveInfluence);
			if (moveBack)
				MoveThrottle += DirXform.TransformDirection(Vector3.back * moveInfluence) * BackAndSideDampen;
			if (moveLeft)
				MoveThrottle += DirXform.TransformDirection(Vector3.left * moveInfluence) * BackAndSideDampen;
			if (moveRight)
				MoveThrottle += DirXform.TransformDirection(Vector3.right * moveInfluence) * BackAndSideDampen;
		}
			
		// Rotate
			
		// compute for key rotation
		float rotateInfluence = DeltaTime * RotationAmount * RotationScaleMultiplier;
		// Hugo - new one for the gamepad
		float gamepadRotateInfluence = DeltaTime * RotationAmount * GamepadRotationScaleMultiplier;

		//reduce by half to avoid getting ill
		if (Input.GetKey(KeyCode.Q)) 
			YRotation -= rotateInfluence * 0.5f;  
		if (Input.GetKey(KeyCode.E)) 
			YRotation += rotateInfluence * 0.5f; 
		
		// * * * * * * * * * * *
		// Mouse input
			
		// Move
			
		// Rotate
		float deltaRotation = 0.0f;
		if(AllowMouseRotation == false)
			deltaRotation = Input.GetAxis("Mouse X") * rotateInfluence * 3.25f;
			
		float filteredDeltaRotation = (sDeltaRotationOld * 0.0f) + (deltaRotation * 1.0f);
		YRotation += filteredDeltaRotation;
		sDeltaRotationOld = filteredDeltaRotation;
			
		// * * * * * * * * * * *
		// XBox controller input	
			
		// Compute this for xinput movement
		moveInfluence = Acceleration * 0.1f * MoveScale * MoveScaleMultiplier;
			
		// Run!
	//	moveInfluence *= 1.0f + OVRGamepadController.GPC_GetAxis((int)OVRGamepadController.Axis.LeftTrigger);
			
		bool isMoving = moveForward || moveBack || moveLeft || moveRight;
		//bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || OVRGamepadController.GPC_GetAxis((int)OVRGamepadController.Axis.LeftTrigger) > 0.0f;

		// Move
		if(DirXform != null)
		{
			float leftAxisY = 
				OVRGamepadController.GPC_GetAxis((int)OVRGamepadController.Axis.LeftYAxis);
				
			float leftAxisX = 
			OVRGamepadController.GPC_GetAxis((int)OVRGamepadController.Axis.LeftXAxis);
						
			// Hugo - TECHNOLUST mode
			if (CurrentControlMode == (int)ControlMode.TECHNOLUST) {
				if (OVRGamepadController.GPC_GetButton((int)OVRGamepadController.Button.R1) || OVRGamepadController.GPC_GetButton((int)OVRGamepadController.Button.X)) {
					MoveThrottle += DirXform.TransformDirection(Vector3.forward * moveInfluence);
				}
				if (OVRGamepadController.GPC_GetButton((int)OVRGamepadController.Button.L1) || OVRGamepadController.GPC_GetButton((int)OVRGamepadController.Button.Y)) {
					MoveThrottle += DirXform.TransformDirection(Vector3.back * moveInfluence) * BackAndSideDampen;
				}
			}

			if(leftAxisY > 0.0f)
	    		MoveThrottle += leftAxisY *
				DirXform.TransformDirection(Vector3.forward * moveInfluence);
				
			if(leftAxisY < 0.0f)
	    		MoveThrottle += Mathf.Abs(leftAxisY) *		
				DirXform.TransformDirection(Vector3.back * moveInfluence) * BackAndSideDampen;

			// Hugo 
			/*if(leftAxisX < 0.0f)
				MoveThrottle += Mathf.Abs(leftAxisX) *
					DirXform.TransformDirection(Vector3.left * moveInfluence) * BackAndSideDampen;
			
			if(leftAxisX > 0.0f)
				MoveThrottle += leftAxisX *
					DirXform.TransformDirection(Vector3.right * moveInfluence) * BackAndSideDampen;*/
			float strafeAxis = leftAxisX;
            //JUMP LOL
            if (OVRGamepadController.GPC_GetButtonDown((int)OVRGamepadController.Button.A))
            {
                Jump();
            }
            if (CurrentControlMode == (int)ControlMode.FreeHeadLS || CurrentControlMode == (int)ControlMode.FixedAngleLS) {
				strafeAxis = OVRGamepadController.GPC_GetAxis((int)OVRGamepadController.Axis.RightXAxis);
			}
			
			if(strafeAxis < 0.0f)
				MoveThrottle += Mathf.Abs(strafeAxis) *
					DirXform.TransformDirection(Vector3.left * moveInfluence) * BackAndSideDampen;
			
			if(strafeAxis > 0.0f)
				MoveThrottle += strafeAxis *
					DirXform.TransformDirection(Vector3.right * moveInfluence) * BackAndSideDampen;

			if (leftAxisY != 0.0f || strafeAxis != 0.0f) {
				isMoving = true;
			}
			
		}
			
		float rightAxisX = 
		OVRGamepadController.GPC_GetAxis((int)OVRGamepadController.Axis.RightXAxis);

		// Hugo
		// Rotate
		//YRotation += rightAxisX * rotateInfluence; 


		float turnAxis = rightAxisX;
		if (CurrentControlMode == (int)ControlMode.FreeHeadLS || CurrentControlMode == (int)ControlMode.FixedAngleLS) {
			turnAxis = OVRGamepadController.GPC_GetAxis((int)OVRGamepadController.Axis.LeftXAxis);
		}
		// Fixed angle turns
		if (CurrentControlMode == (int)ControlMode.FixedAngle || CurrentControlMode == (int)ControlMode.FixedAngleLS || CurrentControlMode == (int)ControlMode.TECHNOLUST) {
			if (Mathf.Abs(turnAxis) >= turnAxisDeadZone) {
				if (!isTurning && canTurn) {
					turnDegs = turnAxis > turnAxisDeadZone ? TurnAngle : -TurnAngle;
					if (SmoothTurn) {
						currentTurnSpeed = TurnSpeed;
						isTurning = true;
					} else {
						YRotation += turnDegs;
					}
					canTurn = false;
				}
			} else {
				canTurn = true;
			}
		// Head Fixed Angle turns
		} else if (CurrentControlMode == (int)ControlMode.FixedAngleHead) { 
			float headYawAngle = OVRCamera.AbsoluteOrientation.y > 0 ? OVRCamera.AbsoluteOrientation.eulerAngles.y : OVRCamera.AbsoluteOrientation.eulerAngles.y - 360;
			float headPitchAngle = OVRCamera.AbsoluteOrientation.x > 0 ? OVRCamera.AbsoluteOrientation.eulerAngles.x : OVRCamera.AbsoluteOrientation.eulerAngles.x - 360;
			
			if (Mathf.Abs(headYawAngle) >= TurnAngle && isMoving && headPitchAngle > -15.0f) {
				if (!isTurning && (Time.time - lastTurnTime >= 0.6f)) {
					turnDegs = headYawAngle > 0 ? TurnAngle : -TurnAngle;
					if (SmoothTurn) {
						currentTurnSpeed = TurnSpeed;
						isTurning = true;
					} else {
						YRotation += turnDegs;
					}
					lastTurnTime = Time.time;
				}
			}
		// Stick Head
		} else if (CurrentControlMode == (int)ControlMode.HeadStick) { 
			float headYawAngle = OVRCamera.AbsoluteOrientation.y > 0 ? OVRCamera.AbsoluteOrientation.eulerAngles.y : OVRCamera.AbsoluteOrientation.eulerAngles.y - 360;
			float headPitchAngle = OVRCamera.AbsoluteOrientation.x > 0 ? OVRCamera.AbsoluteOrientation.eulerAngles.x : OVRCamera.AbsoluteOrientation.eulerAngles.x - 360;

			if (Mathf.Abs(headYawAngle) >= TurnAngle && isMoving && headPitchAngle > -15.0f) {
				BlinkState = BLINK_OUT;
				YRotation += OVRCamera.AbsoluteOrientation.y * DeltaTime * HeadStickRotationScaleMultiplier;
			} else {
				if (BlinkState == BLINK_OUT) {
					BlinkState = BLINK_IN;
				}
			}
		// Rotate
		} else {
			if (BlinkOnRotation) {
				if (Mathf.Abs(turnAxis) >= BlinkDeadZone) {
					BlinkState = BLINK_OUT;
					//CameraController.EnableCameras(false);
				} else if (Mathf.Abs(turnAxis) < BlinkDeadZone && BlinkState == BLINK_OUT) {
					BlinkState = BLINK_IN;
					//CameraController.EnableCameras(true);
				}
			}
			
			YRotation += turnAxis * gamepadRotateInfluence; 
		}
       
		
		// Hugo - Ze turn button!
		if (OVRGamepadController.GPC_GetButtonDown((int)OVRGamepadController.Button.R1)) {
			if (CurrentControlMode == (int)ControlMode.TurnButton) {
				// Y rotation delta between the head and the torso
				turnDegs = Mathf.DeltaAngle(DirXform.rotation.eulerAngles.y, CameraController.transform.rotation.eulerAngles.y);
				Debug.Log("Turndegs: " + turnDegs);
				if (SmoothTurn) {
					currentTurnSpeed = TurnSpeed;
					isTurning = true;
				} else {
					YRotation += turnDegs;
				}
			} else { // Other modes use it for a quick 180°
				turnDegs = 180f;
				if (SmoothTurn) {
					currentTurnSpeed = UTurnSpeed;
					isTurning = true;
				} else {
					YRotation += turnDegs;
				}
			}
		}

		// Progressive turning
		if (isTurning) {
			if (turnDegs != 0) {
				//Debug.Log("Turndegs: " + turnDegs);
				BlinkState = BLINK_OUT;
				float maxFrameRotation = (turnDegs > 0 ? currentTurnSpeed : -currentTurnSpeed) * Time.deltaTime;
				float frameRotation = Mathf.Abs(maxFrameRotation) > Mathf.Abs(turnDegs) ? turnDegs : maxFrameRotation;
				YRotation += frameRotation;
				turnDegs -= frameRotation;
			} else {
				BlinkState = BLINK_IN;
				isTurning = false;
			}
		}


		// Hugo - TECHNOLUST mode
		if (CurrentControlMode == (int)ControlMode.TECHNOLUST) {
			bool technoLustTurnButtonActivated = false;
			if (OVRGamepadController.GPC_GetAxis((int)OVRGamepadController.Axis.RightTrigger) > 0.0f) {
				triggerPressed = true;
			} else if (triggerPressed) {
				triggerPressed = false;
				technoLustTurnButtonActivated = true;
			}

			if (technoLustTurnButtonActivated) {
				if (DirXform != null) {
					Quaternion q = Quaternion.identity;
					DirXform.rotation = q * CameraController.transform.rotation;
				}
			}
		}


		// Update cameras direction and rotation#
		SetCameras();
	}

	// UpdatePlayerControllerRotation
	// This function will be used to 'slide' PlayerController rotation around based on 
	// CameraController. For now, we are simply copying the CameraController rotation into 
	// PlayerController, so that the PlayerController always faces the direction of the 
	// CameraController. When we add a body, this will change a bit..
	public virtual void UpdatePlayerForwardDirTransform()
	{
		if ((DirXform != null) && (CameraController != null))
		{
			Quaternion q = Quaternion.identity;
			if (followHead()) {
				DirXform.rotation = q * CameraController.transform.rotation;
			} else {
				q = Quaternion.Euler(0.0f, YRotation, 0.0f);
				DirXform.rotation = q * transform.rotation;
			}
			/*Debug.Log ("Head: " + CameraController.transform.rotation.eulerAngles.y + 
			           " Torso: " + DirXform.rotation.eulerAngles.y +
			           " Diff: " + (CameraController.transform.rotation.eulerAngles.y - DirXform.rotation.eulerAngles.y));*/
			//Debug.Log (CameraController.transform.rotation.eulerAngles.y - OrientationOffset.eulerAngles.y);

		}
	}
	
	// Hugo
	public bool followHead() {
		return (CurrentControlMode == (int)ControlMode.OculusSDK || 
		        CurrentControlMode == (int)ControlMode.FixedAngle || 
		        CurrentControlMode == (int)ControlMode.FixedAngleLS || 
		        CurrentControlMode == (int)ControlMode.FixedAngleHead || 
		        (CurrentControlMode == (int)ControlMode.TECHNOLUST && OVRGamepadController.GPC_GetAxis((int)OVRGamepadController.Axis.RightTrigger) == 0.0f)) && !isTurning;
	}

	///////////////////////////////////////////////////////////
	// PUBLIC FUNCTIONS
	///////////////////////////////////////////////////////////
	
	// Jump
	public bool Jump()
	{
		if (!Controller.isGrounded)
			return false;

		MoveThrottle += new Vector3(MoveThrottle.x, JumpForce, MoveThrottle.z);

		return true;
	}

	// Stop
	public void Stop()
	{
		Controller.Move(Vector3.zero);
		MoveThrottle = Vector3.zero;
		FallSpeed = 0.0f;
	}	
	
	// InitializeInputs
	public void InitializeInputs()
	{
		// Get our start direction
		OrientationOffset = transform.rotation;
		// Make sure to set y rotation to 0 degrees
		YRotation = 0.0f;
	}
	
	// SetCameras
	public void SetCameras()
	{
		if(CameraController != null)
		{
			// Make sure to set the initial direction of the camera 
			// to match the game player direction
			CameraController.SetOrientationOffset(OrientationOffset);
			CameraController.SetYRotation(YRotation);
		}
	}
	
	// Get/SetMoveScaleMultiplier
	public void GetMoveScaleMultiplier(ref float moveScaleMultiplier)
	{
		moveScaleMultiplier = MoveScaleMultiplier;
	}
	public void SetMoveScaleMultiplier(float moveScaleMultiplier)
	{
		MoveScaleMultiplier = moveScaleMultiplier;
	}
	
	// Get/SetRotationScaleMultiplier
	public void GetRotationScaleMultiplier(ref float rotationScaleMultiplier)
	{
		rotationScaleMultiplier = RotationScaleMultiplier;
	}
	public void SetRotationScaleMultiplier(float rotationScaleMultiplier)
	{
		RotationScaleMultiplier = rotationScaleMultiplier;
	}
	
	// Get/SetAllowMouseRotation
	public void GetAllowMouseRotation(ref bool allowMouseRotation)
	{
		allowMouseRotation = AllowMouseRotation;
	}
	public void SetAllowMouseRotation(bool allowMouseRotation)
	{
		AllowMouseRotation = allowMouseRotation;
	}
	
	// Get/SetHaltUpdateMovement
	public void GetHaltUpdateMovement(ref bool haltUpdateMovement)
	{
		haltUpdateMovement = HaltUpdateMovement;
	}
	public void SetHaltUpdateMovement(bool haltUpdateMovement)
	{
		HaltUpdateMovement = haltUpdateMovement;
	}

	// Hugo - Update various options here...
	void UpdateOptions() {
		// Hugo - Toggle blink on rotation. Make sure we're not currently blinking though!
		if (Input.GetKeyDown(KeyCode.CapsLock) || OVRGamepadController.GPC_GetButtonDown((int)OVRGamepadController.Button.RStick) /*&& BlinkState == BLINK_NONE*/)	{		
			BlinkOnRotation = !BlinkOnRotation;
		}

		// Hugo - Progressive turning
		if (Input.GetKeyDown(KeyCode.ScrollLock) || OVRGamepadController.GPC_GetButtonDown((int)OVRGamepadController.Button.B))	{		
			SmoothTurn = !SmoothTurn;
		}

		
		// Hugo - Cycle through control modes
		//if (Input.GetKeyDown(KeyCode.KeypadPlus) || OVRGamepadController.GPC_GetButtonDown((int)OVRGamepadController.Button.Right)) {
		//	CurrentControlMode++;
		//	if (CurrentControlMode >= Enum.GetNames(typeof(ControlMode)).Length) {
		//		CurrentControlMode = 0;
		//	}
		//	// Reset
		//	OVRDevice.ResetOrientation(0);
		//	//Debug.Log(currentControlMode + "-" + Enum.GetName(typeof(ControlMode), currentControlMode));
		//} else if (Input.GetKeyDown(KeyCode.KeypadMinus) || OVRGamepadController.GPC_GetButtonDown((int)OVRGamepadController.Button.Left)) {
		//	CurrentControlMode--;
		//	if (CurrentControlMode < 0) {
		//		CurrentControlMode = (Enum.GetNames(typeof(ControlMode)).Length - 1);
		//	}
		//	// Reset
		//	OVRDevice.ResetOrientation(0);
		//	//Debug.Log(currentControlMode + "-" + Enum.GetName(typeof(ControlMode), currentControlMode));
		//}

		if (CurrentControlMode >= (int)ControlMode.HeadStick) {
			// Hugo - Turn angle
			if (CurrentControlMode != (int)ControlMode.TurnButton) {
				if(Input.GetKeyDown(KeyCode.End) && TurnAngle > MIN_TURN_ANGLE) {	
					TurnAngle -= TURN_ANGLE_INC;
				} else if (Input.GetKeyDown(KeyCode.Home) && TurnAngle < MAX_TURN_ANGLE) {
					TurnAngle += TURN_ANGLE_INC;
				}
			}
		}
		// Hugo - Rotation scale
		if (CurrentControlMode >= (int)ControlMode.FixedAngle) {
			// Hugo - Smooth Turn speed
			if(Input.GetKeyDown(KeyCode.PageDown) && TurnSpeed > MIN_TURN_SPEED) {	
				TurnSpeed -= TURN_SPEED_INC;
			} else if (Input.GetKeyDown(KeyCode.PageUp) && TurnSpeed < MAX_TURN_SPEED) {
				TurnSpeed += TURN_SPEED_INC;
			}
		} else {
			if (CurrentControlMode == (int)ControlMode.HeadStick) {
				if(Input.GetKeyDown(KeyCode.PageDown) && HeadStickRotationScaleMultiplier > 1) {
					HeadStickRotationScaleMultiplier -= 1;
				} else if (Input.GetKeyDown(KeyCode.PageUp)) {
					HeadStickRotationScaleMultiplier += 1;
				}
			} else {
				if(Input.GetKeyDown(KeyCode.PageDown) && GamepadRotationScaleMultiplier > 1) {
					GamepadRotationScaleMultiplier -= 1;
				} else if (Input.GetKeyDown(KeyCode.PageUp)) {
					GamepadRotationScaleMultiplier += 1;
				}
			}
		}		
		/*if (OVRGamepadController.GPC_GetButtonDown((int)OVRGamepadController.Button.B)) {
			Debug.Log("B Down");
		}
		if (OVRGamepadController.GPC_GetButtonUp((int)OVRGamepadController.Button.B)) {
			Debug.Log("B Up");
		}*/
	}


}

