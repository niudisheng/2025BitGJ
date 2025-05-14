using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Tilemaps;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    [SerializeField] protected float impactForce = 5f;
    protected Rigidbody2D rb;
    private IObjectPool<Bullet> _pool;

    [Header("音效设置")]
    [SerializeField] protected AudioClip shootClip; // 发射音效
    [SerializeField] protected AudioClip specialEffectClip; // 特殊效果音效

    private float lifeTime = 20f; // 子弹生命周期，单位秒
    private float lifeTimer = 0f; // 当前计时器

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetPool(IObjectPool<Bullet> pool) => _pool = pool;

    public virtual void Launch()
    {
        rb.velocity = transform.right * speed;
        PlayShootSound();
        lifeTimer = lifeTime;  // 启动生命周期计时
    }

    protected virtual void Update()
    {
        if (lifeTimer > 0)
        {
            lifeTimer -= Time.deltaTime;
            if (lifeTimer <= 0)
            {
                ReturnToPool(); // 超过生命周期后回池
            }
        }
    }

    protected virtual void ReturnToPool()
    {
        rb.velocity = Vector2.zero;
        // 通知 BulletManager 子弹已销毁
        BulletManager.Instance.UnregisterBullet();

        _pool?.Release(this);
    }

    protected void ApplyForce(GameObject target, Vector2 forceDirection)
    {
        if (target.TryGetComponent<Rigidbody2D>(out var targetRb))
        {
            targetRb.AddForce(forceDirection.normalized * impactForce, ForceMode2D.Impulse);
        }
    }

    protected void PlayShootSound()
    {
        if (shootClip != null)
        {
            SoundManager.Instance.PlaySound(shootClip);
        }
    }

    protected void PlaySpecialEffectSound()
    {
        if (specialEffectClip != null)
        {
            SoundManager.Instance.PlaySound(specialEffectClip);
        }
    }
}

