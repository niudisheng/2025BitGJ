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
            MyEventManager.Instance.AddEventListener(EventName.NewGame,LoadPickUpScene);
        }

        private void OnEnable()
        {
            Debug.Log("MenuPanel OnEnable");
            // newGameButton.onClick.AddListener( () => MyEventManager.Instance.EventTrigger(EventName.NewGame));
            newGameButton.onClick.AddListener( OnNewGameButtonClicked);
            quitButton.onClick.AddListener(OnQuitButtonClicked);
        }

        private void Start()
        {
            SceneLoadManager.SetActiveScene(0);
        }

        private void OnQuitButtonClicked() => Application.Quit();

        [ContextMenu("开始游戏测试")]
        public void OnNewGameButtonClicked()
        {
            MyEventManager.Instance.EventTrigger(EventName.NewGame);

        }

        private void LoadPickUpScene()
        {
            SceneLoadManager.Instance.LoadScene(GamesceneIndex);
        }


    }

}