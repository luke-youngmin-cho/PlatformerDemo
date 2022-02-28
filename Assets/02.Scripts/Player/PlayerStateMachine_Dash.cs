using UnityEngine;
public class PlayerStateMachine_Dash : PlayerStateMachine
{
    public float dashSpeedMultiplier;
    public override bool IsExecuteOK()
    {
        bool isOK = false;
        if (manager.oldPlayerState == PlayerState.Idle ||
            manager.oldPlayerState == PlayerState.Run  ||
            manager.oldPlayerState == PlayerState.Jump ||
            manager.oldPlayerState == PlayerState.Fall)
            isOK = true;
        return isOK;
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
                if (elapsedTime < (animationTime * animationManager.speed * 0.5f))
                {
                    manager.move.x = manager.direction * dashSpeedMultiplier;
                }
                else if (elapsedTime < (animationTime * animationManager.speed * 0.8f))
                {
                    manager.move.x = manager.direction;
                }
                else if (elapsedTime < (animationTime * animationManager.speed))
                {
                    manager.move.x = manager.direction / 4;
                }
                else
                {
                    manager.move.x = 0;
                    state = State.Finished;
                }
                elapsedTime += Time.deltaTime;
                break;
            case State.Finished:
                state = State.Idle;
                break;
            default:
                break;
        }
    }
}