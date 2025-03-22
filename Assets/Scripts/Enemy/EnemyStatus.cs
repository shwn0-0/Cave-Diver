using UnityEngine;

class EnemyStatus : Status
{
    private EnemyController _controller;
    private Status _targetStatus;
    private Transform _targetTransform;
    private Transform _transform;

    public bool IsTargetInRange =>
        Target != null && Vector2.Distance(_transform.position, Target.position) <= AttackRange;
    public Transform Target {
        get => _targetTransform;
        set {
            _targetTransform = value;
            value.TryGetComponent(out _targetStatus);
        }
    }
    public Status TargetStatus => _targetStatus;
    public ObjectType Type { get; set; }

    public void Awake()
    {
        _controller = GetComponent<EnemyController>();
        _transform = transform;
    }

    public override void Init()
    {
        Debug.Log("Calling the Init function");
        base.Init();
        gameObject.SetActive(true);
        _controller.ChangeStates(new IdleState(_controller));
        IsControllable = true;
    }

    public override void ApplyKnockbackFrom(Vector2 position, float knockbackForce) =>
        _controller.ApplyKnockbackFrom(position, knockbackForce);
}