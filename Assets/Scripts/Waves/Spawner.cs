using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.Random;

class Spawner : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private float radius = 5f;

    private Transform _transform;
    private ObjectCache _objCache;

    void Awake()
    {
        _transform = transform;
        _objCache = FindFirstObjectByType<ObjectCache>();
    }

    public EnemyStatus Spawn(ObjectType type)
    {
        EnemyStatus enemy = _objCache.GetObject<EnemyStatus>(type);
        enemy.Type = type;
        enemy.transform.position = RandomPosition();
        enemy.Target = _player;
        enemy.Init();
        return enemy;
    }

    // Generate a random position in the half circle infront of spawner
    private Vector3 RandomPosition()
    {
        float angle = Range(-math.PIHALF, math.PIHALF); // Generate a random angle between -PI/2 and PI/2.
        float r = Range(0.5f, radius); // Generate random radius between 0.5 and radius.
        
        float dx = r * math.cos(angle);
        float dy = r * math.sin(angle);
        
        return _transform.position + _transform.TransformDirection(new Vector3(dx, dy, 0f));
    }
}