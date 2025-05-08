using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("死亡设置")]
    [SerializeField] protected Animator animator;
    [SerializeField] protected AudioClip deathSound;

    private EnemyCounter enemyCounter;

    protected bool isDying = false;

    private void Start()
    {
        // 获取敌人计数器引用
        enemyCounter = FindObjectOfType<EnemyCounter>();
    }

    // 由Animation Event调用
    public void OnDeathAnimationEnd()
    {
        // 通知计数器
        if (enemyCounter != null)
        {
            enemyCounter.EnemyDestroyed();
        }
        Destroy(gameObject);
    }

    public virtual void Die()
    {
        if (isDying) return;

        isDying = true;

        // 禁用碰撞体和物理效果
        var collider = GetComponent<Collider2D>();
        if (collider != null) collider.enabled = false;

        var rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.simulated = false;


        // 播放死亡动画
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // 播放死亡音效
        if (deathSound != null)
        {
            SoundManager.Instance.PlaySound(deathSound);
        }
    }
}
