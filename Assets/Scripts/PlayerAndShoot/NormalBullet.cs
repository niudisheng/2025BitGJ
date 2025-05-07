using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBullet : Bullet
{
    [SerializeField] private int maxBounces = 3;  // ��󷴵�����
    private int currentBounces;                   // ��ǰʣ�෴������

    private void OnEnable()
    {
        currentBounces = maxBounces;  // ÿ�δӶ����ȡ��ʱ���÷�������
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") || collision.gameObject.layer == LayerMask.NameToLayer("StrongWall"))
        {
            if (currentBounces <= 0)
            {
                ReturnToPool();  // ��������������꣬�����ӵ�
                return;
            }

            PlaySpecialEffectSound(); // ���ŷ�����Ч
            Vector2 normal = CalculateNormal(collision);
            Reflect(normal);
            currentBounces--;
            return;
        }

        if (collision.CompareTag("Enemy") || collision.CompareTag("Box"))
        {
            // ʩ����������Ϊ�ӵ��˶�����
            ApplyForce(collision.gameObject, rb.velocity);
            if (collision.CompareTag("Enemy"))
            {
                var enemy = collision.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.Die();
                    // ������������
                }
            }
            ReturnToPool();
        }
    }

    //���߼���
    private Vector2 CalculateNormal(Collider2D collider)
    {
        //ʹ����ײ��߽���㷨��
        Vector2 bulletPos = transform.position;
        Vector2 closestPoint = collider.ClosestPoint(bulletPos);
        Vector2 normal = (bulletPos - closestPoint).normalized;

        // ���������Ϊ�������������ϲ�Ӧ�÷���������ʹ��Ĭ�����Ϸ���
        if (normal == Vector2.zero)
        {
            normal = Vector2.up;
        }

        return normal;
    }

    // ��������
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
