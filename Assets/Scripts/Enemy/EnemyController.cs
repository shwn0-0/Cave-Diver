using UnityEngine;

class EnemyController : MonoBehaviour
{
    private Animator _animator;
    private float _attackCooldown;
    private bool _attacked;
    private State _currentState;
    private Vector2 _knockbackVelocity;
    private Rigidbody2D _rb;
    private EnemyStatus _status;
    private Vector2 _walkingVelocity;
    private WaveController _waveController;

    public Vector2 Position => _rb.position;
    public bool IsStunned => _status.IsStunned;
    public bool IsDead => _status.IsDead;
    public bool IsTargetInRange =>
        _status.Target != null 
        && (_status.TargetStatus == null || !_status.TargetStatus.IsDead)
        && Vector2.Distance(Position, _status.Target.position) <= _status.AttackRange;

    // Avoid frequent state flips by staying in AttackState until after Attack is off cooldown
    // It would be better to stay in Idle until we can attack but I don't feel like
    // going through the effort of updating the State Machine now. Maybe enemies just get tired
    // after trying to attack or something idk.
    public bool FinishedAttacking => _attacked && _attackCooldown <= 0f;
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

    private void HandleMovement()
    {
        _rb.MovePosition(Position + (_walkingVelocity + _knockbackVelocity) * Time.fixedDeltaTime);
        _knockbackVelocity -= _knockbackVelocity * _status.KnockbackFriction; // static friction applied to knockback only
    }

    private void HandleCooldowns()
    {
        if (_attackCooldown > 0f)
            _attackCooldown -= Time.deltaTime;
    }

    public void ChangeStates(State newState)
    {
        if (newState is not RunningState)
        {
            _animator.SetBool("IsRunning", false);
            _walkingVelocity = Vector2.zero;
        }

        if (newState is not StunnedState)
            _animator.SetBool("IsStunned", false);

        // Reset attacked once we transition out of Attack state
        if (_currentState is AttackState && newState is IdleState)
            _attacked = false;

        _currentState = newState;
    }

    public void BeRunning()
    {
        _walkingVelocity = _status.MoveSpeed * TargetDirection;
        _animator.SetBool("IsRunning", true);
    }

    public void BeStunned()
    {
        _animator.SetBool("IsStunned", true);
    }

    public void BeAttacking()
    {
        if (_attackCooldown > 0f) return;
        _attackCooldown = 1 / _status.AttackSpeed;
        _attacked = true;

        if (_status.TargetStatus == null || !_status.TargetStatus.IsDead)
            _animator.SetTrigger("Attack");
    }

    public void AnimationEventHandler(string animEvent)
    {
        if (animEvent == "Hit")
        {
            _knockbackVelocity = _status.AttackSelfKnockforward * TargetDirection;

            if (_status.TargetStatus != null && !_status.TargetStatus.IsDead && IsTargetInRange)
            {
                _status.TargetStatus.ApplyDamage(_status.AttackDamage);
                _status.TargetStatus.ApplyKnockbackFrom(Position, _status.AttackKnockback);            
                if (_status.StunOnAttack)
                    _status.TargetStatus.AddEffect(new StunnedEffect(_status.StunDuration));
            }
        }
        else if (animEvent == "Die")
        {
            _waveController.OnDeath(_status);
        }
    }

    public void Die()
    {
        _status.IsControllable = false;
        _animator.SetTrigger("Die");
    }

    public void ApplyKnockbackFrom(Vector2 position, float knockback)
    {
        if (knockback <= float.Epsilon) return;
        Vector2 direction = Position - position;
        _knockbackVelocity = knockback * direction.normalized;
    }

    public void Reset()
    {
        ChangeStates(State.Idle);

        _knockbackVelocity = Vector2.zero;

        // Reset the animator the IDLE so enemies don't spawn attacking
        _animator.SetFloat("dx", 0);
        _animator.SetFloat("dy", 0);
        _animator.SetBool("IsStunned", false);
        _animator.SetBool("IsRunning", false);
        _animator.ResetTrigger("Attack");
        _animator.ResetTrigger("Die");
        _animator.Update(0f);
        _animator.Play("Idle.IdleDown", 0, 0f);
    }
}