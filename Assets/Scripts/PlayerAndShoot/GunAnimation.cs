using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimation : MonoBehaviour
{
    [Header("动画设置")]
    [SerializeField] private Animator gunAnimator;
    public bool isFire = false;


    private void Awake()
    {
        if (gunAnimator == null)
        {
            gunAnimator = GetComponent<Animator>();
        }
    }
    private void Update()
    {
        SetAnimation();
    }

    private void SetAnimation()
    {
        gunAnimator.SetBool("IsFire",isFire);
    }

    // 外部调用，开始射击动画
    public void StartFire()
    {
        isFire = true;
        Debug.Log("StartFire: isFire = " + isFire);
    }

    // 动画事件调用，结束射击动画
    public void EndFire()
    {
        isFire = false;
        Debug.Log("EndFire: isFire = " + isFire);
    }
}
