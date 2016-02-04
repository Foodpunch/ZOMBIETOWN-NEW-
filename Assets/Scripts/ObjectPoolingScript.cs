using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPoolingScript : MonoBehaviour {

    public static ObjectPoolingScript current;
    public GameObject _hitParticles; //object you wanna pool
	public GameObject _fakeBullets; //another object I wanna pool

    public int pooledAmount = 5;
    public bool willGrow; // allows the list to grow

    public List<GameObject> particlesPool;
	public List<GameObject> bulletPool;


    
    void Awake()
    {
        current = this; //to make sure the static class that is instantiated is this.
    }


	// Use this for initialization
	void Start () {
		particlesPool = new List<GameObject>();
		bulletPool = new List<GameObject>();
        for (int i = 0; i < pooledAmount; i++) //forloop to spawn the bullets
        {
			GameObject obj = (GameObject)Instantiate(_hitParticles); //instantiates gameobjects and casts them as pooledObj
			GameObject obj2 = (GameObject)Instantiate(_fakeBullets); //instantiates fakebullets and casts them as pooledObj
			obj2.SetActive(false); //sets active to false;
            obj.SetActive(false); //sets active to false;
			particlesPool.Add(obj); //adds to list
			bulletPool.Add(obj2);//adds to list
        }
		
	
	}

    public GameObject GetParticles() //creates a function for you to call too access the object pool
    {
		for (int i = 0; i < particlesPool.Count; i++)
        {
			if (!particlesPool[i].activeInHierarchy) //checks what is NOT active
            {
				return particlesPool[i];
            }
        }
        if (willGrow) //if you allow the list to grow, spawn more things for it to use
        {
			GameObject obj = (GameObject)Instantiate(_hitParticles);
			particlesPool.Add(obj);
            return obj;
        }
        return null;
    }
	public GameObject GetBullets() //same thing as above, but for bullets
	{
		for (int j = 0; j < bulletPool.Count; j++)
		{
			if (!bulletPool[j].activeInHierarchy)
			{
				return bulletPool[j];
			}
		}
		if (willGrow)
		{
			GameObject obj2 = (GameObject)Instantiate(_fakeBullets);
			bulletPool.Add(obj2);
			return obj2;
		}
		return null;
	}
	
}

