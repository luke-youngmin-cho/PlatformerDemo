using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class InventoryItemHandlerBase : MonoBehaviour, IPointerClickHandler
{
    [HideInInspector] public int slotNumber = -1;
    public GameObject inventoryItemObject;
    [HideInInspector] public Item item;
    [HideInInspector] public GameObject itemPrefab;
    
    private int _itemNum;
    public int itemNum
    {
        set { 
            _itemNum = value;
            if (_itemNum > 1)
                inventoryItemObject.transform.GetChild(2).GetComponent<Text>().text = _itemNum.ToString();
            else if (_itemNum == 1)
                inventoryItemObject.transform.GetChild(2).GetComponent<Text>().text = "";
            else
            {
                InventoryView.instance.GetItemsViewByItemType(item.type).GetSlot(slotNumber).Clear();
                InventoryDataManager.instance.data.RemoveData(item.type, item.name, slotNumber);
                Destroy(this.gameObject);
            }
            if(DataManager.isApplied)
                InventoryDataManager.instance.data.AddData(item.type, item.name, _itemNum, slotNumber);
        }
        get { return _itemNum; }
    }
    public int addPossibleNum { get { return item.numMax - itemNum; } }

    // UI Raycast event
    [HideInInspector] public GraphicRaycaster _Raycaster;
    [HideInInspector] public PointerEventData _PointerEventData;
    [HideInInspector] public EventSystem _EventSystem;
    private void Start()
    {
        _Raycaster = UIManager.instance.playerUI.GetComponent<GraphicRaycaster>();
        _EventSystem = FindObjectOfType<EventSystem>();
        inventoryItemObject.transform.GetChild(1).GetComponent<Image>().sprite = item.icon;
        itemPrefab = ItemAssets.instance.GetItemPrefabByName(item.itemName);
    }
    virtual public void UseItem()
    {
        if (itemNum < 1) return;
        itemPrefab.GetComponent<ItemController>().OnUseEvent();
        itemNum--;
    }
    virtual public void DropItem()
    {
        GameObject go = Instantiate(itemPrefab, Player.instance.transform.position,Quaternion.identity);
        go.GetComponent<ItemController>().num = _itemNum;

        go.transform.SetParent(null);
        InventoryView.instance.GetItemsViewByItemType(item.type).GetSlot(slotNumber).Clear();
        InventoryDataManager.instance.data.RemoveData(item.type, item.name, slotNumber);
        Destroy(this.gameObject);
    }
    public void SelectItem()
    {
        InventoryView.instance.GetItemsViewByItemType(item.type).selectedItem = this.gameObject;
        transform.SetParent(UIManager.instance.playerUI.transform);
    }
    public void DeselectItem()
    {
        InventoryView.instance.GetItemsViewByItemType(item.type).selectedItem = null;
        InventorySlot slot = InventoryView.instance.GetItemsViewByItemType(item.type).GetSlot(slotNumber);
        transform.SetParent(slot.transform);
        transform.position = transform.parent.position;
    }
    virtual public void OnPointerClick(PointerEventData eventData)
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
            else if(eventData.button == PointerEventData.InputButton.Left)
            {
                _PointerEventData = new PointerEventData(_EventSystem);
                _PointerEventData.position = Input.mousePosition;
                List<RaycastResult> results = new List<RaycastResult>();
                _Raycaster.Raycast(_PointerEventData,results);

                InventorySlot slot = null;
                ShortCut shortCut = null;
                ShortCutClone shortCutClone = null;
                CanvasRenderer canvasRenderer = null;
                foreach (RaycastResult result in results)
                {
                    //Check InventorySlot
                    if(result.gameObject.TryGetComponent(out InventorySlot tmpSlot)){
                        slot = tmpSlot;
                    }
                    // Check ShortKey
                    if(result.gameObject.TryGetComponent(out ShortCut tmpShortCut)){
                        shortCut = tmpShortCut;
                    }
                    // Check ShortCutClone
                    if (result.gameObject.TryGetComponent(out ShortCutClone tmpShortCutClone)){
                        shortCutClone = tmpShortCutClone;
                    }
                    //Check All UI. (if not exist, drop item to field)
                    if (result.gameObject.TryGetComponent<CanvasRenderer>(out CanvasRenderer tmpCanvasRenderer)){
                        if(tmpCanvasRenderer.gameObject != this.gameObject)
                            canvasRenderer = tmpCanvasRenderer;
                    }
                    Debug.Log(result.gameObject.name);
                }
                // Clicked on slot
                if(slot != null)
                {
                    InventoryView.instance.GetItemsViewByItemType(item.type).GetSlot(slotNumber).Clear();
                    slot.SetItemHere(this);
                }

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
