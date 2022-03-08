using System;
using UnityEngine;
using UnityEngine.UI;
public class BasicKey_Menu : BasicKey
{
    public override void OnUse()
    {
        base.OnUse();
        GameObject menuView = UIManager.instance.menuView;
        menuView.SetActive(!menuView.activeSelf);
    }
}
