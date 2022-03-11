using System;
using UnityEngine;
using UnityEngine.UI;
public class BasicKey_Stats : BasicKey
{
    public override void OnUse()
    {
        base.OnUse();
        GameObject statsView = UIManager.instance.statsView;
        statsView.SetActive(!statsView.activeSelf);
    }
}
