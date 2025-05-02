using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Tilemaps;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    [SerializeField] protected float impactForce = 5f;
    protected Rigidbody2D rb;
    private IObjectPool<Bullet> _pool;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetPool(IObjectPool<Bullet> pool) => _pool = pool;

    public virtual void Launch()
    {
        rb.velocity = transform.right * speed;
    }

    protected void ReturnToPool()
    {
        rb.velocity = Vector2.zero;
        _pool?.Release(this);
    }

    protected void ApplyForce(GameObject target, Vector2 forceDirection)
    {
        if (target.TryGetComponent<Rigidbody2D>(out var targetRb))
        {
            targetRb.AddForce(forceDirection.normalized * impactForce, ForceMode2D.Impulse);
        }
    }
}

