using System.Collections.Generic;
using NUnit.Framework.Internal.Builders;
using UnityEngine;

enum ObjectType
{
    Lure,
    C4, Slime
}

class ObjectCache : MonoBehaviour
{
    [SerializeField] GameObject _lurePrefab;
    [SerializeField] GameObject _c4Prefab;
    [SerializeField] GameObject _slimePrefab;

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
        ObjectType.Slime => _slimePrefab,
        _ => null,
    };

    public T GetObject<T>(ObjectType type, Vector3 position, IObjectConfig objConfig) where T : ICacheableObject
    {
        Queue<GameObject> objectQueue = GetObjectQueue(type);
        GameObject gameObj = (objectQueue.Count == 0) ? Instantiate(GetPrefab(type)) : objectQueue.Dequeue();
        gameObj.SetActive(true);
        gameObj.transform.position = position;
        T obj = gameObj.GetComponent<T>();
        obj.Init(objConfig);
        return obj;
    }

    public void ReturnObject(ObjectType type, MonoBehaviour obj)
    {
        Queue<GameObject> objectQueue = GetObjectQueue(type);
        GameObject gameObj = obj.gameObject;
        gameObj.SetActive(false);
        objectQueue.Enqueue(gameObj);
    }
}