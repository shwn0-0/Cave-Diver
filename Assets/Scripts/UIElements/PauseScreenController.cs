using UnityEngine;

class PauseScreenController : MonoBehaviour
{
    CanvasGroup _cg;

    void Awake()
    {
        _cg = GetComponent<CanvasGroup>();
    }

    public void Show()
    {
        _cg.alpha = 1;
        _cg.blocksRaycasts = true;
    }

    public void Hide()
    {
        _cg.alpha = 0;
        _cg.blocksRaycasts = false;
    }
}