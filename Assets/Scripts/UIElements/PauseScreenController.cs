using UnityEngine;

class PauseScreenController : MonoBehaviour
{
    CanvasGroup _cg;
    StatsDisplay _statsController;

    void Awake()
    {
        _cg = GetComponent<CanvasGroup>();
        _statsController = GetComponentInChildren<StatsDisplay>();
    }

    public void Show()
    {
        _statsController.UpdateWithLatest();
        _cg.alpha = 1;
        _cg.interactable = true;
        _cg.blocksRaycasts = true;
    }

    public void Hide()
    {
        _cg.alpha = 0;
        _cg.interactable = false;
        _cg.blocksRaycasts = false;
    }
}