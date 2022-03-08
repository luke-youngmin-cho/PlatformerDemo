using UnityEngine;
public class PlayerStateMachine_WallSlide: PlayerStateMachine
{
    GroundDetector groundDetector;
    WallSlideDetector wallSlideDetector;
    public override void Awake()
    {
        base.Awake();
        playerStateType = PlayerState.WallSlide;
        groundDetector = GetComponent<GroundDetector>();
        wallSlideDetector = GetComponent<WallSlideDetector>();
    }
    public override bool IsExecuteOK()
    {
        bool isOK = false;
        if (wallSlideDetector.isDetected &&
            groundDetector.isDetected == false &&
            (manager.oldPlayerState == PlayerState.EdgeGrab || 
             manager.oldPlayerState == PlayerState.Jump ||
             manager.oldPlayerState == PlayerState.Fall))
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
                if (groundDetector.isDetected)
                {
                    nextPlayerState = PlayerState.CrouchFromFall;
                    state = State.Finished;
                }
                else if (wallSlideDetector.isDetected == false)
                {
                    nextPlayerState = PlayerState.Fall;
                    state = State.Finished;
                }
                else if (Input.GetKey(KeyCode.DownArrow) == false)
                {
                    nextPlayerState = PlayerState.Idle;
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