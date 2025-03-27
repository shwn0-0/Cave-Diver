using System.Collections.Generic;
using System.Collections;
using UnityEngine;

class WaveController : MonoBehaviour
{
    [SerializeField] WavesConfig _config;

    private Spawner[] _spawners;
    private ObjectCache _objCache;
    private readonly HashSet<EnemyStatus> _enemies = new();
    private GameController _gameController;
    private int _waveNumber;

    public int CurrentWave => _waveNumber;

    void Awake()
    {
        _spawners = FindObjectsByType<Spawner>(FindObjectsSortMode.None);
        _objCache = FindFirstObjectByType<ObjectCache>();
        _gameController = GetComponent<GameController>();
    }

    public void NextWave()
    {
        _waveNumber += 1;
        var (num_slimes, num_skeletons, num_orcs) = _config.NumEnemies(_waveNumber);

        for (int i = 0; i < _spawners.Length; i++)
        {
            _spawners[i].Spawn(ObjectType.Orc, Split(num_orcs, _spawners.Length, i), OnSpawn);
            _spawners[i].Spawn(ObjectType.Skeleton, Split(num_skeletons, _spawners.Length, i), OnSpawn);
            _spawners[i].Spawn(ObjectType.Slime, Split(num_slimes, _spawners.Length, i), OnSpawn);
        }
    }

    private void OnSpawn(EnemyStatus enemy)
    {
        if (_gameController.IsDemoMode)
            enemy.AddEffect(new DemoEffect());
        _enemies.Add(enemy);
    }

    private int Split(int total, int n, int curr)
    {
        int amount = total / n;
        int overlap = total % n;
        return amount + ((curr < overlap) ? 1 : 0);
    }

    public void OnDeath(EnemyStatus enemy)
    {
        _enemies.Remove(enemy);
        _objCache.ReturnObject(enemy.Type, enemy);

        if (_enemies.Count == 0)
            _gameController.OnWaveEnd();
    }
}