using UnityEngine;
using UnityEngine.UI; // подключение пространства имен для компонента Text

public class DisplayCitation : MonoBehaviour
{
    public Text citationText; // ссылку теперь делаем на стандартный компонент Text
    LoadCitations citationsLoader;

    private float timer = 0f;
    private const float interval = 20f; // интервал в секундах (каждую минуту)

    void Start()
    {
        citationsLoader = FindObjectOfType<LoadCitations>();
        ShowRandomCitation(); // сразу покажем первую цитату
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= interval)
        {
            ShowRandomCitation();
            timer -= interval; // сбросим таймер обратно
        }
    }

    void ShowRandomCitation()
    {
        if (citationsLoader != null && citationText != null)
        {
            citationText.text = citationsLoader.GetRandomCitation();
        }
    }
}