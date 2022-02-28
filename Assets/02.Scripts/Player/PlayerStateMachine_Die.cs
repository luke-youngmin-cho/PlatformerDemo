using UnityEngine;
public class PlayerStateMachine_Die : PlayerStateMachine
{
    Player player;
    public override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
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