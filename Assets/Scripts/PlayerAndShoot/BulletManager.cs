using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BulletManager : MonoBehaviour
{
    public static BulletManager Instance { get; private set; }

    [System.Serializable]
    public class BulletPool
    {
        public Bullet prefab;
        public int defaultCapacity = 10;
        public int maxSize = 100;
    }

    [SerializeField] private BulletPool normalBullet;
    [SerializeField] private BulletPool bombBullet;
    [SerializeField] private BulletPool penetratingBullet;
    [SerializeField] private BulletPool destroyWallBullet;

    private ObjectPool<Bullet> _normalPool;
    private ObjectPool<Bullet> _bombPool;
    private ObjectPool<Bullet> _penetratingPool;
    private ObjectPool<Bullet> _destroyWallPool;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            CreatePools(); // 确保池在Awake中创建
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        MyEventManager.Instance.AddEventListener(EventName.NewGame, OnNewGame);
    }

    private void OnDestroy()
    {
        MyEventManager.Instance.RemoveEventListener(EventName.NewGame, OnNewGame);
    }

    private void OnNewGame()
    {
        // 清空所有对象池
        _normalPool?.Clear();
        _bombPool?.Clear();
        _penetratingPool?.Clear();
        _destroyWallPool?.Clear();

        // 重新创建池
        CreatePools();

        Debug.Log("子弹管理器已重置");
    }

    private void CreatePools()
    {
        _normalPool = new ObjectPool<Bullet>(
            () => CreateBullet(normalBullet.prefab),
            bullet => bullet.gameObject.SetActive(true),
            bullet => bullet.gameObject.SetActive(false),
            bullet => Destroy(bullet.gameObject),
            false, normalBullet.defaultCapacity, normalBullet.maxSize);

        _bombPool = new ObjectPool<Bullet>(
            () => CreateBullet(bombBullet.prefab),
            bullet => bullet.gameObject.SetActive(true),
            bullet => bullet.gameObject.SetActive(false),
            bullet => Destroy(bullet.gameObject),
            false, bombBullet.defaultCapacity, bombBullet.maxSize);

        _penetratingPool = new ObjectPool<Bullet>(
            () => CreateBullet(penetratingBullet.prefab),
            bullet => bullet.gameObject.SetActive(true),
            bullet => bullet.gameObject.SetActive(false),
            bullet => Destroy(bullet.gameObject),
            false, penetratingBullet.defaultCapacity, penetratingBullet.maxSize);

        _destroyWallPool = new ObjectPool<Bullet>(
            () => CreateBullet(destroyWallBullet.prefab),
            bullet => bullet.gameObject.SetActive(true),
            bullet => bullet.gameObject.SetActive(false),
            bullet => Destroy(bullet.gameObject),
            false, destroyWallBullet.defaultCapacity, destroyWallBullet.maxSize);
    }

    private Bullet CreateBullet(Bullet prefab)
    {
        var bullet = Instantiate(prefab);
        bullet.SetPool(GetPoolForType(bullet));
        return bullet;
    }

    private IObjectPool<Bullet> GetPoolForType(Bullet bullet)
    {
        return bullet switch
        {
            NormalBullet => _normalPool,
            BombBullet => _bombPool,
            PenetratingBullet => _penetratingPool,
            DestroyWallBullet => _destroyWallPool,
            _ => null
        };
    }

    public Bullet GetNormalBullet() => _normalPool.Get();
    public Bullet GetBombBullet() => _bombPool.Get();
    public Bullet GetPenetratingBullet() => _penetratingPool.Get();
    public Bullet GetDestroyWallBullet() => _destroyWallPool.Get();
}
