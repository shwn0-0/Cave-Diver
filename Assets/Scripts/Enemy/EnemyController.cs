using UnityEngine;

class EnemyController : MonoBehaviour
{
    private Animator _animator;
    private Rigidbody2D _rb;
    private EnemyStatus _status;
    private WaveController _waveController;

    private bool _attacking;
    private float _attackCooldown;
    private float _attackCheckCooldown;
    private bool _checked;
    private State _currentState;
    private Vector2 _knockbackVelocity;
    private Vector2 _walkingVelocity;

    // Avoid frequent state flips by staying in AttackState for a few milliseconds 
    // after changing from IdleState if attack is still on cooldown.
    // It would be better to stay in Idle until we can attack but I don't feel like
    // going through the effort of updating the State Machine now.
    // attacking bool ensures we don't exit before the animation ends
    // checked bool ensures we try to attack at least once before exiting
    public bool FinishedAttacking => !_attacking && _checked && _attackCheckCooldown <= 0f;
    public bool IsStunned => _status.IsStunned;
    public bool IsDead => _status.IsDead;
    public bool IsTargetInRange =>
        _status.Target != null // have a target
        && (_status.TargetStatus == null || !_status.TargetStatus.IsDead) // target can't die (Lure) or is not dead (Player)
        && Vector2.Distance(Position, _status.Target.position) <= _status.AttackRange; // target in attack range
    public Vector2 Position => _rb.position;
    private Vector2 TargetDirection => (_status.Target.position - Position).normalized;

    void Awake()
    {
        _waveController = FindFirstObjectByType<WaveController>();
        _rb = GetComponent<Rigidbody2D>();
        _status = GetComponent<EnemyStatus>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!_status.IsControllable) return;
        HandleCooldowns();

        var direction = TargetDirection;
        _animator.SetFloat("dx", direction.x);
        _animator.SetFloat("dy", direction.y);
        _currentState.Run(this);
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleCooldowns()
    {
        if (_attackCooldown > 0f)
            _attackCooldown -= Time.deltaTime;

        if (_attackCheckCooldown > 0f)
            _attackCheckCooldown -= Time.deltaTime;
    }

    private void HandleMovement()
    {
        _rb.MovePosition(Position + (_walkingVelocity + _knockbackVelocity) * Time.fixedDeltaTime);
        _knockbackVelocity -= _knockbackVelocity * _status.KnockbackFriction; // static friction applied to knockback only
    }

    public void ApplyKnockbackFrom(Vector2 position, float knockback)
    {
        if (knockback <= float.Epsilon) return;
        Vector2 direction = Position - position;
        _knockbackVelocity = knockback * direction.normalized;
    }

    public void Reset()
    {
        _attackCheckCooldown = 0f;
        _attackCooldown = 0f;
        _currentState = State.Idle;
        _knockbackVelocity = Vector2.zero;
        _walkingVelocity = Vector2.zero;
    }

    public void ChangeStates(State newState)
    {
        if (newState is not RunningState)
        {
            _animator.SetBool("IsRunning", false);
            _walkingVelocity = Vector2.zero;
        }

        _animator.SetBool("IsStunned", _currentState is StunnedState);

        // Only reset checked after leaving attack state
        if (_currentState is AttackState && newState is IdleState)
            _checked = false;

        _currentState = newState;
    }

    public void BeRunning()
    {
        // stop moving if the target is dead
        if (_status.TargetStatus != null && _status.TargetStatus.IsDead)
        {
            _walkingVelocity = Vector2.zero;
            _animator.SetBool("IsRunning", false);
        }
        else
        {
            _walkingVelocity = _status.MoveSpeed * TargetDirection;
            _animator.SetBool("IsRunning", true);
        }
    }

    public void BeStunned() { }

    public void BeAttacking()
    {
        if (_attackCheckCooldown > 0f) return;
        _checked = true;
        _attackCheckCooldown = 0.2f; // stay in attack state for 0.2 seconds before we're allowed to leave

        if (_attackCooldown > 0f) return;
        _attackCooldown = 1 / _status.AttackSpeed;

        if (_status.TargetStatus == null || !_status.TargetStatus.IsDead)
        {
            _animator.SetTrigger("Attack");
            _attacking = true;
        }
    }

    public void Die()
    {
        _status.IsControllable = false;
        _animator.SetTrigger("Die");
    }

    public void AnimationEventHandler(string animEvent)
    {
        if (animEvent == "Hit")
        {
            _attacking = false;
            _knockbackVelocity += _status.AttackSelfKnockforward * TargetDirection;

            if (_status.TargetStatus != null && !_status.TargetStatus.IsDead && IsTargetInRange)
            {
                _status.TargetStatus.ApplyDamage(_status.AttackDamage);
                _status.TargetStatus.ApplyKnockbackFrom(Position, _status.AttackKnockback);
                if (_status.StunOnAttack)
                    _status.TargetStatus.AddEffect(new StunnedEffect(_status.StunDuration));
            }

            if (_status.DieOnAttack)
            {
                _status.Health = 0f;
            }
        }
        else if (animEvent == "Die")
        {
            _waveController.OnEnemyDeath(_status);
        }
    }
}