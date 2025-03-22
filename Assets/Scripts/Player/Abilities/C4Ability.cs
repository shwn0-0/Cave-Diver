using UnityEngine;

class C4Ability : IAbility
{
    private readonly float _cooldown;
    private readonly GameObject _prefab;
    private readonly PlayerStatus _target;
    private readonly C4.Config _config;
    private C4 _c4;

    private float _remainingCooldown;

    public float Cooldown => _remainingCooldown;
    public bool IsAvailable => _remainingCooldown <= 0.0f;
    public bool IsUpgraded { get; private set; } = false ;
    public string Name => "C4";
    public bool Placed => _c4 != null;

    public C4Ability(PlayerAbilitiesConfig config, PlayerStatus target, GameObject prefab)
    {
        _cooldown = config.C4AbilityCooldown;
        _config = new(config.C4AbilityDamage, config.C4AbilityDuration, config.C4AbilityKnockbackForce, config.C4AbilityStunDuration);
        _prefab = prefab;
        _target = target;
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

        // TODO: Eventually make an Object system to cache objects so we don't have to always instantiate a new one
        GameObject c4Obj = Object.Instantiate(_prefab, _target.transform.position, Quaternion.identity);
        _c4 = c4Obj.GetComponent<C4>();
        _c4.Init(_config, IsUpgraded);

        return true;
    }

    public void Update()
    {
        if (Placed && !_c4.IsActive)
        {
            _remainingCooldown = _cooldown;
            // TODO: Better cleanup if we save the object.
            Object.Destroy(_c4.gameObject);
            _c4 = null;
        }
        else if (!IsAvailable)
        {
            _remainingCooldown -= Time.deltaTime;
        }
    }

    public void Upgrade() => IsUpgraded = true;
}