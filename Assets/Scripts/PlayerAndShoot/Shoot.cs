using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Shoot : MonoBehaviour
{
    public static Shoot Instance { get; private set; }

    [Header("射击设置")]
    [SerializeField] private Transform firePoint;  // 基础发射点
    [SerializeField] private Vector2 fireOffset = Vector2.right * 0.5f; // 发射位置偏移
    [SerializeField] private float fireRate = 0.5f;
    private float nextFireTime = 0f;

    [Header("弹夹设置")]
    [SerializeField] private int maxNormalAmmo = 30;
    [SerializeField] private int maxBombAmmo = 5;
    [SerializeField] private int maxPenetratingAmmo = 10;
    [SerializeField] private int maxDestroyWallAmmo = 15;
    private Dictionary<BulletType, int> currentAmmo = new Dictionary<BulletType, int>();

    [Header("音效")]
    [SerializeField] private AudioClip switchWeaponClip;
    [SerializeField] private AudioClip emptyAmmoClip;

    public static event Action<bool> OnQKeyStateChanged;  // true=按下, false=松开
    public static event Action<bool> OnEKeyStateChanged;

    //子弹类型
    public enum BulletType { Normal, Bomb, Penetrating, DestroyWall }
    private BulletType currentBulletType = BulletType.Normal;

    public PlayerInputController inputControl;
    private GunAnimation gunAnimation;
    private SelectWeapon selectWeaponUI;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this; // 设置单例
            DontDestroyOnLoad(gameObject);
        }

        inputControl = new PlayerInputController();

        inputControl.Player.Shoot.started += OnShoot;
        inputControl.Player.NextBullet.performed += OnNextBullet;
        inputControl.Player.NextBullet.canceled += OnNextBullet;

        inputControl.Player.PrevBullet.performed += OnPrevBullet;
        inputControl.Player.PrevBullet.canceled += OnPrevBullet;


        // 初始化弹药字典
        currentAmmo = new Dictionary<BulletType, int>
    {
        { BulletType.Normal, maxNormalAmmo },
        { BulletType.Bomb, maxBombAmmo },
        { BulletType.Penetrating, maxPenetratingAmmo },
        { BulletType.DestroyWall, maxDestroyWallAmmo }
    };

        gunAnimation = GetComponent<GunAnimation>();
        // 获取 SelectWeapon 组件
        selectWeaponUI = FindObjectOfType<SelectWeapon>();
        if (selectWeaponUI == null)
        {
            Debug.LogWarning("SelectWeapon UI not found in scene!");
        }
    }

    private void Start()
    {
        InitAmmoByScene();
    }

    private void OnEnable()
    {
        inputControl?.Enable();
    }

    private void OnDisable()
    {
        inputControl?.Disable();
    }

    private void InitAmmoByScene()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        LevelData data = LevelConfigManager.Instance.GetLevelData(sceneIndex);

        if (data != null)
        {
            maxNormalAmmo = data.normalAmmo;
            maxBombAmmo = data.bombAmmo;
            maxPenetratingAmmo = data.penetratingAmmo;
            maxDestroyWallAmmo = data.destroyWallAmmo;

            currentAmmo[BulletType.Normal] = maxNormalAmmo;
            currentAmmo[BulletType.Bomb] = maxBombAmmo;
            currentAmmo[BulletType.Penetrating] = maxPenetratingAmmo;
            currentAmmo[BulletType.DestroyWall] = maxDestroyWallAmmo;

            Debug.Log($"弹药初始化完毕：普通{data.normalAmmo} 炸弹{data.bombAmmo} 穿透{data.penetratingAmmo} 爆墙{data.destroyWallAmmo}");
        }
        else
        {
            Debug.LogWarning($"未找到该关卡（sceneIndex = {sceneIndex}）的配置数据，使用默认弹药");
        }

        nextFireTime = 0f;
        currentBulletType = BulletType.Normal;
    }



    private void OnShoot(InputAction.CallbackContext context)
    {
        if (Time.time < nextFireTime) return;

        if (currentAmmo[currentBulletType] <= 0)
        {
            //音效
            if (emptyAmmoClip != null)
            {
                SoundManager.Instance.PlaySound(emptyAmmoClip);
            }
            Debug.Log($"{currentBulletType} 弹药已耗尽!");
            return;
        }
        // 发射前注册子弹
        BulletManager.Instance.RegisterBullet();

        // 触发开枪动画
        if (gunAnimation != null)
        {
            gunAnimation.StartFire();
        }

        Vector3 finalPosition = firePoint.position +
                              firePoint.right * fireOffset.x +
                              firePoint.up * fireOffset.y;
        Bullet bullet = GetBullet();
        if (bullet == null) return;

        bullet.transform.position = finalPosition;
        bullet.transform.rotation = firePoint.rotation;
        bullet.Launch();

        // 减少弹药并显示
        currentAmmo[currentBulletType]--;
        CheckAmmo();
        Debug.Log($"{currentBulletType} 剩余弹药: {currentAmmo[currentBulletType]}" +
            $"/{GetMaxAmmo(currentBulletType)}");

        nextFireTime = Time.time + fireRate;
    }

    private int GetMaxAmmo(BulletType type)
    {
        return type switch
        {
            BulletType.Normal => maxNormalAmmo,
            BulletType.Bomb => maxBombAmmo,
            BulletType.Penetrating => maxPenetratingAmmo,
            BulletType.DestroyWall => maxDestroyWallAmmo,
            _ => 0
        };
    }

    private void OnNextBullet(InputAction.CallbackContext context)
    {
        // 通知 UI 改变颜色
        OnEKeyStateChanged?.Invoke(context.phase != InputActionPhase.Canceled);

        if (context.phase == InputActionPhase.Performed)
        {
            currentBulletType = (BulletType)(((int)currentBulletType + 1) % 4);
            Debug.Log("当前子弹: " + currentBulletType);
            //音效
            if (switchWeaponClip != null)
            {
                SoundManager.Instance.PlaySound(switchWeaponClip);
            }

            // 更新UI选择
            if (selectWeaponUI != null)
            {
                selectWeaponUI.UpdateWeaponSelection(currentBulletType);
            }
        }
    }

    private void OnPrevBullet(InputAction.CallbackContext context)
    {
        // 通知 UI 改变颜色
        OnQKeyStateChanged?.Invoke(context.phase != InputActionPhase.Canceled);

        if (context.phase == InputActionPhase.Performed)
        {
            currentBulletType = (BulletType)(((int)currentBulletType + 3) % 4);
            Debug.Log("当前子弹: " + currentBulletType);
            //音效
            if (switchWeaponClip != null)
            {
                SoundManager.Instance.PlaySound(switchWeaponClip);
            }

            // 更新UI选择
            if (selectWeaponUI != null)
            {
                selectWeaponUI.UpdateWeaponSelection(currentBulletType);
            }
        }
    }

    private Bullet GetBullet()
    {
        if (BulletManager.Instance == null)
        {
            Debug.LogError("BulletManager.Instance is null!");
            return null;
        }

        return currentBulletType switch
        {
            BulletType.Normal => BulletManager.Instance.GetNormalBullet(),
            BulletType.Bomb => BulletManager.Instance.GetBombBullet(),
            BulletType.Penetrating => BulletManager.Instance.GetPenetratingBullet(),
            BulletType.DestroyWall => BulletManager.Instance.GetDestroyWallBullet(),
            _ => BulletManager.Instance.GetNormalBullet()
        };
    }

    private void CheckAmmo()
    {
        bool allAmmoDepleted = true;

        foreach (var ammo in currentAmmo)
        {
            if (ammo.Value > 0)
            {
                allAmmoDepleted = false;
                break;
            }
        }

        if (allAmmoDepleted)
        {
            // 如果弹药耗尽且仍有活跃子弹，延迟判定失败
            if (BulletManager.Instance.GetActiveBullets() > 0)
            {
                Debug.Log("弹药耗尽，等待子弹销毁...");
            }
            else
            {
                // 如果没有活跃子弹，直接判定失败
                StartCoroutine(DelayedDefeat());
            }
        }
    }

    private IEnumerator DelayedDefeat()
    {
        yield return null;
        GameManager.Instance.Defeat();
    }

    #region 外部调用
    public bool IsAllAmmoEmpty()
    {
        foreach (var ammo in currentAmmo)
        {
            if (ammo.Value > 0) // 只要有一个弹药未耗尽
            {
                return false;
            }
        }
        return true; // 所有弹药已耗尽
    }

    public string GetAmmoText(BulletType type)
    {
        return currentAmmo.ContainsKey(type)
            ? $"{currentAmmo[type]}/{GetMaxAmmo(type)}"
            : "0/0";
    }

    public void ResetAmmo()
    {
        InitAmmoByScene();
    }
    #endregion

    private void OnDrawGizmos()
    {
        if (firePoint != null)
        {
            // 获取FollowMouseRotation组件（如果有）
            FollowMouseRotation rotationScript = GetComponent<FollowMouseRotation>();
            Vector2 pivotOffset = rotationScript != null ? rotationScript.rotationPivotOffset : Vector2.zero;

            // 计算旋转轴位置（枪托位置）
            Vector3 pivotPosition = firePoint.position +
                                  firePoint.right * pivotOffset.x +
                                  firePoint.up * pivotOffset.y;

            // 绘制旋转轴位置（红色）
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pivotPosition, 0.1f);

            // 绘制偏移后的实际发射位置（枪口位置，绿色）
            Vector3 finalPosition = firePoint.position +
                                  firePoint.right * fireOffset.x +
                                  firePoint.up * fireOffset.y;
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(finalPosition, 0.1f);

            // 绘制从旋转轴到枪口的连线（黄色）
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pivotPosition, finalPosition);

            // 绘制发射方向（蓝色）
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(finalPosition, finalPosition + firePoint.right * 0.5f);
        }
    }
}
