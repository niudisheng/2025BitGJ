using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Configs/LevelConfig", order = 1)]
public class LevelConfig : ScriptableObject
{
    public List<LevelData> levels = new List<LevelData>();
}


// 单一关的数据最好拿出来也做成一个SO
[System.Serializable]
public class LevelData
{
    [Tooltip("Scene Build Index")]
    public int levelIndex;

    public int normalAmmo;
    public int bombAmmo;
    public int penetratingAmmo;
    public int destroyWallAmmo;

    [TextArea]
    public string guideMessage;//教学字幕
    
    
    public int GetNextLevelIndex()
    {
        return levelIndex+1;
    }
}
