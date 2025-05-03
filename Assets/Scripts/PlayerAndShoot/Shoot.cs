using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shoot : MonoBehaviour
{
    [Header("射击设置")]
    [SerializeField] private Transform firePoint;  // 基础发射点
    [SerializeField] private Vector2 fireOffset = Vector2.right * 0.5f; // 发射位置偏移
    [SerializeField] private float fireRate = 0.5f;
    private float nextFireTime = 0f;

    public PlayerInputController inputControl;

    //子弹类型
    public enum BulletType { Normal, Bomb, Penetrating, DestroyWall }
    private BulletType currentBulletType = BulletType.Normal;

    private void Awake()
    {
        inputControl = new PlayerInputController();

        inputControl.Player.Shoot.started += OnShoot;
        inputControl.Player.NextBullet.started += OnNextBullet;
        inputControl.Player.PrevBullet.started += OnPrevBullet;
    }

    private void OnEnable()
    {
        inputControl.Enable();
    }

    private void OnDisable()
    {
        inputControl.Disable();
    }

    private void OnShoot(InputAction.CallbackContext context)
    {
        if (Time.time >= nextFireTime)
        {
            Vector3 finalPosition = firePoint.position +
                                  firePoint.right * fireOffset.x +
                                  firePoint.up * fireOffset.y;
            Bullet bullet = GetBullet();
            bullet.transform.position = finalPosition;
            bullet.transform.rotation = firePoint.rotation;
            bullet.Launch();
            nextFireTime = Time.time + fireRate;
        }
    }

    private void OnNextBullet(InputAction.CallbackContext context)
    {
        currentBulletType = (BulletType)(((int)currentBulletType + 1) % 4);
        Debug.Log("当前子弹: " + currentBulletType);
    }

    private void OnPrevBullet(InputAction.CallbackContext context)
    {
        currentBulletType = (BulletType)(((int)currentBulletType + 3) % 4);
        Debug.Log("当前子弹: " + currentBulletType);
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
