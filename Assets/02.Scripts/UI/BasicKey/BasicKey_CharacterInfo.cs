using System;
using UnityEngine;
using UnityEngine.UI;
public class BasicKey_CharacterInfo : BasicKey
{
    public override void OnUse()
    {
        base.OnUse();
        GameObject characterInfoView = UIManager.instance.characterInfoView;
        characterInfoView.SetActive(!characterInfoView.activeSelf);
    }
}
