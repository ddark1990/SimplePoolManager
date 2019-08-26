using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePoolManager : MonoBehaviour
{
    public static SimplePoolManager Instance;

    public List<GameObject> PooledObjects;

    public List<ObjectPool> Pools;
    public Dictionary<string, Queue<GameObject>> PoolDictionary;

    public bool ExpandIfEmpty;

    public int ActiveObjects;
    public int InactiveObjects;
    public int TotalPooledObjects;

    private GameObject _poolParent;
    private GameObject _categoryPoolParent;
    private GameObject _objToSpawn;
     
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        SpawnPools();
        StartCoroutine(UpdateObjectCounts());
    }

    private IEnumerator UpdateObjectCounts()
    {
        while (true)
        {
            yield return new WaitForSeconds(.2f);
            UpdateCounts();
        }
    }

    private void SpawnPools()
    {
        PoolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (var item in Enum.GetValues(typeof(ObjectPool.PoolType))) 
        {
            var num = Convert.ToInt32(item);

            _categoryPoolParent = new GameObject(item + "Pools");
            _categoryPoolParent.transform.SetParent(this.transform);

            foreach (var pool in Pools)
            {
                if (pool.PoolSize == 0)
                {
                    Debug.LogWarning("You're pool size is 0");
                    return;
                }

                Queue<GameObject> objectPool = new Queue<GameObject>();

                if (num != (int) pool.poolType) continue;

                _poolParent = new GameObject(pool.PoolName + "Pool");
                _poolParent.transform.SetParent(_categoryPoolParent.transform);

                for (int i = 0; i < pool.PoolSize; i++)
                {
                    switch (pool.poolType)
                    {
                        case ObjectPool.PoolType.Object:
                            var obj = Instantiate(pool.ObjectToPool, new Vector3(0, 0, 0), Quaternion.identity);
                            objectPool.Enqueue(obj);
                            PooledObjects.Add(obj);
                            obj.transform.SetParent(_poolParent.transform);

                            obj.name = obj.name.Replace("(Clone)", ""); //gets rid of (Clone) on a newly instantiated object so it can pool correctly

                            obj.SetActive(false);
                            Debug.Log("Creating " + obj);
                            break;

                        default:
                            Debug.Log("Empty");
                            break;
                    }
                }

                PoolDictionary.Add(pool.ObjectToPool.name, objectPool);
                Debug.Log("SpawningPools");
            }
        }
    }

    /// <summary>
    /// Spawn an object from a pool based on a poolName to a specific position.
    /// Replace GameObject.Instantiate with this method.
    /// To get the dictionary with pool names &amp; objects use SimplePoolManager.Instance.PoolDictionary.ToList()
    /// </summary>
    /// <param name="poolName">Use the name of the object pooled as the name for the pool</param>
    /// <param name="pos">position of object</param>
    /// <param name="rot">rotation of object</param>
    public GameObject SpawnFromPool(string poolName, Vector3 pos, Quaternion rot)
    {
        if (PoolDictionary[poolName].Count.Equals(0) && !ExpandIfEmpty)
        {
            Debug.Log("Queue is empty, check the ExpandIfEmpty box to allow the pool to expand if more object are needed");
            return null;
        }

        if (PoolDictionary[poolName].Count.Equals(0) && ExpandIfEmpty)
        {
            var obj = Instantiate(_objToSpawn, pos, rot);
            PooledObjects.Add(obj);
            PoolDictionary[poolName].Enqueue(obj);

            obj.transform.SetParent(_poolParent.transform);
            obj.name = obj.name.Replace("(Clone)", ""); //gets rid of (Clone) on a newly instantiated object so it can pool correctly
        }

        _objToSpawn = PoolDictionary[poolName].Dequeue(); //takes it out of the queue, must be placed back in by calling ReturnToPool method

        //Debug.Log("SpawningFromPool: " + _objToSpawn + " | " + tag);

        _objToSpawn.SetActive(true);

        _objToSpawn.transform.position = pos;
        _objToSpawn.transform.rotation = rot;

        var pooledObject = GetPooledObject(_objToSpawn);
        pooledObject?.OnObjectSpawn(_objToSpawn); //sends the event to the interface

        return _objToSpawn;
    }

    /// <summary>
    /// Return the object back into the pool for later reuse. Replaces the GameObject.Destroy() method. To get the List of all pooled objects, use SimplePoolManager.Instance.PooledObjects
    /// </summary>
    /// <param name="obj">Object to return to pool</param>
    public void ReturnToPool(GameObject obj)
    {
        var pooledObject = GetPooledObject(obj);
        pooledObject?.OnObjectDespawn(obj);

        PoolDictionary[obj.name].Enqueue(obj); //queue's the object back into the pool

        obj.SetActive(false); 
    }

    private static IPooledObject GetPooledObject(GameObject obj)
    {
        return obj.GetComponent<IPooledObject>();
    }

    private void UpdateCounts()
    {
        List<object> tempActive = new List<object>();
        List<object> tempInactive = new List<object>();

        for (int i = 0; i < PooledObjects.Count; i++)
        {
            var obj = PooledObjects[i];

            if (obj.activeInHierarchy)
            {
                tempActive.Add(obj);
                tempInactive.Remove(obj);
            }
            else
            {
                tempInactive.Add(obj);
                tempActive.Remove(obj);
            }
        }

        ActiveObjects = tempActive.Count;
        InactiveObjects = tempInactive.Count;
        TotalPooledObjects = PooledObjects.Count;
    }

    [Serializable]
    public class ObjectPool
    {
        public enum PoolType { Object };
        public PoolType poolType;

        public string PoolName;
        public GameObject ObjectToPool;
        public int PoolSize;
    }
}
