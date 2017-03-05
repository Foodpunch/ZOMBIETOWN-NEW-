using UnityEngine;
using System.Collections;

public class RotationFix : MonoBehaviour {

    [SerializeField]
    Transform targetObj;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation = targetObj.rotation;
	}
}
