using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ShortCutClone : MonoBehaviour , IPointerClickHandler
{
    private KeyCode _keyCode;
    private ShortCutType _type;
    private Image _image;
    public delegate void TryKeyEvent();
    TryKeyEvent TKE;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }
    public void SetClone(ShortCutType type, Sprite spriteOrigin, KeyCode keyCodeOrigin, TryKeyEvent tryKeyEvent)
    {
        _type = type;
        _image.sprite = spriteOrigin;
        if(spriteOrigin == null) _image.color = Color.clear;
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
        ShortCutManager.instance.TryGetShortCut(_keyCode, out shortCut);
        return shortCut;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        ShortCutManager.instance.ActiveShortCutHandler(_type,_image.sprite, _keyCode, GetOrigin().KE);
    }
}