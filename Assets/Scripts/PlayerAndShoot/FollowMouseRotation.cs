using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowMouseRotation : MonoBehaviour
{
 
    void Update()
    {
        RotateTowardsMousePosition();
    }

    public void RotateTowardsMousePosition()
    {
        // ��ȡ���������ռ��е�λ��
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.transform.position.z));

        // ��ȡ������������ռ��е�λ��
        Vector3 objPosition = transform.position;

        // ���㷽������
        Vector3 direction = worldPosition - objPosition;
        direction.z = 0;  // ȷ������������2Dƽ����
        direction = direction.normalized;  // ��һ����������

        // ������ת�Ƕ�
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // ����Player��localScale.x������angle������
        //if (playerTransform.localScale.x < 0)
        //{
        //    angle = 180 - angle;  // ���Player������࣬�����Ƕ�
        //}

        // �������������ת
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

}
