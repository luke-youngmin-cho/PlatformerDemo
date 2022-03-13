using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShortCutsView : MonoBehaviour
{
    public static ShortCutsView instance;
    public bool isReady = false;
    KeyCode keyInput;
    public Dictionary<KeyCode, ShortCut> shortCuts = new Dictionary<KeyCode, ShortCut>();

    [SerializeField] ShortCutHandler shortCutHandler;

    Dictionary<KeyCode, float> keyDelayDictionary = new Dictionary<KeyCode, float>();
    private float delay = 0.5f;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        ShortCut[] tmpShortCuts = transform.Find("ShortCutSettingsPanel").Find("ShortCuts").GetComponentsInChildren<ShortCut>();
        for (int i = 0; i < tmpShortCuts.Length; i++)
        {
            shortCuts.Add(tmpShortCuts[i]._keyCode, tmpShortCuts[i]);
            //Debug.Log($"Shortcut registered : {tmpShortCuts[i]._keyCode}");
        }
        isReady = true;
    }
    private void Update()
    {
        UpdateKeyEventDelayDictionary();
        ExecuteKeyEvent();        
    }
    public void UpdateKeyEventDelayDictionary()
    {
        for (int i = keyDelayDictionary.Count - 1; i > -1; i--)
        {
            KeyValuePair<KeyCode, float> pair = keyDelayDictionary.ElementAt(i);
            if (Time.time - pair.Value > delay)
                keyDelayDictionary.Remove(pair.Key);
        }
    }
    public void ExecuteKeyEvent()
    {
        if (!keyDelayDictionary.ContainsKey(keyInput))
        {
            ShortCut shortCut = null;
            if (shortCuts.TryGetValue(keyInput, out shortCut))
            {
                keyDelayDictionary.Add(keyInput, Time.time);
                shortCut.TryKeyEvent();
            }
        }
        keyInput = KeyCode.None;
    }
    public void ActiveShortCutHandler(ShortCutType type, Sprite icon, KeyCode keyCode, ShortCut.KeyEvent keyEvent)
    {
        shortCutHandler.ActivateWithInfo(type, icon, keyCode, keyEvent);
    }
    public bool TryGetShortCut(KeyCode keyCode,out ShortCut shortCut)
    {
        //Debug.Log($"Try get short cut : {keyCode} , {shortCuts.ContainsKey(keyCode)}");
        return shortCuts.TryGetValue(keyCode, out shortCut);
    }
    public bool TryGetShortCut(string iconName, out ShortCut shortCut)
    {
        Debug.Log($"Tried to get short cut for {iconName}");
        bool isExist = false;
        shortCut = null;
        foreach (var sub in shortCuts){
            if (sub.Value._image.sprite != null &&
                sub.Value._image.sprite.name == iconName){
                shortCut = sub.Value;
                isExist = true;
                break;
            }   
        }
        return isExist;
    }
    public void SaveData()
    {
        ShortCutDataManager.instance.data.Clear();
        foreach (var sub in shortCuts)
        {
            KeyCode tmpKeyCode = sub.Key;
            ShortCut tmpShortCut = sub.Value;
            Debug.Log($"Saving {tmpKeyCode} , {tmpShortCut._type}");
            switch (tmpShortCut._type)
            {   
                case ShortCutType.None:
                    break;
                case ShortCutType.BasicKey:
                    if(tmpShortCut._image.sprite != null)
                    {
                        ShortCutData_BasicKey shortCutData_BasicKey = new ShortCutData_BasicKey()
                        {
                            keyCode = tmpKeyCode,
                            type = ShortCutType.BasicKey,
                            basicKeyName = tmpShortCut._image.sprite.name
                        };
                        ShortCutDataManager.instance.data.AddShortCutBasicKeyData(shortCutData_BasicKey);
                    }
                    break;
                case ShortCutType.Item:
                    Debug.Log($"Try to save item shortcut : {tmpShortCut._image.sprite.name}");
                    if (InventoryView.instance.GetItemsViewByItemType(ItemType.Spend)
                        .TryGetInventoryItemHandlerByName(tmpShortCut._image.sprite.name, out InventoryItemHandlerBase inventoryItemHandlerBase))
                    {
                        ShortCutData_SpendItem shortCutData_SpendItem = new ShortCutData_SpendItem()
                        {
                            keyCode = tmpKeyCode,
                            type = ShortCutType.Item,
                            itemType = ItemType.Spend,
                            slotNum = inventoryItemHandlerBase.slotNumber
                        };
                        ShortCutDataManager.instance.data.AddShortCutItemData(shortCutData_SpendItem);
                    }
                    break;
                case ShortCutType.Skill:
                    if(PlayerStateMachineManager.instance.
                        TryGetStateMachineByKeyCode(tmpKeyCode, out PlayerStateMachine playerStateMachine))
                    {
                        ShortCutData_Skill shortCutData_Skill = new ShortCutData_Skill()
                        {
                            keyCode = tmpKeyCode,
                            type = ShortCutType.Skill,
                            playerState = playerStateMachine.playerStateType
                        };
                        ShortCutDataManager.instance.data.AddShortCutSkillData(shortCutData_Skill);
                    }
                    break;
                default:
                    break;
            }
        }
        ShortCutDataManager.instance.SaveData();
    }
    private void OnGUI()
    {
        Event e = Event.current;
        if (e.isKey && e.keyCode != KeyCode.None)
            keyInput = e.keyCode;
    }
}
