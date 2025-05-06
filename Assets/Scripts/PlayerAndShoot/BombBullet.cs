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
        PlaySpecialEffectSound(); // ���ű�ը��Ч
        //��ⱬը��Χ����ײ��
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in colliders)
        {
            if (hit.CompareTag("Enemy") || hit.CompareTag("Box"))
            {
                // ��ը���ķ����Ǵӱ�ը����ָ��Ŀ��
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
