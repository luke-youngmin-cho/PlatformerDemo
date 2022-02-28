using UnityEngine;
public class PlayerStateMachine_Hurt : PlayerStateMachine
{
    public override bool IsExecuteOK()
    {
        bool isOK = false;
        if (manager.onActiveSkill == false)
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
                if(elapsedTime > animationTime * animationManager.speed)
                {
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