using UnityEngine;

class ShieldAbility : IAbility
{
    private readonly float _cooldown;
    private readonly float _duration;
    private readonly float _healPercentage;
    private readonly PlayerStatus _target;

    private float _remainingCooldown;

    public bool IsAvailable => _remainingCooldown <= 0.0f;
    public string Name => "Sheild";
    public bool IsUpgraded { get; private set; } = false;

    public ShieldAbility(PlayerAbilitiesConfig config, PlayerStatus target)
    {
        _cooldown = config.ShieldAbilityCooldown;
        _duration = config.ShieldAbilityDuration;
        _healPercentage = config.ShieldAbilityHealPercentage;
        _target = target;
    }

    public bool Activate()
    {
        if (!IsAvailable) return false;
        _remainingCooldown = _cooldown;

        _target.AddEffect(new ShieldEffect(_duration));

        if (IsUpgraded)
            _target.PercentHeal(_healPercentage);

        return true;
    }

    public void Update()
    {
        if (!IsAvailable)
            _remainingCooldown -= Time.deltaTime;
    }

    public void Upgrade() => IsUpgraded = true;
}