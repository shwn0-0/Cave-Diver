using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class Lure : MonoBehaviour, ICacheableObject
{
    private readonly List<Transform> _enemies = new();

    private float _remainingTime;
    private bool _isActive = false;
    private bool _isUpgraded = false;
    private Transform _transform;

    public bool IsActive => _isActive;


    void Update()
    {
        if (!_isActive) return;

        if (_remainingTime > 0f)
        {
            _remainingTime -= Time.deltaTime;
            return;
        }

        _enemies.Clear();
        _isActive = false;
    }

    public void Init(IObjectConfig objConfig)
    {
        if (objConfig is Config config)
        {
            _remainingTime = config.duration;
            _isUpgraded = config.isUpgraded;
            _transform = transform;
            _isActive = true;
        }
        else
        {
            Debug.LogError("Passed Invalid Config to Lure");
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            EnemyStatus enemy = collider.GetComponent<EnemyStatus>();
            Debug.Log("Applying Lure to Enemy");

            Transform target;
            if (_isUpgraded && _enemies.Count >= 1)
            {
                Vector2 position = enemy.transform.position;
                target = _enemies
                    .OrderBy(other => Vector2.Distance(position, other.position))
                    .First();
            }
            else
            {
                target = _transform;
            }

            enemy.AddEffect(new LuredEffect(_remainingTime, target));
            _enemies.Add(enemy.transform);
        }
    }

    public readonly struct Config : IObjectConfig
    {
        public readonly float duration;
        public readonly bool isUpgraded;

        public Config(float duration, bool isUpgraded)
        {
            this.duration = duration;
            this.isUpgraded = isUpgraded;
        }
    }
}