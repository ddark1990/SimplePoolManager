using UnityEngine;

public class ScenePrefab : MonoBehaviour, IPooledObject
{
    public void OnObjectSpawn(GameObject obj)
    {
        UIController.Instance.PooledObjects.Add(obj);
    }

    public void OnObjectDespawn(GameObject obj)
    {
        UIController.Instance.PooledObjects.Remove(obj);
    }
}
