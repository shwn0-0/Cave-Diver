using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }
    public bool IsDemoMode { get; private set; }

    void OnEnable()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
    }

    public void GoToMainScene(bool IsDemo = false)
    {
        IsDemoMode = IsDemo;
        SceneManager.LoadSceneAsync("MainScene");
    }

    public void GoToMainMenu() =>
        SceneManager.LoadSceneAsync("MainMenu");
}
