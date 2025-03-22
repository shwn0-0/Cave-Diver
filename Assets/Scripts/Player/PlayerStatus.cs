using UnityEngine;

class PlayerStatus : Status
{
    [SerializeField] private PlayerAbilitiesConfig _abilitiesConfig;
    
    private HUDController _hudController;
    private PlayerController _controller;
    private ObjectCache _objCache;

    public override bool IsInvulnerable
    {
        get => base.IsInvulnerable;
        set
        {
            base.IsInvulnerable = value;
            Debug.Log(_hudController);
            _hudController.SetShieldPercent(ShieldPercent);
        }
    }


    void Awake()
    {
        _controller = GetComponent<PlayerController>();
        _hudController = FindFirstObjectByType<HUDController>();
        _objCache = FindFirstObjectByType<ObjectCache>();
        Init();
    }

    public void AddUpgrade(Upgrade upgrade)
    {
        switch (upgrade)
        {
            case Upgrade.BoostAbility:
                BoostAbility boostAbility = _controller.GetAbility<BoostAbility>();
                if (boostAbility != null)
                    boostAbility.Upgrade();
                else
                    UnlockAbility(new BoostAbility(_abilitiesConfig, this));
                break;
            case Upgrade.ShieldAbility:
                ShieldAbility shieldAbility = _controller.GetAbility<ShieldAbility>();
                if (shieldAbility != null)
                    shieldAbility.Upgrade();
                else
                    UnlockAbility(new ShieldAbility(_abilitiesConfig, this));
                break;
            case Upgrade.LureAbility:
                LureAbility lureAbility = _controller.GetAbility<LureAbility>();
                if (lureAbility != null)
                    lureAbility.Upgrade();
                else
                    UnlockAbility(new LureAbility(_abilitiesConfig, this, _objCache));
                break;
            case Upgrade.C4Ability:
                C4Ability c4Ability = _controller.GetAbility<C4Ability>();
                if (c4Ability != null)
                    c4Ability.Upgrade();
                else
                    UnlockAbility(new C4Ability(_abilitiesConfig, this, _objCache));
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

    public override void ApplyDamage(float damage)
    {
        base.ApplyDamage(damage);
        _hudController.SetHealthPercent(HealthPercent);
        _hudController.SetShieldPercent(ShieldPercent);
    }

    public override void ApplyKnockbackFrom(Vector2 position, float knockbackForce) =>
        _controller.ApplyKnockbackFrom(position, knockbackForce);
}