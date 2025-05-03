using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBullet : Bullet
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            // 普通子弹中处理反弹逻辑
            ContactPoint2D[] contacts = new ContactPoint2D[1];
            collision.GetContacts(contacts);
            if (contacts.Length > 0)
                Reflect(contacts[0].normal);
            return;
        }

        if (collision.CompareTag("Enemy") || collision.CompareTag("Box"))
        {
            // 施加力，方向为子弹运动方向
            ApplyForce(collision.gameObject, rb.velocity);
            if (collision.CompareTag("Enemy")) Destroy(collision.gameObject);
            ReturnToPool();
        }
    }

    // 反弹方法
    private void Reflect(Vector2 normal)
    {
        Vector2 direction = Vector2.Reflect(rb.velocity.normalized, normal);
        rb.velocity = direction * speed;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
