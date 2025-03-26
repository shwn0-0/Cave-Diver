using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.Random;

class Spawner : MonoBehaviour
{
    [SerializeField] private float _maxRadius = 7f;
    [SerializeField] private float _minRadius = 2f;
    [SerializeField] private float _angle = 45;
    [SerializeField] private float _spawnDelay = 1f;

    private Transform _transform;
    private ObjectCache _objCache;
    private Rigidbody2D _player;

    void Awake()
    {
        _transform = transform;
        _objCache = FindFirstObjectByType<ObjectCache>();
        _player = FindAnyObjectByType<PlayerStatus>().GetComponent<Rigidbody2D>();
    }

    public void Spawn(ObjectType type, int count, Action<EnemyStatus> OnSpawn) {
        if (count <= 0) return;
        StartCoroutine(HandleEnemySpawns(type, count, OnSpawn));
    }

    public IEnumerator HandleEnemySpawns(ObjectType type, int count, Action<EnemyStatus> OnSpawn)
    {
        for (int i = 0; i < count; i++)
        {
            EnemyStatus enemy = _objCache.GetObject<EnemyStatus>(type, RandomPosition(), new EnemyStatus.Config(_player, type));
            OnSpawn(enemy);
            yield return new WaitForSeconds(_spawnDelay);
        }
    }

    // Generate a random position in the half circle infront of spawner
    private Vector3 RandomPosition()
    {
        float angle = Range(-_angle, _angle);
        float r = Range(_minRadius, _maxRadius);

        float dx = r * math.cos(math.radians(angle));
        float dy = r * math.sin(math.radians(angle));
        
        return _transform.position + _transform.TransformDirection(new Vector3(dx, dy, 0f));
    }
}