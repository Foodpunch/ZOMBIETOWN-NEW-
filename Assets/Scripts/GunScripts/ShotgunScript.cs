using UnityEngine;
using System.Collections;

public class ShotgunScript : WeaponBase { //old retired script

	int pelletCount = 8;
	public float timeTillMaxSpreadAngle = 2f;
	public float maxBulletSpreadAngle = 15f;
	public AnimationCurve bulletSpreadCurve;



	protected override void PrimaryFire()
	{

		for (int i = 0; i < pelletCount; i++) //for each pellet, shoot one in a random direction based off the cone
		{

			RaycastHit hit;

			//Bullet recoil stuff and it's direction
			Quaternion fireRot = Quaternion.LookRotation(transform.forward);
			Quaternion randomRot = Random.rotation;
			float currentSpread = bulletSpreadCurve.Evaluate(fireTime / timeTillMaxSpreadAngle) * maxBulletSpreadAngle; //changes bulllet angle
			fireRot = Quaternion.RotateTowards(fireRot, randomRot, Random.Range(0.0f, currentSpread)); //randomises it within said cone


			if (Physics.Raycast(transform.position, fireRot * Vector3.forward, out hit, Mathf.Infinity))
			{
				HitInfo hitInfo = new HitInfo(); //send hitInfo stuff
				hitInfo.damage = damage;
				hitInfo.raycastHit = hit;
				hit.collider.gameObject.SendMessage("GunHitInfo", hitInfo, SendMessageOptions.DontRequireReceiver); //send damage and position of hit

			}
		}
		
	}

	
}
