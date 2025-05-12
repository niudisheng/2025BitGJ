using UnityEngine;

public class LevelConfigManager : MonoBehaviour
{
    public static LevelConfigManager Instance { get; private set; }

    public LevelConfig levelConfig;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public LevelData GetLevelData(int sceneIndex)
    {
        return levelConfig.levels.Find(l => l.levelIndex == sceneIndex);
    }
}
