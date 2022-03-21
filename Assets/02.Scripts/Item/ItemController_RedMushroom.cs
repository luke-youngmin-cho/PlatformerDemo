using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController_RedMushroom : ItemController
{
    public override void OnUseEvent()
    {
        base.OnUseEvent();
        Player.instance.hp += 1000;
    }
}
