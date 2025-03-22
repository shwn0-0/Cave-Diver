using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector2 _walkingVelocity;
    private Vector2 _knockbackVelocity;
    private PlayerStatus _status;
    private Transform _transform;
    private readonly List<IAbility> _abilities = new();
    private readonly HashSet<EnemyStatus> _enemies = new();

    void Start()
    {
        _transform = transform;
        _status = GetComponent<PlayerStatus>();
    }

    void Update()
    {
        _abilities.ForEach(ability => ability.Update());
        HandleMovement();

        if (_status.IsDead)
            Die();
    }

    private void HandleMovement()
    {
        Vector2 displacement = (_knockbackVelocity + _walkingVelocity) * Time.deltaTime;
        _knockbackVelocity -= _knockbackVelocity * _status.KnockbackFriction; // static friction
        _transform.position += (Vector3)displacement;
    }

    public void Move(Vector2 dir) =>
        _walkingVelocity = dir.normalized * _status.MoveSpeed;

    public void UseAbility(int number)
    {
        if (number > _abilities.Count)
        {
            Debug.Log($"Ability {number} not unlocked.");
            return;
        }
        Debug.Log($"Using Ability {number}.");
        var ability = _abilities[number - 1];
        if (!ability.Activate())
        {
            Debug.Log($"{ability.Name} is on Cooldown.");
        }
    }

    public void UnlockAbility(IAbility ability) =>
        _abilities.Add(ability);

    public void UpgradeAbility(int number)
    {
        if (number > _abilities.Count)
        {
            Debug.Log($"Ability {number} not unlocked.");
            return;
        }
        Debug.Log($"Upgraded Ability {number}.");
        _abilities[number - 1].Upgrade();
    }

    public void Attack()
    {
        Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 currentPos = _transform.position;
        Vector2 targetDirection = targetPos - currentPos;
        _knockbackVelocity += _status.AttackSelfKnockforward * targetDirection.normalized;

        // Apply Damage and Knockback to enemies in range
        foreach (EnemyStatus enemy in _enemies)
        {
            Vector2 enemyDirection = (Vector2)enemy.transform.position - currentPos;
            if (enemyDirection.magnitude > _status.AttackRange || Vector2.Angle(targetDirection, enemyDirection) > _status.AttackAngle)
                continue;
            enemy.ApplyDamage(_status.AttackDamage);
            enemy.ApplyKnockbackFrom(currentPos, _status.AttackKnockback);
        }
    }

    private void Die()
    {
        gameObject.SetActive(false); // Set inactive when dead
    }

    public void ApplyKnockbackFrom(Vector2 position, float knockback)
    {
        if (knockback <= float.Epsilon) return;

        Vector2 currentPosition = _transform.position;
        Vector2 direction = currentPosition - position;
        float distance = direction.magnitude;
        float inverseSquaredDistance = math.min(1 / (distance * distance), 2);
        _knockbackVelocity += inverseSquaredDistance * knockback * direction.normalized;
    }

    public T GetAbility<T>() =>
        _abilities.Where(a => a is T).Select(a => (T)a).SingleOrDefault();

    // Maintain a set of nearby enemies
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            _enemies.Add(collision.GetComponent<EnemyStatus>());
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            _enemies.Remove(collider.GetComponent<EnemyStatus>());
        }
    }
}
