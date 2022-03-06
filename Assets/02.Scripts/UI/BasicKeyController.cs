using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
// need to consider 
// BasicKey's function almost same with ShortCut , 
// But when it derives ShortCut, InventoryItem or Skill and so on also can put them on this. 
// if there have good way to prevent other short-cut relative items, 
// It would be nice way that BasicKey is derived from ShortCut
public class BasicKeyController : MonoBehaviour, IPointerClickHandler
{
    public int slotNumber;
    public BasicKey basicKey;
    
    // UI Raycast event
    GraphicRaycaster _Raycaster;
    PointerEventData _PointerEventData;
    EventSystem _EventSystem;

    private void Start()
    {
        _Raycaster = InventoryManager.instance.transform.parent.GetComponent<GraphicRaycaster>();
        _EventSystem = FindObjectOfType<EventSystem>();
        gameObject.GetComponent<Image>().sprite = basicKey.icon.sprite;
    }
    public void Select()
    {
        BasicKeyManager.instance.selectedBasicKey = this.gameObject;
        transform.SetParent(BasicKeyManager.instance.transform);
    }
    public void Deselect()
    {
        BasicKeyManager.instance.selectedBasicKey = null;
        BasicKeySlot slot = BasicKeyManager.instance.GetSlot(slotNumber);
        transform.SetParent(slot.transform);
        transform.position = transform.parent.position;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (BasicKeyManager.instance.selectedBasicKey == null)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
                Select();
        }
        else
        {
            if (eventData.button == PointerEventData.InputButton.Right)
                Deselect();
            else if (eventData.button == PointerEventData.InputButton.Left)
            {
                _PointerEventData = new PointerEventData(_EventSystem);
                _PointerEventData.position = Input.mousePosition;
                List<RaycastResult> results = new List<RaycastResult>();
                _Raycaster.Raycast(_PointerEventData, results);

                BasicKeySlot slot = null;
                ShortCut shortCut = null;
                CanvasRenderer canvasRenderer = null;
                foreach (RaycastResult result in results)
                {
                    //Check InventorySlot
                    BasicKeySlot tmpSlot = null;
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
                    slot.SetHere(this);
                // Clicked on Shortcut
                if (shortCut != null)
                {
                    if(shortCut._type == ShortCutType.BasicKey)
                    {
                        BasicKey oldBasicKey = BasicKeyAssets.instance.GetBasicKeyBySpriteName(shortCut._image.sprite.name);
                        BasicKeyManager.instance.CreateBasicKeyObjectOnSlot(oldBasicKey,slotNumber);
                    }
                    shortCut.RegisterIconAndEvent(ShortCutType.BasicKey, basicKey.icon.sprite, basicKey.OnUse);
                    BasicKeyManager.instance.GetSlot(slotNumber).Clear();
                    Destroy(this.gameObject);
                }

                if (canvasRenderer == null)
                    Deselect();
                else
                    Debug.Log(canvasRenderer.name);
            }
        }
    }


}
