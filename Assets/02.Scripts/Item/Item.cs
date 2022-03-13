
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/Create New Item")]
public class Item : ScriptableObject
{
    public ItemType type;
    public string itemName;
    public int price;
    public int numMax = 99;
    public Sprite icon;
}

[System.Serializable]
public enum ItemType
{
    Equip,
    Spend,
    ETC,
    Cash
}
