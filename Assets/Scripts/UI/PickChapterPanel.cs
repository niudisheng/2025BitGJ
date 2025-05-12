using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickChapterPanel : MonoBehaviour
{
    public Button[] ChapterButtons;

    private void Start()
    {
        for (int i = 0; i < ChapterButtons.Length; i++)
        {
            var i1 = i;
            ChapterButtons[i].onClick.AddListener(() => LoadChapter(i1+1));
        }
    }

    private void LoadChapter(int chapterIndex)
    {
        int sceneIndex = chapterIndex + 2;
        SceneLoadManager.Instance.LoadScene(sceneIndex);
        GameManager.Instance.ResetGame();

    }


}
