using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneInformation : MonoBehaviour
{
    static public SceneInformation instance;
    static public bool sceneLoaded;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
    }
    static public string oldSceneName;
    static public string newSceneName;

    private void Start()
    {
        SceneManager.sceneUnloaded += delegate
        {
            sceneLoaded = false;
        };
        SceneManager.sceneLoaded += delegate
        {
            oldSceneName = newSceneName;
            Scene scene = SceneManager.GetActiveScene();
            newSceneName = scene.name;
            sceneLoaded = true;
        };
    }
}
