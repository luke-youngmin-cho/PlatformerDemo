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

    public GameObject GetItemPrefabByName(string name)
    {
        GameObject prefab = null;
        foreach (var item in items)
        {
            if (item.name == name)
               prefab = item;
        }
        return prefab;
    }

    public List<GameObject> items = new List<GameObject> ();
}
