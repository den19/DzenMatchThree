using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LoadCitations : MonoBehaviour
{
    public string resourcePath = "Citations/Citations"; // название ресурса без расширения
    List<string> citationsList = new List<string>();

    void Start()
    {
        ReadFile();
    }

    void ReadFile()
    {
        TextAsset textAsset = Resources.Load(resourcePath) as TextAsset;
        if (textAsset != null)
        {
            string[] lines = textAsset.text.Split('\n');
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                if (!string.IsNullOrEmpty(trimmedLine))
                    citationsList.Add(trimmedLine);
            }
        }
        else
        {
            Debug.LogError("Файл цитат не найден!");
        }
    }

    public string GetRandomCitation()
    {
        return citationsList.Count > 0 ? citationsList[Random.Range(0, citationsList.Count)] : "";
    }
}