using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public static UIController Instance;

    public Transform SpawnPoint;
    public List<GameObject> PooledObjects;

    private SimplePoolManager _poolMan;

    private void Start()
    {
        Instance = this;
        _poolMan = SimplePoolManager.Instance;
    }

    public void SpawnObjectFromPool()
    {
        var target = _poolMan.PoolDictionary.ToList();

        var obj = SimplePoolManager.Instance.SpawnFromPool(target[0].Key, SpawnPoint.position, SpawnPoint.rotation);

        if (obj == null) return;
        var randomFloat = Random.Range(.1f, 10f);
        obj.GetComponent<Rigidbody>().AddForce(new Vector3(randomFloat, randomFloat, randomFloat));
    }

    public void ReturnToPool()
    {
        for (int i = 0; i < _poolMan.PooledObjects.Count; i++)
        {
            var pooledObj = _poolMan.PooledObjects[i];

            if(pooledObj.activeInHierarchy)
                SimplePoolManager.Instance.ReturnToPool(_poolMan.PooledObjects[i]);
        }
    }
}
