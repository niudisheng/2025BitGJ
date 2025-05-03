using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenetratingBullet : Bullet
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            ReturnToPool();
            return;
        }

        if (collision.CompareTag("Enemy")) Destroy(collision.gameObject);
    }
}
