using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class InventoryItemController : MonoBehaviour, IPointerClickHandler
{
    public int slotNumber;
    public KeyCode shortCutKeyCode = KeyCode.None;
    public GameObject inventoryItemObject;
    public GameObject itemPrefab;
    private string _itemName;
    public string itemName
    {
        set
        {
            _itemName = value;
            transform.GetChild(0).GetComponent<Text>().text = _itemName;
        }
        get { return _itemName; }
    }
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
                // Clear relatives
                InventoryManager.instance.GetSlot(slotNumber).ResetSlot();
                if(shortCutKeyCode != KeyCode.None)
                {
                    ShortCut shortCut = null;
                    if (ShortCutManager.instance.TryGetShortCut(shortCutKeyCode,out shortCut))
                    {
                        shortCut.Clear();
                    }
                        
                }
                Debug.Log("is destoried");
                Destroy(this.gameObject);
            }
        }
        get { return _itemNum; }
    }
    public int itemNumMax;
    public int addPossibleNum { get { return itemNumMax - itemNum; } }
    private Sprite _itemIcon;
    public Sprite itemIcon
    {
        set
        {
            _itemIcon = value;
            inventoryItemObject.transform.GetChild(1).GetComponent<Image>().sprite = _itemIcon;
        }
        get { return _itemIcon; }
    }
    public ItemType type;
    public int price;

    // UI Raycast event
    GraphicRaycaster _Raycaster;
    PointerEventData _PointerEventData;
    EventSystem _EventSystem;
    private void Awake()
    {
        
    }
    private void Start()
    {
        _Raycaster = InventoryManager.instance.transform.parent.GetComponent<GraphicRaycaster>();
        _EventSystem = FindObjectOfType<EventSystem>();
        itemPrefab = ItemAssets.instance.GetItemPrefabByName(itemName);
    }
    virtual public void UseItem()
    {
        if (itemNum < 1) return;
        itemPrefab.GetComponent<ItemController>().OnUseEvent();
        itemNum--;
    }
    virtual public void DropItem()
    {
        GameObject go = Instantiate(itemPrefab, InventoryManager.instance.player.transform.position,Quaternion.identity);
        go.GetComponent<ItemController>().num = _itemNum;

        go.transform.SetParent(null);
        InventoryManager.instance.GetSlot(slotNumber).ResetSlot();
        Destroy(this.gameObject);
    }
    public void SelectItem()
    {
        InventoryManager.instance.selectedItem = this.gameObject;
        transform.SetParent(InventoryManager.instance.transform);
    }
    public void DeselectItem()
    {
        InventoryManager.instance.selectedItem = null;
        InventorySlot slot = InventoryManager.instance.GetSlot(slotNumber);
        transform.SetParent(slot.transform);
        transform.position = transform.parent.position;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (InventoryManager.instance.selectedItem == null)
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
                    InventorySlot tmpSlot = null;
                    if(result.gameObject.TryGetComponent(out tmpSlot)){
                        slot = tmpSlot;
                    }
                    // Check ShortKey
                    ShortCut tmpShortCut = null;
                    if(result.gameObject.TryGetComponent(out tmpShortCut)){
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
                    if (result.gameObject.TryGetComponent<CanvasRenderer>(out tmpCanvasRenderer)){
                        if(tmpCanvasRenderer.gameObject != this.gameObject)
                            canvasRenderer = tmpCanvasRenderer;
                    }
                    Debug.Log(result.gameObject.name);
                }
                // Clicked on slot
                if(slot != null)
                    slot.SetItemHere(this);

                // Clicked on ShortCutClone
                if (shortCutClone != null)
                    shortCut = shortCutClone.GetOrigin();
                // Clicked on Shortcut
                if (shortCut != null)
                {
                    if(shortCutKeyCode != KeyCode.None)
                    {
                        ShortCut oldShortCut = null;
                        if(ShortCutManager.instance.TryGetShortCut(shortCutKeyCode, out oldShortCut))
                        {
                            oldShortCut.Clear();
                        }
                    }
                    shortCut.RegisterIconAndEvent(ShortCutType.Item, itemIcon, UseItem);
                    shortCutKeyCode = shortCut._keyCode;
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
