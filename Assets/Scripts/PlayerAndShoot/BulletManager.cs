using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

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
        MyEventManager.Instance.AddEventListener(EventName.LoadChapter, OnNewGame);
    }

    private void OnDestroy()
    {
        MyEventManager.Instance.RemoveEventListener(EventName.LoadChapter, OnNewGame);
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
        var parent = GetBulletParent();
        if (parent == null)
        {
            Debug.LogError("无法获取子弹父对象，将创建在场景根目录");
            parent = null;
        }

        var bullet = Instantiate(prefab, parent);
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

    private static Transform GetBulletParent()
    {
        // 查找玩家对象（跨场景）
        var player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            Debug.LogError("找不到Player对象！");
            return null;
        }

        // 获取Player所在的场景
        var playerScene = player.scene;

        // 在Player场景中查找或创建Bullets容器
        var rootObjects = playerScene.GetRootGameObjects();
        Transform bulletsContainer = null;

        // 查找现有的Bullets容器
        foreach (var go in rootObjects)
        {
            if (go.name == "Bullets")
            {
                bulletsContainer = go.transform;
                break;
            }
        }

        // 如果不存在则创建新的
        if (bulletsContainer == null)
        {
            var newContainer = new GameObject("Bullets");
            SceneManager.MoveGameObjectToScene(newContainer, playerScene);
            bulletsContainer = newContainer.transform;

            // 可选：设置与Player相同的位置
            bulletsContainer.position = player.transform.position;
        }

        return bulletsContainer;
    }

    public Bullet GetNormalBullet() => _normalPool.Get();
    public Bullet GetBombBullet() => _bombPool.Get();
    public Bullet GetPenetratingBullet() => _penetratingPool.Get();
    public Bullet GetDestroyWallBullet() => _destroyWallPool.Get();
}
