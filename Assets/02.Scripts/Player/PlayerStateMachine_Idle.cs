using UnityEngine;
public class PlayerStateMachine_Idle : PlayerStateMachine
{
    public override void Awake()
    {
        base.Awake();
        playerStateType = PlayerState.Idle;
    }
    public override void UpdateWorkflow()
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Prepare:
                animationManager.ChangeAnimationState(animationName);
                state = State.OnAction;
                break;
            case State.Casting:
                break;
            case State.OnAction:
                break;
            case State.Finished:
                break;
            default:
                break;
        }
    }
}