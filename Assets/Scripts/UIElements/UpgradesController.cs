using System.Linq;
using Unity.Mathematics;
using UnityEngine;

class UpgradesController : MonoBehaviour
{
    [SerializeField] private float _fadeDuration = 0.5f;
    private ILookup<bool, UpgradeButton> _upgradeButtons;
    private PlayerStatus _playerStatus;
    private WaveController _waveController;
    private CanvasGroup _canvasGroup;
    private int _remainingUpgrades = 0;
    private float _targetAlpha = 0;

    public void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _upgradeButtons = GetComponentsInChildren<UpgradeButton>().ToLookup(btn => btn.Upgrade.IsAbilityUpgrade());
        _playerStatus = FindAnyObjectByType<PlayerStatus>();
        _waveController = FindAnyObjectByType<WaveController>();
    }

    void Update()
    {
        if (math.abs(_targetAlpha - _canvasGroup.alpha) <= float.Epsilon) return;
        _canvasGroup.alpha += math.sign(_targetAlpha - _canvasGroup.alpha) * Time.deltaTime / _fadeDuration;
        _canvasGroup.blocksRaycasts = _canvasGroup.alpha > 0.95f;
    }

    public void Show(int count)
    {
        foreach (UpgradeButton btn in _upgradeButtons[true])
            btn.IsEnabled = _playerStatus.HasUpgradeableAbility(btn.Upgrade);
        foreach (UpgradeButton btn in _upgradeButtons[false])
            btn.IsEnabled = true;
        _targetAlpha = 1f;
        _remainingUpgrades = count;
    }

    public void OnUpgrade(UpgradeButton button)
    {
        if (_remainingUpgrades <= 0) return; // Prevent spamming buttons to unlock more stuff for free
        button.IsEnabled = false; // Only upgrade one thing at a time
        _remainingUpgrades -= 1;
        _playerStatus.AddUpgrade(button.Upgrade);

        if (_remainingUpgrades <= 0)
        {
            _targetAlpha = 0f;
            _waveController.NextWave();
        }
    }
}