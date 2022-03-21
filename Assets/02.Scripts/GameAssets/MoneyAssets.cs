using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyAssets : MonoBehaviour
{
    private static MoneyAssets _instance;
    public static MoneyAssets instance
    {
        get
        {
            if (_instance == null)
                _instance = Instantiate(Resources.Load<MoneyAssets>("Assets/MoneyAssets"));
            return _instance;
        }
    }
    public Money money_CopperPenny;
    public Money money_SilverPenny;
    public Money money_GoldPenny;
    public Money money_PaperBundle;
    public Money money_GoldBag;
    public GameObject money_CopperPennyPrefab;
    public GameObject money_SilverPennyPrefab;
    public GameObject money_GoldPennyPrefab;
    public GameObject money_PaperBundlePrefab;
    public GameObject money_GoldBagPrefab;
    public Money GetMoneyByPrice(int price)
    {
        Money money = null;
        MoneyType moneyType = MoneyType.CopperPenny;

        if (price > money_CopperPenny.maxPrice)
            moneyType = MoneyType.SilverPenny;
        if (price > money_SilverPenny.maxPrice)
            moneyType = MoneyType.GoldPenny;
        if (price > money_GoldPenny.maxPrice)
            moneyType = MoneyType.PaperBundle;
        if (price > money_PaperBundle.maxPrice)
            moneyType = MoneyType.GoldBag;

        switch (moneyType)
        {
            case MoneyType.CopperPenny:
                money = money_CopperPenny;
                break;
            case MoneyType.SilverPenny:
                money = money_SilverPenny;
                break;
            case MoneyType.GoldPenny:
                money = money_GoldPenny;
                break;
            case MoneyType.PaperBundle:
                money = money_PaperBundle;
                break;
            case MoneyType.GoldBag:
                money = money_GoldBag;
                break;
            default:
                break;
        }
        return money;
    }
    public GameObject GetMoneyPrefabByPrice(int price)
    {
        GameObject moneyPrefab = null;
        MoneyType moneyType = MoneyType.CopperPenny;

        if (price > money_CopperPenny.maxPrice)
            moneyType = MoneyType.SilverPenny;
        if (price > money_SilverPenny.maxPrice)
            moneyType = MoneyType.GoldPenny;
        if (price > money_GoldPenny.maxPrice)
            moneyType = MoneyType.PaperBundle;
        if (price > money_PaperBundle.maxPrice)
            moneyType = MoneyType.GoldBag;

        switch (moneyType)
        {
            case MoneyType.CopperPenny:
                moneyPrefab = money_CopperPennyPrefab;
                break;
            case MoneyType.SilverPenny:
                moneyPrefab = money_SilverPennyPrefab;
                break;
            case MoneyType.GoldPenny:
                moneyPrefab = money_GoldPennyPrefab;
                break;
            case MoneyType.PaperBundle:
                moneyPrefab = money_PaperBundlePrefab;
                break;
            case MoneyType.GoldBag:
                moneyPrefab = money_GoldBagPrefab;
                break;
            default:
                break;
        }
        return moneyPrefab;
    }
}
