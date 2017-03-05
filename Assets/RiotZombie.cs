using UnityEngine;
using System.Collections;

public class RiotZombie : ZombieBase {
    [SerializeField] LayerMask _explosionLayer;
    [SerializeField] LayerMask _playerLayer;
    [SerializeField] AudioClip _kaboomSound;
    [SerializeField] GameObject bloodExplosion;
    HitInfo _info;
	// Use this for initialization
	protected override void Start () {
        base.Start();
        damage = 10;
        health = 50;
        HealthBar.maxValue = health;
        HealthBar.value = health;
        _info.raycastHit.point = transform.position;
        _info.damage = 30;
        _info.bulletForce = 1000f;
        _info._forceMode = HitInfo.ForceType.EXPLOSION;
     	
	}
    protected override void SpecialAttack()
    {
        ZombieSource.clip = _kaboomSound;
        ZombieSource.Play();
        bloodExplosion.SetActive(true);
        Collider[] _col = Physics.OverlapSphere(transform.position, 3, _explosionLayer);
        foreach(Collider col in _col)
        {
            col.gameObject.SendMessage("Damage", _info, SendMessageOptions.DontRequireReceiver);
        }
        Collider[] _col2 = Physics.OverlapSphere(transform.position, 5, _playerLayer);
        foreach (Collider col in _col2)
        {
            col.gameObject.SendMessage("Damage", damage, SendMessageOptions.DontRequireReceiver);
        }
        base.SpecialAttack();
    }
}
