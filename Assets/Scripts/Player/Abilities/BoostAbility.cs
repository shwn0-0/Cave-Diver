using UnityEngine;

class BoostAbility : IAbility
{
    private readonly float _amount;
    private readonly float _cooldown;
    private readonly float _duration;
    private readonly PlayerStatus _player;

    private float _remainingCooldown;

    public float Cooldown => _remainingCooldown;
    public bool IsAvailable => _remainingCooldown <= 0.0f;
    public bool IsUpgraded { get; private set; } = false;
    public string Name => "Boost";

    public BoostAbility(PlayerAbilitiesConfig config, PlayerStatus player)
    {
        _amount = config.BoostAbilityAmount;
        _cooldown = config.BoostAbilityCooldown;
        _duration = config.BoostAbilityDuration;
        _player = player;
    }

    public bool Activate()
    {
        if (!IsAvailable) return false;
        _remainingCooldown = _cooldown * (1 - _player.AbilityHaste);

        _player.AddEffect(new SpeedBoostEffect(_amount, _duration));

        if (IsUpgraded)
            _player.AddEffect(new DamageBoostEffect(_amount, _duration));

        return true;
    }

    public void Update(bool IsControllable)
    {
        if (!IsAvailable && IsControllable)
            _remainingCooldown -= Time.deltaTime;
    }

    public void Upgrade() => IsUpgraded = true;
}