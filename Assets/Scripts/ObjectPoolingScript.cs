using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPoolingScript : MonoBehaviour {

    public static ObjectPoolingScript current;

    public Dictionary<string, Queue<GameObject>> poolDictionary; //makes a dictionary pool with string as tag and "list" of Game OBj

 //   public GameObject _hitParticles; //object you wanna pool
	//public GameObject _fakeBullets; //another object I wanna pool
 //   public GameObject _fakeRockets; //Rockets I wanna pool
 //   public GameObject _explosionParticles; //Explosion I wanna pool
 //   public GameObject _bloodSpurt; //blood particles I wanna pool

    public int pooledAmount = 5;
   // public bool willGrow; // allows the list to grow

 //   public List<GameObject> particlesPool;
	//public List<GameObject> bulletPool;
 //   public List<GameObject> rocketPool;
 //   public List<GameObject> explosionPool;
 //   public List<GameObject> bloodPool;

    #region Singleton

    void Awake()
    {
        current = this; //to make sure the static class that is instantiated is this.
    }

    #endregion

    // Use this for initialization
[System.Serializable]
        public class Pool           //class for the pool, items should contain this info. Maybe use struct?
    {
        public string tag;                  //string for item tag
        public GameObject prefab;           //To store prefab
        public int size;                    //size to spawn
        public short despawnTimer;             //how long before despawn
       // public bool willGrow;               //if list is allowed to grow
    }
    public List<Pool> pools;

    void Start () {

        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            for (short v = 0; v < pool.size; v++)       //forloop to spawn pooled Objs
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);    //adds to queue

            }

            poolDictionary.Add(pool.tag, objectPool); //adds pool to dictionary
        }


        #region Obsolete


        //particlesPool = new List<GameObject>();
        //bulletPool = new List<GameObject>();
        //      rocketPool = new List<GameObject>();
        //      explosionPool = new List<GameObject>();
        //      bloodPool = new List<GameObject>();
        //     for (int i = 0; i < pooledAmount; i++) //forloop to spawn the bullets
        //     {
        //GameObject obj = (GameObject)Instantiate(_hitParticles); //instantiates gameobjects and casts them as pooledObj
        //GameObject obj2 = (GameObject)Instantiate(_fakeBullets); //instantiates fakebullets and casts them as pooledObj
        //         GameObject obj3 = (GameObject)Instantiate(_fakeRockets); //instantiates fakebullets and casts them as pooledObj
        //         GameObject obj4 = (GameObject)Instantiate(_explosionParticles); //instantiants explosion and casts them as pooledObj;
        //         GameObject obj5 = (GameObject)Instantiate(_bloodSpurt); //instantiates bloodspurts and casts them as pooledObj;
        //         obj5.SetActive(false);
        //         obj4.SetActive(false);
        //         obj3.SetActive(false); //sets active to false;
        //         obj2.SetActive(false); //sets active to false;
        //         obj.SetActive(false); //sets active to false;
        //particlesPool.Add(obj); //adds to list
        //bulletPool.Add(obj2);//adds to list
        //         rocketPool.Add(obj3);//adds to list
        //         explosionPool.Add(obj4);
        //         bloodPool.Add(obj5);
        //     }

        #endregion
    }

    public GameObject SpawnFromPool (string tag, Vector3 pos, Quaternion rot)
    {

        if(!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
            return null;
        }

        GameObject objToSpawn = poolDictionary[tag].Dequeue(); //pulls out first element from pool and spawns
        objToSpawn.SetActive(true);
        objToSpawn.transform.position = pos;
        objToSpawn.transform.rotation = rot;

        IPooledObj pooledObj =  objToSpawn.GetComponent<IPooledObj>();
        if(pooledObj != null)
        {
            pooledObj.OnObjSpawn();
        }
        poolDictionary[tag].Enqueue(objToSpawn); //adds back to queue
        return objToSpawn;
    }


    #region Obsoleted Code


    //    public GameObject GetParticles() //creates a function for you to call too access the object pool
    //    {
    //		for (int i = 0; i < particlesPool.Count; i++)
    //        {
    //			if (!particlesPool[i].activeInHierarchy) //checks what is NOT active
    //            {
    //				return particlesPool[i];
    //            }
    //        }
    //        if (willGrow) //if you allow the list to grow, spawn more things for it to use
    //        {
    //			GameObject obj = (GameObject)Instantiate(_hitParticles);
    //			particlesPool.Add(obj);
    //            return obj;
    //        }
    //        return null;
    //    }
    //	public GameObject GetBullets() //same thing as above, but for bullets
    //	{
    //		for (int j = 0; j < bulletPool.Count; j++)
    //		{
    //			if (!bulletPool[j].activeInHierarchy)
    //			{
    //				return bulletPool[j];
    //			}
    //		}
    //		if (willGrow)
    //		{
    //			GameObject obj2 = (GameObject)Instantiate(_fakeBullets);
    //			bulletPool.Add(obj2);
    //			return obj2;
    //		}
    //		return null;
    //	}
    //    public GameObject GetRockets()
    //    {
    //        for(int k = 0; k < rocketPool.Count; k++)
    //        {
    //            if(!rocketPool[k].activeInHierarchy)
    //            {
    //                return rocketPool[k];
    //            }
    //        }
    //        if(willGrow)
    //        {
    //            GameObject obj3 = (GameObject)Instantiate(_fakeRockets);
    //            rocketPool.Add(obj3);
    //            return obj3;
    //        }
    //        return null;
    //    }
    //    public GameObject GetExplosion()
    //    {
    //        for(int g = 0; g < explosionPool.Count; g++)
    //        {
    //            if(!explosionPool[g].activeInHierarchy)
    //            {
    //                return explosionPool[g];
    //            }
    //        }
    //        if(willGrow)
    //        {
    //            GameObject obj4 = (GameObject)Instantiate(_explosionParticles);
    //            explosionPool.Add(obj4);
    //            return obj4;
    //        }
    //        return null;
    //    }
    //    public GameObject GetBlood()
    //    {
    //        for(int h = 0; h < bloodPool.Count; h++)
    //        {
    //            if(!bloodPool[h].activeInHierarchy)
    //            {
    //                return bloodPool[h];
    //            }
    //            if (willGrow)
    //            {
    //                GameObject obj5 = (GameObject)Instantiate(_bloodSpurt);
    //                bloodPool.Add(obj5);
    //                return obj5;
    //            }
    //        }
    //        return null;
    //    }
    #endregion
    }


    //Interface for Obj that can be pooled. Implement for them to call the OnObjSpawn instead of using OnEnable.

    public interface IPooledObj
{
    void OnObjSpawn();
}