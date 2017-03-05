using UnityEngine;
using System.Collections;

public class HitParticleScript : MonoBehaviour {

    [SerializeField] int hideTime;

	//Script for particle 
	void OnEnable ()  //On Enable for the object pooling
	{
		Invoke("HideMe", hideTime); //calls the function to hide it in 1 second
    }
	void HideMe()
	{
		gameObject.SetActive(false); //hides object
	}
	void OnDisable()
	{
		CancelInvoke("HideMe"); //If disabled on it's own by pooling script, cancel the invoke lol.
	}
}
