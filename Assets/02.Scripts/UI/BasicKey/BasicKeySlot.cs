using System;
using UnityEngine;
public class BasicKeySlot : MonoBehaviour
{
    public bool isEmpty = true;
    public int num;
    public BasicKeyHandler controller;
    public void SetHere(BasicKeyHandler newController)
    {
        if (controller != null)
        {
            BasicKeysView.instance.GetSlot(newController.slotNumber).controller = controller;
            controller.slotNumber = newController.slotNumber;
            controller.Deselect();
        }
        else
        {
            BasicKeysView.instance.GetSlot(newController.slotNumber).Clear();
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