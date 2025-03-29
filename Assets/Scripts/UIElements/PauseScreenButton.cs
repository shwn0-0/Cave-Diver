using System;
using UnityEngine;
using UnityEngine.UI;

class PauseScreenButton : MonoBehaviour
{
    [SerializeField] ButtonType _buttonType;

    GameController _controller;
    Button _button;
    bool _isEnabled;

    public bool IsEnabled {
        get => _isEnabled; 
        set {
            _isEnabled = value;
            _button.interactable = value;
        }
    }

    void Awake()
    {
        _controller = FindFirstObjectByType<GameController>();
        _button = GetComponent<Button>();
    }

    public void OnClick()
    {
        switch (_buttonType)
        {
            case ButtonType.Continue:
                _controller.TogglePause();
                break;

            case ButtonType.MainMenu:
                SceneController.Instance.GoToMainMenu();
                break;
            
            default:
                throw new NotImplementedException($"Unhandled Pause Screen Button {_buttonType}");
        }
    }

    enum ButtonType
    {
        Continue,
        MainMenu
    }
}