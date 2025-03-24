using System.Linq;
using Unity.Mathematics;
using UnityEngine;

class UpgradesController : MonoBehaviour
{
    private UpgradeButton[] _abilityUpgradeButtons;
    private PlayerStatus _playerStatus;
    private WaveController _waveController;
    private CanvasGroup _canvasGroup;
    private int _remainingUpgrades = 0;
    private float _targetAlpha;
    private float _duration = 0.5f;

    public void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _abilityUpgradeButtons = GetComponentsInChildren<UpgradeButton>().Where(btn => btn.IsAbilityUpgrade).ToArray();
        _playerStatus = FindAnyObjectByType<PlayerStatus>();
        _waveController = FindAnyObjectByType<WaveController>();
    }

    void Update()
    {
        if (math.abs(_targetAlpha - _canvasGroup.alpha) <= float.Epsilon) return;
        _canvasGroup.alpha += math.sign(_targetAlpha - _canvasGroup.alpha) * Time.deltaTime / _duration;
        _canvasGroup.interactable = _canvasGroup.alpha > 0.90f;
    }

    public void Show(int count)
    {
        _targetAlpha = 1f;
        _remainingUpgrades = count;
        foreach (UpgradeButton button in _abilityUpgradeButtons)
        {
            button.IsEnabled = _playerStatus.HasUpgradeableAbility(button.Upgrade);
        }
    }

    public void OnUpgrade(Upgrade upgrade)
    {
        if (_remainingUpgrades <= 0)
        {
            _targetAlpha = 0f;
            _waveController.NextWave();
            return;
        }
        _playerStatus.AddUpgrade(upgrade);
        _remainingUpgrades -= 1;
    }
}