using UnityEngine;

class LureAbility : IAbility
{
    private readonly float _cooldown;
    private readonly float _duration;
    private readonly ObjectCache _cache;
    private readonly PlayerStatus _player;

    private Lure _lure;
    private float _remainingCooldown;

    public float Cooldown => _remainingCooldown;
    public bool IsAvailable => _remainingCooldown <= 0.0f && !IsPlaced;
    public bool IsPlaced => _lure != null;
    public bool IsUpgraded { get; private set; } = false ;
    public string Name => "Lure";

    public LureAbility(PlayerAbilitiesConfig config, PlayerStatus player, ObjectCache cache)
    {
        _cache = cache;
        _cooldown = config.LureAbilityCooldown;
        _duration = config.LureAbilityDuration;
        _player = player;
    }

    public bool Activate()
    {
        if (!IsAvailable) return false;
        _remainingCooldown = _cooldown + _duration;
        _lure = _cache.GetObject<Lure>(ObjectType.Lure);
        _lure.Init(_duration, _player.transform.position, IsUpgraded);
        return true;
    }

    public void Update()
    {
        if (IsPlaced && !_lure.IsActive)
        {
            _cache.ReturnObject(ObjectType.Lure, _lure);
            _lure = null;
        } else if (_remainingCooldown > 0.0f)
        {
            _remainingCooldown -= Time.deltaTime;
        }
    }

    public void Upgrade() => IsUpgraded = true;
}