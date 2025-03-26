using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public bool IsDemoMode { get; private set; }


    void Awake()
    {
        // Singleton
        SceneController[] controllers = FindObjectsByType<SceneController>(FindObjectsSortMode.None);
        if (controllers.Length > 1) Destroy(gameObject); 
        else DontDestroyOnLoad(gameObject);
    }

    public void GoToMainScene(bool IsDemo = false)
    {
        IsDemoMode = IsDemo;
        SceneManager.LoadSceneAsync("MainScene");
    }

    public void GoToMainMenu() =>
        SceneManager.LoadSceneAsync("MainMenu");
}
