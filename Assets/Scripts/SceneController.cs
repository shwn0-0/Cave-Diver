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


    public void OnMainMenuButtonClick(string btn)
    {
        switch (btn)
        {
            case "Demo":
                IsDemoMode = true;
                SceneManager.LoadSceneAsync("MainScene");
                break;
            case "Play":
                SceneManager.LoadSceneAsync("MainScene");
                break;
            case "Quit":
                // TODO: Handle quit
                break;
            default:
                Debug.LogError("Unhandled MainMenu Button");
                break;
        }
    }
}
