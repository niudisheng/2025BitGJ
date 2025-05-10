using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BombBullet : Bullet
{
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private GameObject explosionPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Explode();
        ReturnToPool();
    }

    private void Explode()
    {

        PlaySpecialEffectSound(); // 播放爆炸音效

        // 生成爆炸动画
        if (explosionPrefab != null)
        {
            // 获取Player所在的场景
            Scene playerScene = GameObject.FindGameObjectWithTag("Player").scene;

            // 实例化爆炸特效
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            // 将爆炸特效移动到Player的场景
            SceneManager.MoveGameObjectToScene(explosion, playerScene);

            Destroy(explosion, 0.3f); // 1秒后销毁爆炸动画（根据动画时长调整）
        }

        //检测爆炸范围内碰撞体
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in colliders)
        {
            if (hit.CompareTag("Enemy") || hit.CompareTag("Box"))
            {
                // 爆炸力的方向是从爆炸中心指向目标
                Vector2 forceDirection = hit.transform.position - transform.position;
                ApplyForce(hit.gameObject, forceDirection);
                if (hit.CompareTag("Enemy"))
                {
                    var enemy = hit.GetComponent<Enemy.Enemy>();
                    if (enemy != null)
                    {
                        enemy.Die();
                    }
                }
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
