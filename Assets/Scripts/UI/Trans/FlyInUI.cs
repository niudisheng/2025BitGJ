using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FlyInUI : MonoBehaviour
{
    [Header("引用")] public RectTransform startPoint;
    public RectTransform endPoint;
    public RectTransform target; // 要飞入的UI对象

    [Header("动画参数")] public float duration = 0.5f;
    public float delay = 0f;
    public Ease easeType = Ease.OutCubic;
    public bool isFading = false;


    [Header("自动播放")] public bool playOnStart = true;

    private Vector2 originalEndPos;
    private bool isPlaying = false;

    void Start()
    {
        if (playOnStart)
            Play();
    }

    private void Update()
    {
        test();
    }

    private void test()
    {
        if (Input.GetMouseButton(1))
        {
            Debug.Log("test");
            Play();
        }
    }

    public void Play()
    {
        if (isPlaying)
        {
            Debug.LogWarning("FlyInUI: 正在播放中！");
            return;
        }


        if (target == null || startPoint == null || endPoint == null)
        {
            Debug.LogWarning("FlyInUI: 引用未设置！");
            isPlaying = false;
            return;
        }
        else
        {
            isPlaying = true;
        }

        // 记录终点
        originalEndPos = endPoint.anchoredPosition;

        // 设置起始位置
        target.anchoredPosition = startPoint.anchoredPosition;


        // 开始移动
        Sequence seq = DOTween.Sequence();
        seq.AppendInterval(delay);
        seq.Append(target.DOAnchorPos(originalEndPos, duration).SetEase(easeType));
        if (isFading)
        {
            // 可选：初始透明度 0（Fade 效果）
            CanvasGroup group = target.GetComponent<CanvasGroup>();
            if (group != null)
                group.alpha = 0;
            if (group != null)
                seq.Join(group.DOFade(1, duration));
        }

        seq.onComplete += () => { isPlaying = false; };


        seq.Play();
    }
}