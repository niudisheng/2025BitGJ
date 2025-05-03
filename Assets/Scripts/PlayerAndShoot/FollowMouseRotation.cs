using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowMouseRotation : MonoBehaviour
{
    [SerializeField] private Transform playerTransform; // ��ɫ��Transform
    [SerializeField] private bool flipPlayerBasedOnMouse = true; // �Ƿ�������λ�÷�ת��ɫ
    [SerializeField] public Vector2 rotationPivotOffset = Vector2.zero; // ��ת����ƫ����

    void Update()
    {
        RotateTowardsMousePosition();
    }

    public void RotateTowardsMousePosition()
    {
        // ��ȡ���������ռ��е�λ��
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.transform.position.z));

        // ������ת����λ�ã�Ӧ��ƫ�ƣ�
        Vector3 pivotPosition = transform.position + transform.right * rotationPivotOffset.x + transform.up * rotationPivotOffset.y;

        // ���㷽������
        Vector3 direction = worldPosition - pivotPosition;
        direction.z = 0;  // ȷ������������2Dƽ����
        direction = direction.normalized;  // ��һ����������

        // ������ת�Ƕ�
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // ������ý�ɫ��ת
        if (flipPlayerBasedOnMouse && playerTransform != null)
        {
            // �ж�����ڽ�ɫ����໹���Ҳ�
            bool mouseOnLeft = worldPosition.x < playerTransform.position.x;

            // ���ý�ɫ����
            Vector3 scale = playerTransform.localScale;
            scale.x = mouseOnLeft ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
            playerTransform.localScale = scale;

        }

        // �������������ת
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Ӧ����ת����ƫ��
        transform.position = pivotPosition - (transform.right * rotationPivotOffset.x + transform.up * rotationPivotOffset.y);
    }

}
