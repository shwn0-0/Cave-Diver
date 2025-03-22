using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.Random;

class Spawner : MonoBehaviour
{
    [SerializeField] private float radius = 5f;

    private Transform _transform;
    private ObjectCache _objCache;
    private Transform _player;

    void Awake()
    {
        _transform = transform;
        _objCache = FindFirstObjectByType<ObjectCache>();
        _player = FindAnyObjectByType<PlayerStatus>().transform;
    }

    public EnemyStatus Spawn(ObjectType type) => 
        _objCache.GetObject<EnemyStatus>(type, RandomPosition(), new EnemyStatus.Config(_player, type));

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