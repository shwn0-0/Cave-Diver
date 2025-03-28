using System;
using UnityEngine;

class PauseScreenButton : MonoBehaviour
{
    [SerializeField] Button _button;

    GameController _controller;

    void Awake()
    {
        _controller = FindFirstObjectByType<GameController>();
    }

    public void OnClick()
    {
        switch (_button)
        {
            case Button.Continue:
                _controller.TogglePause();
                break;

            case Button.MainMenu:
                SceneController.Instance.GoToMainMenu();
                break;
            
            default:
                throw new NotImplementedException($"Unhandled Pause Screen Button {_button}");
        }
    }

    enum Button
    {
        Continue,
        MainMenu
    }
}