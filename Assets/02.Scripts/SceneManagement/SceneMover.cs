using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// loads scene
/// </summary>
public class SceneMover : MonoBehaviour
{
    public void MoveTo(string sceneName) =>
        SceneManager.LoadScene(sceneName);
    public void MoveTo(int sceneIndex) =>
        SceneManager.LoadScene(sceneIndex);
}
