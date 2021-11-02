using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.SceneSystem;

public class SceneLoaderV2 : MonoBehaviour
{
    public GameObject canvas;


    public void LoadScene()
    {
    //Disable UI 
        canvas.SetActive(false);
    
    //Consider disabling all other components and re-enabling them here 
        
    }
}