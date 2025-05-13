using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class QEKeyVisualizer : MonoBehaviour
{
    [Header("按键UI引用")]
    [SerializeField] private Image qKeyImage;
    [SerializeField] private Image eKeyImage;

    [Header("颜色设置")]
    [SerializeField] private Color pressedColor = Color.black;
    private Color normalColor = Color.white;

    private void Awake()
    {
        // 初始化颜色
        if (qKeyImage != null) qKeyImage.color = normalColor;
        if (eKeyImage != null) eKeyImage.color = normalColor;
    }

    private void OnEnable()
    {
        // 订阅按下和松开事件
        Shoot.OnQKeyStateChanged += UpdateQKeyState;
        Shoot.OnEKeyStateChanged += UpdateEKeyState;
    }

    private void OnDisable()
    {
        Shoot.OnQKeyStateChanged -= UpdateQKeyState;
        Shoot.OnEKeyStateChanged -= UpdateEKeyState;
    }

    private void UpdateQKeyState(bool isPressed)
    {
        if (qKeyImage != null)
            qKeyImage.color = isPressed ? pressedColor : normalColor;
    }

    private void UpdateEKeyState(bool isPressed)
    {
        if (eKeyImage != null)
            eKeyImage.color = isPressed ? pressedColor : normalColor;
    }
}