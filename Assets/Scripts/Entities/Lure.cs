using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class Lure : MonoBehaviour, ICacheableObject
{
    private readonly List<Rigidbody2D> _enemies = new();

    private float _remainingTime;
    private bool _isUpgraded = false;
    private Rigidbody2D _rb;

    public bool IsActive { get; private set; }

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!IsActive) return;

        if (_remainingTime > 0f)
        {
            _remainingTime -= Time.deltaTime;
            return;
        }

        IsActive = false;
    }

    public void Init(IObjectConfig objConfig)
    {
        if (objConfig is Config config)
        {
            _remainingTime = config.duration;
            _isUpgraded = config.isUpgraded;
            IsActive = true;
        }
        else
        {
            Debug.LogError("Passed Invalid Config to Lure");
        }
    }

    public void Destroy()
    {
        IsActive = false;
        _remainingTime = 0f;
        _enemies.Clear();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            EnemyStatus enemy = collider.GetComponent<EnemyStatus>();

            Rigidbody2D target;
            if (_isUpgraded && _enemies.Count >= 1)
            {
                Vector2 position = enemy.transform.position;
                target = _enemies
                    .OrderBy(other => Vector2.Distance(position, other.position))
                    .First();
            }
            else
            {
                target = _rb;
            }

            enemy.AddEffect(new LuredEffect(_remainingTime, target));
            _enemies.Add(enemy.GetComponent<Rigidbody2D>());
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