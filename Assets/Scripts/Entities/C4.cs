using System.Collections.Generic;
using UnityEngine;

class C4 : MonoBehaviour, ICacheableObject
{
    private readonly HashSet<EnemyStatus> _enemies = new();
    private float _remainingTime;
    private Transform _transform;
    private Config _config;
    private Animator _animator;

    public bool IsActive { get; private set; }
    public bool IsTriggered { get; private set; }

    void Awake()
    {
        _transform = transform;
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!IsActive || IsTriggered) return;

        if (_remainingTime > 0f)
        {
            _remainingTime -= Time.deltaTime;
            return;
        }

        IsTriggered = true;
        _animator.SetTrigger("Trigger");
        foreach (EnemyStatus enemy in _enemies)
        {
            enemy.ApplyDamage(_config.damage);
            enemy.ApplyKnockbackFrom(_transform.position, _config.knockbackForce);
            if (_config.isUpgraded)
                enemy.AddEffect(new StunnedEffect(_config.stunDuration));
        }
    }

    public void Init(IObjectConfig objConfig)
    {
        if (objConfig is Config config)
        {
            _config = config;
            _remainingTime = config.duration;
            IsActive = true;
        }
    }

    public void Destroy()
    {
        IsTriggered = false;
        IsActive = false;
        _remainingTime = 0f;
        _enemies.Clear();
    }

    public void Trigger() => _remainingTime = 0.0f;

    public void AnimationEventHandler(string animEvent)
    {
        if (animEvent == "Exploded")
            IsActive = false;
    }

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