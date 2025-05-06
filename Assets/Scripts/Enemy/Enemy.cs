using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("��������")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected AudioClip deathSound;

    protected bool isDying = false;

    // ��Animation Event����
    public void OnDeathAnimationEnd()
    {
        Destroy(gameObject);
    }

    public virtual void Die()
    {
        if (isDying) return;

        isDying = true;

        // ������ײ�������Ч��
        var collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;

        // ������������
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // ����������Ч
        if (deathSound != null)
        {
            SoundManager.Instance.PlaySound(deathSound);
        }
    }
}
