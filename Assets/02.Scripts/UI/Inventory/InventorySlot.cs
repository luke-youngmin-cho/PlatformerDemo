﻿using UnityEngine;
using UnityEngine.EventSystems;
public class InventorySlot : MonoBehaviour
{
    public bool isEmpty = true;
    public int num;
    public InventoryItemHandler controller;
    public void SetItemHere(InventoryItemHandler newController)
    {   
        if(controller != null)
        {
            InventoryView.instance.GetSlot(newController.slotNumber).controller = controller;
            controller.slotNumber = newController.slotNumber;
            controller.DeselectItem();
        }
        else
        {
            InventoryView.instance.GetSlot(newController.slotNumber).ResetSlot();
        }
        

        controller = newController;
        controller.slotNumber = num;
        controller.DeselectItem();
        isEmpty = false;
    }
    public void ResetSlot()
    {
        isEmpty = true;
        controller = null;
    }

}
