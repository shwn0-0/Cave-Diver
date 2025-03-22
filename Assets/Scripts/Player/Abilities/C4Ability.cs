using UnityEngine;

class C4Ability : IAbility
{
    private readonly ObjectCache _cache;
    private readonly float _cooldown;
    private readonly C4.Config _config;
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
        _cooldown = config.C4AbilityCooldown;
        _config = new(config.C4AbilityDamage, config.C4AbilityDuration, config.C4AbilityKnockbackForce, config.C4AbilityStunDuration);
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

        _c4 = _cache.GetObject<C4>(ObjectType.C4);
        _c4.Init(_config, _player.transform.position, IsUpgraded);

        return true;
    }

    public void Update()
    {
        if (Placed && !_c4.IsActive)
        {
            _remainingCooldown = _cooldown;
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