using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool isGameOver = false;
    private Shoot _playerShoot;
    
    public LevelConfig levelConfig;
    
    /// <summary>
    /// 当前关卡数据
    /// </summary>
    public LevelData currentLevelData;

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


    public void Victory()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("游戏胜利！所有敌人都被消灭了！");
        GameOverPanel.instance.OpenGameOverUI(true);
        LockPlayerControls();
    }

    public void Defeat()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("游戏失败！弹药耗尽了！");
        GameOverPanel.instance.OpenGameOverUI(false);
        LockPlayerControls();
    }


    private void LockPlayerControls()
    {
        // 禁用玩家输入
        var playerInput = FindObjectOfType<PlayerInput>(true);
        if (playerInput != null)
        {
            playerInput.enabled = false;
            playerInput.DeactivateInput();
        }

        // 动态获取 Shoot 组件并禁用
        Shoot shoot = GetPlayerShoot();
        if (shoot != null)
        {
            shoot.enabled = false;
            shoot.inputControl.Disable();
            Debug.Log("已禁用射击");
        }
    }

    private Shoot GetPlayerShoot()
    {
        if (_playerShoot == null)
        {
            _playerShoot = FindObjectOfType<Shoot>(true); // 包含隐藏对象
            if (_playerShoot == null)
            {
                Debug.LogWarning("未找到 Shoot 组件！");
            }
        }

        return _playerShoot;
    }

    /// <summary>
    /// 重置游戏
    /// </summary>
    public void ResetGame(int sceneIndex)
    {
        isGameOver = false;
        
        // 初始化当前关卡数据
        // int sceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        currentLevelData = LevelConfigManager.Instance.GetLevelData(sceneIndex);
        if (currentLevelData == null)
        {
            Debug.LogWarning($"未能找到当前场景（index: {sceneIndex}）的关卡配置！");
        }

        // 重新启用控制
        var playerInput = FindObjectOfType<PlayerInput>(true);
        if (playerInput != null) playerInput.enabled = true;

        Shoot shoot = GetPlayerShoot();
        if (shoot != null)
        {
            shoot.enabled = true;
            shoot.inputControl.Enable();
            shoot.ResetAmmo();
            Debug.Log("已启用射击");
        }
        BulletManager.Instance?.Reset();
        BulletManager.Instance?.RecreatePools();
    }

    public void LoadChapter(int sceneIndex)
    {
        if (sceneIndex > levelConfig.levels[-1].levelIndex)
        {
            LoadChapter(levelConfig.levels[-1].levelIndex);
            return;
        }
        SceneLoadManager.Instance.LoadScene(sceneIndex);
        GameManager.Instance.ResetGame(sceneIndex);
        MyEventManager.Instance.EventTrigger(EventName.LoadChapter);
    }

}