using TMPro;
using UnityEngine;

public class EnemyCounter : MonoBehaviour
{
    public TMP_Text enemyCountText;
    private int enemyCount = 0;

    private void Start()
    {
        UpdateEnemyCount(); // 初始化时强制统计所有敌人
    }

    // 强制更新计数（全场景统计）
    public void UpdateEnemyCount()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        enemyCount = enemies.Length;
        UpdateUI();
    }

    // 单个敌人死亡时调用（高效递减）
    public void EnemyDestroyed()
    {
        enemyCount--;
        UpdateUI();
    }

    // 统一更新UI显示
    private void UpdateUI()
    {
        if (enemyCountText != null)
        {
            enemyCountText.text = $"剩余敌人: {enemyCount}";
        }
    }
}