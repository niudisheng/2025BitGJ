using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestroyWallBullet : Bullet
{
    [SerializeField] private float destroyRadius = 0.5f; // �ƻ��뾶

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Tilemap>(out var tilemap))
        {
            PlaySpecialEffectSound(); // ���Ŵݻ�ǽ����Ч
            // ��ȡ�ӵ�λ����Χһ����Χ�ڵ�������Ƭ
            Vector3Int centerCell = tilemap.WorldToCell(transform.position);

            // ���3x3��Χ�ڵ���Ƭ
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector3Int cell = new Vector3Int(centerCell.x + x, centerCell.y + y, centerCell.z);
                    if (tilemap.GetTile(cell) != null)
                    {
                        // �����Ƭ�������ӵ��ľ���
                        Vector3 tileWorldPos = tilemap.GetCellCenterWorld(cell);
                        if (Vector3.Distance(transform.position, tileWorldPos) <= destroyRadius)
                        {
                            tilemap.SetTile(cell, null);
                        }
                    }
                }
            }
            ReturnToPool();
            return;
        }

        if (collision.CompareTag("Enemy") || collision.CompareTag("Box"))
        {
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
}
