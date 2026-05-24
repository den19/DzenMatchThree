using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public LevelConfiguration levelConfiguration;

    [Header("Настройки лимитов очков (Баланс)")]
    [Tooltip("Количество очков для прохождения Уровня 1")]
    public int level1ScoreThreshold = 1400;

    [Tooltip("Количество очков для прохождения Уровня 2")]
    public int level2ScoreThreshold = 3000;

    [Tooltip("Количество очков для прохождения Уровня 3")]
    public int level3ScoreThreshold = 4400;

    private int currentLevelIndex = 0;
    private int currentScore = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Создаем конфигурацию по умолчанию, если она не задана
            if (levelConfiguration == null)
            {
                levelConfiguration = ScriptableObject.CreateInstance<LevelConfiguration>();
                levelConfiguration.levels = new LevelConfiguration.LevelData[]
                {
                    new LevelConfiguration.LevelData { rows = 6, columns = 6, scoreThreshold = level1ScoreThreshold, sceneName = "Level1", enableHints = false },
                    new LevelConfiguration.LevelData { rows = 7, columns = 6, scoreThreshold = level2ScoreThreshold, sceneName = "Level2", enableHints = true },
                    new LevelConfiguration.LevelData { rows = 8, columns = 6, scoreThreshold = level3ScoreThreshold, sceneName = "Level3", enableHints = false }
                };
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }

    public LevelConfiguration.LevelData GetCurrentLevelConfig()
    {
        if (levelConfiguration != null && currentLevelIndex < levelConfiguration.levels.Length)
        {
            return levelConfiguration.levels[currentLevelIndex];
        }
        return null;
    }

    public void SetScore(int score)
    {
        currentScore = score;
        CheckLevelProgression();
    }

    public int GetCurrentScore()
    {
        return currentScore;
    }

    private void CheckLevelProgression()
    {
        LevelConfiguration.LevelData currentLevel = GetCurrentLevelConfig();
        if (currentLevel != null && currentScore >= currentLevel.scoreThreshold)
        {
            if (currentLevelIndex < levelConfiguration.levels.Length - 1)
            {
                LoadNextLevel();
            }
            else
            {
                Debug.Log("Поздравляем! Все уровни пройдены!");
            }
        }
    }

    public void LoadNextLevel()
    {
        currentLevelIndex++;
        currentScore = 0;
        LoadLevel(currentLevelIndex);
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelConfiguration != null && levelIndex >= 0 && levelIndex < levelConfiguration.levels.Length)
        {
            currentLevelIndex = levelIndex;
            currentScore = 0;
            SceneManager.LoadScene(levelConfiguration.levels[levelIndex].sceneName);
        }
    }

    public void LoadMainMenu()
    {
        currentLevelIndex = 0;
        currentScore = 0;
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartCurrentLevel()
    {
        currentScore = 0;
        if (levelConfiguration != null && currentLevelIndex < levelConfiguration.levels.Length)
        {
            SceneManager.LoadScene(levelConfiguration.levels[currentLevelIndex].sceneName);
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                if (activity != null)
                {
                    activity.Call("finishAndRemoveTask");
                    return;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error trying to finish and remove task: " + e.Message);
        }
#endif

        Application.Quit();
    }
}
