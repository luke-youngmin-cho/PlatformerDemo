﻿using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ShortCut : MonoBehaviour, IPointerClickHandler
{   
    public KeyCode _keyCode;
    [HideInInspector] public ShortCutType _type;
    [HideInInspector] public Image _image;
    public delegate void KeyEvent();
    public KeyEvent KE;
    public ShortCutClone clone;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }
    private void Start()
    {
        if (clone != null)
            clone.SetClone(_type,_image.sprite, _keyCode, null);
    }
    public void RegisterIconAndEvent(ShortCutType type, Sprite icon, KeyEvent keyEvent)
    {
        _type = type;
        _image.sprite = icon;
        _image.color = Color.white;
        KE = keyEvent;

        if(clone != null)
            clone.SetClone(_type, _image.sprite, _keyCode, TryKeyEvent);


        if (DataManager.isApplied)
            ShortCutsView.instance.SaveData();
            
    }
    public void Clear()
    {
        _type = ShortCutType.None;
        _image.sprite = null;
        _image.color = Color.clear;
        KE = null;
        if(clone!=null)
            clone.ResetClone();
    }
    public void TryKeyEvent()
    {
        if ((_keyCode != KeyCode.None) &&
            (KE != null))
        {
            KE();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ShortCutsView.instance.ActiveShortCutHandler(_type, _image.sprite, _keyCode, KE);
    }
}

public enum ShortCutType
{
    None = 0,
    BasicKey,
    Item,
    Skill
}