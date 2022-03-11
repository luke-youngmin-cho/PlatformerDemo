using System;
using UnityEngine;
using UnityEngine.UI;
public class BasicKey_Equipments : BasicKey
{
    public override void OnUse()
    {
        base.OnUse();
        GameObject equipmentsView = UIManager.instance.equipmentsView;
        equipmentsView.SetActive(!equipmentsView.activeSelf);
    }
}
