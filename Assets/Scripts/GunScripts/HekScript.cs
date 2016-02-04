using UnityEngine;
using System.Collections;

public class HekScript : WeaponBase {

	/* Weapon Description
	 * - Semi Automatic
	 * - High damage PER PELLET
	 * - Maybe 4/90
	 * - probably no recoil
	*/

	int pelletCount = 9;  //Number of pellets it fires
	float spreadAngle = 15f; //Sangle of shot spread

	protected override void Start()
	{
		base.Start(); //Get tags for ammo counter and reload thing
		damage = 3;
		maxAmmo = 4;
		totalAmmo = 90;
		currentAmmo = maxAmmo;
		reloadDelay = 0.3f;
		fireDelay = 0.8f;
		_bulletForce = 500f;
		automatic = false;

		//Weapon anim stuff
		_playerAnim.SetBool("FullAuto_b", false);
		_playerAnim.SetInteger("WeaponType_int", 4);
		_playerAnim.SetFloat("Body_Horizontal_f", 0.6f);
		_playerAnim.SetFloat("Body_Vertical_f", 0.0f);
		
	}

	protected override void PrimaryFire()
	{
		for (int i = 0; i < pelletCount; i++)
		{
			RaycastHit hit; //for raycast hit
			Quaternion fireRot = Quaternion.LookRotation(transform.forward);
			Quaternion randomRot = Random.rotation;
			fireRot = Quaternion.RotateTowards(fireRot, randomRot, Random.Range(0.0f, spreadAngle));
			if (Physics.SphereCast(transform.position,0.05f, fireRot * Vector3.forward, out hit, Mathf.Infinity))
			{
				HitInfo hitInfo = new HitInfo();
				hitInfo.damage = damage;
				hitInfo.raycastHit = hit;
				hitInfo.bulletForce = _bulletForce;
				hitInfo.shooterPos = gameObject.transform.position;
				SpawnFakeBullet(hitInfo);
			//	hit.collider.gameObject.SendMessage("GunHitInfo", hitInfo, SendMessageOptions.DontRequireReceiver);
				SpawnParticles(hitInfo);
			}
		}
	}
}
