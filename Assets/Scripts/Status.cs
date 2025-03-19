using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

class Status : MonoBehaviour
{
    [SerializeField] StatusConfig _config;

    private readonly List<IStatusEffect> _effects = new();
    private bool _isInvulnerable;
    private Status _targetStatus;
    private Transform _targetTransform;
    private IController _controller;
    private Transform _transform;

    public float AttackDamage => _config.AttackDamage * DamageMultiplier;
    public float AttackRadius => _config.AttackRadius;
    public float AttackRange => _config.AttackRange;
    public float AttackSpeed => _config.AttackSpeed;
    public float AttackKnockback => _config.AttackKnockback;
    public float AttackSelfKnockforward => _config.AttackSelfKnockforward;
    public float DamageMultiplier { get; set; }
    public float Health { get; set; }
    public bool IsDead => Health <= 0.0f;
    public bool IsInvulnerable
    {
        get => _isInvulnerable;
        set
        {
            if (value) Shields = _config.MaxShields;
            _isInvulnerable = value;
        }
    }
    public bool IsStunned { get; set; }
    public float KnockbackFriction => _config.KnockbackFriction;
    public float MoveSpeed => _config.MoveSpeed * MoveSpeedMultiplier;
    public float MoveSpeedMultiplier { get; set; }
    public float Shields { get; set; }
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

    void Awake()
    {
        _controller = GetComponent<IController>();
        _transform = transform;
    }

    void OnEnable()
    {
        Health = _config.MaxHealth;
        Shields = _config.MaxHealth;
        IsStunned = false;
        DamageMultiplier = 1.0f;
        MoveSpeedMultiplier = 1.0f;
    }

    public void AddEffect(IStatusEffect effect) =>
        StartCoroutine(HandleEffect(effect));

    private IEnumerator HandleEffect(IStatusEffect effect)
    {
        _effects.Add(effect);
        yield return effect.Apply(this);
        _effects.Remove(effect);
    }

    public void PercentHeal(float percentage) =>
        Health = math.min(_config.MaxHealth, Health * (1f + percentage));

    public void ApplyDamage(float damage)
    {
        Debug.Log($"{this.GetType().Name} took {damage} damage!");
        Health -= math.max(0, damage - Shields);
        Shields = math.max(0, Shields - damage);
    }

    public void ApplyKnockbackFrom(Vector2 position, float knockbackForce) =>
        _controller.ApplyKnockbackFrom(position, knockbackForce);
}