using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class MenuPanel : MonoBehaviour
    {
        public Button newGameButton, quitButton;
        public int GamesceneIndex = 1;

        private void Awake()
        {
            MyEventManager.Instance.AddEventListener(EventName.NewGame,OnNewGameButtonClicked);
        }

        private void OnEnable()
        {
            Debug.Log("MenuPanel OnEnable");
            SceneLoadManager.SetActiveScene(0);
            newGameButton.onClick.AddListener( () => MyEventManager.Instance.EventTrigger(EventName.NewGame));
            quitButton.onClick.AddListener(OnQuitButtonClicked);
        }

        private void OnQuitButtonClicked() => Application.Quit();

        [ContextMenu("开始游戏测试")]
        public void OnNewGameButtonClicked()
        {
            Debug.Log("New Game Button Clicked");
            SceneLoadManager.LoadScene(GamesceneIndex);
        }
        
    }
}