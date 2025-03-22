using UnityEngine;

class EnemyStatus : Status
{
    private EnemyController _controller;
    private Status _targetStatus;
    private Transform _targetTransform;
    private Transform _transform;

    public Transform Target {
        get => _targetTransform;
        set {
            _targetTransform = value;
            value.TryGetComponent(out _targetStatus);
        }
    }
    public Status TargetStatus => _targetStatus;
    public bool IsTargetInRange =>
        Target != null && Vector2.Distance(_transform.position, Target.position) <= AttackRange;

    public void Awake()
    {
        _controller = GetComponent<EnemyController>();
        _transform = transform;
    }

    public override void ApplyKnockbackFrom(Vector2 position, float knockbackForce) =>
        _controller.ApplyKnockbackFrom(position, knockbackForce);
}