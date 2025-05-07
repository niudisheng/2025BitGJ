using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimation : MonoBehaviour
{
    [Header("��������")]
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

    // �ⲿ���ã���ʼ�������
    public void StartFire()
    {
        isFire = true;
        Debug.Log("StartFire: isFire = " + isFire);
    }

    // �����¼����ã������������
    public void EndFire()
    {
        isFire = false;
        Debug.Log("EndFire: isFire = " + isFire);
    }
}
