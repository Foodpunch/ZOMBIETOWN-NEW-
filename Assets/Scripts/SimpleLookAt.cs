using UnityEngine;
using System.Collections;

public class SimpleLookAt : MonoBehaviour {
	Transform target;

	// Use this for initialization
	void Start () {
		target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
		//gameObject.transform.LookAt(target);
		Vector3 targetPostition = new Vector3(target.position.x,this.transform.position.y,target.position.z);
		this.transform.LookAt(targetPostition);	
	}
}
