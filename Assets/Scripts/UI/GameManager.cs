using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool isGameOver = false;
    private Shoot _playerShoot;

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
        LockPlayerControls();
    }

    public void Defeat()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("游戏失败！弹药耗尽了！");
        LockPlayerControls();
    }


    private void LockPlayerControls()
    {
        // 禁用玩家输入
        var playerInput = FindObjectOfType<PlayerInput>(true);
        if (playerInput != null)
        {
            playerInput.enabled =false;
            playerInput.DeactivateInput();
        }

        // 动态获取 Shoot 组件并禁用
        Shoot shoot = GetPlayerShoot();
        if (shoot != null)
        {
            shoot.enabled =false;
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
    public void ResetGame()
    {
        isGameOver = false;

        // 重新启用控制
        var playerInput = FindObjectOfType<PlayerInput>(true);
        if (playerInput != null) playerInput.enabled = true;

        // 动态获取 Shoot 组件并启用
        Shoot shoot = GetPlayerShoot();
        if (shoot != null)
        {
            shoot.enabled =true;            
            shoot.inputControl.Enable();
            Debug.Log("已启用射击");
        }
    }
}