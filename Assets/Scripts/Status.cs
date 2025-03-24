using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

abstract class Status : MonoBehaviour
{
    [SerializeField] private StatusConfig _config;

    private readonly List<IStatusEffect> _effects = new();
    private bool _isInvulnerable = false;
    private float _bonusHealth = 0f;
    private float _bonusSheild = 0f;

    public float AbilityHaste { get; set; }
    public float AttackDamage => (_config.AttackDamage + BonusDamage) * DamageMultiplier;
    public float AttackAngle => _config.AttackAngle;
    public float AttackRange => _config.AttackRange;
    public float AttackSpeed => _config.AttackSpeed * AttackSpeedMultiplier;
    public float AttackSpeedMultiplier { get; set; }
    public float AttackKnockback => _config.AttackKnockback;
    public float AttackSelfKnockforward => _config.AttackSelfKnockforward;
    public float BonusDamage { get; set; }
    public float BonusHealth { 
        get => _bonusHealth;
        set {
            // NOTE: HUD gets updated when health changes so need to update health last.
            var oldValue = _bonusHealth;
            _bonusHealth = value;
            Health += _bonusHealth - oldValue; // Add new value and subtract old value
        } 
    }
    public float BonusMoveSpeed { get; set; }
    public float BonusShield {
        get => _bonusSheild;
        set {
            // NOTE: HUD gets updated when shield changes so need to update shield last.
            var oldValue = _bonusSheild;
            _bonusSheild = value;
            Shield += _bonusSheild - oldValue; // Add new value and subtract old value
        }
    }
    public float DamageMultiplier { get; set; }
    public virtual float Health { get; set; }
    public float MaxHealth => _config.MaxHealth + BonusHealth;
    public bool IsControllable { get; set; }
    public bool IsDead => Health <= 0f;
    public bool IsInvulnerable
    {
        get => _isInvulnerable;
        set
        {
            if (value) Shield = MaxShield;
            _isInvulnerable = value;
        }
    }
    public bool IsStunned { get; set; }
    public float KnockbackFriction => _config.KnockbackFriction;
    public float MoveSpeed => (_config.MoveSpeed + BonusMoveSpeed) * MoveSpeedMultiplier;
    public float MoveSpeedMultiplier { get; set; }
    public virtual float Shield { get; set; }
    public float MaxShield => _config.MaxShield + BonusShield;
    public bool StunOnAttack => _config.StunOnAttack;
    public float StunDuration => _config.StunDuration;

    public virtual void Init()
    {
        DamageMultiplier = 1.0f;
        MoveSpeedMultiplier = 1.0f;
        AttackSpeedMultiplier = 1.0f;

        AbilityHaste = 0f;
        BonusDamage = 0f;
        BonusHealth = 0f;
        BonusMoveSpeed = 0f;
        BonusShield = 0f;

        Health = MaxHealth;
        Shield = MaxShield;
        IsStunned = false;
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
        Health = math.min(MaxHealth, Health * (1f + percentage));

    public virtual void ApplyDamage(float damage)
    {
        if (IsInvulnerable) return;
        Health = math.max(0f, Health - math.max(0f, damage - Shield));
        Shield = math.max(0f, Shield - damage);
    }

    public abstract void ApplyKnockbackFrom(Vector2 position, float knockbackForce);
}