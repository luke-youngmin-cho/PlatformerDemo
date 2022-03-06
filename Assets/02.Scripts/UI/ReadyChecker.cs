using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyChecker : MonoBehaviour
{
    public bool isReady = false;
    public GameObject inventoryManagerGO;
    private void Awake()
    {
        inventoryManagerGO.SetActive(true);
    }
    private void Start()
    {
        StartCoroutine(E_CheckAllReady());
    }

    IEnumerator E_CheckAllReady()
    {
        yield return new WaitUntil(() => inventoryManagerGO.GetComponent<InventoryManager>().isReady);
        inventoryManagerGO.SetActive(false);
        isReady = true;
    }
}
