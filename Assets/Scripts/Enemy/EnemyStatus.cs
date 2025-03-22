using UnityEngine;

class EnemyStatus : Status, ICacheableObject
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
    public ObjectType Type { get; private set; }

    public void Awake()
    {
        _controller = GetComponent<EnemyController>();
        _transform = transform;
    }

    public void Init(IObjectConfig objConfig)
    {
        if (objConfig is Config config)
        {
            base.Init();
            Type = config.type;
            Target = config.target;
            _controller.ChangeStates(State.Idle);
            IsControllable = true;
        }
        else
        {
            Debug.LogError("Passed Invalid Config to EnemyStatus");
        }
    }

    public override void ApplyKnockbackFrom(Vector2 position, float knockbackForce) =>
        _controller.ApplyKnockbackFrom(position, knockbackForce);

    public readonly struct Config : IObjectConfig
    {
        public readonly Transform target;
        public readonly ObjectType type;

        public Config(Transform target, ObjectType type)
        {
            this.target = target;
            this.type = type;
        }
    }
}