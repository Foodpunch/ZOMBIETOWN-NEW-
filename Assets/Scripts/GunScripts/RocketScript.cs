using UnityEngine;
using System.Collections;

public class RocketScript : MonoBehaviour {

    [SerializeField] ParticleSystem _SmokeTrail; //sweet sweet smoke trail
    [SerializeField] MeshRenderer _RocketMesh;   //mesh of the rocket
    [SerializeField] Collider _RocketCollider;   //collider of the rocket
    [SerializeField] LayerMask _explosionLayer;  //layer for physics
    HitInfo RocketInfo;                          //hit info for rocket
    AudioSource _ExplosionAudio;                 //audio source for the kaboom

    void Start()
    {
        //typical grabbing of components
        _ExplosionAudio = GetComponent<AudioSource>();
        _SmokeTrail = _SmokeTrail.GetComponent<ParticleSystem>();
        _RocketMesh = _RocketMesh.GetComponent<MeshRenderer>();
        _RocketCollider = _RocketCollider.GetComponent<Collider>();
     

    }
    void OnEnable()  //for objectpooling
    {
        _SmokeTrail.Play();         //plays smoke trail when activated
        _RocketMesh.enabled = true;    //shows the rocket mesh
        _RocketCollider.enabled = true;    //enables the collider
    }

	void OnCollisionEnter() //if it hits something, anything
    {
        //   _Explosion.SetActive(true);
        Explode();              //trigger explosion
        GameObject explosionClone = ObjectPoolingScript.current.GetExplosion();  //gets explosion particle
        if(System.Object.ReferenceEquals(explosionClone,null)) //prevents null errors
        {
            return;
        }
        explosionClone.transform.position = transform.position; //sets explosion particle position
        explosionClone.SetActive(true);                        //enables it

        if(!_ExplosionAudio.isPlaying)       //if explosion sound is not playing
        {
            _ExplosionAudio.Play();         //then play explosion sound 
        }
        _SmokeTrail.Stop();                //stop the smoke trail because it's not moving
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;    //reset the velocity just in case
        _RocketMesh.enabled = false;                //hide the mesh since the rocket exploded
        _RocketCollider.enabled = false;            //also hide the collider
        
        Invoke("Hide", 3f);                         //hide everything after 3 seconds because that's about the same time as explosion
        
    }
    void Explode()  //function for explosion
    {
        Collider[] _Colliders = Physics.OverlapSphere(transform.position, 10, _explosionLayer);
        foreach(Collider col in _Colliders)
        {
            col.gameObject.SendMessage("Damage", RocketInfo, SendMessageOptions.DontRequireReceiver);
     //       col.gameObject.GetComponent<Rigidbody>().AddExplosionForce(RocketInfo.bulletForce, transform.position, 15, 2);
         //   col.gameObject.SendMessage("KnockBack", RocketInfo);
        }
    }
    public void SetDamageInfo(HitInfo _info)
    {
        RocketInfo = _info;
    }

    void Hide()
    {
        // _Explosion.SetActive(false);
        //   _SmokeTrail.SetActive(false);

        gameObject.transform.position = Vector3.zero;
        gameObject.transform.rotation = Quaternion.identity;
        gameObject.SetActive(false);

    }
    void OnDisable()
    {
        CancelInvoke("Hide");
    }
}
