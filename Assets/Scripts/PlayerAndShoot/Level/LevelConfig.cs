using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "Configs/LevelConfig", order = 1)]
public class LevelConfig : ScriptableObject
{
    public List<LevelData> levels = new List<LevelData>();
}

[System.Serializable]
public class LevelData
{
    public int levelIndex;

    public int normalAmmo;
    public int bombAmmo;
    public int penetratingAmmo;
    public int destroyWallAmmo;

    [TextArea]
    public string guideMessage;//教学字幕
}
