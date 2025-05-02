using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Shoot : MonoBehaviour
{
    [Header("�������")]
    [SerializeField] private Transform firePoint;  // ���������
    [SerializeField] private Vector2 fireOffset = Vector2.right * 0.5f; // ����λ��ƫ��
    [SerializeField] private float fireRate = 0.5f;
    private float nextFireTime = 0f;

    public PlayerInputController inputControl;

    //�ӵ�����
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
        Debug.Log("��ǰ�ӵ�: " + currentBulletType);
    }

    private void OnPrevBullet(InputAction.CallbackContext context)
    {
        currentBulletType = (BulletType)(((int)currentBulletType + 3) % 4);
        Debug.Log("��ǰ�ӵ�: " + currentBulletType);
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
}
