using System.Collections;
using TMPro;
using UnityEngine;

public class EnemyCounter : MonoBehaviour
{
    public TMP_Text enemyCountText;
    private int enemyCount = 0;
    public static EnemyCounter Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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


        // 添加胜利检测
        if (enemyCount <= 0)
        {
            GameManager.Instance.Victory();
        }
    }

    // 单个敌人死亡时调用（高效递减）
    public void EnemyDestroyed()
    {
        enemyCount--;
        UpdateUI();
        // 添加延迟胜利检测（确保所有动画播放完成）
        if (enemyCount <= 0)
        {
            StartCoroutine(DelayedVictoryCheck());
        }
    }

    private IEnumerator DelayedVictoryCheck()
    {
        // 等待一帧确保所有敌人销毁逻辑完成
        yield return null;

        // 再次检查（防止竞争条件）
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
        {
            GameManager.Instance.Victory();
        }
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