using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

abstract class Status : MonoBehaviour
{
    [SerializeField] private StatusConfig _config;

    private readonly List<IStatusEffect> _effects = new();
    private bool _isInvulnerable = false;

    public float AttackDamage => _config.AttackDamage * DamageMultiplier;
    public float AttackAngle => _config.AttackAngle;
    public float AttackRange => _config.AttackRange;
    public float AttackSpeed => _config.AttackSpeed;
    public float AttackKnockback => _config.AttackKnockback;
    public float AttackSelfKnockforward => _config.AttackSelfKnockforward;
    public float DamageMultiplier { get; set; }
    public float Health { get; set; }
    public float HealthPercent => Health / _config.MaxHealth;
    public bool IsControllable { get; set; }
    public bool IsDead => Health <= 0.0f;
    public virtual bool IsInvulnerable
    {
        get => _isInvulnerable;
        set
        {
            if (value) Shield = _config.MaxShields;
            _isInvulnerable = value;
        }
    }
    public bool IsStunned { get; set; }
    public float KnockbackFriction => _config.KnockbackFriction;
    public float MoveSpeed => _config.MoveSpeed * MoveSpeedMultiplier;
    public float MoveSpeedMultiplier { get; set; }
    public float Shield { get; set; }
    public float ShieldPercent => Shield / _config.MaxShields;

    public virtual void Init()
    {
        Health = _config.MaxHealth;
        Shield = _config.MaxHealth;
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

    public virtual void ApplyDamage(float damage)
    {
        if (IsInvulnerable) return;
        Health -= math.max(0, damage - Shield);
        Shield = math.max(0, Shield - damage);
    }

    public abstract void ApplyKnockbackFrom(Vector2 position, float knockbackForce);
}