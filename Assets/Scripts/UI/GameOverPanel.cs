using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    public static GameOverPanel instance;
    public Button BackToPickButton;
    public GameObject WinPanel;
    public GameObject LosePanel;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        BackToPickButton.onClick.AddListener(BackToPick);
        WinPanel.SetActive(false);
        LosePanel.SetActive(false);
    }

    private void BackToPick()
    {
        SceneLoadManager.Instance.LoadPickPanel();
        WinPanel.SetActive(false);
        LosePanel.SetActive(false);
    }
    
    /// <summary>
    /// 打开游戏结束界面,胜利或失败
    /// </summary>
    /// <param name="isWin"></param>
    public void OpenGameOverUI(bool isWin)
    {
        // TODO: 打开游戏结束界面
        Debug.Log("打开游戏结束界面");
        if (isWin)
        {
            WinPanel.SetActive(true);
        }
        else
        {
            LosePanel.SetActive(true);
        }
    }
}
