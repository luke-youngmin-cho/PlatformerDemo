using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// game money & item data
/// </summary>
public class InventoryData
{
    public int gameMoney;
    public List<InventoryItemData> items;

    //============================================================================
    //************************* Public Methods *********************************
    //============================================================================

    public InventoryData()
    {
        items = new List<InventoryItemData>();
    }

    /// <summary>
    /// to add item
    /// </summary>
    /// <param name="type"> Item category type. (equip, spend, etc, cash ) </param>
    /// <param name="itemName"> Item name of item scriptable object </param>
    /// <param name="num"> number of items </param>
    /// <param name="slotNum"> index number of slot which is include the item </param>
    public void AddData(ItemType type, string itemName, int num, int slotNum)
    {
        InventoryItemData matchedData =
            items.Find(x => x.type == type && x.slotNum == slotNum);
        items.Remove(matchedData);
        items.Add(new InventoryItemData { type = type, itemName = itemName, num = num, slotNum = slotNum });
        InventoryDataManager.instance.SaveData();
    }

    /// <summary>
    /// try to find item with type & name , remove data
    /// </summary>
    public void RemoveData(ItemType type, string itemName, int slotNum)
    {
        InventoryItemData matchedData =
            items.Find(x => x.type == type && x.itemName == itemName && x.slotNum == slotNum);
        items.Remove(matchedData);
        InventoryDataManager.instance.SaveData();
    }
}

/// <summary>
/// Item types in inventory
/// </summary>
[System.Serializable]
public struct InventoryItemData
{
    public ItemType type;
    public string itemName;
    public int num;
    public int slotNum;
}