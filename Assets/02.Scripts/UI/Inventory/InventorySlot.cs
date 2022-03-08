using UnityEngine;
using UnityEngine.EventSystems;
public class InventorySlot : MonoBehaviour
{
    public bool isEmpty = true;
    public int num;
    public InventoryItemController controller;
    public void SetItemHere(InventoryItemController newController)
    {   
        if(controller != null)
        {
            InventoryManager.instance.GetSlot(newController.slotNumber).controller = controller;
            controller.slotNumber = newController.slotNumber;
            controller.DeselectItem();
        }
        else
        {
            InventoryManager.instance.GetSlot(newController.slotNumber).ResetSlot();
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
