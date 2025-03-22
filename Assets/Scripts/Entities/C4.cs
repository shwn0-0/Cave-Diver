using System.Collections.Generic;
using UnityEngine;

class C4 : MonoBehaviour, ICacheableObject
{
    private readonly HashSet<EnemyStatus> _enemies = new();
    private bool _isActive;
    private float _remainingTime;
    private Transform _transform;
    private Config _config;

    public bool IsActive => _isActive;

    void Update()
    {
        if (!_isActive) return;

        if (_remainingTime > 0f)
        {
            _remainingTime -= Time.deltaTime;
            return;
        }


        foreach (EnemyStatus enemy in _enemies)
        {
            enemy.ApplyDamage(_config.damage);
            enemy.ApplyKnockbackFrom(_transform.position, _config.knockbackForce);
            if (_config.isUpgraded)
                enemy.AddEffect(new StunnedEffect(_config.stunDuration));
        }

        _enemies.Clear();
        _isActive = false;
    }

    public void Init(IObjectConfig objConfig)
    {
        if (objConfig is Config config)
        {
            _config = config;
            _remainingTime = config.duration;
            _transform = transform;
            _isActive = true;
        }

    }

    public void Trigger() => _remainingTime = 0.0f;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            EnemyStatus enemy = collider.GetComponent<EnemyStatus>();
            _enemies.Add(enemy);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            EnemyStatus enemy = collider.GetComponent<EnemyStatus>();
            _enemies.Remove(enemy);
        }
    }

    public readonly struct Config : IObjectConfig
    {
        public readonly float damage;
        public readonly float duration;
        public readonly float knockbackForce;
        public readonly float stunDuration;
        public readonly bool isUpgraded;

        public Config(float damage, float duration, float knockbackForce, float stunDuration, bool isUpgraded)
        {
            this.duration = duration;
            this.damage = damage;
            this.knockbackForce = knockbackForce;
            this.stunDuration = stunDuration;
            this.isUpgraded = isUpgraded;
        }
    }
}