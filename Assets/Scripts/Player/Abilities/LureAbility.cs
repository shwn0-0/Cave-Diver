using UnityEngine;

class LureAbility : IAbility
{
    private readonly PlayerAbilitiesConfig _config;
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
        _config = config;
        _player = player;
    }

    public bool Activate()
    {
        if (!IsAvailable) return false;
        _remainingCooldown = _config.LureAbilityCooldown * (1 - _player.AbilityHaste);
        _lure = _cache.GetObject<Lure>(
            ObjectType.Lure,
            _player.transform.position, 
            new Lure.Config(_config.LureAbilityDuration, IsUpgraded)
        );
        return true;
    }

    public void Update(bool IsControllable)
    {
        if (IsPlaced && !_lure.IsActive)
        {
            _cache.ReturnObject(ObjectType.Lure, _lure);
            _lure = null;
        } else if (_remainingCooldown > 0.0f && IsControllable)
        {
            _remainingCooldown -= Time.deltaTime;
        }
    }

    public void Upgrade() => IsUpgraded = true;
}