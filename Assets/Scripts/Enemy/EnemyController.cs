using System.Collections;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

// TODO: When reusing object Slime is still the dead sprite

class EnemyController : MonoBehaviour
{
    private static readonly string[] ANIMATION_DEATH_STATES =
    {
        "Dead.DieUp",
        "Dead.DieDown",
        "Dead.DieLeft",
        "Dead.DieRight"
    };

    private Animator _animator;
    private float _attackCooldown;
    private bool _attacked;
    private State _currentState;
    private Vector2 _knockbackVelocity;
    private EnemyStatus _status;
    private Transform _transform;
    private Vector2 _walkingDisplacement;
    private WaveController _waveController;

    public bool IsStunned => _status.IsStunned;
    public bool IsDead => _status.IsDead;
    public bool IsTargetInRange => _status.IsTargetInRange;

    // Avoid frequent state flips by staying in AttackState until after Attack is off cooldown
    // It would be better to stay in Idle until we can attack but I don't feel like
    // going through the effort of updating the State Machine now. Maybe enemies just get tired
    // after trying to attack or something idk.
    public bool FinishedAttacking => _attacked && _attackCooldown <= 0f;

    void Awake()
    {
        _waveController = FindFirstObjectByType<WaveController>();
        _transform = transform;
        _status = GetComponent<EnemyStatus>();
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!_status.IsControllable) return;
        _currentState.Run(this);
        HandleCooldowns();
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector2 knockbackDisplacement = _knockbackVelocity * Time.deltaTime;
        _knockbackVelocity -= _knockbackVelocity * _status.KnockbackFriction; // static friction applied to knockback only
        _transform.position += (Vector3)(_walkingDisplacement + knockbackDisplacement);
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
            _walkingDisplacement = Vector2.zero;
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
        // TODO: Add fancy path finding
        Vector2 direction = _status.Target.position - _transform.position;
        Vector2 normalizedDirection = direction.normalized;
        float maxDisplacement = _status.MoveSpeed * Time.deltaTime;
        _walkingDisplacement = normalizedDirection * math.min(maxDisplacement, direction.magnitude);
        _animator.SetBool("IsRunning", true);
        _animator.SetFloat("dx", normalizedDirection.x);
        _animator.SetFloat("dy", normalizedDirection.y);
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

        if (_status.TargetStatus != null && !_status.TargetStatus.IsDead)
        {
            _status.TargetStatus.ApplyDamage(_status.AttackDamage);
            _status.TargetStatus.ApplyKnockbackFrom(_transform.position, _status.AttackKnockback);
        }

        Vector2 targetDirection = (_status.Target.position - _transform.position).normalized;
        _knockbackVelocity += _status.AttackSelfKnockforward * targetDirection;

        _animator.SetFloat("dx", targetDirection.x);
        _animator.SetFloat("dy", targetDirection.y);
        _animator.SetTrigger("Attack");
    }

    public void Die()
    {
        _status.IsControllable = false;
        StartCoroutine(HandleDying());
    }

    private IEnumerator HandleDying()
    {
        _animator.SetTrigger("Die");

        // Wait until we enter a death animation state
        while (!ANIMATION_DEATH_STATES.Any(state => _animator.GetCurrentAnimatorStateInfo(0).IsName(state)))
            yield return null;

        // Wait until animation stops
        while (math.frac(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime) < 0.99f)
            yield return null;

        _waveController.OnDeath(_status);
    }

    public void ApplyKnockbackFrom(Vector2 position, float knockback)
    {
        Vector2 currentPosition = _transform.position;
        Vector2 direction = currentPosition - position;
        float distance = direction.magnitude;
        float inverseSquaredDistance = math.clamp(1 / (distance * distance), 1, 2);
        _knockbackVelocity += inverseSquaredDistance * knockback * direction.normalized;
    }
}