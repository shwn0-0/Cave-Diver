using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool _isHoldingAttack;
    private float _attackCooldown;
    private Vector2 _walkingVelocity;
    private Vector2 _knockbackVelocity;
    private PlayerStatus _status;
    private Transform _transform;
    private readonly List<IAbility> _abilities = new();
    private readonly HashSet<EnemyStatus> _enemies = new();

    public int AbilityCount => _abilities.Count;

    void Start()
    {
        _transform = transform;
        _status = GetComponent<PlayerStatus>();
    }

    void Update()
    {
        if (!_status.IsControllable) return;

        if (_status.IsDead)
            Die();
        else if (_status.IsStunned)
            BeStunned();
        else
            HandleAttack();

        HandleMovement();
        HandleCooldowns();
    }

    private void HandleMovement()
    {
        Vector2 displacement = (_knockbackVelocity + _walkingVelocity) * Time.deltaTime;
        _knockbackVelocity -= _knockbackVelocity * _status.KnockbackFriction; // static friction
        _transform.position += (Vector3)displacement;
    }

    private void HandleCooldowns()
    {
        _abilities.ForEach(ability => ability.Update());
        if (_attackCooldown > 0f)
            _attackCooldown -= Time.deltaTime;
    }
    
    private void HandleAttack()
    {
        if (!_isHoldingAttack || _attackCooldown > float.Epsilon) return;
        _attackCooldown += 1 / _status.AttackSpeed;

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

    public void Move(Vector2 dir)
    {    
        if (_status.IsStunned) return;
        _walkingVelocity = dir.normalized * _status.MoveSpeed;
    }

    public void UseAbility(int number)
    {
        if (!_status.IsControllable || _status.IsStunned) return;
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

    public void Attack(bool attack) => _isHoldingAttack = attack;

    private void Die()
    {
        _status.IsControllable = false; // Stop player from doing anything if dead
    }

    private void BeStunned()
    {
        _walkingVelocity = Vector2.zero;
    }

    public void ApplyKnockbackFrom(Vector2 position, float knockback)
    {
        if (knockback <= float.Epsilon) return;

        Vector2 direction = (Vector2)_transform.position - position;
        float distance = direction.magnitude;
        float inverseSquaredDistance = math.min(1 / (distance * distance), 2);
        _knockbackVelocity += inverseSquaredDistance * knockback * direction.normalized;
    }

    public void UnlockAbility(IAbility ability) =>
        _abilities.Add(ability);

    public T GetAbility<T>() =>
        (T)_abilities.Where(a => a is T).SingleOrDefault();

    // Maintain a set of nearby enemies
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
            _enemies.Add(collision.GetComponent<EnemyStatus>());
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
            _enemies.Remove(collider.GetComponent<EnemyStatus>());
    }
}
