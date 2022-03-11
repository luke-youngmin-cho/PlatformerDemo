using Newtonsoft.Json;
using System;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager instance;
    public bool isReady = false;
    public bool isLoaded{ get { return data != null; } }
    public bool isApplied = false;
    public PlayerData data;
    public PlayerData[] playerDatas;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
        LoadAllDatas();
    }
    public void CreateData(string nickName)
    {
        data = LoadDefaultData();
        data.nickName = nickName;
        string jsonPath = $"{Application.persistentDataPath}/PlayerDatas/Player_{data.nickName}.json";
        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        System.IO.File.WriteAllText(jsonPath, jsonData);
        Debug.Log($"Player data of {nickName} Created");
        
    }
    public PlayerData LoadDefaultData()
    {
        PlayerData tmpData = null;
        string jsonPath = $"{Application.persistentDataPath}/DefaultDatas/Player_Default.json";
        if (System.IO.File.Exists(jsonPath))
        {
            string jsonData = System.IO.File.ReadAllText(jsonPath);
            tmpData = JsonConvert.DeserializeObject<PlayerData>(jsonData);
        }
        else
            Debug.Log($"Failed to load Player Default , Default path -> {jsonPath}");
        return tmpData;
    }
    public void SaveData()
    {
        if(data == null) return;
        string jsonPath = $"{Application.persistentDataPath}/PlayerDatas/Player_{data.nickName}.json";
        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        System.IO.File.WriteAllText(jsonPath,jsonData);
        // refresh skill UI
        if (SkillsView.instance != null && SkillsView.instance.isReady && Player.instance.isReady)
            SkillsView.instance.RefreshSkillList();
        Debug.Log($"Player data Saved");
    }
    public PlayerData GetPlayerData(string nickName)
    {
        string jsonPath = $"{Application.persistentDataPath}/PlayerDatas/Player_{nickName}.json";
        string jsonData = System.IO.File.ReadAllText(jsonPath); 
        PlayerData data = JsonConvert.DeserializeObject<PlayerData>(jsonData);
        return data;
    }
    public void LoadData(string nickName)
    {
        string jsonPath = $"{Application.persistentDataPath}/PlayerDatas/Player_{nickName}.json";
        if (System.IO.File.Exists(jsonPath))
        {
            string jsonData = System.IO.File.ReadAllText(jsonPath);
            data = JsonConvert.DeserializeObject<PlayerData>(jsonData);
            Debug.Log($"Player data of {nickName} Loaded");
        }
        else
            Debug.Log($"Failed to load Player data of {nickName} -> {jsonPath}");
    }
    public void LoadAllDatas()
    {
        string[] pathNames = System.IO.Directory.GetFiles($"{Application.persistentDataPath}/PlayerDatas/");
        string[] fileNames = new string[pathNames.Length];
        string[] nickNames = new string[pathNames.Length];
        PlayerData[] datas = new PlayerData[pathNames.Length];
        for (int i = 0; i < pathNames.Length; i++)
        {            
            fileNames[i] = pathNames[i].Replace($"{Application.persistentDataPath}/PlayerDatas/","");
            string fileName = fileNames[i].Replace("Player_", "").Replace("\\","");
            fileName = fileName.Remove(fileName.IndexOf(".json"));
            nickNames[i] = fileName;
            datas[i] = GetPlayerData(nickNames[i]);
        }
        playerDatas = datas;
        isReady = true;
    }
    public void ApplyData()
    {
        Player.instance.stats = data.stats;
        Player.instance.skillStatsList = data.skillstatsList;
        Debug.Log("Player data applied");
        isApplied = true;
    }
    public void RemoveData(string nickName)
    {
        string jsonPath = $"{Application.persistentDataPath}/PlayerDatas/Player_{nickName}.json";
        if (System.IO.File.Exists(jsonPath))
        {
            System.IO.File.Delete(jsonPath);
            Debug.Log($"Player data of {nickName} Removed");
            LoadAllDatas();
        }
    }
}