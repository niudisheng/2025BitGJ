using UnityEngine;
using DG.Tweening;
using System;

public class UIFader : MonoBehaviour
{
    [Header("引用")]
    public CanvasGroup canvasGroup;

    [Header("默认动画参数")]
    public float fadeDuration = 0.5f;
    public float startDelay = 1f;
    public bool fadeInOnStart = true;

    [Header("自动隐藏")]
    public bool deactivateOnFadeOut = true;

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        // 初始化为不可见（如果不自动播放）
        if (!fadeInOnStart)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    void Start()
    {
        if (fadeInOnStart)
        {
            FadeIn(startDelay);
        }
    }

    // 渐入
    public void FadeIn(float delay = 0f, Action onComplete = null)
    {
        gameObject.SetActive(true); // 确保UI可见
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        canvasGroup.DOFade(1f, fadeDuration)
            .SetDelay(delay)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => { onComplete?.Invoke(); });
    }

    // 渐出
    public void FadeOut(float delay = 0f, Action onComplete = null)
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        canvasGroup.DOFade(0f, fadeDuration)
            .SetDelay(delay)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                if (deactivateOnFadeOut)
                    gameObject.SetActive(false);
                onComplete?.Invoke();
            });
    }
}