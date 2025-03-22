using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.Random;

class Spawner : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private float radius = 5f;

    Transform _transform;

    void Awake()
    {
        _transform = transform;
    }

    public IEnumerator Spawn(GameObject enemy, int number)
    {
        for (int i = 0; i < number; i++)
        {
            enemy.transform.position = RandomPosition();
            var enemyStatus = enemy.GetComponent<EnemyStatus>();
            enemyStatus.Target = _player;
            enemy.SetActive(true);
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