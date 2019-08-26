using UnityEngine;

public interface IPooledObject
{
    //Must be called from the object that is being spawned, in any script,
    //just add the IPooledObject interface and you're good to go
    void OnObjectSpawn(GameObject obj);
    void OnObjectDespawn(GameObject obj);
}
