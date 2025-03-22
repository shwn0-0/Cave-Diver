using UnityEngine;

class ShieldAbility : IAbility
{
    private readonly float _cooldown;
    private readonly float _duration;
    private readonly float _healPercentage;
    private readonly PlayerStatus _player;

    private float _remainingCooldown;

    public float Cooldown => _remainingCooldown;
    public bool IsAvailable => _remainingCooldown <= 0.0f;
    public bool IsUpgraded { get; private set; } = false;
    public string Name => "Sheild";

    public ShieldAbility(PlayerAbilitiesConfig config, PlayerStatus player)
    {
        _cooldown = config.ShieldAbilityCooldown;
        _duration = config.ShieldAbilityDuration;
        _healPercentage = config.ShieldAbilityHealPercentage;
        _player = player;
    }

    public bool Activate()
    {
        if (!IsAvailable) return false;
        _remainingCooldown = _cooldown * (1 - _player.AbilityHaste);

        _player.AddEffect(new ShieldEffect(_duration));

        if (IsUpgraded)
            _player.PercentHeal(_healPercentage);

        return true;
    }

    public void Update()
    {
        if (!IsAvailable)
            _remainingCooldown -= Time.deltaTime;
    }

    public void Upgrade() => IsUpgraded = true;
}