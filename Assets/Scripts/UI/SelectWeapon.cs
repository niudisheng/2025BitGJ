using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // 别忘了引入 UI 命名空间

public class SelectWeapon : MonoBehaviour
{
    public List<RectTransform> weapons;        // 武器按钮列表
    public RectTransform selector;             // 高亮框
    public List<Sprite> animationSprites;      // 帧动画精灵序列
    public float frameRate = 10f;              // 每秒播放几帧
    public Image animationImage;               // 显示帧动画的 Image 组件

    private Coroutine currentAnimation;        // 保存当前协程，用于停止

    private void Start()
    {
        selector = this.GetComponent<RectTransform>(); // 找到高亮框
        animationImage = this.GetComponent<Image>(); // 找到帧动画 Image 组件
        PlayAnimation();
    }

    [ContextMenu("Select Weapon")]
    public void SelectWeapon1()
    {
        Select(1);
    }

    public void Select(int index)
    {
        if (index >= 0 && index < weapons.Count)
        {
            // 移动 selector 到目标武器位置
            selector.anchoredPosition = weapons[index].anchoredPosition;



            
        }
    }

    private void Update()
    {
        
    }

    private void PlayAnimation()
    {
        // 播放帧动画
        if (currentAnimation != null)
            StopCoroutine(currentAnimation);

        currentAnimation = StartCoroutine(PlayFrameAnimation());
    }


    private IEnumerator PlayFrameAnimation()
    {
        if (animationSprites == null || animationSprites.Count == 0 || animationImage == null)
            yield break;

        int index = 0;
        float delay = 1f / frameRate;

        while (true)
        {
            animationImage.sprite = animationSprites[index];
            index = (index + 1) % animationSprites.Count;
            yield return new WaitForSeconds(delay);
        }
    }
}