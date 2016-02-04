using UnityEngine;
using System.Collections;

public class BratonScript : WeaponBase {

	/* Weapon Description
	 * - High Fire Rate
	 * - Somewhat Okay Damage
	 * - maybe 30/150 ammo capacity
	 * - Recoil starts slow then ramps up with high kick
	 * - Bullet force  of 2000 
	*/
	
	
	//this is rifle with recoil
	float timeTillMaxSpreadAngle = 2f;
	float maxBulletSpreadAngle = 15f;
	public AnimationCurve bulletSpreadCurve;

	protected override void Start()
	{
		base.Start(); //Gets the tags for reload and ammo 
		//Basic Gun Variables
		damage = 3;
		fireDelay = 0.08f;
		maxAmmo = 300;
		totalAmmo = 180;
		currentAmmo = maxAmmo;
		reloadDelay = 3f;
		automatic = true;
		_bulletForce = 100f;

		//Animator stuff
		_playerAnim.SetBool("FullAuto_b", true);
		_playerAnim.SetInteger("WeaponType_int", 2);
		_playerAnim.SetFloat("Body_Horizontal_f",0.6f);
		_playerAnim.SetFloat("Body_Vertical_f",0.0f);
	}
	
	protected override void PrimaryFire()
	{
		RaycastHit hit;

		//Bullet recoil stuff (DIFFERENT FROM SHOTGUNS)
		Quaternion fireRot = Quaternion.LookRotation(transform.forward);
		Quaternion randomRot = Random.rotation;
		float currentSpread = bulletSpreadCurve.Evaluate(fireTime / timeTillMaxSpreadAngle) * maxBulletSpreadAngle; //uses curve to change gun's accuracy while firing
		fireRot = Quaternion.RotateTowards(fireRot, randomRot, Random.Range(0.0f, currentSpread));
		
		if (Physics.SphereCast(transform.position,0.05f, fireRot * Vector3.forward, out hit, Mathf.Infinity)) //EDIT
		{
			HitInfo hitInfo = new HitInfo();
			hitInfo.damage = damage;
			hitInfo.raycastHit = hit;
			hitInfo.bulletForce = _bulletForce;
			hitInfo.shooterPos = gameObject.transform.position;
			//hit.collider.gameObject.SendMessage("GunHitInfo", hitInfo, SendMessageOptions.DontRequireReceiver);
			SpawnParticles(hitInfo);
			SpawnFakeBullet(hitInfo);
		//	DrawLine(hitInfo);
			//if(hit.collider.gameObject.GetComponent<NormalZombie>())
		}
	}
}
