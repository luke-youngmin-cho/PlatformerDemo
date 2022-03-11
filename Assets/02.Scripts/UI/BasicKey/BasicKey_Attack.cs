using System;
using UnityEngine;
using UnityEngine.UI;
public class BasicKey_Attack : BasicKey
{
    public override void OnUse()
    {
        base.OnUse();
        if(PlayerStateMachineManager.instance.
            TryGetStateMachineByState(PlayerState.Attack, out PlayerStateMachine attackMachine))
        {
            PlayerStateMachineManager.instance.keyInput = attackMachine.keyCode;
        }
    }
}
