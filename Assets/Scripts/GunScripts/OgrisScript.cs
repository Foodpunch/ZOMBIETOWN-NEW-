using UnityEngine;
using System.Collections;
using System;

public class OgrisScript : WeaponBase {

    /*Weapon Description
    * - Low Fire Rate
    * - High AOE damage
    * - Low Ammo Capacity maybe 1/9
    * - Semi - Auto like the shotgun
    * - Explosion Force of 5000
    */

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        damage = 100;
        maxAmmo = 4;
        totalAmmo = 20;
        currentAmmo = maxAmmo;
        reloadDelay = 2.5f;
        fireDelay = 1.5f;
        _bulletForce = 5000;
        automatic = true;

        //Weapon anim stuff
        _playerAnim.SetBool("FullAuto_b", false);
        _playerAnim.SetInteger("WeaponType_int", 8);
        _playerAnim.SetFloat("Body_Horizontal_f", 0.6f);
        _playerAnim.SetFloat("Body_Vertical_f", 0.0f);
    }
    void OnEnable() //sets the animator back when you renable them
    {
        //Weapon anim stuff
        _playerAnim.SetBool("FullAuto_b", false);
        _playerAnim.SetInteger("WeaponType_int", 8);
        _playerAnim.SetFloat("Body_Horizontal_f", 0.6f);
        _playerAnim.SetFloat("Body_Vertical_f", 0.0f);
    }
    protected override void PrimaryFire()
    {
        RaycastHit pointWhereRocketHits;
        Quaternion fireRot = Quaternion.LookRotation(transform.forward);

        if(Physics.SphereCast(transform.position,0.05f,fireRot * Vector3.forward,out pointWhereRocketHits,Mathf.Infinity))
        {
            //POINT WHERE THE ROCKET SHOULD HIT//
            HitInfo hitInfo = new HitInfo();
            hitInfo.damage = damage;
            hitInfo.raycastHit = pointWhereRocketHits;
            hitInfo.bulletForce = _bulletForce;
            hitInfo._forceMode = HitInfo.ForceType.EXPLOSION;
            hitInfo.shooterPos = gameObject.transform.position;
            SpawnFakeBullet(hitInfo);

        }
    }
    protected override void SpawnFakeBullet(HitInfo _info)
    {
        GameObject fakeRocketClone = objPooler.SpawnFromPool("Rocket", shootPoint.position, Quaternion.LookRotation(_info.raycastHit.point-transform.position)); //gets bullets
        if (System.Object.ReferenceEquals(fakeRocketClone, null))
        {
            return;
        }
        //sets bullet's transform and target direction
     //   fakeRocketClone.transform.localPosition = shootPoint.position;
      //  fakeRocketClone.transform.LookAt(_info.raycastHit.point); //makes it face where it should hit
        fakeRocketClone.SetActive(true);                        //sets it to true so you can see it
        fakeRocketClone.GetComponent<Rigidbody>().AddForce(fakeRocketClone.transform.forward * 25f, ForceMode.Impulse); //applies force to shoot it
        fakeRocketClone.GetComponent<RocketScript>().SetDamageInfo(_info);
    }
    protected override void CheatCode()
    {
        maxAmmo = 99;
        currentAmmo = maxAmmo;
        totalAmmo = 999;
        reloadDelay = 1.5f;
        fireDelay = 0.5f;
    }
}
