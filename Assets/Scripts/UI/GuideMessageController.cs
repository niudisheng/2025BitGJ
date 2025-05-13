using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GuideMessageController : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TextMeshProUGUI messageText;

    public float showDuration = 3f;
    public float fadeDuration = 1f;

    private void Start()
    {
        LoadAndShowGuideMessage();
    }

    private void LoadAndShowGuideMessage()
    {
        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        LevelData data = LevelConfigManager.Instance?.GetLevelData(sceneIndex);

        if (data != null && !string.IsNullOrWhiteSpace(data.guideMessage))
        {
            ShowMessage(data.guideMessage);
        }
        else
        {
            // 没有教学信息则关闭自身
            gameObject.SetActive(false);
        }
    }

    public void ShowMessage(string message)
    {
        messageText.text = message;
        StartCoroutine(ShowAndFade());
    }

    private IEnumerator ShowAndFade()
    {
        gameObject.SetActive(true);
        canvasGroup.alpha = 1f;

        yield return new WaitForSeconds(showDuration);

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }
}
