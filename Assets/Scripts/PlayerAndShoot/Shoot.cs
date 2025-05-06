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

    [Header("��������")]
    [SerializeField] private int maxNormalAmmo = 30;
    [SerializeField] private int maxBombAmmo = 5;
    [SerializeField] private int maxPenetratingAmmo = 10;
    [SerializeField] private int maxDestroyWallAmmo = 15;
    private Dictionary<BulletType, int> currentAmmo = new Dictionary<BulletType, int>();

    //�ӵ�����
    public enum BulletType { Normal, Bomb, Penetrating, DestroyWall }
    private BulletType currentBulletType = BulletType.Normal;

    public PlayerInputController inputControl;

    private void Awake()
    {
        inputControl = new PlayerInputController();

        inputControl.Player.Shoot.started += OnShoot;
        inputControl.Player.NextBullet.started += OnNextBullet;
        inputControl.Player.PrevBullet.started += OnPrevBullet;

        // ��ʼ����ҩ
        currentAmmo[BulletType.Normal] = maxNormalAmmo;
        currentAmmo[BulletType.Bomb] = maxBombAmmo;
        currentAmmo[BulletType.Penetrating] = maxPenetratingAmmo;
        currentAmmo[BulletType.DestroyWall] = maxDestroyWallAmmo;
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
        if (Time.time < nextFireTime) return;

        if (currentAmmo[currentBulletType] <= 0)
        {
            Debug.Log($"{currentBulletType} ��ҩ�Ѻľ�!");
            return;
        }

        Vector3 finalPosition = firePoint.position +
                              firePoint.right * fireOffset.x +
                              firePoint.up * fireOffset.y;
        Bullet bullet = GetBullet();
        if (bullet == null) return;

        bullet.transform.position = finalPosition;
        bullet.transform.rotation = firePoint.rotation;
        bullet.Launch();

        // ���ٵ�ҩ����ʾ
        currentAmmo[currentBulletType]--;
        Debug.Log($"{currentBulletType} ʣ�൯ҩ: {currentAmmo[currentBulletType]}" +
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

    private void OnDrawGizmos()
    {
        if (firePoint != null)
        {
            // ��ȡFollowMouseRotation���������У�
            FollowMouseRotation rotationScript = GetComponent<FollowMouseRotation>();
            Vector2 pivotOffset = rotationScript != null ? rotationScript.rotationPivotOffset : Vector2.zero;

            // ������ת��λ�ã�ǹ��λ�ã�
            Vector3 pivotPosition = firePoint.position +
                                  firePoint.right * pivotOffset.x +
                                  firePoint.up * pivotOffset.y;

            // ������ת��λ�ã���ɫ��
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(pivotPosition, 0.1f);

            // ����ƫ�ƺ��ʵ�ʷ���λ�ã�ǹ��λ�ã���ɫ��
            Vector3 finalPosition = firePoint.position +
                                  firePoint.right * fireOffset.x +
                                  firePoint.up * fireOffset.y;
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(finalPosition, 0.1f);

            // ���ƴ���ת�ᵽǹ�ڵ����ߣ���ɫ��
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(pivotPosition, finalPosition);

            // ���Ʒ��䷽����ɫ��
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(finalPosition, finalPosition + firePoint.right * 0.5f);
        }
    }
}
