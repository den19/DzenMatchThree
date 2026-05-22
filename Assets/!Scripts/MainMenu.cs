using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button startButton;
    public Button settingsButton;
    public Button aboutButton;
    public Button quitButton;

    void Start()
    {
        Debug.Log("MainMenu Start() called");
        
        if (startButton != null)
        {
            Debug.Log("Start button found, adding listener");
            startButton.onClick.AddListener(StartGame);
        }
        else
        {
            Debug.LogError("Start button is NULL! Assign it in the Inspector.");
        }
        
        if (settingsButton != null)
        {
            Debug.Log("Settings button found, adding listener");
            settingsButton.onClick.AddListener(OpenSettings);
        }
        else
        {
            Debug.LogError("Settings button is NULL! Assign it in the Inspector.");
        }

        if (aboutButton != null)
        {
            Debug.Log("About button found, adding listener");
            aboutButton.onClick.AddListener(OpenAbout);
        }
        else
        {
            Debug.LogWarning("About button is NULL! Assign it in the Inspector if needed.");
        }
        
        if (quitButton != null)
        {
            Debug.Log("Quit button found, adding listener");
            quitButton.onClick.AddListener(QuitGame);
        }
        else
        {
            Debug.LogError("Quit button is NULL! Assign it in the Inspector.");
        }
    }

    void StartGame()
    {
        Debug.Log("StartGame() called");
        if (GameManager.Instance != null)
        {
            Debug.Log("GameManager found, loading level 0");
            GameManager.Instance.LoadLevel(0);
        }
        else
        {
            Debug.LogError("GameManager.Instance is NULL! Make sure GameManager exists in the scene.");
        }
    }

    void OpenSettings()
    {
        Debug.Log("OpenSettings() called - Settings menu not implemented yet");
    }

    void OpenAbout()
    {
        Debug.Log("OpenAbout() called - About dialog not implemented yet");
    }

    void QuitGame()
    {
        Debug.Log("QuitGame() called");
        if (GameManager.Instance != null)
        {
            GameManager.Instance.QuitGame();
        }
        else
        {
            Application.Quit();
            
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
