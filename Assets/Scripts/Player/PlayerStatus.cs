using System.Linq;
using UnityEngine;

class PlayerStatus : Status
{
    [SerializeField] private PlayerAbilitiesConfig _abilitiesConfig;
    [SerializeField] private GameObject _c4Prefab;
    [SerializeField] private GameObject _lurePrefab;
    
    private HUDController _hudController;
    private PlayerController _controller;


    public override bool IsInvulnerable
    {
        get => base.IsInvulnerable;
        set
        {
            base.IsInvulnerable = value;
            _hudController.SetShieldPercent(ShieldPercent);
        }
    }


    void Awake()
    {
        _controller = GetComponent<PlayerController>();
        _hudController = FindFirstObjectByType<HUDController>();
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
                    UnlockAbility(new LureAbility(_abilitiesConfig, this, _lurePrefab));
                break;
            case Upgrade.C4Ability:
                C4Ability c4Ability = _controller.GetAbility<C4Ability>();
                if (c4Ability != null)
                    c4Ability.Upgrade();
                else
                    UnlockAbility(new C4Ability(_abilitiesConfig, this, _c4Prefab));
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