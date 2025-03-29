using System.Collections.Generic;
using NUnit.Framework.Internal.Builders;
using UnityEngine;

enum ObjectType
{
    Lure,
    C4, 
    Slime,
    Orc,
    Troll
}

class ObjectCache : MonoBehaviour
{
    [SerializeField] GameObject _lurePrefab;
    [SerializeField] GameObject _c4Prefab;
    [SerializeField] GameObject _slimePrefab;
    [SerializeField] GameObject _orcPrefab;
    [SerializeField] GameObject _trollPrefab;

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
        ObjectType.Orc => _orcPrefab,
        ObjectType.Troll => _trollPrefab,
        _ => null,
    };

    public T GetObject<T>(ObjectType type, Vector3 position, IObjectConfig objConfig) where T : ICacheableObject
    {
        Queue<GameObject> objectQueue = GetObjectQueue(type);
        GameObject gameObj;
        if (objectQueue.Count == 0) 
        {
            gameObj = Instantiate(GetPrefab(type), position, Quaternion.identity);
        } 
        else
        {
            gameObj = objectQueue.Dequeue();
            gameObj.transform.position = position;
        }

        gameObj.SetActive(true);
        T obj = gameObj.GetComponent<T>();
        obj.Init(objConfig);
        return obj;
    }

    public void ReturnObject<T>(ObjectType type, T obj) where T : MonoBehaviour, ICacheableObject
    {
        Queue<GameObject> objectQueue = GetObjectQueue(type);
        GameObject gameObj = obj.gameObject;
        obj.Destroy();
        gameObj.SetActive(false);
        objectQueue.Enqueue(gameObj);
    }
}