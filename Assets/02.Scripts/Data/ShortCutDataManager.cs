using Newtonsoft.Json;
using UnityEngine;
public class ShortCutDataManager : MonoBehaviour
{
    public static ShortCutDataManager instance;
    public ShortCutData data;
    private void Awake()
    {
        instance = this;
    }
    public void SaveShortCutData()
    {
        string jsonPath = Application.persistentDataPath + "/" + "ShortCut_" + PlayerDataManager.instance.currentPlayerData.nickName + ".json";
        string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
        System.IO.File.WriteAllText(jsonPath,jsonData);
    }
    public void LoadShortCutData()
    {
        string jsonPath = Application.persistentDataPath + "/" + "ShortCut_" + PlayerDataManager.instance.currentPlayerData.nickName + ".json";
        string jsonData = System.IO.File.ReadAllText(jsonPath);
        ShortCutData data = JsonConvert.DeserializeObject<ShortCutData>(jsonData);
        this.data = data;
    }
    private void ApplyShortCutData()
    {
        foreach (var sub in data.itemsData)
        {
            ShortCut shortCut = null;
            if(ShortCutsView.instance.TryGetShortCut(sub.keyCode, out shortCut))
            {
                InventoryItemHandler inventoryItemController =
                    InventoryView.instance.GetInventoryItemControllerBySlotNum(sub.slotNum);
                shortCut.RegisterIconAndEvent(ShortCutType.Item, 
                                              inventoryItemController.itemIcon, 
                                              inventoryItemController.UseItem);
            };
        }
        foreach (var sub in data.basicKeysData)
        {
            ShortCut shortCut = null;
            if(ShortCutsView.instance.TryGetShortCut(sub.keyCode, out shortCut))
            {
                BasicKeyHandler basicKeyController =
                    BasicKeysView.instance.GetBasicKeyControllerByName(sub.basicKeyName);
                shortCut.RegisterIconAndEvent(ShortCutType.BasicKey, 
                                              basicKeyController.basicKey.icon, 
                                              basicKeyController.basicKey.OnUse);
            }
        }
        foreach (var sub in data.skillsData)
        {
            ShortCut shortCut = null;
            if (ShortCutsView.instance.TryGetShortCut(sub.keyCode, out shortCut))
            {
                shortCut.RegisterIconAndEvent(ShortCutType.Skill,
                                              SkillAssets.instance.GetSkillByState(sub.playerState).icon, 
                                              delegate {PlayerStateMachineManager.instance.keyInput = shortCut._keyCode; });
            }
        }
    }
}

