using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoadManager : MonoBehaviour
{
    
    private static void UnloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.UnloadSceneAsync(scene.buildIndex);
    }

    public static void LoadScene(int scene)
    {
        UnloadScene();
        var loadSceneTask = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        loadSceneTask.completed += (AsyncOperation obj) => SetActiveScene(scene);
        
        SetActiveScene(scene);
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
}