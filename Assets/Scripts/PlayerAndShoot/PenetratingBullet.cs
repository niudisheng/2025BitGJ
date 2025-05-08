using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenetratingBullet : Bullet
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") || collision.gameObject.layer == LayerMask.NameToLayer("StrongWall"))
        {
            ReturnToPool();
            return;
        }


        if (collision.CompareTag("Enemy"))
        {
            PlaySpecialEffectSound(); // 播放穿透音效
            var enemy = collision.GetComponent<Enemy.Enemy>();
            if (enemy != null)
            {
                enemy.Die();
                // 触发死亡流程
            }
        }
        
    }
}
