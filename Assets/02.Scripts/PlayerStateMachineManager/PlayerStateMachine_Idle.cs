using UnityEngine;
class PlayerStateMachine_Idle : PlayerStateMachine
{
    public override void Awake()
    {
        animationName = "Idle";
        base.Awake();
    }
    public override PlayerState UpdateWorkflow()
    {
        PlayerState nextState = machineType;
        playerAnimation.ChangeAnimationState("Idle");
        return nextState;
    }
    public override void FixedUpdateWorkflow()
    {
        
    }
    
}