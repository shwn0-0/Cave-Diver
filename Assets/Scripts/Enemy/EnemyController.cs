using Unity.Mathematics;
using UnityEngine;

class EnemyController : MonoBehaviour
{
    private State _currentState;
    private EnemyStatus _status;
    private Transform _transform;
    private Vector2 _knockbackVelocity;
    private Vector2 _walkingDisplacement;
    private bool _attacked;
    private float _attackTime;
    private float _attackCooldown;
    private WaveController _waveController;

    public bool IsStunned => _status.IsStunned;
    public bool IsDead => _status.IsDead;
    public bool IsTargetInRange => _status.IsTargetInRange;
    public bool FinishedAttacking => _attacked;

    void Awake()
    {
        _waveController = FindFirstObjectByType<WaveController>();
        _transform = transform;
        _status = GetComponent<EnemyStatus>();
        _attackTime = 1 / _status.AttackSpeed;
    }

    void Update()
    {
        if (!_status.IsControllable) return;
        HandleCooldowns();
        HandleMovement();
        _currentState.Run();
    }

    private void HandleMovement()
    {
        Vector2 knockbackDisplacement = _knockbackVelocity * Time.deltaTime;
        _knockbackVelocity -= _knockbackVelocity * _status.KnockbackFriction; // static friction applied to knockback only
        _transform.position += (Vector3)(_walkingDisplacement + knockbackDisplacement);
    }
    
    private void HandleCooldowns()
    {
        if (_attackCooldown > 0.0f)
            _attackCooldown -= Time.deltaTime;
        else
            _attacked = false;
    }

    public void ChangeStates(State newState)
    {
        if (newState is not RunningState)
            _walkingDisplacement = Vector2.zero;
        _currentState = newState;
    }

    public void BeRunning()
    {
        // TODO: Add fancy path finding
        // TODO: Play Run Animation
        Vector2 direction = _status.Target.position - _transform.position;
        float maxDisplacement = _status.MoveSpeed * Time.deltaTime;
        _walkingDisplacement = direction.normalized * math.min(maxDisplacement, direction.magnitude);
    }

    public void BeStunned()
    {
        // TODO: Do being stunned stuff
    }

    public void BeAttacking()
    {
        if (_attacked) return;
        _attackCooldown = _attackTime;
        _attacked = true;

        // TODO: Do attacking stuff
        if (_status.TargetStatus != null)
        {
            _status.TargetStatus.ApplyDamage(_status.AttackDamage);
            _status.TargetStatus.ApplyKnockbackFrom(_transform.position, _status.AttackKnockback);
        }
        
        Vector2 targetDirection = _status.Target.position - _transform.position;
        _knockbackVelocity += _status.AttackSelfKnockforward * targetDirection.normalized;
    }

    public void Die()
    {
        // TODO: Play death animation.
        // TODO: After death animation finished, fade and then cleanup gameObject.
        _status.IsControllable = false;
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