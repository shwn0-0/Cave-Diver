using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool _isHoldingAttack;
    private float _attackCooldown;
    private int _attackCount;
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

        HandleDie();
        HandleStunned();
        HandleAttack();
        HandleCooldowns();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector2 walkingVelocity =  _status.IsControllable ? _dir * _status.MoveSpeed : Vector2.zero;
        bool isRunning = walkingVelocity.magnitude > float.Epsilon;
        _animator.SetBool("IsRunning", isRunning);

        // Only update direction if we're actually moving
        if (isRunning)
        {
            _animator.SetFloat("dx", _dir.x);
            _animator.SetFloat("dy", _dir.y);
        }

        _rb.MovePosition(Position + (_knockbackVelocity + walkingVelocity) * Time.fixedDeltaTime);
        _knockbackVelocity -= _knockbackVelocity * _status.KnockbackFriction; // static friction
    }

    private void HandleCooldowns()
    {
        if (_attackCooldown > 0f)
            _attackCooldown -= Time.deltaTime;
    }
    
    private void HandleAttack()
    {
        if (_status.IsDead || _status.IsStunned || !_isHoldingAttack || _attackCooldown > float.Epsilon) return;
        Debug.Log($"Attack Count: {_attackCount}");
        _attackCooldown += 1 / _status.AttackSpeed;

        Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 currentPos = Position;
        _targetDirection = (targetPos - currentPos).normalized;

        foreach (EnemyStatus enemy in _enemies)
        {
            Vector2 enemyDirection = enemy.Position - currentPos;
            if (enemyDirection.magnitude > _status.AttackRange || 
                (_attackCount < 2 && Vector2.Angle(_targetDirection, enemyDirection) > _status.AttackAngle))
                continue;
            
            _enemiesToDamage.Add(enemy);
        }

        _animator.SetFloat("aDx", _targetDirection.x);
        _animator.SetFloat("aDy", _targetDirection.y);
        // Set both directions in case player doesn't move after/during attack
        _animator.SetFloat("dx", _targetDirection.x);
        _animator.SetFloat("dy", _targetDirection.y);
        _animator.SetInteger("AttackCount", _attackCount);
        _animator.SetTrigger("Attack");
        _attackCount = (_attackCount + 1) % 3;
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
            _gameController.OnPlayerDeath();
        }
    }

    public void Move(Vector2 dir)
    {
        if (_status.IsControllable)
            _dir = dir;
    }

    public void UseAbility(int number)
    {
        if (!_status.IsControllable || _status.IsStunned || number > _abilities.Count) return;

        IAbility ability = _abilities[number - 1];
        if (!ability.Activate())
        {
            Debug.Log($"{ability.Name} is on Cooldown.");
        }
    }

    public void Attack(bool attack)
    {
        _isHoldingAttack = attack;
        _attackCount = 0; // reset attack count on attack or release hold
    } 

    private void HandleDie()
    {
        if (!_status.IsDead) return;
        _dir = Vector2.zero;
        _animator.SetTrigger("Die");
        _status.IsControllable = false;
    }

    private void HandleStunned()
    {
        _animator.SetBool("IsStunned", _status.IsStunned);
        if (_status.IsStunned)
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

    public void LookDown()
    {
        // Stop moving
        _dir = Vector2.zero;
        _knockbackVelocity = Vector2.zero;

        // Play down idle animation and make sure we don't go anywhere
        _animator.SetFloat("dx", 0f);
        _animator.SetFloat("dy", 0f);
        _animator.SetBool("IsRunning", false);
        _animator.SetBool("IsStunned", false);
        _animator.ResetTrigger("Die");
        _animator.ResetTrigger("Attack");
        _animator.Play("Idle.IdleDown");
    }

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
