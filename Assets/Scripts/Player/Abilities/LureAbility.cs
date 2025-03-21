using UnityEngine;

class LureAbility : IAbility
{
    private readonly float _cooldown;
    private readonly float _duration;
    private readonly GameObject _prefab;
    private readonly PlayerStatus _target;

    private Lure _lure;
    private float _remainingCooldown;

    public float Cooldown => _remainingCooldown;
    public bool IsAvailable => _remainingCooldown <= 0.0f && !IsPlaced;
    public bool IsPlaced => _lure != null;
    public bool IsUpgraded { get; private set; } = false ;
    public string Name => "Lure";

    public LureAbility(PlayerAbilitiesConfig config, PlayerStatus target, GameObject prefab)
    {
        _cooldown = config.LureAbilityCooldown;
        _duration = config.LureAbilityDuration;
        _prefab = prefab;
        _target = target;
    }

    public bool Activate()
    {
        if (!IsAvailable) return false;
        _remainingCooldown = _cooldown + _duration;

        // TODO: Eventually make an Object system to cache objects so we don't have to always instantiate a new one
        GameObject lureObj = Object.Instantiate(_prefab, _target.transform.position, Quaternion.identity);
        _lure = lureObj.GetComponent<Lure>();
        _lure.Init(_duration, IsUpgraded);

        return true;
    }

    public void Update()
    {
        if (IsPlaced && !_lure.IsActive)
        {
            // TODO: Better cleanup if we save the object.
            // Object.Destroy(_lure.gameObject);
            _lure = null;
        } else if (_remainingCooldown > 0.0f)
        {
            _remainingCooldown -= Time.deltaTime;
        }
    }

    public void Upgrade() => IsUpgraded = true;
}