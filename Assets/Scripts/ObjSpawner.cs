using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjSpawner : MonoBehaviour {

    ObjectPoolingScript objPooler;

	// Use this for initialization
	void Start () {
        objPooler = ObjectPoolingScript.current; 
	}
	
	// Update is called once per frame
	void Update () {
        objPooler.SpawnFromPool("Hit Particles", transform.position, Quaternion.identity);
        objPooler.SpawnFromPool("Fake Bullets", transform.position, Quaternion.identity);
        objPooler.SpawnFromPool("Fake Rockets", transform.position, Quaternion.identity);
        objPooler.SpawnFromPool("Blood Spurt", transform.position, Quaternion.identity);


    }
}
