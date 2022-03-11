using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
public class ShortCutDataManager : MonoBehaviour
{
    public static ShortCutDataManager instance;
    public bool isLoaded { get { return data != null; } }
    public bool isApplied = false;
    public ShortCutData data;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }
    public void CreateData(string nickName)
    {
        data = LoadDefaultData();
        string jsonPath = $"{Application.persistentDataPath}/ShortCutDatas/ShortCut_{nickName}.json";
        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        System.IO.File.WriteAllText(jsonPath, jsonData);
     
        Debug.Log($"Short cut data of {nickName} Created");
    }
    public ShortCutData LoadDefaultData()
    {
        ShortCutData tmpData = null;
        string jsonPath = $"{Application.persistentDataPath}/DefaultDatas/ShortCut_Default.json";
        if (System.IO.File.Exists(jsonPath))
        {
            string jsonData = System.IO.File.ReadAllText(jsonPath);
            tmpData = JsonConvert.DeserializeObject<ShortCutData>(jsonData);
        }
        else
            Debug.Log($"Failed to Load Short cut Default data, default path -> {jsonPath}");
        return tmpData;
    }
    public void SaveData()
    {
        if (data == null) return;
        string jsonPath = $"{Application.persistentDataPath}/ShortCutDatas/ShortCut_{PlayerDataManager.instance.data.nickName}.json";
        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        System.IO.File.WriteAllText(jsonPath,jsonData);
        Debug.Log("Short cut data Saved");
    }
    public void LoadData(string nickName)
    {
        string jsonPath = $"{Application.persistentDataPath}/ShortCutDatas/ShortCut_{nickName}.json";
        if (System.IO.File.Exists(jsonPath))
        {
            string jsonData = System.IO.File.ReadAllText(jsonPath);
            data = JsonConvert.DeserializeObject<ShortCutData>(jsonData);
            Debug.Log($"Short cut data of {nickName} Loaded");
        }
        else
            Debug.Log($"Failed to Load Short cut data of {nickName} -> {jsonPath}");
    }
    public void ApplyData()
    {
        foreach (var sub in data.itemsData)
        {
            ShortCut shortCut = null;
            if(ShortCutsView.instance.TryGetShortCut(sub.keyCode, out shortCut))
            {
                InventoryItemHandlerBase inventoryItemHandler =
                    InventoryView.instance.GetItemsViewByItemType(ItemType.Spend).GetInventoryItemHandlerBySlotNum(sub.slotNum);
                if(inventoryItemHandler != null)
                    shortCut.RegisterIconAndEvent(ShortCutType.Item, 
                                                  inventoryItemHandler.item.icon, 
                                                  inventoryItemHandler.UseItem);
                else
                    Debug.Log($"Tried to apply short cut data {sub.keyCode}, item({sub.slotNum}), but no item handler");
            };
        }
        foreach (var sub in data.basicKeysData)
        {
            ShortCut shortCut = null;
            if(ShortCutsView.instance.TryGetShortCut(sub.keyCode, out shortCut))
            {
                BasicKey basicKey = BasicKeyAssets.instance.GetBasicKeyBySpriteName(sub.basicKeyName);
                if (basicKey != null)
                {
                    shortCut.RegisterIconAndEvent(ShortCutType.BasicKey,
                                                  basicKey.icon,
                                                  basicKey.OnUse);
                    Destroy(BasicKeysView.instance.GetBasicKeyHandlerByName(sub.basicKeyName).gameObject);
                    Debug.Log($"Basic Key {BasicKeysView.instance.GetBasicKeyHandlerByName(sub.basicKeyName).gameObject.name} short cut registered, Destroyed gameobejct");
                }
                    
                else
                    Debug.Log($"Tried to apply short cut data {sub.keyCode}, basickey({sub.basicKeyName}), but no basicKey");
            }

        }
        foreach (var sub in data.skillsData)
        {
            ShortCut shortCut = null;
            if (ShortCutsView.instance.TryGetShortCut(sub.keyCode, out shortCut))
            {
                Skill skill = SkillAssets.instance.GetSkillByState(sub.playerState);
                if(skill != null)
                {
                    Debug.Log($"Short Cut data skill: {skill.playerState}");
                    if (PlayerStateMachineManager.instance.TryGetStateMachineByState(skill.playerState, out PlayerStateMachine newStateMachine))
                    {
                        newStateMachine.keyCode = shortCut._keyCode;
                        PlayerStateMachineManager.instance.RefreshMachineDictionaries();
                        shortCut.RegisterIconAndEvent(ShortCutType.Skill,
                                                  SkillAssets.instance.GetSkillByState(sub.playerState).icon,
                                                  delegate { PlayerStateMachineManager.instance.keyInput = shortCut._keyCode; });
                    }
                }   
                else
                    Debug.Log($"Tried to apply skill data {sub.keyCode}, skill({sub.playerState}), but no skill");
            }   
        }
        Debug.Log("Short cut data Applied");
        isApplied = true;
    }
    public void RemoveData(string nickName)
    {
        string jsonPath = $"{Application.persistentDataPath}/ShortCutDatas/ShortCut_{nickName}.json";
        if (System.IO.File.Exists(jsonPath))
        {
            System.IO.File.Delete(jsonPath);
            Debug.Log($"Short cut data of {nickName} Removed");
        }
            
    }
}

