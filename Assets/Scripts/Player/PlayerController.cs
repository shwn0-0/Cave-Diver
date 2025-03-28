using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool _isHoldingAttack;
    private float _attackCooldown;
    private Animator _animator;
    private Vector2 _dir;
    private Vector2 _knockbackVelocity;
    private PlayerStatus _status;
    private Rigidbody2D _rb;
    private GameController _gameController;
    private readonly List<IAbility> _abilities = new();
    private readonly HashSet<EnemyStatus> _enemies = new();
    private readonly List<EnemyStatus> _enemiesToDamage = new();
    private Vector2 _targetDirection;

    public int AbilityCount => _abilities.Count;
    public Vector2 Position => _rb.position;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _gameController = FindFirstObjectByType<GameController>();
        _status = GetComponent<PlayerStatus>();
        _animator = GetComponent<Animator>();
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
        HandleMovement();
    }

    private void HandleMovement()
    {
        bool isRunning = _dir.magnitude > float.Epsilon;
        _animator.SetBool("IsRunning", isRunning);

        // Only update direction if we're actually moving
        if (isRunning)
        {
            _animator.SetFloat("dx", _dir.x);
            _animator.SetFloat("dy", _dir.y);
        }

        _rb.MovePosition(Position + (_knockbackVelocity + _dir * _status.MoveSpeed) * Time.fixedDeltaTime);
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
        Vector2 currentPos = Position;
        _targetDirection = (targetPos - currentPos).normalized;

        foreach (EnemyStatus enemy in _enemies)
        {
            Vector2 enemyDirection = enemy.Position - currentPos;
            if (enemyDirection.magnitude > _status.AttackRange || Vector2.Angle(_targetDirection, enemyDirection) > _status.AttackAngle)
                continue;
            
            _enemiesToDamage.Add(enemy);
        }

        _animator.SetFloat("aDx", _targetDirection.x);
        _animator.SetFloat("aDy", _targetDirection.y);
        // Set both directions in case player doesn't move after/during attack
        _animator.SetFloat("dx", _targetDirection.x);
        _animator.SetFloat("dy", _targetDirection.y);
        _animator.SetTrigger("Attack");
    }

    public void AnimationEventHandler(string animEvent)
    {
        if (animEvent == "Hit")
        {
            _knockbackVelocity += _status.AttackSelfKnockforward * _targetDirection;

            // Apply Damage and Knockback to enemies in range
            foreach (EnemyStatus enemy in _enemiesToDamage)
            {
                enemy.ApplyDamage(_status.AttackDamage);
                enemy.ApplyKnockbackFrom(Position, _status.AttackKnockback);
                if (_status.StunOnAttack)
                    enemy.AddEffect(new StunnedEffect(_status.StunDuration));
            }

            _enemiesToDamage.Clear();
        }
        else if (animEvent == "Die")
        {
            _gameController.OnDeath(_status);
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
        _animator.SetTrigger("Die");
        _status.IsControllable = false;
        _dir = Vector2.zero;
    }

    private void BeStunned()
    {
        _dir = Vector2.zero;
    }

    public void ApplyKnockbackFrom(Vector2 position, float knockback)
    {
        if (knockback <= float.Epsilon) return;
        Vector2 direction = Position - position;
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
