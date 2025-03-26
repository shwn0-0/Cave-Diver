using System;
using UnityEngine;

class MainMenuButton : MonoBehaviour
{
    [SerializeField] private Button button;
    private SceneController _sceneController;

    void Awake()
    {
        _sceneController = FindAnyObjectByType<SceneController>();
    }

    public void OnButtonClick()
    {
        switch (button)
        {
            case Button.Play:
                _sceneController.GoToMainScene();
                break;
            case Button.Demo:
                _sceneController.GoToMainScene(true);
                break;
            case Button.Quit:
                throw new NotImplementedException();
            default:
                Debug.LogError("Unhandled MainMenu Button");
                break;
        }
    }

    enum Button
    {
        Play,
        Demo,
        Quit
    }
}