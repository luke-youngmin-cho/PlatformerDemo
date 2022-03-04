using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyChecker : MonoBehaviour
{
    public bool isReady = false;
    public GameObject inventoryManagerGO;
    public GameObject shortCutManagerGO;

    private void Awake()
    {
        inventoryManagerGO.SetActive(true);
        shortCutManagerGO.SetActive(true);
    }
    private void Start()
    {
        StartCoroutine(E_CheckAllReady());
    }

    IEnumerator E_CheckAllReady()
    {
        yield return new WaitUntil(() => inventoryManagerGO.GetComponent<InventoryManager>().isReady);
        inventoryManagerGO.SetActive(false);
        yield return new WaitUntil(() => shortCutManagerGO.GetComponent<ShortCutManager>().isReady);
        shortCutManagerGO.SetActive(false);
        isReady = true;
    }
}
