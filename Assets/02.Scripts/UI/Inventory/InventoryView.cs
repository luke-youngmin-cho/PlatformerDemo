using System;
using System.Collections;
using UnityEngine;
public class InventoryView : MonoBehaviour
{
    public static InventoryView instance;
    public bool isReady { get {
            return equipItemsViewInstance.isReady &&
                   spendItemsViewInstance.isReady &&
                   etcItemsViewInstance.isReady &&
                   cashItemsViewInstance.isReady;
        } }
    public InventoryItemsView equipItemsViewInstance;
    public InventoryItemsView spendItemsViewInstance;
    public InventoryItemsView etcItemsViewInstance;
    public InventoryItemsView cashItemsViewInstance;
    public InventoryMoneyView moneyView;
    private void Awake()
    {
        instance = this;
        equipItemsViewInstance.gameObject.SetActive(true);
        spendItemsViewInstance.gameObject.SetActive(true);
        etcItemsViewInstance.gameObject.SetActive(true);    
        cashItemsViewInstance.gameObject.SetActive(true);
    }
    private void Update()
    {
        
    }
    private void Start()
    {
        StartCoroutine(E_Start());
    }
    IEnumerator E_Start()
    {
        yield return new WaitUntil(() => isReady);
        equipItemsViewInstance.gameObject.SetActive(false);
        spendItemsViewInstance.gameObject.SetActive(false);
        etcItemsViewInstance.gameObject.SetActive(false);
        cashItemsViewInstance.gameObject.SetActive(false);
    }
    public InventoryItemsView GetItemsViewByItemType(ItemType type)
    {
        InventoryItemsView itemsView = null;
        switch (type)
        {
            case ItemType.Equip:
                itemsView = equipItemsViewInstance;
                break;
            case ItemType.Spend:
                itemsView = spendItemsViewInstance;
                break;
            case ItemType.ETC:
                itemsView = etcItemsViewInstance;
                break;
            case ItemType.Cash:
                itemsView = cashItemsViewInstance;
                break;
            default:
                break;
        }
        return itemsView;
    }
    
    
}