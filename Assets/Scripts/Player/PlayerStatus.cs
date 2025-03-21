using UnityEngine;

class PlayerStatus : Status
{
    [SerializeField] private PlayerAbilitiesConfig _abilitiesConfig;
    [SerializeField] private GameObject _c4Prefab;
    [SerializeField] private GameObject _lurePrefab;
    
    
    private HUDController _hudController;
    
    private PlayerController _controller;


    void Awake()
    {
        _controller = GetComponent<PlayerController>();
        _hudController = FindFirstObjectByType<HUDController>();
    }


    void Start()
    {
        var ability = new LureAbility(_abilitiesConfig, this, _lurePrefab);
        _controller.UnlockAbility(ability);
        _hudController.UnlockAbility(ability);
    }

    public override void ApplyDamage(float damage)
    {
        base.ApplyDamage(damage);
        _hudController.SetHealthPercent(HealthPercent);
        _hudController.SetShieldPercent(ShieldPercent);
    }
}