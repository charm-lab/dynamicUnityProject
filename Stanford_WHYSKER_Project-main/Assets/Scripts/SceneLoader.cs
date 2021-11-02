using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SceneSystem;

public class SceneLoader : MonoBehaviour
{
    // Type in the name of the Scene you would like to load in the Inspector
    public string m_Scene;
    public GameObject canvas;
//    public GameObject camera;

    


    void Start()
    {
        
    }
    
    public async void LoadScene()
    {
        IMixedRealitySceneSystem sceneSystem = MixedRealityToolkit.Instance.GetService<IMixedRealitySceneSystem>();
        System.Diagnostics.Debug.Write("LoadScene");
        
        await sceneSystem.LoadContent(m_Scene);
        Destroy(canvas);
        
        
//        StartCoroutine(LoadYourAsyncScene());
        
    }

//    IEnumerator LoadYourAsyncScene()
//    {
//        // Set the current Scene to be able to unload it later
//        Scene currentScene = SceneManager.GetActiveScene();
//
//        // The Application loads the Scene in the background at the same time as the current Scene.
//        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(m_Scene, LoadSceneMode.Additive);
//        
//        
//
//        // Wait until the last operation fully loads to return anything
//        while (!asyncLoad.isDone)
//        {
//            yield return null;
//        }
//        
//        // Unload the previous Scene
//        SceneManager.MoveGameObjectToScene(mrtkplayspace, SceneManager.GetSceneByName(m_Scene));
//        SceneManager.MoveGameObjectToScene(mrtk, SceneManager.GetSceneByName(m_Scene));
////        SceneManager.MoveGameObjectToScene(camera, SceneManager.GetSceneByName(m_Scene));
//        
//        SceneManager.UnloadSceneAsync(currentScene);
//    }
}