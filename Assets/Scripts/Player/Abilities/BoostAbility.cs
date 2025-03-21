using UnityEngine;

class BoostAbility : IAbility
{
    private readonly float _amount;
    private readonly float _cooldown;
    private readonly float _duration;
    private readonly PlayerStatus _target;

    private float _remainingCooldown;

    public float Cooldown => _remainingCooldown;
    public bool IsAvailable => _remainingCooldown <= 0.0f;
    public bool IsUpgraded { get; private set; } = false;
    public string Name => "Boost";

    public BoostAbility(PlayerAbilitiesConfig config, PlayerStatus target)
    {
        _amount = config.BoostAbilityAmount;
        _cooldown = config.BoostAbilityCooldown;
        _duration = config.BoostAbilityDuration;
        _target = target;
    }

    public bool Activate()
    {
        if (!IsAvailable) return false;

        _remainingCooldown = _cooldown;
        _target.AddEffect(new SpeedBoostEffect(_amount, _duration));

        if (IsUpgraded)
            _target.AddEffect(new DamageBoostEffect(_amount, _duration));

        return true;
    }

    public void Update()
    {
        if (!IsAvailable)
            _remainingCooldown -= Time.deltaTime;
    }

    public void Upgrade() => IsUpgraded = true;
}