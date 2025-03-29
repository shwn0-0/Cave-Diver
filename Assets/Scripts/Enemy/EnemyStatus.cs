using UnityEngine;

class EnemyStatus : Status, ICacheableObject
{
    private EnemyController _controller;
    private Status _targetStatus;
    private Rigidbody2D _targetRigidBody;

    public Rigidbody2D Target {
        get => _targetRigidBody;
        set {
            _targetRigidBody = value;
            value.TryGetComponent(out _targetStatus);
        }
    }
    public Status TargetStatus => _targetStatus;
    public ObjectType Type { get; private set; }
    public Vector2 Position => _controller.Position;

    new void Awake()
    {
        base.Awake();
        _controller = GetComponent<EnemyController>();
    }

    public void Init(IObjectConfig objConfig)
    {
        if (objConfig is Config config)
        {
            Init();
            _controller.Reset();
            Type = config.type;
            Target = config.target;
            IsControllable = true;
        }
        else
        {
            Debug.LogError("Passed Invalid Config to EnemyStatus");
        }
    }

    public void Destroy()
    {
        IsControllable = false;
        _controller.Reset();
    }

    public override void ApplyKnockbackFrom(Vector2 position, float knockbackForce) =>
        _controller.ApplyKnockbackFrom(position, knockbackForce);

    public readonly struct Config : IObjectConfig
    {
        public readonly Rigidbody2D target;
        public readonly ObjectType type;

        public Config(Rigidbody2D target, ObjectType type)
        {
            this.target = target;
            this.type = type;
        }
    }
}