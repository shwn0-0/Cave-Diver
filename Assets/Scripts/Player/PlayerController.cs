using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool _isHoldingAttack;
    private float _attackCooldown;
    private Vector2 _dir;
    private Vector2 _knockbackVelocity;
    private PlayerStatus _status;
    private Rigidbody2D _rb;
    private GameController _gameController;
    private readonly List<IAbility> _abilities = new();
    private readonly HashSet<EnemyStatus> _enemies = new();

    public int AbilityCount => _abilities.Count;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _gameController = FindFirstObjectByType<GameController>();
        _status = GetComponent<PlayerStatus>();
    }

    void Update()
    {
        _abilities.ForEach(ability => ability.Update(_status.IsControllable));

        if (!_status.IsControllable) return;

        if (_status.IsDead)
            Die();
        else if (_status.IsStunned)
            BeStunned();
        else
            HandleAttack();

        HandleCooldowns();
    }

    void FixedUpdate()
    {
        if (!_status.IsControllable) return;
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector2 walkingVelocity = _dir * _status.MoveSpeed;
        _rb.MovePosition(_rb.position + (_knockbackVelocity + walkingVelocity) * Time.fixedDeltaTime);
        _knockbackVelocity -= _knockbackVelocity * _status.KnockbackFriction; // static friction
    }

    private void HandleCooldowns()
    {
        if (_attackCooldown > 0f)
            _attackCooldown -= Time.deltaTime;
    }
    
    private void HandleAttack()
    {
        if (!_isHoldingAttack || _attackCooldown > float.Epsilon) return;
        _attackCooldown += 1 / _status.AttackSpeed;

        Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 currentPos = _rb.position;
        Vector2 targetDirection = (targetPos - currentPos).normalized;
        _knockbackVelocity += _status.AttackSelfKnockforward * targetDirection;

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

    public void Move(Vector2 dir) => _dir = dir;

    public void UseAbility(int number)
    {
        if (!_status.IsControllable || _status.IsStunned || number > _abilities.Count) return;

        IAbility ability = _abilities[number - 1];
        if (!ability.Activate())
        {
            Debug.Log($"{ability.Name} is on Cooldown.");
        }
    }

    public void Attack(bool attack) => _isHoldingAttack = attack;

    private void Die()
    {
        // TODO: Play death animation and then call on death
        _gameController.OnDeath(_status);
    }

    private void BeStunned()
    {
        _dir = Vector2.zero;
    }

    public void ApplyKnockbackFrom(Vector2 position, float knockback)
    {
        if (knockback <= float.Epsilon) return;
        Vector2 direction = _rb.position - position;
        _knockbackVelocity += knockback * direction.normalized;
    }

    public void UnlockAbility(IAbility ability) =>
        _abilities.Add(ability);

    public T GetAbility<T>() where T : IAbility =>
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
