using UnityEngine;
using System.Collections;


public class VectisScript : WeaponBase {
    /* Weapon Description
     * - Semi Automatic?
     * - High Damage, PENETRATING shot
     * - 1/25? or 5/25?
     * - Almost like shotgun? Or almost like pistol? (no recoil/MAX RECOIL)
     */

    //In case of recoil
    float timeTillMaxSpreadAngle = 3f;
    float maxBulletSpreadAngle = 30f;
    float currFOV;
    [SerializeField] LayerMask sniperMask;
    [SerializeField] AnimationCurve bulletSpreadCurve;
    [SerializeField] OVRCameraController FOVComponent;
	protected override void Start () {
        base.Start();
        damage = 80f;
        maxAmmo = 5;
        totalAmmo = 25;
        currentAmmo = maxAmmo;
        reloadDelay = 1.5f;
        fireDelay = 2.5f;
        _bulletForce = 5000f;
        automatic = true; //semi auto but long ass firedelay;
        FOVComponent = FOVComponent.GetComponent<OVRCameraController>();
        currFOV = FOVComponent.verticalFOV;

        //Animator stuff 
        _playerAnim.SetBool("FullAuto_b", false);
        _playerAnim.SetInteger("WeaponType_int", 5);
        _playerAnim.SetFloat("Body_Horizontal_f", 0.6f);
        _playerAnim.SetFloat("Body_Vertical_f", 0.0f);
    }

    void OnEnable() //sets the animator back when you renable them
    {
        //Animator stuff
        _playerAnim.SetBool("FullAuto_b", false);
        _playerAnim.SetInteger("WeaponType_int", 5);
        _playerAnim.SetFloat("Body_Horizontal_f", 0.6f);
        _playerAnim.SetFloat("Body_Vertical_f", 0.0f);
    }

    protected override void PrimaryFire()
    {
        // throw new NotImplementedException();
        RaycastHit[] hits;
        Quaternion fireRot = Quaternion.LookRotation(transform.forward);
        Quaternion randomRot = Random.rotation;
        float currentSpread = bulletSpreadCurve.Evaluate(fireTime / timeTillMaxSpreadAngle) * maxBulletSpreadAngle; //Uses curves to determine accuracy while firing
        fireRot = Quaternion.RotateTowards(fireRot, randomRot, Random.Range(0.0f, currentSpread));

        hits = Physics.SphereCastAll(transform.position, 0.05f, fireRot * Vector3.forward, Mathf.Infinity, sniperMask);
        foreach(RaycastHit _hit in hits)
        {
            HitInfo hitInfo = new HitInfo();
            hitInfo.damage = damage;
            hitInfo.raycastHit = _hit;
            hitInfo.shooterPos = gameObject.transform.position;
            hitInfo.bulletForce = _bulletForce;
            hitInfo._forceMode = HitInfo.ForceType.NORMAL;

            if(_hit.collider.gameObject.CompareTag("zombie"))
            {
                SpawnBlood(hitInfo);
            }
            else
            {
                SpawnParticles(hitInfo);
            }
            SpawnFakeBullet(hitInfo);
        }
    }
    protected override void CheckInput()
    {
        base.CheckInput();
        float secondAxis = GamepadManager.triggerL;
        if (OVRGamepadController.GPC_GetAxis((int)OVRGamepadController.Axis.LeftTrigger) > 0)
        {
            FOVComponent.verticalFOV = Mathf.Lerp(currFOV,30f,secondAxis);
        }
        else
        {
            FOVComponent.verticalFOV = currFOV;
        }
    }

    protected override void CheatCode()
    {
        maxAmmo = 99;
        currentAmmo = maxAmmo;
        totalAmmo = 999;
        reloadDelay = 0.5f;
        fireDelay = 0.3f;
        _bulletForce = 10000f;
        damage = 200f;
    }



}
