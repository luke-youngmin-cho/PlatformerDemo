using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InventorySpendItemHandler : InventoryItemHandlerBase
{
    override public void OnPointerClick(PointerEventData eventData)
    {
        if (InventoryView.instance.GetItemsViewByItemType(item.type).selectedItem == null)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                SelectItem();
            else if (eventData.button == PointerEventData.InputButton.Right)
                UseItem();
        }
        else
        {
            if (eventData.button == PointerEventData.InputButton.Right)
                DeselectItem();
            else if (eventData.button == PointerEventData.InputButton.Left)
            {
                _PointerEventData = new PointerEventData(_EventSystem);
                _PointerEventData.position = Input.mousePosition;
                List<RaycastResult> results = new List<RaycastResult>();
                _Raycaster.Raycast(_PointerEventData, results);

                InventorySlot slot = null;
                ShortCut shortCut = null;
                ShortCutClone shortCutClone = null;
                CanvasRenderer canvasRenderer = null;
                foreach (RaycastResult result in results)
                {
                    //Check InventorySlot
                    InventorySlot tmpSlot = null;
                    if (result.gameObject.TryGetComponent(out tmpSlot))
                    {
                        slot = tmpSlot;
                    }
                    // Check ShortKey
                    ShortCut tmpShortCut = null;
                    if (result.gameObject.TryGetComponent(out tmpShortCut))
                    {
                        shortCut = tmpShortCut;
                    }
                    // Check ShortCutClone
                    ShortCutClone tmpShortCutClone = null;
                    if (result.gameObject.TryGetComponent(out tmpShortCutClone))
                    {
                        shortCutClone = tmpShortCutClone;
                    }
                    //Check All UI. (if not exist, drop item to field)
                    CanvasRenderer tmpCanvasRenderer = null;
                    if (result.gameObject.TryGetComponent<CanvasRenderer>(out tmpCanvasRenderer))
                    {
                        if (tmpCanvasRenderer.gameObject != this.gameObject)
                            canvasRenderer = tmpCanvasRenderer;
                    }
                    Debug.Log(result.gameObject.name);
                }
                // Clicked on slot
                if (slot != null)
                    slot.SetItemHere(this);

                // Clicked on ShortCutClone
                if (shortCutClone != null)
                    shortCut = shortCutClone.GetOrigin();
                // Clicked on Shortcut
                if (shortCut != null)
                {
                    shortCut.RegisterIconAndEvent(ShortCutType.Item, item.icon, UseItem);
                    DeselectItem();
                }

                if (canvasRenderer == null)
                    DropItem();
                else
                    Debug.Log(canvasRenderer.name);
            }
        }
    }

}
