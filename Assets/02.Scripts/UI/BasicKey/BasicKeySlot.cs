using System;
using UnityEngine;

/// <summary>
/// Fit position for basic key 
/// </summary>
public class BasicKeySlot : MonoBehaviour
{
    public bool isEmpty = true;
    public int num;
    public BasicKeyHandler controller;


    //============================================================================
    //************************* Public Methods ***********************************
    //============================================================================

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