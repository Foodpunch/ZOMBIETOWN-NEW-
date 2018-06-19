using UnityEngine;
using System.Collections;

public class BulletParticleScript : MonoBehaviour,IPooledObj {

    [SerializeField]
    bool hasTrailRenderer;

    public void OnObjSpawn()
    {
        Invoke("ShowMe", 0.05f);
        Invoke("Hide", 1f);
    }
 //   void OnEnable()
	//{
	//	Invoke("ShowMe", 0.05f);
	//	Invoke("Hide", 1f);
	//}
	void Hide()
	{
		gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
		gameObject.transform.position = Vector3.zero;
		gameObject.transform.rotation = Quaternion.identity;
        if(hasTrailRenderer)
        {
            gameObject.GetComponent<TrailRenderer>().time = -1f;
        }
		
		gameObject.SetActive(false);

	}
	void ShowMe()
	{
		if(hasTrailRenderer)
        {
            gameObject.GetComponent<TrailRenderer>().time = 0.1f;
        }
	}
	void OnDisable()
	{
		CancelInvoke("Hide");
		CancelInvoke("ShowMe");
	}
}
