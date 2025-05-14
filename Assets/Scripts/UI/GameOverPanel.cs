using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    public static GameOverPanel instance;
    [Header("返回关卡选择")]
    public Button BackToPickButton;
    [Header("下一关")]
    public Button NextLevelButton;
    [Header("重开")]
    public Button RetryButton;
    public GameObject WinPanel;
    public GameObject LosePanel;
    public GameObject ButtonGroup;
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
        SetButtonFunc(BackToPickButton, BackToPick);
        SetButtonFunc(NextLevelButton, NextLevel);
        SetButtonFunc(RetryButton, ResetLevel);
        WinPanel.SetActive(false);
        LosePanel.SetActive(false);
        ButtonGroup.SetActive(false);
    }

    private void SetButtonFunc(Button button, UnityAction action)
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
        button.onClick.AddListener(() => { ButtonGroup.SetActive(false); });
    }

    private void BackToPick()
    {
        WinPanel.SetActive(false);
        LosePanel.SetActive(false);
        ButtonGroup.SetActive(false);
        SceneLoadManager.Instance.LoadPickPanel();
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
        ButtonGroup.SetActive(true);
    }



    /// <summary>
    /// 重置关卡
    /// </summary>
    public void ResetLevel()
    {
        WinPanel.SetActive(false);
        LosePanel.SetActive(false);
        ButtonGroup.SetActive(false);
        GameManager.Instance.LoadChapter(GameManager.Instance.currentLevelData.levelIndex);
    }


    /// <summary>
    /// 下一关
    /// </summary>
    public void NextLevel()
    {
        WinPanel.SetActive(false);
        LosePanel.SetActive(false);
        ButtonGroup.SetActive(false);
        GameManager.Instance.LoadChapter(GameManager.Instance.currentLevelData.GetNextLevelIndex());
    }
}
