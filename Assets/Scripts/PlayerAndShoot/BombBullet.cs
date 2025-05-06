using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBullet : Bullet
{
    [SerializeField] private float explosionRadius = 3f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Explode();
        ReturnToPool();
    }

    private void Explode()
    {
        PlaySpecialEffectSound(); // 播放爆炸音效
        //检测爆炸范围内碰撞体
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in colliders)
        {
            if (hit.CompareTag("Enemy") || hit.CompareTag("Box"))
            {
                // 爆炸力的方向是从爆炸中心指向目标
                Vector2 forceDirection = hit.transform.position - transform.position;
                ApplyForce(hit.gameObject, forceDirection);
                if (hit.CompareTag("Enemy"))
                {
                    var enemy = hit.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.Die();
                    }
                }
            }
        }
    }

}
