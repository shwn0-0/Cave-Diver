using System;
using Unity.Mathematics;
using UnityEngine;

class PlayerStatus : Status
{
    [SerializeField] private PlayerAbilitiesConfig _abilitiesConfig;
    [SerializeField] private UpgradesConfig _upgradesConfig;

    private HUDController _hudController;
    private PlayerController _controller;
    private ObjectCache _objCache;
    private int _abilitySlots = 0;

    public override float Health
    {
        get => base.Health;
        set
        {
            base.Health = value;
            _hudController.SetHealth(Health, MaxHealth);
        }
    }

    public override float Shield
    {
        get => base.Shield;
        set
        {
            base.Shield = value;
            _hudController.SetShield(Shield, MaxShield);
        }
    }

    new void Awake()
    {
        base.Awake();
        _controller = GetComponent<PlayerController>();
        _hudController = FindFirstObjectByType<HUDController>();
        _objCache = FindFirstObjectByType<ObjectCache>();
    }

    void Start()
    {
        Init();
    }

    public void UnlockAbilitySlot(int count = 1)
    {
        count = math.clamp(count, 0, 4 - _abilitySlots);
        _abilitySlots += count;
        _hudController.UnlockAbilitySlot(count);
    }

    public void AddUpgrade(Upgrade upgrade)
    {
        switch (upgrade)
        {
            case Upgrade.BoostAbility:
            case Upgrade.ShieldAbility:
            case Upgrade.LureAbility:
            case Upgrade.C4Ability:
                HandleAbilityUpgrade(upgrade);
                break;
            case Upgrade.AttackDamage_MoveSpeed:
                BonusAttackDamage += _upgradesConfig.BonusAttackDamage;
                BonusMoveSpeed += _upgradesConfig.BonusMoveSpeed;
                break;
            case Upgrade.Health_Shield:
                BonusHealth += _upgradesConfig.BonusHealth;
                BonusShield += _upgradesConfig.BonusShield;
                break;
            default:
                throw new NotImplementedException($"Unhandled Upgrade {upgrade}");
        }
    }

    private void UnlockAbility(IAbility ability)
    {
        _controller.UnlockAbility(ability);
        _hudController.UnlockAbility(ability);
    }

    public override void ApplyKnockbackFrom(Vector2 position, float knockbackForce) =>
        _controller.ApplyKnockbackFrom(position, knockbackForce);

    public bool HasUpgradeableAbility(Upgrade upgrade)
    {
        IAbility ability = GetUpgradeAbility(upgrade);
        return ability != null && !ability.IsUpgraded;
    }

    public bool HasAvailableSlot(Upgrade upgrade)
    {
        if (!HasAvailableSlots()) return false;
        return GetUpgradeAbility(upgrade) == null;
    }

    public bool HasAvailableSlots() =>
        _abilitySlots > _controller.AbilityCount;

    private void HandleAbilityUpgrade(Upgrade upgrade)
    {
        IAbility ability = GetUpgradeAbility(upgrade);
        if (ability == null)
            UnlockAbility(CreateUpgradeAbility(upgrade));
        else if (!ability.IsUpgraded)
            ability.Upgrade();
    }

    private IAbility GetUpgradeAbility(Upgrade upgrade) => upgrade switch
    {
        Upgrade.BoostAbility => _controller.GetAbility<BoostAbility>(),
        Upgrade.ShieldAbility => _controller.GetAbility<ShieldAbility>(),
        Upgrade.LureAbility => _controller.GetAbility<LureAbility>(),
        Upgrade.C4Ability => _controller.GetAbility<C4Ability>(),
        _ => null
    };

    private IAbility CreateUpgradeAbility(Upgrade upgrade) => upgrade switch
    {
        Upgrade.BoostAbility => new BoostAbility(_abilitiesConfig, this),
        Upgrade.ShieldAbility => new ShieldAbility(_abilitiesConfig, this),
        Upgrade.LureAbility => new LureAbility(_abilitiesConfig, this, _objCache),
        Upgrade.C4Ability => new C4Ability(_abilitiesConfig, this, _objCache),
        _ => null
    };

    public void LookDown() => _controller.LookDown();
}