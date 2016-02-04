using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class ZombieBase : MonoBehaviour{

	float gameTime;
	float attackCooldown;
	float aggroRange = 10f;
	float currRange;
	//Zombie stat stuff
	//protected float maxHealth;
	protected float health;
	protected float damage;
	
	//Zombie Movement RelatedVariables
	Transform target;

	Vector3 direction;
	Quaternion rotate;
	Vector3 leftRayDirection;
	Vector3 rightRayDirection;
	RaycastHit hitLeft;
	RaycastHit hitRight;
	Vector3 hitNormal;
	[SerializeField] float speed = 2f;
	[SerializeField] float steerForce;
	[SerializeField] float rotateSpeed;
	[SerializeField] float translateSpeed;
	[SerializeField] float minimumDistanceToAvoid;

	//Pathfinding Variables
	float rotationSpeed = 5f;
	float minDistToReach = 3f;
	[SerializeField]protected Vector3 areaCenter;
	[SerializeField]protected Vector3 rectSize;
	[SerializeField]protected float rectMagnitude = 2f;
	protected Vector3 _wayPoint;


	[SerializeField]
	Rigidbody[] _ragdollParts;
	bool bRagdoll;


	Animator ZombieAnim;
	//Rigidbody _rbZombie;

	//UI related stuff
	protected Slider HealthBar;
	
	protected enum State
	{
 		IDLE,		//Zombie IDLE
		WANDER,		//Roam or wander whatever, Patrol?
		CHASE,		//Chase player
		FLEE,		//Run from player
		DEATH,		//Die
		//Zombie should have an attack, but he touches to hurt you.
		//touches you, then goes to idle.
	}
	[SerializeField]
	protected State ZombieState;
	State previousState;

	protected virtual void Start() //Zombie start
	{
		target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>(); //Target (player)
		HealthBar = gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<Slider>();
		ZombieAnim = GetComponent<Animator>(); //zombie animator component
		FindNewTargetPosition();
		//_wayPoint = areaCenter + (Utilities.OnUnitRect(rectSize.x, rectSize.z)) * rectMagnitude;
	//	_rbZombie = GetComponent<Rigidbody>(); //zombie's rigidbody component
	
}
	
	protected virtual void Update()
	{
		gameTime += Time.deltaTime; //calculates time elapsed. Essentially a timer
		attackCooldown += Time.deltaTime; //Attack cooldown timer
		CheckHealth();		 //Updates the slider
		
	}
	void FixedUpdate()
	{
		//Placed in fixed update because it contains movement and physics
		ZombieLogic();       //contains zombie's machine
	}
	
	protected void ZombieLogic()
	{
		currRange = Utilities.instance.DistanceBetween(transform.position, target.position);
		if (currRange < aggroRange)
		{
			ChangeState(0f, State.CHASE);
		}
		switch (ZombieState)
		{
 			//Idle state, when lose aggro and stuff
			case State.IDLE:
				ZombieAnim.SetBool("bIdle", true);
				ChangeState(5f, State.WANDER);
				
				break;
			//Wander state, not necessarily looking for player, just walking around, doing zombie stuff.
			case State.WANDER:
				//maybe vary, sometimes patrol, sometimes wander?
				ZombieAnim.SetBool("bWalk", true);
				MovementLogic(transform.position, _wayPoint, speed/2, true);
				break;
			//Found the player, chase after him NOW
			case State.CHASE:
				ZombieAnim.SetBool("bWalk", true);
				MovementLogic(transform.position,target.position,speed);  //Logic for chasing and not walking through walls
				if (currRange > aggroRange) //range checker
				{
					ChangeState(0f, State.WANDER);
				}
				break;
			//Player does something scary :C RUNNNN
			case State.FLEE:
				break;
			//If I die, I die.
			case State.DEATH:
			//	ZombieAnim.Play("Death_02");
				HealthBar.gameObject.SetActive(false);
				ActivateRagdoll();
			
//				gameObject.GetComponent<Collider>().enabled = false;
				ZombieAnim.enabled = false;

				break;

		}
	}
	void Reset(State lastState) //state reset logic
	{
		gameTime = 0.0f; //resets time
		previousState = lastState; //saves previous state just incase?
		ZombieAnim.SetBool("bIdle", false);
		ZombieAnim.SetBool("bWalk", false);
		ZombieAnim.SetBool("bEating", false);
	}
	void ChangeState(float timeBeforeStateChange, State nextState) //state changer. 
	{
		if (gameTime > timeBeforeStateChange) //if the time exceeds the time limit for the state
		{
			ZombieState = nextState; //change curr state to next state
			Reset(ZombieState); //reset the state, and send curr state to save as prev state
		}
	}
	void MovementLogic(Vector3 A, Vector3 B, float _speed,bool isRoaming = false)
	{
		Vector3 direction = (B - A).normalized;
		direction.y = 0;
		Quaternion endRotation = Quaternion.LookRotation(direction);
		if (isRoaming) 
		{
			if(Utilities.instance.DistanceBetween(A,B)<minDistToReach)
			{
				FindNewTargetPosition();
				ChangeState(0, State.IDLE);
			}
		}
		transform.rotation = Quaternion.Slerp(transform.rotation, endRotation, Time.deltaTime * rotateSpeed);
		transform.localPosition += transform.forward * Time.deltaTime * speed;
	}
	void FindNewTargetPosition()
	{
		_wayPoint = areaCenter + (Utilities.OnUnitRect(rectSize.x,rectSize.z))*rectMagnitude;
	}

	void ChaseLogic() //chase logic for zombie
	{
		direction = (target.position - transform.position).normalized; //current direction

		leftRayDirection = transform.TransformDirection(new Vector3(-1, 0, 1)); //left feeler
		rightRayDirection = transform.TransformDirection(new Vector3(1, 0, 1)); //right feeler

		RaycastHit hit;		//middle ray hit info
		// MIDDLE FEELER
		if (Physics.Raycast(transform.position, transform.forward, out hit, 4))
		{
			if (hit.transform != transform)
			{ // Intersection with own collider is omitted
				Vector3 MiddleHitNormal = hit.normal;
				Debug.DrawLine(transform.position, hit.point, Color.green);
				MiddleHitNormal.y = 0.0f;
				direction += hit.normal * 50;
			}
		}
		//LEFT FEELER
		if (Physics.Raycast(transform.position, leftRayDirection, out hitLeft, minimumDistanceToAvoid))
		{
			if (hitLeft.transform != transform)
			{ // Intersection with own collider is omitted
				MoveLeft();
			}
		}
		//RIGHT FEELER
		if (Physics.Raycast(transform.position, rightRayDirection, out hitRight, minimumDistanceToAvoid))
		{
			if (hitRight.transform != transform)
			{ // Intersection with own collider is omitted
				MoveRight();
			}
		}
		direction.y = 0;
		rotate = Quaternion.LookRotation(direction.normalized);
		transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime * rotateSpeed);
		transform.position += transform.forward * Time.deltaTime * translateSpeed;
	
	}
	void MoveLeft() //Decide what happens when left feeler touches something
	{
		Vector3 leftHitNormal = hitLeft.normal;
		Debug.DrawRay(hitLeft.point, leftHitNormal, Color.red);
		leftHitNormal.y = 0.0f; // Restrict movement in y direction
		direction = transform.forward + leftHitNormal * steerForce;
	}
	void MoveRight() //Decide what happens when right feeler touches something
	{
		Vector3 rightHitNormal = hitRight.normal;
		Debug.DrawRay(hitRight.point, rightHitNormal, Color.blue);
		rightHitNormal.y = 0.0f; // Restrict movement in y direction
		direction = transform.forward + rightHitNormal * steerForce;
	}
	void CheckHealth()
	{
		HealthBar.value = health; //displays health
		if (HealthBar.value == 0) //if health 0, change state
		{
			//ZombieAnim.Play("Death_02");
			//Invoke("ActivateRagdoll", 0);
			ChangeState(0, State.DEATH);
		}
	}
	public void CalculateDamage(BodyDamageInfo _damageTaken)
	{
	//	DamageEffect _dmgEffect = new DamageEffect();
		switch (_damageTaken._bodyParts)
		{
 			case BodyDamageInfo.Parts.HEAD:
				//activate rigidbody
	//			bRagdoll = true;
				//ActivateRagdoll();
				break;
			case BodyDamageInfo.Parts.ARM:
				//lower damage of AI
			//	_dmgEffect.effectsDelegate += _dmgEffect.WeaknessEffect;
				break;
			case BodyDamageInfo.Parts.LEG:
				//slow AI
			//	_dmgEffect.effectsDelegate += _dmgEffect.SlowEffect;
				break;
		}
	//	_dmgEffect.effectsDelegate();
		health -= _damageTaken.fixedDamage;
	}
	public void CalculateDamage(float info)
	{
		health -= info;
	}
	void ActivateRagdoll() //function to enable the ragoll
	{
		
		foreach (Rigidbody _limb in _ragdollParts)
		{
			_limb.isKinematic = false;
		}

	}
	//public void Damage(HitInfo _hit) //function for taking damage (Interface function)
	//{
	//	health -= _hit.damage;
	//}
	//public void KnockBack(HitInfo _info) //function for knockback (Interface function)
	//{
	//	if (health < _info.damage)
	//	{
	//		Debug.Log("Still Alive");
	//	//	_rbZombie.AddForce(-transform.forward * _info.bulletForce);
	//	}
	//}
	protected virtual void OnCollisionEnter(Collision col) //If Zombie touches player 
	{
		
		if (col.gameObject.CompareTag("Player"))
		{
			Debug.Log("Hit");
			if (attackCooldown > 2f) //Zombies can only hurt you ever 2 seconds
			{
				attackCooldown = 0;
				col.gameObject.SendMessage("Damage", damage, SendMessageOptions.DontRequireReceiver); //send damage to player
			}
		}
		
	}
	void OnDrawGizmos() //Draws the cube and stuff just to show you the bounding boxes of its roaming
	{
		Gizmos.color = Color.black;
		float offsetX = rectSize.x / 2;
		float offsetZ = rectSize.z / 2;
		Vector3 cubePos = new Vector3(offsetX, 0, offsetZ);
		Gizmos.DrawWireCube(areaCenter + (cubePos * rectMagnitude), rectSize * rectMagnitude);
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(_wayPoint, new Vector3(1, 1, 1));
		Gizmos.DrawSphere(hitRight.point, 0.05f);
	}
	
}
