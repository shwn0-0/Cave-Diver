using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class Lure : MonoBehaviour
{
    private float _remainingTime;
    private bool _isActive = false;
    private bool _isUpgraded = false;
    private readonly List<Transform> _enemies = new();

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
        gameObject.SetActive(false);
    }

    public void Init(float duration, bool isUpgraded)
    {
        _remainingTime = duration;
        _isActive = true;
        _isUpgraded = isUpgraded;
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            EnemyStatus enemy = collider.GetComponent<EnemyStatus>();
            Debug.Log("Applying Lure to Enemy");

            Transform target = transform;
            if (_isUpgraded && _enemies.Count >= 1)
            {
                Vector2 position = enemy.transform.position;
                target = _enemies
                    .OrderBy(other => Vector2.Distance(position, other.position))
                    .First();
            }

            enemy.AddEffect(new LuredEffect(_remainingTime, target));
            _enemies.Add(enemy.transform);
        }
    }
}