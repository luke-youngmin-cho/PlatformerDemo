using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortCutManager : MonoBehaviour
{
    public static ShortCutManager instance;
    public bool isReady = false;
    KeyCode keyInput;
    Dictionary<KeyCode, ShortCut> shortCuts = new Dictionary<KeyCode, ShortCut>();

    [SerializeField] ShortCutHandler shortCutHandler;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        ShortCut[] tmpShortCuts = transform.Find("ShortCutSettingsPanel").Find("ShortCuts").GetComponentsInChildren<ShortCut>();
        for (int i = 0; i < tmpShortCuts.Length; i++)
        {
            shortCuts.Add(tmpShortCuts[i]._keyCode, tmpShortCuts[i]);
            Debug.Log($"Shortcut registered : {tmpShortCuts[i]._keyCode}");
        }
        isReady = true;
    }
    private void Update()
    {
        ExecuteKeyEvent();
    }
    public void ExecuteKeyEvent()
    {
        ShortCut shortCut = null;
        if (shortCuts.TryGetValue(keyInput, out shortCut))
        {
            shortCut.TryKeyEvent();
            keyInput = KeyCode.None;
        }
    }
    public void ActiveShortCutHandler(ShortCutType type, Sprite icon, KeyCode keyCode, ShortCut.KeyEvent keyEvent)
    {
        shortCutHandler.ActivateWithInfo(type, icon, keyCode, keyEvent);
    }
    public bool TryGetShortCut(KeyCode keyCode,out ShortCut shortCut)
    {
        Debug.Log($"Try get short cut : {keyCode} , {shortCuts.ContainsKey(keyCode)}");
        return shortCuts.TryGetValue(keyCode, out shortCut);

    }
    
    private void OnGUI()
    {
        Event e = Event.current;
        if (e.isKey && e.keyCode != KeyCode.None)
            keyInput = e.keyCode;
    }
}
