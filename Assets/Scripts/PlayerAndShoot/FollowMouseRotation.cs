using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowMouseRotation : MonoBehaviour
{
    [SerializeField] private Transform playerTransform; // 角色的Transform
    [SerializeField] private bool flipPlayerBasedOnMouse = true; // 是否根据鼠标位置翻转角色
    [SerializeField] public Vector2 rotationPivotOffset = Vector2.zero; // 旋转中心偏移量

    void Update()
    {
        RotateTowardsMousePosition();
    }

    public void RotateTowardsMousePosition()
    {
        // 获取鼠标在世界空间中的位置
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.transform.position.z));

        // 计算旋转中心位置（应用偏移）
        Vector3 pivotPosition = transform.position + transform.right * rotationPivotOffset.x + transform.up * rotationPivotOffset.y;

        // 计算方向向量
        Vector3 direction = worldPosition - pivotPosition;
        direction.z = 0;  // 确保方向向量在2D平面内
        direction = direction.normalized;  // 归一化方向向量

        // 计算旋转角度
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 如果启用角色翻转
        if (flipPlayerBasedOnMouse && playerTransform != null)
        {
            // 判断鼠标在角色的左侧还是右侧
            bool mouseOnLeft = worldPosition.x < playerTransform.position.x;

            // 设置角色朝向
            Vector3 scale = playerTransform.localScale;
            scale.x = mouseOnLeft ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
            playerTransform.localScale = scale;

        }

        // 设置子物体的旋转
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // 应用旋转中心偏移
        transform.position = pivotPosition - (transform.right * rotationPivotOffset.x + transform.up * rotationPivotOffset.y);
    }

}
