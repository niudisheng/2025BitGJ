using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.EventSystems;  // 需要导入 EventSystems 以支持 Pointer 事件

public class ButtonHoverClickEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Vector3 originalScale;      // 初始大小
    private Color ImageOriginalColor;        // 初始颜色
    private Color TextOriginalColor;        // 初始颜色
    private Button button;
    private Image buttonImage;          // 按钮背景图像
    public TextMeshProUGUI buttonText;             // 按钮文字
    
    public Color hoverColor;           // 鼠标悬浮时按钮颜色
    public Color clickColor;           // 按钮被点击时按钮颜色
    void Start()
    {
        button = GetComponent<Button>();
        buttonImage = button.GetComponent<Image>();
        originalScale = transform.localScale;
        ImageOriginalColor = buttonImage.color;
        TextOriginalColor = buttonText.color;

        // 监听点击事件
        button.onClick.AddListener(OnClick);
    }

    // 悬浮事件：鼠标进入按钮时触发
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 悬浮时：放大 + 改变颜色
        transform.DOScale(originalScale * 1.1f, 0.2f).SetEase(Ease.OutQuad);   // 放大
        buttonImage.DOColor(hoverColor, 0.3f);  // 改变颜色
        buttonText.DOColor(hoverColor, 0.3f);  // 按钮文字也改变颜色
    }

    // 悬浮移开事件：鼠标离开按钮时触发
    public void OnPointerExit(PointerEventData eventData)
    {
        // 悬浮移开时：恢复原大小 + 恢复原颜色
        transform.DOScale(originalScale, 0.2f).SetEase(Ease.OutQuad);  // 恢复原始大小
        buttonImage.DOColor(ImageOriginalColor, 0.3f);  // 恢复原始颜色
        buttonText.DOColor(TextOriginalColor, 0.3f);   // 恢复文字颜色
    }

    // 点击事件：按钮被点击时触发
    void OnClick()
    {
        // 点击时：缩小 + 恢复大小 + 改变背景颜色
        transform.DOScale(originalScale * 0.9f, 0.1f)  // 按下时缩小
            .OnComplete(() =>
            {
                transform.DOScale(originalScale, 0.2f).SetEase(Ease.OutBack);  // 弹回原始大小
                buttonImage.DOColor(clickColor, 0.3f);  // 改变按钮背景颜色
                buttonText.DOColor(clickColor, 0.3f);   // 改变按钮文字颜色
            });
    }
}