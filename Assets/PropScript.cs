using UnityEngine;
using System.Collections;

public class PropScript : MonoBehaviour,IDamagable
{
    //maybe implement some health for the props
    //so they can be destroyed after a while 
    //e.g exploding barrels that catch on fire
    //after X health, then it reduces it by time.delta time
    //when in countdown mode then KABOOM
    //maybe make a prop script, so zombie props can be child


    Rigidbody _rb;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void Damage(HitInfo _info)
    {
        switch (_info._forceMode)
        {
            case HitInfo.ForceType.NORMAL:
                _rb.AddForce((_info.raycastHit.point - _info.shooterPos).normalized * _info.bulletForce); //adds force in correct direction
                break;
            case HitInfo.ForceType.EXPLOSION:
                _rb.AddExplosionForce(_info.bulletForce, _info.raycastHit.point, 10f, 2f); // if it's an explosion
                break;
        }
       
    }
}
