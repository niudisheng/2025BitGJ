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
        // 获取鼠标在世界空间中的位置
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.transform.position.z));

        // 获取子物体在世界空间中的位置
        Vector3 objPosition = transform.position;

        // 计算方向向量
        Vector3 direction = worldPosition - objPosition;
        direction.z = 0;  // 确保方向向量在2D平面内
        direction = direction.normalized;  // 归一化方向向量

        // 计算旋转角度
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 根据Player的localScale.x来决定angle的正负
        //if (playerTransform.localScale.x < 0)
        //{
        //    angle = 180 - angle;  // 如果Player面向左侧，调整角度
        //}

        // 设置子物体的旋转
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

}
