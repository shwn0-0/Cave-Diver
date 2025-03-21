using UnityEngine;

class PlayerStatus : Status
{
    [SerializeField] private PlayerAbilitiesConfig _abilitiesConfig;
    [SerializeField] private GameObject _c4Prefab;
    [SerializeField] private GameObject _lurePrefab;
    [SerializeField] private HUDController _hudController;


    public override void ApplyDamage(float damage)
    {
        base.ApplyDamage(damage);
        _hudController.SetHealthPercent(HealthPercent);
        _hudController.SetShieldPercent(ShieldPercent);
    }
}