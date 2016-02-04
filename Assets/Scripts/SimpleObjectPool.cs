using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleObjectPool : MonoBehaviour {

	public static SimpleObjectPool current;
	public GameObject _firstObj; //object you wanna pool
	//public GameObject _secondObj; //another object you wanna pool

	public int pooledAmount = 5;
	public bool willGrow; // allows the list to grow

	public List<GameObject> _objPool;
	//public List<GameObject> _secondObjPool; //pool for the second object



	void Awake()
	{
		current = this; //to make sure the static class that is instantiated is this.
	}


	// Use this for initialization
	void Start()
	{
		_objPool = new List<GameObject>();
	//	_secondObjPool = new List<GameObject>();
		for (int i = 0; i < pooledAmount; i++) //forloop to spawn the bullets
		{
			GameObject obj = (GameObject)Instantiate(_firstObj); //instantiates gameobjects and casts them as pooledObj
		//	GameObject obj2 = (GameObject)Instantiate(_secondObj); //instantiates fakebullets and casts them as pooledObj
			obj.SetActive(false); //sets active to false;
			_objPool.Add(obj); //adds to list
		//	obj2.SetActive(false); //sets active to false;
		//	_secondObjPool.Add(obj2);//adds to list
		}


	}

	public GameObject GetFirstObj() //creates a function for you to call too access the object pool
	{
		for (int i = 0; i < _objPool.Count; i++)
		{
			if (!_objPool[i].activeInHierarchy) //checks what is NOT active
			{
				return _objPool[i];
			}
		}
		if (willGrow) //if you allow the list to grow, spawn more things for it to use
		{
			GameObject obj = (GameObject)Instantiate(_firstObj);
			_objPool.Add(obj);
			return obj;
		}
		return null;
	}
	//public GameObject GetSecondObj() //same thing as above, but for bullets
	//{
	//	for (int j = 0; j < _secondObjPool.Count; j++)
	//	{
	//		if (!_secondObjPool[j].activeInHierarchy)
	//		{
	//			return _secondObjPool[j];
	//		}
	//	}
	//	if (willGrow)
	//	{
	//		GameObject obj2 = (GameObject)Instantiate(_secondObj);
	//		_secondObjPool.Add(obj2);
	//		return obj2;
	//	}
	//	return null;
	//}
}
