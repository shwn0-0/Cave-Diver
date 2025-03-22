using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

class UpgradesController : MonoBehaviour
{
    private UpgradeButton[] _abilityUpgradeButtons;
    private PlayerStatus _playerStatus;
    private WaveController _waveController;
    private int _remainingUpgrades = 0;

    public void Awake()
    {
        _abilityUpgradeButtons = GetComponentsInChildren<UpgradeButton>().Where(btn => btn.IsAbilityUpgrade).ToArray();
        _playerStatus = FindAnyObjectByType<PlayerStatus>();
        _waveController = FindAnyObjectByType<WaveController>();
    }

    public void Show(int count)
    {
        gameObject.SetActive(true);
        _remainingUpgrades = count;
        foreach (UpgradeButton button in _abilityUpgradeButtons)
        {
            button.IsEnabled = _playerStatus.HasUpgradeableAbility(button.Upgrade);
        }
    }

    public void OnUpgrade(Upgrade upgrade)
    {
        _playerStatus.AddUpgrade(upgrade);
        _remainingUpgrades -= 1;
        if (_remainingUpgrades <= 0)
        {
            _waveController.NextWave();
            gameObject.SetActive(false);
        }
    }
}