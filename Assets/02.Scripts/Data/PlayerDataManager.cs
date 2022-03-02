using Newtonsoft.Json;
using System;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager instance;
    public bool isLoaded = false;

    public PlayerData currentPlayerData;
    public PlayerData[] playerDatas;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    private void Start()
    {
        LoadAllPlayerDatas();
    }
    public void CreatePlayerData(string nickName)
    {
        PlayerData playerData = new PlayerData();
        playerData.nickName = nickName;
        playerData.stats = PlayerSettings.basicStats;
        playerData.skills = PlayerSettings.basicSkills;
        currentPlayerData = playerData;
        SavePlayerData(playerData);
    }
    public void SavePlayerData(Player player)
    {
        PlayerData data = new PlayerData
        {
            nickName = currentPlayerData.nickName,
            stats = player.stats,
            skills = player.skills,
        };
        string jsonPath = Application.persistentDataPath + "/" + "Player_" + data.nickName + ".json";
        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        System.IO.File.WriteAllText(jsonPath,jsonData);
    }
    public void SavePlayerData(PlayerData data)
    {
        string jsonPath = Application.persistentDataPath + "/" + "Player_" + data.nickName + ".json";
        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        System.IO.File.WriteAllText(jsonPath, jsonData);
    }
    public PlayerData GetPlayerData(string nickName)
    {
        string jsonPath = Application.persistentDataPath + "/" + "Player_" + nickName + ".json";
        string jsonData = System.IO.File.ReadAllText(jsonPath);
        PlayerData data = JsonConvert.DeserializeObject<PlayerData>(jsonData);
        return data;
    }
    public void LoadPlayerData(string nickName)
    {
        Debug.Log($"Load Player Data : {nickName}");
        string jsonPath = Application.persistentDataPath + "/" + "Player_" + nickName + ".json";
        string jsonData = System.IO.File.ReadAllText(jsonPath);
        PlayerData data = JsonConvert.DeserializeObject<PlayerData>(jsonData);
        currentPlayerData = data;
    }
    public void LoadAllPlayerDatas()
    {
        string[] pathNames = System.IO.Directory.GetFiles(Application.persistentDataPath);
        string[] fileNames = new string[pathNames.Length];
        string[] nickNames = new string[pathNames.Length];
        PlayerData[] datas = new PlayerData[pathNames.Length];
        for (int i = 0; i < pathNames.Length; i++)
        {
            fileNames[i] = pathNames[i].Replace(Application.persistentDataPath,"");
            string fileName = fileNames[i].Replace("Player_", "").Replace("\\","");
            fileName = fileName.Remove(fileName.IndexOf(".json"));
            nickNames[i] = fileName;
            datas[i] = GetPlayerData(nickNames[i]);
        }
        playerDatas = datas;
        isLoaded = true;
    }
    public void RemovePlayerData(string nickName)
    {
        string jsonPath = Application.persistentDataPath + "/" + "Player_" + nickName + ".json";
        System.IO.File.Delete(jsonPath);
        LoadAllPlayerDatas();
    }
}