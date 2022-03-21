using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 1. Compare loaded scene name & place player on here.
/// 2. load scene event.
/// </summary>
public class Portal : MonoBehaviour
{
    [SerializeField] string sceneNameToMove;

    private void Start()
    {
        StartCoroutine(E_CheckSceneAndPutPlayerHere());
    }

    IEnumerator E_CheckSceneAndPutPlayerHere()
    {
        yield return new WaitUntil(()=> SceneInformation.sceneLoaded);
        Debug.Log($"{SceneInformation.oldSceneName} {sceneNameToMove}");
        if (SceneInformation.oldSceneName == sceneNameToMove)
            Player.instance.transform.position = transform.position;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision != null)
        {
            if(collision.gameObject.layer == LayerMask.NameToLayer("Player") &&
               Input.GetKey(KeyCode.UpArrow))
            {
                SceneManager.LoadScene(sceneNameToMove);
            }
        }
    }
}
