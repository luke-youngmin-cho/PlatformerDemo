using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Clons for short cut so user can easily access to short cut without opening short cut key menu.
/// </summary>
public class ShortCutClone : MonoBehaviour , IPointerClickHandler
{
    private KeyCode _keyCode;
    private ShortCutType _type;
    private Image _image;
    public delegate void TryKeyEvent();
    TryKeyEvent TKE;

    //============================================================================
    //************************* Public Methods ***********************************
    //============================================================================
    public void SetClone(ShortCutType type, Sprite spriteOrigin, KeyCode keyCodeOrigin, TryKeyEvent tryKeyEvent)
    {
        _type = type;
        _image.sprite = spriteOrigin;
        if (spriteOrigin == null) _image.color = Color.clear;
        else _image.color = Color.white;
        _keyCode = keyCodeOrigin;
        TKE = tryKeyEvent;
    }
    public void ResetClone()
    {
        _type = ShortCutType.None;
        _image.sprite = null;
        _image.color = Color.clear;
        TKE = null;
    }
    public ShortCut GetOrigin()
    {
        ShortCut shortCut = null;
        ShortCutsView.instance.TryGetShortCut(_keyCode, out shortCut);
        return shortCut;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        ShortCutsView.instance.ActiveShortCutHandler(_type, _image.sprite, _keyCode, GetOrigin().KE);
    }


    //============================================================================
    //************************* Private Methods **********************************
    //============================================================================

    private void Awake()
    {
        _image = GetComponent<Image>();
    }
   
}