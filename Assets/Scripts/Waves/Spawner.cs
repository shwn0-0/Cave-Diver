using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.Random;

class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private Transform _player;
    [SerializeField] private float radius = 5f;

    Transform _transform;

    void Awake()
    {
        _transform = transform;
    }

    void Start()
    {
        StartCoroutine(Spawn(5));
    }

    public IEnumerator Spawn(int number)
    {
        for (int i = 0; i < number; i++)
        {
            var obj = Instantiate(_enemyPrefab, RandomPosition(), quaternion.identity);
            var enemy = obj.GetComponent<EnemyStatus>();
            enemy.Target = _player;
            yield return new WaitForSeconds(Range(0.5f, 1f)); // Delay between spawns
        }
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