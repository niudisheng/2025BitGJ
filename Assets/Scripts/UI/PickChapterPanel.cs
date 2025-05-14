using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickChapterPanel : MonoBehaviour
{
    public Button[] ChapterButtons;
    public LevelConfig levelConfig;
    private void Start()
    {
        for (int i = 0; i < ChapterButtons.Length; i++)
        {
            var i1 = i;
            int chapterIndex = levelConfig.levels[i1].levelIndex; 
            ChapterButtons[i].onClick.AddListener(() => GameManager.Instance.LoadChapter(chapterIndex));
        }
    }

    


}
