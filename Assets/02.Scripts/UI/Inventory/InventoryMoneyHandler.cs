using System;
using UnityEngine;
using UnityEngine.UI;
public class InventoryMoneyHandler : MonoBehaviour
{
    public InputField dropMoneyPrice;

    public void DropMoney()
    {
        int amount = Convert.ToInt32(dropMoneyPrice.text);
        InventoryView.instance.moneyView.money -= amount;
        GameObject moneyGO = Instantiate(MoneyAssets.instance.GetMoneyPrefabByPrice(amount), Player.instance.transform.position, Quaternion.identity);
        moneyGO.GetComponent<MoneyController>().price = amount;
    }
}