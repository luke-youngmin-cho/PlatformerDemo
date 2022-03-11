using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAssets : MonoBehaviour
{
    private static ItemAssets _instance;
    public static ItemAssets instance
    {
        get
        {
            if (_instance == null)
                _instance = Instantiate(Resources.Load<ItemAssets>("Assets/ItemAssets"));
            return _instance;
        }
    }

    public Item GetItemByName(string name)
    {
        Item tmpItem = null;
        foreach (var item in items)
        {
            if(item.name == name)
            {
                tmpItem = item;
                break;
            }   
        }
        return tmpItem;
    }
    public GameObject GetItemPrefabByName(string name)
    {
        GameObject tmpPrefab = null;
        foreach (var item in itemPrefabs)
        {
            if (item.name == name)
               tmpPrefab = item;
        }
        return tmpPrefab;
    }

    public List<Item> items = new List<Item>();
    public List<GameObject> itemPrefabs = new List<GameObject> ();
}
