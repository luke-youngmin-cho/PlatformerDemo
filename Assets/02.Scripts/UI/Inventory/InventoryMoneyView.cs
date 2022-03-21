using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Presenter for interaction among inventoryUI <-> Battle field & shop(for the future)
/// </summary>
public class InventoryMoneyView : MonoBehaviour
{
    private int _money;
    public int money
    {
        set
        {
            _money = value;
            moneyText.text = _money.ToString();
            if(DataManager.isApplied)
                InventoryDataManager.instance.data.gameMoney = _money;
        }
        get { return _money; }
    }
    public Text moneyText;

    public GameObject moneyHandler;
    public void ToggleMoneyHandler()
    {
        moneyHandler.SetActive(!moneyHandler.activeSelf);
    }
}