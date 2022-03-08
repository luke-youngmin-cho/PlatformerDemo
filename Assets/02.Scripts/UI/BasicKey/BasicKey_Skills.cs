using System;
using UnityEngine;
using UnityEngine.UI;
public class BasicKey_Skills : BasicKey
{
    public override void OnUse()
    {
        base.OnUse();
        GameObject skillsView = UIManager.instance.skillsView;
        skillsView.SetActive(!skillsView.activeSelf);
    }
}
