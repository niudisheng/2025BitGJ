using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBullet : Bullet
{
    [SerializeField] private int maxBounces = 3;  // 最大反弹次数
    private int currentBounces;                   // 当前剩余反弹次数

    private void OnEnable()
    {
        currentBounces = maxBounces;  // 每次从对象池取出时重置反弹次数
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") || collision.gameObject.layer == LayerMask.NameToLayer("StrongWall"))
        {
            if (currentBounces <= 0)
            {
                ReturnToPool();  // 如果反弹次数用完，回收子弹
                return;
            }

            PlaySpecialEffectSound(); // 播放反弹音效
            Vector2 normal = CalculateNormal(collision);
            Reflect(normal);
            currentBounces--;
            return;
        }

        if (collision.CompareTag("Enemy") || collision.CompareTag("Box"))
        {
            // 施加力，方向为子弹运动方向
            ApplyForce(collision.gameObject, rb.velocity);
            if (collision.CompareTag("Enemy"))
            {
                var enemy = collision.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.Die();
                    // 触发死亡流程
                }
            }
            ReturnToPool();
        }
    }

    //法线计算
    private Vector2 CalculateNormal(Collider2D collider)
    {
        //使用碰撞体边界计算法线
        Vector2 bulletPos = transform.position;
        Vector2 closestPoint = collider.ClosestPoint(bulletPos);
        Vector2 normal = (bulletPos - closestPoint).normalized;

        // 如果计算结果为零向量（理论上不应该发生），则使用默认向上法线
        if (normal == Vector2.zero)
        {
            normal = Vector2.up;
        }

        return normal;
    }

    // 反弹方法
    private void Reflect(Vector2 normal)
    {
        normal = normal.normalized;
        Vector2 incomingDirection = rb.velocity.normalized;
        Vector2 reflectedDirection = Vector2.Reflect(incomingDirection, normal);


        rb.velocity = reflectedDirection * speed;
        float angle = Mathf.Atan2(reflectedDirection.y, reflectedDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
