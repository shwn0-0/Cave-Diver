using UnityEngine;

class LureAbility : IAbility
{
    private readonly float _cooldown;
    private readonly GameObject _prefab;
    private readonly PlayerStatus _target;

    private Lure _lure;
    private float _remainingCooldown;

    public bool IsAvailable => _remainingCooldown <= 0.0f;
    public bool IsPlaced => _lure != null;
    public string Name => "Lure";
    public bool IsUpgraded { get; private set; } = false ;

    public LureAbility(PlayerAbilitiesConfig config, PlayerStatus target, GameObject prefab)
    {
        _cooldown = config.LureAbilityCooldown;
        _prefab = prefab;
        _target = target;
    }

    public bool Activate()
    {
        if (!IsAvailable || IsPlaced) return false;

        // TODO: Eventually make an Object system to cache objects so we don't have to always instantiate a new one
        GameObject lureObj = Object.Instantiate(_prefab, _target.transform.position, Quaternion.identity);
        _lure = lureObj.GetComponent<Lure>();
        _lure.Init(IsUpgraded);

        return true;
    }

    public void Update()
    {
        if (IsPlaced && !_lure.IsActive)
        {
            _remainingCooldown = _cooldown;
            // TODO: Better cleanup if we save the object.
            Object.Destroy(_lure.gameObject);
            _lure = null;
        } else if (!IsAvailable)
        {
            _remainingCooldown -= Time.deltaTime;
        }
    }

    public void Upgrade() => IsUpgraded = true;
}