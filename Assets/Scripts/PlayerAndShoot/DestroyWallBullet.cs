using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestroyWallBullet : Bullet
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Tilemap>(out var tilemap))
        {
            Vector3Int cell = tilemap.WorldToCell(transform.position);
            tilemap.SetTile(cell, null);
            ReturnToPool();
            return;
        }

        if (collision.CompareTag("Enemy") || collision.CompareTag("Box"))
        {
            ApplyForce(collision.gameObject, rb.velocity);
            if (collision.CompareTag("Enemy")) Destroy(collision.gameObject);
            ReturnToPool();
        }
    }
}
