using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimation : MonoBehaviour
{
    [Header("��������")]
    [SerializeField] private Animator gunAnimator;


    private void Awake()
    {
        if (gunAnimator == null)
        {
            gunAnimator = GetComponent<Animator>();
        }
    }
    private void Update()
    {
        
    }

    
}
