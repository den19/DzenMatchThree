using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfiguration", menuName = "ScriptableObjects/Level Configuration")]
public class LevelConfiguration : ScriptableObject
{
    public LevelData[] levels;

    [System.Serializable]
    public class LevelData
    {
        public int rows;
        public int columns;
        public int scoreThreshold;
        public string sceneName;
    }
}
