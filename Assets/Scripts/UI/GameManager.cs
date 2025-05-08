using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool isGameOver = false;
    private Shoot playerShoot;

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
        playerShoot = FindObjectOfType<Shoot>();
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
        var playerInput = FindObjectOfType<PlayerInput>();
        if (playerInput != null
    )
        {
            playerInput.enabled = false;
            playerInput.DeactivateInput();
            // 确保输入系统被禁用
        }

        // 禁用射击
        if (playerShoot != null
    )
        {
            playerShoot.enabled = false;
            playerShoot.inputControl.Disable();
            // 显式禁用输入控制
        }
    }
    
    
    /// <summary>
    /// 重置游戏
    /// </summary>
    public void ResetGame()
    {
        isGameOver = false;

        // 重新启用控制
        var playerInput = FindObjectOfType<PlayerInput>();
        if (playerInput != null) playerInput.enabled = true;

        if (playerShoot != null) playerShoot.enabled = true;
    }
}