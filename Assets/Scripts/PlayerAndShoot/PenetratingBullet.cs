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
            PlaySpecialEffectSound(); // ���Ŵ�͸��Ч
            if (collision.CompareTag("Enemy"))
            {
                var enemy = collision.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.Die();
                    // ������������
                }
            }
        }
    }
}
