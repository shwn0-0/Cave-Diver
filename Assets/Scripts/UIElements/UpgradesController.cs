using System.Linq;
using Unity.Mathematics;
using UnityEngine;

class UpgradesController : MonoBehaviour
{
    [SerializeField] private float _fadeDuration = 0.5f;
    private ILookup<bool, UpgradeButton> _upgradeButtons;
    private PlayerStatus _player;
    private GameController _gameController;
    private CanvasGroup _canvasGroup;
    private int _remainingUpgrades = 0;
    private float _targetAlpha = 0;
    private RemainingUpgradesDisplay _ruDisplay;

    public void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _upgradeButtons = GetComponentsInChildren<UpgradeButton>()
            .ToLookup(btn => btn.Upgrade.IsAbilityUpgrade());
        _player = FindFirstObjectByType<PlayerStatus>();
        _gameController = FindFirstObjectByType<GameController>();
        _ruDisplay = FindFirstObjectByType<RemainingUpgradesDisplay>();
    }

    void Update()
    {
        if (math.abs(_targetAlpha - _canvasGroup.alpha) <= float.Epsilon) return;
        _canvasGroup.alpha += math.sign(_targetAlpha - _canvasGroup.alpha) * Time.unscaledDeltaTime / _fadeDuration;
        _canvasGroup.blocksRaycasts = _canvasGroup.alpha > 0.95f;
    }

    public void Show(int count)
    {
        _targetAlpha = 1f;
        _remainingUpgrades = count;
        _ruDisplay.SetRemainingUpgrades(_remainingUpgrades);
        UpdateEnabledButtons();
    }

    public void OnUpgrade(UpgradeButton button)
    {
        if (!IsButtonAbilityUnlock(button))
        {
            _remainingUpgrades -= 1;
            _ruDisplay.SetRemainingUpgrades(_remainingUpgrades);
        }
        _player.AddUpgrade(button.Upgrade);
        UpdateEnabledButtons();


        if (_remainingUpgrades <= 0 && !_player.HasAvailableSlots())
        {
            _targetAlpha = 0f;
            _gameController.OnFinishedUpgrading();
        }
    }

    private void UpdateEnabledButtons()
    {
        foreach (UpgradeButton btn in _upgradeButtons[true])
        {
            btn.IsEnabled = _player.HasAvailableSlot(btn.Upgrade)
                || (_remainingUpgrades > 0 && _player.HasUpgradeableAbility(btn.Upgrade));
        }

        foreach (UpgradeButton btn in _upgradeButtons[false])
        {
            btn.IsEnabled = _remainingUpgrades > 0;
        }
    }

    private bool IsButtonAbilityUnlock(UpgradeButton button) =>
        button.Upgrade.IsAbilityUpgrade() && _player.HasAvailableSlot(button.Upgrade);
}