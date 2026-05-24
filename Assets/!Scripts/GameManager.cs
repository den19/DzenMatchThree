using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("Настройки вибрации")]
    [Tooltip("Продолжительность вибрации при прохождении уровня (в секундах)")]
    public float vibrationDuration = 10f;

    [Header("Настройки UI")]
    [Tooltip("Шрифт для динамически создаваемых окон победы. Если не задан, выберется встроенный шрифт.")]
    public Font defaultUiFont;

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
            // Предотвращаем многократный повторный вызов при продолжающихся матчах
            if (GameObject.Find("CompletionOverlay") != null) return;

            bool isLastLevel = (currentLevelIndex >= levelConfiguration.levels.Length - 1);
            if (isLastLevel)
            {
                ShowCompletionPopup("Вы прошли игру! Победили своей усидчивостью алгоритмы усидчивого разработчика игры! Вы - молодец\\умница!", true);
            }
            else
            {
                ShowCompletionPopup($"Вы прошли уровень Уровень {currentLevelIndex + 1}!", false);
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

    private void ShowCompletionPopup(string message, bool isGameFinished)
    {
        // Находим Canvas в сцене
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("Canvas не найден в сцене!");
            return;
        }

        // Безопасный поиск шрифта для UI
        Font activeFont = defaultUiFont;
        if (activeFont == null)
        {
            try
            {
                activeFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            }
            catch {}
        }
        if (activeFont == null)
        {
            try
            {
                activeFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            }
            catch {}
        }
        if (activeFont == null)
        {
            Font[] allFonts = Resources.FindObjectsOfTypeAll<Font>();
            if (allFonts != null && allFonts.Length > 0)
            {
                activeFont = allFonts[0];
            }
        }

        if (activeFont == null)
        {
            Debug.LogWarning("Не удалось загрузить ни один шрифт для UI!");
        }

        // 1. Фоновый оверлей, блокирующий клики
        GameObject overlayGo = new GameObject("CompletionOverlay");
        overlayGo.transform.SetParent(canvas.transform, false);
        
        RectTransform overlayRt = overlayGo.AddComponent<RectTransform>();
        overlayRt.anchorMin = Vector2.zero;
        overlayRt.anchorMax = Vector2.one;
        overlayRt.sizeDelta = Vector2.zero;

        Image overlayImg = overlayGo.AddComponent<Image>();
        overlayImg.color = new Color(0f, 0f, 0f, 0.85f); // Стильный глубокий полупрозрачный черный
        overlayImg.raycastTarget = true; // Блокируем клики под ним

        // 2. Карточка диалогового окна
        GameObject cardGo = new GameObject("PopupCard");
        cardGo.transform.SetParent(overlayGo.transform, false);
        
        RectTransform cardRt = cardGo.AddComponent<RectTransform>();
        cardRt.anchorMin = new Vector2(0.5f, 0.5f);
        cardRt.anchorMax = new Vector2(0.5f, 0.5f);
        cardRt.sizeDelta = new Vector2(850f, 550f); // Оптимальные пропорции для Tecno Pova

        Image cardImg = cardGo.AddComponent<Image>();
        cardImg.color = new Color(0.12f, 0.12f, 0.16f, 0.95f); // Стильный глубокий темно-синий ночной цвет

        // Красивая золотая обводка к карточке
        Outline cardOutline = cardGo.AddComponent<Outline>();
        cardOutline.effectColor = new Color(1f, 0.84f, 0f, 0.8f); // Золотой контур
        cardOutline.effectDistance = new Vector2(4f, 4f);

        // 3. Текст сообщения
        GameObject textGo = new GameObject("MessageText");
        textGo.transform.SetParent(cardGo.transform, false);
        
        RectTransform textRt = textGo.AddComponent<RectTransform>();
        textRt.anchorMin = new Vector2(0.05f, 0.35f);
        textRt.anchorMax = new Vector2(0.95f, 0.95f);
        textRt.sizeDelta = Vector2.zero;

        Text textComp = textGo.AddComponent<Text>();
        if (activeFont != null)
        {
            textComp.font = activeFont;
        }
        textComp.fontSize = 32;
        textComp.fontStyle = FontStyle.Bold;
        textComp.color = Color.white;
        textComp.alignment = TextAnchor.MiddleCenter;
        textComp.horizontalOverflow = HorizontalWrapMode.Wrap;
        textComp.verticalOverflow = VerticalWrapMode.Truncate;
        textComp.text = message;

        // 4. Кнопка "Ок"
        GameObject btnGo = new GameObject("OkButton");
        btnGo.transform.SetParent(cardGo.transform, false);
        
        RectTransform btnRt = btnGo.AddComponent<RectTransform>();
        btnRt.anchorMin = new Vector2(0.5f, 0.18f);
        btnRt.anchorMax = new Vector2(0.5f, 0.18f);
        btnRt.sizeDelta = new Vector2(280f, 90f); // Крупная удобная кнопка
        btnRt.anchoredPosition = Vector2.zero;

        Image btnImg = btnGo.AddComponent<Image>();
        btnImg.color = new Color(1f, 0.84f, 0f, 1f); // Золотая кнопка
        
        Button btnComp = btnGo.AddComponent<Button>();
        ColorBlock btnColors = btnComp.colors;
        btnColors.normalColor = new Color(1f, 0.84f, 0f, 1f);
        btnColors.highlightedColor = new Color(1f, 0.9f, 0.3f, 1f);
        btnColors.pressedColor = new Color(0.8f, 0.65f, 0f, 1f);
        btnComp.colors = btnColors;

        Outline btnOutline = btnGo.AddComponent<Outline>();
        btnOutline.effectColor = Color.black;
        btnOutline.effectDistance = new Vector2(2f, -2f);

        // Текст на кнопке Ок
        GameObject btnTextGo = new GameObject("OkText");
        btnTextGo.transform.SetParent(btnGo.transform, false);
        
        RectTransform btnTextRt = btnTextGo.AddComponent<RectTransform>();
        btnTextRt.anchorMin = Vector2.zero;
        btnTextRt.anchorMax = Vector2.one;
        btnTextRt.sizeDelta = Vector2.zero;

        Text btnTextComp = btnTextGo.AddComponent<Text>();
        if (activeFont != null)
        {
            btnTextComp.font = activeFont;
        }
        btnTextComp.fontSize = 28;
        btnTextComp.fontStyle = FontStyle.Bold;
        btnTextComp.color = Color.black; // Контрастный черный
        btnTextComp.alignment = TextAnchor.MiddleCenter;
        btnTextComp.text = "Ок";

        // 5. Навешиваем событие клика
        btnComp.onClick.AddListener(() => {
            StopVibration();
            Destroy(overlayGo);

            if (isGameFinished)
            {
                LoadMainMenu();
            }
            else
            {
                LoadNextLevel();
            }
        });

        // 6. Запуск вибрации
        TriggerVibration();
    }

    private void TriggerVibration()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                if (currentActivity != null)
                {
                    AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
                    if (vibrator != null)
                    {
                        long durationMs = (long)(vibrationDuration * 1000f);
                        
                        // Получаем версию SDK Android
                        AndroidJavaClass buildVersion = new AndroidJavaClass("android.os.Build$VERSION");
                        int sdkInt = buildVersion.GetStatic<int>("SDK_INT");

                        if (sdkInt >= 26) // Android 8.0+
                        {
                            AndroidJavaClass vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
                            int defaultAmplitude = vibrationEffectClass.GetStatic<int>("DEFAULT_AMPLITUDE");
                            AndroidJavaObject vibrationEffect = vibrationEffectClass.CallStatic<AndroidJavaObject>("createOneShot", durationMs, defaultAmplitude);
                            vibrator.Call("vibrate", vibrationEffect);
                        }
                        else
                        {
                            vibrator.Call("vibrate", durationMs);
                        }
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error triggering Android vibration: " + e.Message);
        }
#else
        // В редакторе используем легкое встроенное вибро
        Handheld.Vibrate();
#endif
    }

    private void StopVibration()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                if (currentActivity != null)
                {
                    AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
                    if (vibrator != null)
                    {
                        vibrator.Call("cancel");
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error stopping Android vibration: " + e.Message);
        }
#endif
    }

    /// <summary>
    /// Этот фиктивный метод никогда не будет вызван во время игры, но его наличие в коде
    /// заставляет компилятор Unity автоматически добавить разрешение <uses-permission android:name="android.permission.VIBRATE" />
    /// в генерируемый по умолчанию AndroidManifest.xml. Благодаря этому иконка игры останется на месте,
    /// а вибрация будет работать без необходимости ручного редактирования XML манифестов.
    /// </summary>
    private void DummyVibrationTrick()
    {
#if UNITY_ANDROID
        if (UnityEngine.Time.time < -1000f)
        {
            Handheld.Vibrate();
        }
#endif
    }
}
