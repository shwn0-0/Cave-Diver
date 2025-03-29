using System;
using UnityEngine;

class MainMenuButton : MonoBehaviour
{
    [SerializeField] private ButtonType _buttonType;    

    public void OnButtonClick()
    {
        switch (_buttonType)
        {
            case ButtonType.Play:
                SceneController.Instance.GoToMainScene();
                break;
            case ButtonType.Demo:
                SceneController.Instance.GoToMainScene(true);
                break;
            case ButtonType.Quit:
#if UNITY_STANDALONE
                Application.Quit();
#endif
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                break;
            default:
                throw new NotImplementedException($"Unhandled Main Menu Button {_buttonType}");
        }
    }

    enum ButtonType
    {
        Play,
        Demo,
        Quit
    }
}