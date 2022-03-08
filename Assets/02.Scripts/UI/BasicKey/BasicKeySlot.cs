using System;
using UnityEngine;
public class BasicKeySlot : MonoBehaviour
{
    public bool isEmpty = true;
    public int num;
    public BasicKeyController controller;
    public void SetHere(BasicKeyController newController)
    {
        if (controller != null)
        {
            BasicKeyManager.instance.GetSlot(newController.slotNumber).controller = controller;
            controller.slotNumber = newController.slotNumber;
            controller.Deselect();
        }
        else
        {
            BasicKeyManager.instance.GetSlot(newController.slotNumber).Clear();
        }
        controller = newController;
        controller.slotNumber = num;
        controller.Deselect();
        isEmpty = false;
    }
    public void Clear()
    {
        isEmpty = true;
        controller = null;
    }
}