using System;
using System.Collections.Generic;
using UnityEngine;

enum ObjectType
{
    Lure,
    C4
}

class ObjectCache : MonoBehaviour
{
    [SerializeField] GameObject _lurePrefab;
    [SerializeField] GameObject _c4Prefab;

    private readonly Dictionary<ObjectType, Queue<GameObject>> _objectQueues = new();

    private Queue<GameObject> GetObjectQueue(ObjectType type)
    {
        Queue<GameObject> objectQueue;
        if (!_objectQueues.ContainsKey(type))
            objectQueue = _objectQueues[type] = new();
        else
            objectQueue = _objectQueues[type];
        return objectQueue;
    }

    private GameObject GetPrefab(ObjectType type) => type switch
    {
        ObjectType.Lure => _lurePrefab,
        ObjectType.C4 => _c4Prefab,
        _ => null,
    };

    public T GetObject<T>(ObjectType type) where T : MonoBehaviour
    {
        Queue<GameObject> objectQueue = GetObjectQueue(type);

        GameObject obj = (objectQueue.Count == 0) ?
            Instantiate(GetPrefab(type)) :
            objectQueue.Dequeue();

        return obj.GetComponent<T>();
    }

    public void ReturnObject(ObjectType type, MonoBehaviour obj)
    {
        Queue<GameObject> objectQueue = GetObjectQueue(type);
        GameObject gameObj = obj.gameObject;
        gameObj.SetActive(false);
        objectQueue.Enqueue(gameObj);
    }
}