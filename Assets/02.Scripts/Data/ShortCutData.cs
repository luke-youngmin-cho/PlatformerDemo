using UnityEngine;
using System;
using System.Collections.Generic;
[System.Serializable]
public class ShortCutData
{
    public List<ShortCutData_Item> itemsData;
    public List<ShortCutData_BasicKey> basicKeysData;
    public List<ShortCutData_Skill> skillsData;
    ShortCutData()
    {
        itemsData = new List<ShortCutData_Item>();
        basicKeysData = new List<ShortCutData_BasicKey>();
        skillsData = new List<ShortCutData_Skill>();
    }

    public void AddShortCutItemData(ShortCutData_Item data)
    {
        for (int i = itemsData.Count -1; i > -1; i--)
        {
            if(data.keyCode == itemsData[i].keyCode)
            {
                itemsData.RemoveAt(i);
                itemsData.Add(data);
                break;
            }   
        }
    }
    public void AddShortCutBasicKeyData(ShortCutData_BasicKey data)
    {
        for (int i = basicKeysData.Count - 1; i > -1; i--)
        {
            if (data.keyCode == basicKeysData[i].keyCode)
            {
                basicKeysData.RemoveAt(i);
                basicKeysData.Add(data);
                break;
            }
        }
    }
    public void AddShortCutSkillData(ShortCutData_Skill data)
    {
        for (int i = skillsData.Count - 1; i > -1; i--)
        {
            if (data.keyCode == skillsData[i].keyCode)
            {
                skillsData.RemoveAt(i);
                skillsData.Add(data);
                break;
            }
        }
    }
}

public struct ShortCutData_Item
{
    public KeyCode keyCode;
    public ShortCutType type;
    public ItemType itemType;
    public int slotNum;
}
public struct ShortCutData_BasicKey
{
    public KeyCode keyCode;
    public ShortCutType type;
    public string basicKeyName;
}
public struct ShortCutData_Skill
{
    public KeyCode keyCode;
    public ShortCutType type;
    public PlayerState playerState;
}