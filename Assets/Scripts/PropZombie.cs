using UnityEngine;
using System.Collections;

public class PropZombie : MonoBehaviour,IDamagable<HitInfo> {


	Animator zombieAnim;		//animator for zombie
	Collider _col;				//collider for zombie
	public enum State			//Zombie animation states
	{
		IDLE,
		EATING,
		WALKING,
		DEATH
	};
	[SerializeField] State ZombieAnimState;  //init state
	void Start()
	{
		//typical get component stuff
		zombieAnim = GetComponent<Animator>();
		_col = GetComponent<Collider>();
	}

	// Update is called once per frame
	void Update () {
		switch (ZombieAnimState)		//Finite state machine
		{
 			case State.EATING:
				zombieAnim.Play("Zombie_Eating");
				break;
			case State.IDLE:
				zombieAnim.Play("Zombie_Idle");
				break;
			case State.WALKING:
				zombieAnim.Play("Zombie_Walk");
				break;
			case State.DEATH:			//plays death animation and disables the damn collider
				zombieAnim.Play("Death_02");
				_col.enabled = false;
				break;

		}
	
	}
	public void Damage(HitInfo _info) //if you take ANY damage, die
	{
		if (_info.damage > 0)
		{
			ZombieAnimState = State.DEATH;
		}
	}
}
