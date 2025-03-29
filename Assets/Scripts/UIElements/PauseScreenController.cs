using UnityEngine;

class PauseScreenController : MonoBehaviour
{
    CanvasGroup _cg;
    StatsDisplay _statsController;
    PauseScreenButton[] _pauseScreenButtons;

    void Awake()
    {
        _cg = GetComponent<CanvasGroup>();
        _statsController = GetComponentInChildren<StatsDisplay>();
        _pauseScreenButtons = GetComponentsInChildren<PauseScreenButton>();
    }

    public void Show()
    {
        _statsController.UpdateWithLatest();
        SetButtonsEnabled(true);
        _cg.alpha = 1;
        _cg.blocksRaycasts = true;
    }

    public void Hide()
    {
        SetButtonsEnabled(false);
        _cg.alpha = 0;
        _cg.blocksRaycasts = false;
    }

    private void SetButtonsEnabled(bool enabled)
    {
        foreach (var button in _pauseScreenButtons)
            button.IsEnabled = enabled; 
    }
}