using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController_BlackMushroom : ItemController
{
    public override void OnUseEvent()
    {
        base.OnUseEvent();
        Player.instance.hp = Player.instance.stats.hpMax;
    }
}
