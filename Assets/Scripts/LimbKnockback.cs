using UnityEngine;
using System.Collections;

public class LimbKnockback : MonoBehaviour,IDamagable<HitInfo> {

	Rigidbody _rb;			//rigidbody for limb
	Collider _col;			//collider for limb
	[SerializeField] ParticleSystem bloodSpurt;		//blood spurt
	[SerializeField] ZombieBase _zombie;			//zombie script
	float _damageReceived;							//damage limb has received
	bool hasDisconnected;

	protected enum ZombieParts						//Identifier thingy for parts
	{
		HEAD,
		ARM,
		LEG,
		BODY
	};
	[SerializeField]ZombieParts _bodyParts;			//init the parts


	void Start()
	{
		//typical get component stuff
		_rb = GetComponent<Rigidbody>();
		_col = GetComponent<Collider>();

		hasDisconnected = false;
	}
	
	

	public void Damage(HitInfo _info)		//what happens if you take damage
	{
        BodyDamageInfo _damageInfo = new BodyDamageInfo();  //instantiates a damage info class
		switch (_bodyParts)
		{
 			case ZombieParts.HEAD:					//if you hit the head (the zombie should die to a headshot)
				_damageInfo._bodyParts = BodyDamageInfo.Parts.HEAD;		//identifies as head
				_damageInfo.damageThreshold = 15f;						//damage required to knock the head off
				_damageInfo.fixedDamage = 100;							//damage the body receives because of that
				break;
			case ZombieParts.BODY:
				_damageInfo._bodyParts = BodyDamageInfo.Parts.BODY;		//Identifies as body, that's all
				_damageInfo.damageThreshold = 9999999f;					//I don't want the body to fly off
				break;
			case ZombieParts.ARM:
				_damageInfo._bodyParts = BodyDamageInfo.Parts.ARM;		//identifies as arm, should half damage because of that
				_damageInfo.damageThreshold = 20f;						//damage required to knock arms off
				_damageInfo.fixedDamage = 15;							//dmg zombie receives when arm flies off
				break;
			case ZombieParts.LEG:
				_damageInfo._bodyParts = BodyDamageInfo.Parts.LEG;		//identifies leg, shoud slow the zombie because legs are gone
				_damageInfo.damageThreshold = 5f;						//damage required to knock legs off
				_damageInfo.fixedDamage = 50;							//50 because 2 legs gone means you dead
				break;
		}
		_damageReceived += _info.damage;
		if (!hasDisconnected) //Checks if the part if disconnected, prevents a disconnected part from still sending damage to main body
		{
			if (_damageReceived > _damageInfo.damageThreshold)	//checks if the damage exceeds the amount the limb can take
			{
				_rb.gameObject.transform.parent = null;			//unparents the object
				_rb.isKinematic = false;						//allows physics to act on it
				switch(_info._forceMode)
                {
                    case HitInfo.ForceType.NORMAL:
                        _rb.AddForce(CalculateHitDir(_info.shooterPos, _info.raycastHit.point) * _info.bulletForce); //adds force in correct direction
                        break;
                    case HitInfo.ForceType.EXPLOSION:
                        _rb.AddExplosionForce(_info.bulletForce, _info.raycastHit.point,10f,2f);
                        break;
                }
				_zombie.GetComponent<ZombieBase>().CalculateDamage(_damageInfo);		//sends fixed damage to the body
			}
			else		//if the damage the limb received doesn't exceed the threshold though, send the damage as per normal to the body
			{
				_zombie.GetComponent<ZombieBase>().CalculateDamage(_info.damage); //sends damage to the body
			}
		}
		else
		{
            switch (_info._forceMode)
            {
                case HitInfo.ForceType.NORMAL:
                    _rb.AddForce(CalculateHitDir(_info.shooterPos, _info.raycastHit.point) * _info.bulletForce); //adds force in correct direction
                    break;
                case HitInfo.ForceType.EXPLOSION:
                    _rb.AddExplosionForce(_info.bulletForce, _info.raycastHit.point, 10f, 2f);
                    break;
            }
        }
		
	}
	Vector3 CalculateHitDir(Vector3 _start,Vector3 _end)
	{
		return ((_end - _start).normalized);
	}
	void OnJointBreak(float breakForce) //function that controls what happens on JointBreaking
	{
		hasDisconnected = true;			//Checks if join is disconnected or not.
		bloodSpurt.Play();				//Plays blood spurt animation	
		StartCoroutine("BloodFade");	//Starts the blood reduction process 
		
	}
	IEnumerator BloodFade()
	{
		yield return new WaitForSeconds(3); //lets the blood spurt for 3 seconds before starting to reduce
		for (short i = 300; i > -1; i--)	//uses forloop to slow the blood spurt
		{
			bloodSpurt.emissionRate = i;	//sets the emission rate for the blood
			yield return null;				//returns control to the game for update every time it reduces once
		}

		//--Commented out because I want the player to see all the stuff that he did
		//gameObject.SetActive(false);
	
	}
	void DisableJoint()
	{
        //--Commented out because ActivateRagdoll in Death State constantly sets isKinematic to false, breaking this logic
        //also I forgot where to put it lol. Most probably put it after bloodspurt ends
        _rb.isKinematic = true;
		_rb.constraints = RigidbodyConstraints.FreezeAll;
		_col.enabled = false;
	}
}
