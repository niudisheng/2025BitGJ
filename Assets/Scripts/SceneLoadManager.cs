using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoadManager : MonoBehaviour
{
    private static SceneLoadManager _instance;
    public static SceneLoadManager Instance
    {
        get
        {
            
        if (_instance == null)
            _instance = new SceneLoadManager();
        return _instance;
        }
    }

    private void Awake()
    {
        // 单例模式，方便跨脚本调用
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
    }
    
    private static void UnloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.UnloadSceneAsync(scene.buildIndex);
    }

    public static void LoadScene1(int scene)
    {
        UnloadScene();
        var loadSceneTask = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        loadSceneTask.completed += (AsyncOperation obj) => SetActiveScene(scene);
        
    }
    

    
    public static void SetActiveScene(int scene)
    {
        // 通过构建索引获取场景
        Scene sceneToActivate = SceneManager.GetSceneByBuildIndex(scene);

        // 调试：检查场景是否有效
        if (sceneToActivate.IsValid())
        {
            // 检查场景是否已加载
            if (sceneToActivate.isLoaded)
            {
                SceneManager.SetActiveScene(sceneToActivate);
                Debug.Log("成功设置活动场景: " + sceneToActivate.name);
            }
            else
            {
                Debug.LogError("场景 " + sceneToActivate.name + " 尚未加载。");
            }
        }
        else
        {
            Debug.LogError("无效的场景，构建索引: " + scene);
        }


    }
    public void LoadScene(int sceneIndex)
    {
        LoadScene1(sceneIndex);
    }

    private IEnumerator LoadSceneCoroutine(int sceneIndex)
    {
        // 1. 获取当前活动场景
        Scene currentScene = SceneManager.GetActiveScene();

        // 2. 异步卸载旧场景
        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentScene);
        if (unloadOp != null)
            yield return unloadOp;

        // 3. 异步加载新场景（Additive 模式）
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
        loadOp.allowSceneActivation = false;

        // 可选：添加加载进度 UI
        while (loadOp.progress < 0.9f)
        {
            Debug.Log($"加载进度：{loadOp.progress * 100}%");
            yield return null;
        }

        // 4. 激活场景
        loadOp.allowSceneActivation = true;
        yield return loadOp;

        // 5. 设置为活动场景
        Scene newScene = SceneManager.GetSceneByBuildIndex(sceneIndex);
        if (newScene.IsValid() && newScene.isLoaded)
        {
            SceneManager.SetActiveScene(newScene);
            Debug.Log("已切换至场景：" + newScene.name);

            //这里触发初始化事件，确保 Shoot 已存在
            MyEventManager.Instance.EventTrigger(EventName.LoadChapter);
        }
        else
        {
            Debug.LogError("场景加载失败或无效");
        }
    }
    
    public void LoadMenu()
    {
        LoadScene(0);
    }

    public void LoadPickPanel()
    {
        LoadScene(1);
    }
}