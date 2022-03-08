using UnityEngine;
public class PlayerStateMachine_Die : PlayerStateMachine
{
    public override void Awake()
    {
        base.Awake();
        playerStateType = PlayerState.Die;
    }
    public override void UpdateWorkflow()
    {
        switch (state)
        {
            case State.Prepare:
                animationManager.ChangeAnimationState(animationName);
                state = State.OnAction;
                break;
            case State.OnAction:
                if(player.hp > 0)
                {
                    state = State.Finished;
                }
                break;
            case State.Finished:
                state = State.Idle;
                break;
        }
    }
}