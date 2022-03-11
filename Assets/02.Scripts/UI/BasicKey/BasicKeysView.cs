using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BasicKeysView : MonoBehaviour
{
    public static BasicKeysView instance;
    public bool isReady = false;
    public GameObject basicKeyControllerPrefab;
    public GameObject basicKeySlotPrefab;
    BasicKeySlot[] slots;
    public int totalSlot = 45;

    public GameObject selectedBasicKey;

    private Transform slotParent;
    private void Awake()
    {
        instance = this;
        slotParent = transform.Find("BasicKeySlots");
    }
    private void Start()
    {
        slots = new BasicKeySlot[totalSlot];
        
        for (int i = 0; i < totalSlot; i++)
        {
            slots[i] = Instantiate(basicKeySlotPrefab, slotParent).GetComponent<BasicKeySlot>();
            slots[i].num = i;
        }
        for (int i = 0; i < BasicKeyAssets.instance.basicKeys.Count; i++)
        {
            CreateBasicKeyObjectOnSlot(BasicKeyAssets.instance.basicKeys[i], i);
        }
        isReady = true;
    }
    private void Update()
    {
        if (selectedBasicKey != null)
        {
            Vector3 pos = Input.mousePosition;
            selectedBasicKey.transform.position = pos;
        }
    }
    /*private BasicKeySlot FindEmptySlot()
    {
        BasicKeySlot slot = null;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].isEmpty)
            {
                slot = slots[i];
                break;
            }
        }
        return slot;
    }*/
    public BasicKeySlot GetSlot(int slotNum)
    {
        return slots[slotNum];
    }
    public void CreateBasicKeyObjectOnSlot(BasicKey basicKey, int slotNum)
    {
        GameObject go = Instantiate(basicKeyControllerPrefab);
        BasicKeyHandler controller = go.GetComponent<BasicKeyHandler>();
        controller.basicKey = basicKey;
        slots[slotNum].SetHere(controller);
    }
    public void CreateBasicKeyObjectOnSlot(string spriteName, BasicKeySlot slot)
    {
        GameObject go = Instantiate(basicKeyControllerPrefab);
        BasicKeyHandler controller = go.GetComponent<BasicKeyHandler>();
        controller.basicKey = BasicKeyAssets.instance.GetBasicKeyBySpriteName(spriteName);
        slot.SetHere(controller);
    }
    public BasicKeyHandler GetBasicKeyHandlerByName(string basicKeyName)
    {
        foreach (var slot in slots)
        {
            BasicKeyHandler handler = slot.GetComponentInChildren<BasicKeyHandler>();
            if (handler != null && handler.basicKey.icon.name == basicKeyName) 
                return handler;
        }
        return null;

    }
}
