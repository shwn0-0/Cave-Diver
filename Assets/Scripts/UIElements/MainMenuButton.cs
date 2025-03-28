using System;
using UnityEngine;

class MainMenuButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    private SceneController _sceneController;

    void Awake()
    {
        _sceneController = FindAnyObjectByType<SceneController>();
    }

    public void OnButtonClick()
    {
        switch (_button)
        {
            case Button.Play:
                _sceneController.GoToMainScene();
                break;
            case Button.Demo:
                _sceneController.GoToMainScene(true);
                break;
            case Button.Quit:
#if UNITY_STANDALONE
                Application.Quit();
#endif
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
                break;
            default:
                throw new NotImplementedException($"Unhandled Main Menu Button {_button}");
        }
    }

    enum Button
    {
        Play,
        Demo,
        Quit
    }
}