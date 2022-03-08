using UnityEngine;
public class PlayerStateMachine_Run : PlayerStateMachine
{
    GroundDetector groundDetector;
    public override void Awake()
    {
        base.Awake();
        playerStateType = PlayerState.Run;
        groundDetector = GetComponent<GroundDetector>();
    }
    public override bool IsExecuteOK()
    {
        bool isOK = false;
        if (manager.oldPlayerState == PlayerState.Idle)
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
                // fall
                if(groundDetector.isDetected == false)
                {
                    nextPlayerState = PlayerState.Fall;
                    state = State.Finished;
                }
                // Jump
                if (Input.GetKeyDown(KeyCode.LeftAlt))
                {
                    nextPlayerState = PlayerState.Jump;
                    state = State.Finished;
                }
                // when user stop move
                if (Input.GetKey(KeyCode.LeftArrow) == false &&
                    Input.GetKey(KeyCode.RightArrow) == false)
                {
                    nextPlayerState = PlayerState.Idle;
                    state = State.Finished;
                }
                break;
            case State.Finished:
                state = State.Idle;
                break;
            default:
                break;
        }
    }
}