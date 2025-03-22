using System;
using UnityEngine;

class WaveController : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;

    void Start()
    {
        var obj = Instantiate(_enemyPrefab);
    }

    void OnDeath(EnemyStatus enemy)
    {
        throw new NotImplementedException();
    }
}