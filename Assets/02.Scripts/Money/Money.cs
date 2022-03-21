using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Money", menuName = "Money/Create New Money")]
public class Money : ScriptableObject
{
    public MoneyType type;
    public int maxPrice;
    public Sprite icon;
}

[System.Serializable]
public enum MoneyType
{
    CopperPenny,
    SilverPenny,
    GoldPenny,
    PaperBundle,
    GoldBag,
}