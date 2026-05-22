using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public LevelConfiguration levelConfiguration;
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
                    new LevelConfiguration.LevelData { rows = 6, columns = 6, scoreThreshold = 700, sceneName = "Level1" },
                    new LevelConfiguration.LevelData { rows = 8, columns = 8, scoreThreshold = 1500, sceneName = "Level2" },
                    new LevelConfiguration.LevelData { rows = 10, columns = 10, scoreThreshold = 2200, sceneName = "Level3" }
                };
            }
        }
        else
        {
            Destroy(gameObject);
        }
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
        Application.Quit();
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
