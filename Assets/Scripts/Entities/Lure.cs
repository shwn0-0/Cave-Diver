using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class Lure : MonoBehaviour
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

    public void Init(float duration, Vector3 position, bool isUpgraded)
    {
        _remainingTime = duration;
        _isUpgraded = isUpgraded;
        gameObject.SetActive(true);
        _transform = transform;
        _transform.position = position;
        _isActive = true;
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
}