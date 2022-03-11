using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ShortCutHandler : MonoBehaviour, IPointerClickHandler
{
    Transform tr;
    ShortCutType _type;
    Image _image;
    KeyCode _keyCode;
    ShortCut.KeyEvent KE;
    // UI Raycast event
    GraphicRaycaster _Raycaster;
    PointerEventData _PointerEventData;
    EventSystem _EventSystem;
    
    private void Awake()
    {
        tr = GetComponent<Transform>();
        _image = GetComponent<Image>();
    }
    private void Start()
    {
        _Raycaster = UIManager.instance.playerUI.GetComponent<GraphicRaycaster>();
        _EventSystem = FindObjectOfType<EventSystem>();
    }
    private void OnEnable()
    {
        // no settings, no activation
        if(_image.sprite == null || KE == null) 
            gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        ResetInfo();
    }
    private void Update()
    {
        if (gameObject.activeSelf)
        {
            Vector3 pos = Input.mousePosition;
            tr.position = pos;
        }
    }
    public void ActivateWithInfo(ShortCutType type, Sprite icon, KeyCode keyCode, ShortCut.KeyEvent keyEvent)
    {
        _type = type;
        _image.sprite = icon;
        _image.color = Color.white;
        _keyCode = keyCode;
        KE = keyEvent;
        gameObject.SetActive(true);
    }
    private void ResetInfo()
    {
        _image.sprite = null;
        _image.color = Color.clear;
        _keyCode = KeyCode.None;
        KE = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ResetInfo();
            gameObject.SetActive(false);
        }
        else if (eventData.button == PointerEventData.InputButton.Left)
        {
            _PointerEventData = new PointerEventData(_EventSystem);
            _PointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            _Raycaster.Raycast(_PointerEventData, results);

            ShortCut newShortCut = null;
            ShortCutClone newShortCutClone = null;
            BasicKeySlot newBasicKeySlot = null;
            GameObject shortCutUI = null;
            foreach (var result in results)
            {
                ShortCut tmpShortCut = null;
                if (result.gameObject.TryGetComponent(out tmpShortCut))
                {
                    newShortCut = tmpShortCut;
                }
                ShortCutClone tmpShortCutClone = null;
                if (result.gameObject.TryGetComponent(out tmpShortCutClone))
                {
                    newShortCutClone = tmpShortCutClone;
                }
                BasicKeySlot tmpBasicKeySlot = null;
                if(result.gameObject.TryGetComponent(out tmpBasicKeySlot))
                {
                    newBasicKeySlot = tmpBasicKeySlot;
                }
                //Check All UI. (if no cut UI exist, deactivate. )
                if (result.gameObject.layer == LayerMask.NameToLayer("UI_ShortCut"))
                {
                    if(result.gameObject != gameObject)
                        shortCutUI = result.gameObject;
                }
            }
            // when Short Cut Clone clicked
            if (newShortCutClone != null)
                newShortCut = newShortCutClone.GetOrigin();
            // when Short Cut clicked
            if (newShortCut != null)
            {
                ShortCut oldShortCut = null;
                if (ShortCutsView.instance.TryGetShortCut(_keyCode, out oldShortCut))
                {
                    oldShortCut.RegisterIconAndEvent(newShortCut._type, newShortCut._image.sprite, newShortCut.KE);
                }
                newShortCut.RegisterIconAndEvent(_type, _image.sprite, KE);

                gameObject.SetActive(false);
            }
            
            // when basic key slot clicked
            if(newBasicKeySlot != null && _type == ShortCutType.BasicKey)
            {
                if (ShortCutsView.instance.TryGetShortCut(_keyCode, out ShortCut oldShortCut))
                {
                    if (newBasicKeySlot.isEmpty == false)
                    {
                        oldShortCut.RegisterIconAndEvent(ShortCutType.BasicKey,
                                                         newBasicKeySlot.controller.basicKey.icon,
                                                         newBasicKeySlot.controller.basicKey.OnUse);
                        Destroy(newBasicKeySlot.controller.gameObject);
                        newBasicKeySlot.Clear();
                    }
                    else
                    {
                        oldShortCut.Clear();
                    }
                }
                    
                BasicKeysView.instance.CreateBasicKeyObjectOnSlot(_image.sprite.name, newBasicKeySlot);

                gameObject.SetActive(false);
            }

            // clicked wrong place
            if (shortCutUI == null)
            {
                if (ShortCutsView.instance.TryGetShortCut(_keyCode, out ShortCut oldShortCut) &&
                   (oldShortCut._type != ShortCutType.BasicKey))
                    oldShortCut.Clear();
                gameObject.SetActive(false);
            }   
            else
                Debug.Log(shortCutUI);

        }
    }
}

