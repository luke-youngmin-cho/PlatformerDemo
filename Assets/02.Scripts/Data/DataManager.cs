using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Manage Create() & Load() & Remove() methods for sub data managers
/// </summary>
public class DataManager : MonoBehaviour
{
    static public DataManager instance;
    static public bool isLoaded = false;
    static public bool isApplied = false;
    public PlayerDataManager playerDataManager;
    public InventoryDataManager inventoryDataManager;
    public ShortCutDataManager shortCutDataManager;

    //============================================================================
    //************************* Public Methods *********************************
    //============================================================================

    public void CreateData(string nickName)
    {
        playerDataManager.CreateData(nickName);
        inventoryDataManager.CreateData(nickName);
        shortCutDataManager.CreateData(nickName);
    }

    public void RemoveData(string nickName)
    {
        playerDataManager.RemoveData(nickName);
        inventoryDataManager.RemoveData(nickName);
        shortCutDataManager.RemoveData(nickName);
    }

    public void LoadAndApplyData(string nickName)
    {
        StartCoroutine(E_LoadAndApplyData(nickName));
    }

    /// <summary>
    /// Wait until other objects are ready to load & apply data.
    /// </summary>
    IEnumerator E_LoadAndApplyData(string nickName)
    {
        yield return new WaitUntil(() => Player.instance != null &&
                                         PlayerStateMachineManager.instance != null &&
                                         UIManager.instance != null &&
                                         UIManager.instance.isReady);
        playerDataManager.LoadData(nickName);

        yield return new WaitUntil(() => playerDataManager.isLoaded);
        inventoryDataManager.LoadData(nickName);

        yield return new WaitUntil(() => inventoryDataManager.isLoaded);
        shortCutDataManager.LoadData(nickName);

        yield return new WaitUntil(() => shortCutDataManager.isLoaded);
        isLoaded = true;
        playerDataManager.ApplyData();

        yield return new WaitUntil(() => playerDataManager.isApplied);
        inventoryDataManager.ApplyData();

        yield return new WaitUntil(() => inventoryDataManager.isApplied);
        yield return new WaitUntil(() => PlayerStateMachineManager.instance != null &&
                                         PlayerStateMachineManager.instance.isReady);
        shortCutDataManager.ApplyData();

        yield return new WaitUntil(()=> shortCutDataManager.isApplied);
        isApplied = true;
    }

    //============================================================================
    //************************* Privates Methods *********************************
    //============================================================================

    /// <summary>
    /// Singleton & Dont destroy this
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
    }

    /// <summary>
    /// Save all data when application quit
    /// </summary>
    private void OnApplicationQuit()
    {
        if (isApplied)
        {
            playerDataManager.SaveData();
            inventoryDataManager.SaveData();
            shortCutDataManager.SaveData();
        }
    }

    /// <summary>
    /// Save all data when application pause
    /// </summary>
    private void OnApplicationPause(bool pause)
    {
        if (isApplied)
        {
            playerDataManager.SaveData();
            inventoryDataManager.SaveData();
            shortCutDataManager.SaveData();
        }
    }
}