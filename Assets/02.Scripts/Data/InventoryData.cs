using System;
using System.Collections.Generic;
using UnityEngine;
public class InventoryData
{
    public List<InventoryItemData> items;
    public InventoryData()
    {
        items = new List<InventoryItemData>();
    }
    public void AddData(ItemType type, string itemName, int num, int slotNum)
    {
        InventoryItemData matchedData =
            items.Find(x => x.type == type && x.slotNum == slotNum);
        items.Remove(matchedData);
        items.Add(new InventoryItemData { type = type, itemName = itemName, num = num, slotNum = slotNum });
        InventoryDataManager.instance.SaveData();
    }
    public void RemoveData(ItemType type, string itemName, int slotNum)
    {
        InventoryItemData matchedData =
            items.Find(x => x.type == type && x.itemName == itemName && x.slotNum == slotNum);
        items.Remove(matchedData);
        InventoryDataManager.instance.SaveData();
    }
    /*public void OverwriteData(ItemType type, string itemName, int num, int slotNum)
    {
        if (num > 0)
        {
            InventoryItemData matchedData =
                items.Find(x => x.type == type && x.itemName == itemName && x.slotNum == slotNum);
            if(matchedData.num != num)
            {
                items.Remove(matchedData);
                items.Add(new InventoryItemData { type = type, itemName = itemName, num = num, slotNum = slotNum });
                InventoryDataManager.instance.SaveData();
            }
        }
        else
            RemoveData(type,itemName, slotNum);
    }*/
}
[System.Serializable]
public struct InventoryItemData
{
    public ItemType type;
    public string itemName;
    public int num;
    public int slotNum;
}