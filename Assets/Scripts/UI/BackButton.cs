using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    private Button _button;
    void Start()
    {
        _button = GetComponent<Button>();
        SetButton();
    }
    private void SetButton()
    {
        _button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        SceneLoadManager.Instance.LoadMenu();
    }




}
