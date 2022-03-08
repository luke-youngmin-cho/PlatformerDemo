using System;
using UnityEngine;
using UnityEngine.UI;
public class BasicKey_Items : BasicKey
{
    public override void OnUse()
    {
        base.OnUse();
        GameObject itemsView = UIManager.instance.itemsView;
        itemsView.SetActive(!itemsView.activeSelf);
    }
}
