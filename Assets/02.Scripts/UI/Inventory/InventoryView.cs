using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class InventoryView : MonoBehaviour
{
    public static InventoryView instance;
    public bool isReady = false;
    public Transform itemContent;
    public GameObject inventoryItemPrefab;
    public InventorySlot[] slots;
    public int totalSlots = 36;
    public GameObject slotPrefab;

    public GameObject selectedItem;
    public Camera cam;

    public Player player;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        for(int i = 0; i < totalSlots; i++)
        {
            InventorySlot tmpSlot = Instantiate(slotPrefab, itemContent).GetComponent<InventorySlot>();
            tmpSlot.num = i;
        }
        slots = GetComponentsInChildren<InventorySlot>();
        isReady = true;
    }
    private void Update()
    {
        if (selectedItem != null)
        {
            Vector3 pos = Input.mousePosition;
            selectedItem.transform.position = pos;
        }
    }
    public void AddItem(Item item, int num)
    {
        int remains = num;
        Debug.Log($"Do Add Item {remains}");
        while (remains > 0)
        {
            InventoryItemHandler controller = FindItemControllerEnoughSpace(item);
            if (controller != null)
            {
                Debug.Log("Increase number of exist item controller");
                if (controller.itemNumMax - controller.itemNum >= remains)
                {
                    controller.itemNum += remains;
                    remains -= remains;
                }   
                else
                {
                    controller.itemNum += (controller.itemNumMax - controller.itemNum);
                    remains -= (controller.itemNumMax - controller.itemNum);
                }
            }
            else
            {
                InventorySlot slot = FindEmptySlot();
                Debug.Log("Newly create Inventory item controller");
                if (slot != null)
                {
                    GameObject go = Instantiate(inventoryItemPrefab, slot.transform);
                    controller = go.GetComponent<InventoryItemHandler>();
                    controller.slotNumber = slot.num;
                    controller.inventoryItemObject = go;
                    controller.type = item.type;
                    controller.itemName = item.itemName;
                    controller.itemIcon = item.icon;
                    controller.price = item.price;
                    controller.itemNumMax = item.numMax;
                    if (controller.itemNumMax - controller.itemNum >= remains)
                    {
                        controller.itemNum += remains;
                        remains -= remains;
                    }
                    else
                    {
                        controller.itemNum += (controller.itemNumMax - controller.itemNum);
                        remains -= (controller.itemNumMax - controller.itemNum);
                    }
                    slot.SetItemHere(controller);
                }
            }
        }
        
    }
    
    private InventorySlot FindEmptySlot()
    {
        InventorySlot slot = null;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].isEmpty)
            {
                slot = slots[i];
                break;
            }   
        }
        return slot;
    }
    private InventoryItemHandler FindItemControllerEnoughSpace(Item item)
    {
        InventoryItemHandler controller = null;
        InventoryItemHandler[] controllers = GetComponentsInChildren<InventoryItemHandler>();
        //Debug.Log(controllers.Length);
        for (int i = 0; i < controllers.Length; i++)
        {
            if ((item.itemName == controllers[i].itemName) &&
                (controllers[i].addPossibleNum > 0))
            {
                controller = controllers[i];
                break;
            }
        }
        return controller;
    }
    public InventoryItemHandler GetInventoryItemControllerBySlotNum(int slotNum)
    {
        InventoryItemHandler[] controllers = GetComponentsInChildren<InventoryItemHandler>();
        foreach (var controller in controllers)
        {
            if (controller.slotNumber == slotNum)
            {   
                return controller;
            }   
        }
        return null;
    }
    public InventorySlot GetSlot(int slotNum)
    {
        return slots[slotNum];
    }
    public void ResetSlot(int slotNum)
    {
        slots[slotNum].ResetSlot();
    }
}
