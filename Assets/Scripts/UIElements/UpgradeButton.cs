using UnityEngine;
using UnityEngine.UI;

class UpgradeButton : MonoBehaviour
{
    [SerializeField] private Upgrade _upgrade;
    private UpgradesController _upgradeController;
    private Button _button;
    private int _count = 0;
    private bool _isEnabled = true;

    public bool IsAbilityUpgrade => 
        _upgrade is Upgrade.BoostAbility || 
        _upgrade is Upgrade.ShieldAbility || 
        _upgrade is Upgrade.LureAbility || 
        _upgrade is Upgrade.C4Ability;

    public bool Enabled { 
        get => _isEnabled; 
        set {
            _isEnabled = value;
            _button.enabled = value && (!IsAbilityUpgrade || _count < 2);
        }
    }

    void Awake()
    {
        _upgradeController = GetComponentInParent<UpgradesController>();
        _button = GetComponent<Button>();
    }

    public void OnClick()
    {
        _upgradeController.OnUpgrade(_upgrade);
        _count += 1;
    }
}