using UnityEngine;
using System.Collections;

public class CharacterLogic : MonoBehaviour {
	public float speed = 5.0f;
	public CharacterController monsterController;
	Vector3 moveDirection = Vector3.zero;
	private float gravity = 0.4f;
	public Vector3 instantVelocity;
	private float orientation = 0;

	void Start() {
		instantVelocity = Vector3.zero;
		monsterController = GetComponent<CharacterController>();
		GetComponent<Animation>().wrapMode = WrapMode.Loop;
	}
	
	void Update() {
		Vector3 position = transform.position;
		
		if(monsterController.isGrounded) {
			moveDirection = new Vector3(0, 0, Input.GetAxis("Vertical"));
			moveDirection = transform.TransformDirection(moveDirection);
			moveDirection *= speed;
		}

		//float 
		orientation += Input.GetAxis("Horizontal") * 5.0f;
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, orientation, transform.eulerAngles.z);
//		transform.eulerAngles.y += Input.GetAxis("Horizontal") * 5;
		moveDirection.y -= gravity * Time.deltaTime;
		monsterController.Move(moveDirection * Time.deltaTime);
		
		instantVelocity = transform.position - position;
	}
}