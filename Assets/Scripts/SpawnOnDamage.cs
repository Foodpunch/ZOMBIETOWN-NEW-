using UnityEngine;
using System.Collections;

public class SpawnOnDamage : MonoBehaviour {


	//Depreciated code, used in weapon base now
	
	//public GameObject objToSpawn;
	//void GunHitInfo(HitInfo hitInfo)
	//{
	//	//Instantiate(objToSpawn, hitInfo.raycastHit.point, Quaternion.LookRotation(hitInfo.raycastHit.normal)); //Old bit of code, obsolete
	//	GameObject obj = ObjectPoolingScript.current.GetPooledObject();
	//	if (!System.Object.ReferenceEquals(obj,null))
	//	{
	//		return;
	//	}
	//	obj.transform.position = hitInfo.raycastHit.point; //sets position of the particle
	//	Vector3 incVector = hitInfo.raycastHit.point-hitInfo.shooterPos; //direction of shot
	//	Vector3 reflVector = Vector3.Reflect(incVector,hitInfo.raycastHit.normal); //calculates particle's direction
	//	obj.transform.rotation = Quaternion.LookRotation(reflVector); //changes the particle's direction
	//	obj.SetActive(true); //shows the particle
	//	gameObject.SendMessage("Damage", hitInfo.damage,SendMessageOptions.DontRequireReceiver); //sends damage IF it has health
	//}

}
public struct HitInfo //info the bullet carries
{
	public RaycastHit raycastHit; //where it hits
	public Vector3 shooterPos; //where the shooter is
	public float damage;		//how much damage
	public float bulletForce;	//how much force
}
public interface IDamagable<T> //if the object can take damage, implement
{
 	void Damage(T damageTaken);
}
public interface IKnockBack<T> //if the object can be knocked back, implement
{
	void KnockBack(T forceHit);
}
public class BodyDamageInfo //class for body parts
{
	public enum Parts		//different body parts
	{
 		HEAD,
		BODY,
		ARM,
		LEG
	}
	public Parts _bodyParts;
	public float damageThreshold;		//how much damage a body part can take
	public int fixedDamage;				//how much damage the zombie takes when it loses this part
}
//public class DamageEffect: BodyDamageInfo
//{

//	public enum EffectType
//	{
//		SLOW,
//		WEAKNESS,
//		POISONED
//	};
//	public EffectType effect;
//	public delegate void EffectDelegate();
//	public EffectDelegate effectsDelegate;

//	public void SlowEffect()
//	{ 
	
//	}
//	public void WeaknessEffect()
//	{
 
//	}
//	public void PoisonedEffect()
//	{
 
//	}
//}

//166.074 , 133.324
