using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestroyWallBullet : Bullet
{
    [SerializeField] private float destroyRadius = 0.5f; // 破坏半径

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Tilemap>(out var tilemap))
        {
            PlaySpecialEffectSound(); // 播放摧毁墙壁音效
            // 获取子弹位置周围一定范围内的所有瓦片
            Vector3Int centerCell = tilemap.WorldToCell(transform.position);

            // 检查3x3范围内的瓦片
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector3Int cell = new Vector3Int(centerCell.x + x, centerCell.y + y, centerCell.z);
                    if (tilemap.GetTile(cell) != null)
                    {
                        // 检查瓦片中心与子弹的距离
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
                    // 触发死亡流程
                }
            }
            ReturnToPool();
        }
    }
}
