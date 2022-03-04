using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortCutManager : MonoBehaviour
{
    public static ShortCutManager instance;
    public bool isReady = false;
    KeyCode keyInput;
    Dictionary<KeyCode, ShortCut> shortCuts = new Dictionary<KeyCode, ShortCut>();

    private void Awake()
    {
        instance = this;
        ShortCut[] tmpShortCuts = transform.Find("ShortCuts").GetComponentsInChildren<ShortCut>();
        for (int i = 0; i < tmpShortCuts.Length; i++)
        {
            shortCuts.Add(tmpShortCuts[i].keyCode, tmpShortCuts[i]);
            Debug.Log($"Shortcut registered : {tmpShortCuts[i].keyCode}");
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
    public bool TryGetShortCut(KeyCode keyCode,out ShortCut shortCut)
    {
        return shortCuts.TryGetValue(keyCode, out shortCut);

    }
    private void OnGUI()
    {
        Event e = Event.current;
        if (e.isKey && e.keyCode != KeyCode.None)
            keyInput = e.keyCode;
    }
}
