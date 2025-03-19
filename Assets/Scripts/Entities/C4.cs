using System.Collections.Generic;
using UnityEngine;

class C4 : MonoBehaviour
{
    [SerializeField] private float _duration = 2.0f;
    [SerializeField] private float _stunDuration = 2.0f;
    [SerializeField] private float _damage = 10.0f;
    [SerializeField] private float _knockbackForce = 1.0f;

    private readonly HashSet<EnemyStatus> _enemies = new();
    private bool _isActive;
    private bool _isUpgraded;
    private float _remainingTime;
    private Transform _transform;

    public bool IsActive => _isActive;

    void Start()
    {
        _transform = transform;
    }

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
            enemy.ApplyDamage(_damage);
            enemy.ApplyKnockbackFrom(_transform.position, _knockbackForce);
            if (_isUpgraded)
                enemy.AddEffect(new StunnedEffect(_stunDuration));
        }

        _enemies.Clear();
        _isActive = false;
        gameObject.SetActive(false);
    }

    public void Init(bool isUpgraded)
    {
        _remainingTime = _duration;
        _isActive = true;
        _isUpgraded = isUpgraded;
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
}