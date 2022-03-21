using UnityEngine;
using System.Collections;
using Newtonsoft.Json;

/// <summary>
/// Create & Load & Save & Apply Inventory Data
/// </summary>
public class InventoryDataManager : MonoBehaviour
{
    public static InventoryDataManager instance;

    public InventoryData data;
    public bool isLoaded 
    { 
        get 
        { 
            return data != null; 
        } 
    }
    public bool isApplied = false;
    

    //============================================================================
    //*************************** Public Methods *********************************
    //============================================================================

    public void CreateData(string nickName)
    {
        string jsonPath = $"{Application.persistentDataPath}/InventoryDatas/Inventory_{nickName}.json";
        data = LoadDefaultData();
        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        Debug.Log($"Inventory data of {nickName} Created");
        System.IO.File.WriteAllText(jsonPath, jsonData);
    }

    public InventoryData LoadDefaultData()
    {
        InventoryData tmpData = null;
        string jsonPath = $"{Application.persistentDataPath}/DefaultDatas/Inventory_Default.json";
        if (System.IO.File.Exists(jsonPath))
        {
            string jsonData = System.IO.File.ReadAllText(jsonPath);
            tmpData = JsonConvert.DeserializeObject<InventoryData>(jsonData);
        }
        else
            Debug.LogError($"Failed to load Default: InventoryData ,default path -> {jsonPath}");

        return tmpData;
    }

    public void SaveData()
    {
        if (data == null) return;

        string jsonPath = $"{Application.persistentDataPath}/InventoryDatas/Inventory_{PlayerDataManager.instance.data.nickName}.json";
        Debug.Log($"save items : {data.items.Count} , {jsonPath}");
        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        Debug.Log($"Inventory data Saved");
        System.IO.File.WriteAllText(jsonPath, jsonData);
    }

    public void LoadData(string nickName)
    {
        string jsonPath = $"{Application.persistentDataPath}/InventoryDatas/Inventory_{nickName}.json";
        if (System.IO.File.Exists(jsonPath))
        {
            string jsonData = System.IO.File.ReadAllText(jsonPath);
            data = JsonConvert.DeserializeObject<InventoryData>(jsonData);
            Debug.Log($"Inventory data of {nickName} Loaded");
        }
        else
            Debug.LogError($"Failed to load : InventoryData ,{nickName} -> {jsonPath}");
    }

    public void ApplyData()
    {
        for (int i = 0; i < data.items.Count; i++)
        {
            Item item = ItemAssets.instance.GetItemByName(data.items[i].itemName);
            Debug.Log($"Applying Inventory Data , item : {item != null}, {data.items[i].itemName} {data.items[i].num}, {data.items[i].slotNum}");
            InventoryView.instance.GetItemsViewByItemType(item.type).SetItem(item, 
                                                                             data.items[i].num,
                                                                             data.items[i].slotNum);
            //Debug.Log($"Inventory data set {data.items[i].num},{data.items[i].slotNum}");
        }
        InventoryView.instance.moneyView.money = data.gameMoney;
        Debug.Log($"Inventory data Applied");
        isApplied = true;
    }

    public void RemoveData(string nickName)
    {
        string jsonPath = $"{Application.persistentDataPath}/InventoryDatas/Inventory_{nickName}.json";
        if(System.IO.File.Exists(jsonPath))
            System.IO.File.Delete(jsonPath);
    }

    //============================================================================
    //*************************** Private Methods ********************************
    //============================================================================

    // singleton
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
}


