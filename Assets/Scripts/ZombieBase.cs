using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class ZombieBase : MonoBehaviour{

	float gameTime;         //state timer
	protected float attackCooldown;   //cooldown for zombie attack
	protected float aggroRange = 10f;     //aggro range for zombie
	float currRange;            //curr range of player and zombie
	//Zombie stat stuff
	//protected float maxHealth;
	protected float health;        //default starting health is 100
	protected float damage;     //technically obsoleted due to new health system
	
	//Zombie Movement RelatedVariables
	protected Transform target;          //technically should name this to player

	Vector3 direction;		    //direction of zombie to target
	Quaternion rotate;             //for rotatating to face target
	Vector3 leftRayDirection;       //vector to point sphere cast in left direction
	Vector3 rightRayDirection;      //vector to point sphere cast in right direction
	RaycastHit hitLeft;             //stores hit data
	RaycastHit hitRight;            //also stores hit data
	Vector3 hitNormal;              //normal direction of hit
	float heightOffset; 
	[SerializeField] float speed = 2f;      //speed of zombie
	[SerializeField] float steerForce;      //how much the zombie should turn away when bumping in to wall
	[SerializeField] float rotateSpeed;     //rotation speed when WANDERING, not chasing   
	[SerializeField] float minimumDistanceToAvoid; //distance before bumping into thing
	[SerializeField] LayerMask zombieLayerMask;     //layer mask to check collisions

	//Pathfinding Variables
//	float rotationSpeed = 5f;
	float minDistToReach = 3f;
	[SerializeField]protected Vector3 areaCenter;
	[SerializeField]protected Vector3 rectSize;
	[SerializeField]protected float rectMagnitude = 2f;
	protected Vector3 _wayPoint;

    //ragdoll stuff
	[SerializeField]
	Rigidbody[] _ragdollParts;
	bool bRagdoll;

    //animator stuff
	Animator ZombieAnim;
    [SerializeField] protected int i_zombieAnim;

    //sound stuff
    protected AudioSource ZombieSource;
    [SerializeField] AudioClip[] zombieSounds;

    //Rigidbody _rbZombie;
    bool _SpecialAttack = false;
	//UI related stuff
	protected Slider HealthBar;
	
	protected enum State
	{
 		IDLE,		//Zombie IDLE
		WANDER,		//Roam or wander whatever, Patrol?
		CHASE,		//Chase player
		//COOLDOWN,	//Cooldown from attacking the player?
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
        ZombieSource = GetComponent<AudioSource>();
		FindNewTargetPosition();
		heightOffset = 2f;
		minimumDistanceToAvoid = 2f;
		//_wayPoint = areaCenter + (Utilities.OnUnitRect(rectSize.x, rectSize.z)) * rectMagnitude;
	//	_rbZombie = GetComponent<Rigidbody>(); //zombie's rigidbody component
	
}
	
	protected virtual void Update()
	{
		attackCooldown += Time.deltaTime; //Attack cooldown timer
		CheckHealth();		 //Updates the slider
		//if (Input.GetKeyDown(KeyCode.H))
		//{
		//	ChangeState(0f, State.IDLE);
		//}
		
	}
	void FixedUpdate()
	{
		//Placed in fixed update because it contains movement and physics
		ZombieLogic();       //contains zombie's machine
	}
	
	protected void ZombieLogic()
	{
		gameTime += Time.deltaTime; //calculates time elapsed. Essentially a timer
		currRange = Utilities.instance.DistanceBetween(transform.position, target.position); //finds range between player and zombie
		
		switch (ZombieState) //zambie state machine
		{
 			//Idle state, when lose aggro and stuff
			case State.IDLE:
                //ZombieAnim.Play(idleAnim); //plays idle animation
                ZombieAnim.SetInteger("iAnimNum", i_zombieAnim);
                ZombieAnim.SetBool("bIdle", true);
				if (!isAggro()) //if player is not in range
				{
                   
                    ChangeState(5f, State.WANDER); //wander about aimlessly, doing zmbie things
				}
				else if (isAggro()) //but if the pplayer is in range..
				{
					if (previousState == State.CHASE) //and zambie hit the player just now,
					{
                       //Put in possible attack stuff
                        ChangeState(2f, State.CHASE); //wait awhile before chasing the player again
                        if(!_SpecialAttack)
                        {
                            _SpecialAttack = true;
                            Invoke("SpecialAttack", 2.2f);
                        }
                        if(!ZombieSource.isPlaying)
                        {
                            ZombieSource.clip = zombieSounds[Mathf.RoundToInt(Random.Range(0, zombieSounds.Length - 1))];
                            ZombieSource.PlayDelayed(2f);
                        }
					}
					else					//but if zambie wasn't chasing the player before,
					{
                          if(!ZombieSource.isPlaying)
                        {
                            ZombieSource.clip = zombieSounds[Mathf.RoundToInt(Random.Range(0, zombieSounds.Length - 1))];
                            ZombieSource.Play();
                        }
                        ChangeState(0f, State.CHASE); //zambie better chase the damn player now.
					}
				}
				//yes I intentionally spelt it as zambie	
				break;
			//Wander state, not necessarily looking for player, just walking around, doing zombie stuff.
			case State.WANDER:
                //maybe vary, sometimes patrol, sometimes wander?
                
                ZombieAnim.SetBool("bWalk", true);
				MovementLogic(transform.position, _wayPoint, speed/2, true);
				if (isAggro())
				{
                    ChangeState(0f, State.CHASE);
				}
				break;
			//Found the player, chase after him NOW
			case State.CHASE:
                
                ZombieAnim.SetBool("bWalk", true);
                
                if (!ZombieSource.isPlaying)
                {
                    ZombieSource.clip = zombieSounds[Mathf.RoundToInt(Random.Range(0, zombieSounds.Length - 1))];
                    ZombieSource.PlayDelayed(2f);
                }
                //	MovementLogic(transform.position,target.position,speed);  //Logic for chasing and not walking through walls
                ChaseLogic();
				if(!isAggro())
				{
					ChangeState(0f, State.WANDER);
				}
				break;
			//Player does something scary :C RUNNNN
			case State.FLEE:
				break;
			//If I die, I die.
			case State.DEATH:
				//should make it chance, sometimes play anim, sometimes ragdoll. (should have higher chance to ragdoll pls thx)
			//	ZombieAnim.Play("Death_02");
				HealthBar.gameObject.SetActive(false);
				if(!bRagdoll)
                {
                    bRagdoll = true;
                    ActivateRagdoll();
                    GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().AddKillCount();
                }
				ZombieAnim.enabled = false;

				break;

		}
	}
	void Reset() //state reset logic
	{
		gameTime = 0.0f; //resets time
        ZombieAnim.StopPlayback();
		ZombieAnim.SetBool("bIdle", false);
		ZombieAnim.SetBool("bWalk", false);
		ZombieAnim.SetBool("bEating", false);
	}
	void ChangeState(float timeBeforeStateChange, State nextState) //state changer. 
	{
		if (gameTime > timeBeforeStateChange) //if the time exceeds the time limit for the state
		{
			previousState = ZombieState; //saves previous state before it changes
			ZombieState = nextState; //change curr state to next state
			Reset(); //reset the state, and send curr state to save as prev state
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
		transform.localPosition += transform.forward * Time.deltaTime * _speed;
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
		Debug.DrawRay(transform.position + (Vector3.up*heightOffset), leftRayDirection, Color.blue);
		Debug.DrawRay(transform.position + (Vector3.up * heightOffset), rightRayDirection, Color.blue);

		RaycastHit hit;		//middle ray hit info
		// MIDDLE FEELER
		if (Physics.SphereCast(transform.position + (Vector3.up * heightOffset), 0.2f, transform.forward, out hit, 2, zombieLayerMask))
		{
			if (hit.transform != transform)
			{ // Intersection with own collider is omitted
				Vector3 MiddleHitNormal = hit.normal;
				Debug.DrawLine(transform.position + (Vector3.up * heightOffset), hit.point, Color.green);
				MiddleHitNormal.y = 0.0f;
				direction += hit.normal * 50;
			}
			CheckCollision(hit);
			
		}
		//LEFT FEELER
		if (Physics.SphereCast(transform.position + (Vector3.up * heightOffset), 0.2f, leftRayDirection, out hitLeft, minimumDistanceToAvoid, zombieLayerMask))
		{
			if (hitLeft.transform != transform)
			{ // Intersection with own collider is omitted
				MoveLeft();
			}
			CheckCollision(hitLeft);
		}
		//RIGHT FEELER
		if (Physics.SphereCast(transform.position + (Vector3.up * heightOffset), 0.2f, rightRayDirection, out hitRight, minimumDistanceToAvoid, zombieLayerMask))
		{
			if (hitRight.transform != transform)
			{ // Intersection with own collider is omitted
				MoveRight();
			}
			CheckCollision(hitRight);
		}
		direction.y = 0;
		rotate = Quaternion.LookRotation(direction.normalized);
		if (Utilities.instance.DistanceBetween(transform.position, target.position) > 1.5f)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, rotate, Time.deltaTime * rotateSpeed);
			transform.position += transform.forward * Time.deltaTime * speed;
		}
	
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
	void CheckCollision(RaycastHit _hit)
	{
		if (_hit.collider.gameObject.CompareTag("Player"))
		{
			if (attackCooldown > 2f) //Zombies can only hurt you ever 2 seconds
			{
				ChangeState(0f, State.IDLE);
				attackCooldown = 0;
				_hit.collider.gameObject.SendMessage("Damage", damage, SendMessageOptions.DontRequireReceiver); //send damage to player
			}
		}
	}
	bool isAggro()
	{
		if (currRange < aggroRange)
		{
			return true;
		}
		else
		{
			return false;
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
    protected virtual void SpecialAttack()
    {
        CancelInvoke("SpecialAttack");
        _SpecialAttack = false;
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
		Gizmos.DrawSphere(hitRight.point, 0.2f);
		Gizmos.DrawSphere(hitLeft.point, 0.2f);
	}
	
}
