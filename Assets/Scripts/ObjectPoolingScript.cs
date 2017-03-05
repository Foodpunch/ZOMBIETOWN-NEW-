using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPoolingScript : MonoBehaviour {

    public static ObjectPoolingScript current;
    public GameObject _hitParticles; //object you wanna pool
	public GameObject _fakeBullets; //another object I wanna pool
    public GameObject _fakeRockets; //Rockets I wanna pool
    public GameObject _explosionParticles; //Explosion I wanna pool
    public GameObject _bloodSpurt; //blood particles I wanna pool

    public int pooledAmount = 5;
    public bool willGrow; // allows the list to grow

    public List<GameObject> particlesPool;
	public List<GameObject> bulletPool;
    public List<GameObject> rocketPool;
    public List<GameObject> explosionPool;
    public List<GameObject> bloodPool;


    void Awake()
    {
        current = this; //to make sure the static class that is instantiated is this.
    }


	// Use this for initialization
	void Start () {
		particlesPool = new List<GameObject>();
		bulletPool = new List<GameObject>();
        rocketPool = new List<GameObject>();
        explosionPool = new List<GameObject>();
        bloodPool = new List<GameObject>();
        for (int i = 0; i < pooledAmount; i++) //forloop to spawn the bullets
        {
			GameObject obj = (GameObject)Instantiate(_hitParticles); //instantiates gameobjects and casts them as pooledObj
			GameObject obj2 = (GameObject)Instantiate(_fakeBullets); //instantiates fakebullets and casts them as pooledObj
            GameObject obj3 = (GameObject)Instantiate(_fakeRockets); //instantiates fakebullets and casts them as pooledObj
            GameObject obj4 = (GameObject)Instantiate(_explosionParticles); //instantiants explosion and casts them as pooledObj;
            GameObject obj5 = (GameObject)Instantiate(_bloodSpurt); //instantiates bloodspurts and casts them as pooledObj;
            obj5.SetActive(false);
            obj4.SetActive(false);
            obj3.SetActive(false); //sets active to false;
            obj2.SetActive(false); //sets active to false;
            obj.SetActive(false); //sets active to false;
			particlesPool.Add(obj); //adds to list
			bulletPool.Add(obj2);//adds to list
            rocketPool.Add(obj3);//adds to list
            explosionPool.Add(obj4);
            bloodPool.Add(obj5);
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
    public GameObject GetRockets()
    {
        for(int k = 0; k < rocketPool.Count; k++)
        {
            if(!rocketPool[k].activeInHierarchy)
            {
                return rocketPool[k];
            }
        }
        if(willGrow)
        {
            GameObject obj3 = (GameObject)Instantiate(_fakeRockets);
            rocketPool.Add(obj3);
            return obj3;
        }
        return null;
    }
    public GameObject GetExplosion()
    {
        for(int g = 0; g < explosionPool.Count; g++)
        {
            if(!explosionPool[g].activeInHierarchy)
            {
                return explosionPool[g];
            }
        }
        if(willGrow)
        {
            GameObject obj4 = (GameObject)Instantiate(_explosionParticles);
            explosionPool.Add(obj4);
            return obj4;
        }
        return null;
    }
    public GameObject GetBlood()
    {
        for(int h = 0; h < bloodPool.Count; h++)
        {
            if(!bloodPool[h].activeInHierarchy)
            {
                return bloodPool[h];
            }
            if (willGrow)
            {
                GameObject obj5 = (GameObject)Instantiate(_bloodSpurt);
                bloodPool.Add(obj5);
                return obj5;
            }
        }
        return null;
    }
	
}

