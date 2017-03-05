using UnityEngine;
using System.Collections;

public class LexScript : WeaponBase {
	/* Weapon Description
	 * - Semi Automatic
	 * - High Damage per shot
	 * - 8/Infinite? or 8/210
	 * - Slow Fire rate, so low recoil
	 */
	
	//In case of recoil
	float timeTillMaxSpreadAngle = 2f;
	float maxBulletSpreadAngle = 15f;
	[SerializeField] AnimationCurve bulletSpreadCurve;


	protected override void Start () 
	{
		base.Start(); //Gets tags for realod and Ammo
		//Basic Gun Variables
		damage = 8;
		maxAmmo = 15;
		totalAmmo = 210;
		currentAmmo = maxAmmo;
		reloadDelay = 2f;
		fireDelay = 0.5f; //Might be needed, might not be needed?
		_bulletForce = 1000f;
		automatic = true; 

		//Animator stuff
		_playerAnim.SetBool("FullAuto_b", false);
		_playerAnim.SetInteger("WeaponType_int", 1);
		_playerAnim.SetFloat("Body_Horizontal_f", 0.25f);
		_playerAnim.SetFloat("Body_Vertical_f", 0.0f);
	
	}
    void OnEnable() //sets the animator back when you renable them
    {
        //Animator stuff
        _playerAnim.SetBool("FullAuto_b", false);
        _playerAnim.SetInteger("WeaponType_int", 1);
        _playerAnim.SetFloat("Body_Horizontal_f", 0.25f);
        _playerAnim.SetFloat("Body_Vertical_f", 0.0f);
    }

	protected override void PrimaryFire()
	{
		RaycastHit hit;
		//Basic firing stuff, should have NO recoil, but in case of recoil,
		Quaternion fireRot = Quaternion.LookRotation(transform.forward);
		Quaternion randomRot = Random.rotation;
		float currentSpread = bulletSpreadCurve.Evaluate(fireTime / timeTillMaxSpreadAngle) * maxBulletSpreadAngle; //Uses curves to determine accuracy while firing
		fireRot = Quaternion.RotateTowards(fireRot, randomRot, Random.Range(0.0f, currentSpread));

		if (Physics.SphereCast(transform.position,0.05f, fireRot * Vector3.forward, out hit, Mathf.Infinity))
		{
			HitInfo lexHit = new HitInfo();
			lexHit.damage = damage;
			lexHit.raycastHit = hit;
			lexHit.shooterPos = gameObject.transform.position;
			lexHit.bulletForce = _bulletForce;
            lexHit._forceMode = HitInfo.ForceType.NORMAL;
            //	hit.collider.gameObject.SendMessage("GunHitInfo", hitInfo, SendMessageOptions.DontRequireReceiver);
            if(hit.collider.gameObject.CompareTag("zombie"))
            {
          
                SpawnBlood(lexHit);
            }
            else
            {
   
                SpawnParticles(lexHit);
            }
            SpawnFakeBullet(lexHit);
		}
	}
    protected override void CheatCode()
    {
        maxAmmo =999;
        currentAmmo = maxAmmo;
        totalAmmo = 999;
        reloadDelay = 0.5f;
        fireDelay = 0.2f;
        _bulletForce = 2000f;
        damage = 20f;
    }
}
