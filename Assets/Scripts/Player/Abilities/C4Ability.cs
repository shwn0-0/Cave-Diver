using UnityEngine;

class C4Ability : IAbility
{
    private readonly ObjectCache _cache;
    private readonly PlayerAbilitiesConfig _config;
    private readonly PlayerStatus _player;

    private C4 _c4;
    private float _remainingCooldown;

    public float Cooldown => _remainingCooldown;
    public bool IsAvailable => _remainingCooldown <= 0.0f;
    public bool IsUpgraded { get; private set; } = false ;
    public string Name => "C4";
    public bool Placed => _c4 != null;

    public C4Ability(PlayerAbilitiesConfig config, PlayerStatus player, ObjectCache cache)
    {
        _cache = cache;
        _config = config;
        _player = player;
    }

    public bool Activate()
    {
        if (!IsAvailable) return false;
        
        // Trigger C4 if already placed.
        if (Placed)
        {
            _c4.Trigger();
            return true;
        }

        _c4 = _cache.GetObject<C4>(
            ObjectType.C4,
            _player.transform.position,
            new C4.Config(_config.C4AbilityDamage, _config.C4AbilityDuration, _config.C4AbilityKnockbackForce, _config.C4AbilityStunDuration, IsUpgraded)
        );
        return true;
    }

    public void Update()
    {
        if (Placed && !_c4.IsActive)
        {
            _remainingCooldown = _config.C4AbilityCooldown * (1 - _player.AbilityHaste);
            _cache.ReturnObject(ObjectType.C4, _c4);
            _c4 = null;
        }
        else if (!IsAvailable)
        {
            _remainingCooldown -= Time.deltaTime;
        }
    }

    public void Upgrade() => IsUpgraded = true;
}