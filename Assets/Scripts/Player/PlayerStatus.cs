using System;
using UnityEngine;

class PlayerStatus : Status
{
    [SerializeField] private PlayerAbilitiesConfig _abilitiesConfig;

    private HUDController _hudController;
    private PlayerController _controller;
    private ObjectCache _objCache;
    public int _abilitySlots = 0;

    public override float Health { 
        get => base.Health;
        set {
            base.Health = value;
            _hudController.SetHealth(Health, MaxHealth);
        } 
    }
 
    public override float Shield { 
        get => base.Shield;
        set {
            base.Shield = value;
            _hudController.SetShield(Shield, MaxShield);
        } 
    }

    void Awake()
    {
        _controller = GetComponent<PlayerController>();
        _hudController = FindFirstObjectByType<HUDController>();
        _objCache = FindFirstObjectByType<ObjectCache>();
        Init();
    }

    public void UnlockAbilitySlot() => _abilitySlots += 1;

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
            // FIXME: Create config for magic numbers and add max values as well
            case Upgrade.AttackDamage:
                BonusDamage += 2f;
                break;
            case Upgrade.AttackSpeed:
                AttackSpeedMultiplier += 0.5f;
                break;
            case Upgrade.AbilityHaste:
                AbilityHaste += 0.05f;
                break;
            case Upgrade.MoveSpeed:
                BonusMoveSpeed += 0.25f;
                break;
            case Upgrade.Health:
                BonusHealth += 25f;
                break;
            case Upgrade.Shield:
                BonusShield += 25f;
                break;
            default:
                Debug.LogError($"Unhandled Upgrade {upgrade}");
                break;
        }
    }

    private void UnlockAbility(IAbility ability)
    {
        _controller.UnlockAbility(ability);
        _hudController.UnlockAbility(ability);
    }

    public override void ApplyKnockbackFrom(Vector2 position, float knockbackForce) =>
        _controller.ApplyKnockbackFrom(position, knockbackForce);

    public bool HasUpgradeableAbility(Upgrade upgrade) {
        IAbility ability = GetUpgradeAbility(upgrade);

        if (ability == null)
            return _abilitySlots > _controller.AbilityCount;
        else
            return !ability.IsUpgraded;
    }

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
}