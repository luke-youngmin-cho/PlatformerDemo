using UnityEngine;
public class PlayerStateMachine_Fall : PlayerStateMachine
{
    Rigidbody2D rb;
    GroundDetector groundDetector;
    float fallPositionMark;
    public float fallingToCrouchDistance = 1.5f;
    public override void Awake()
    {
        base.Awake();
        playerStateType = PlayerState.Fall;
        rb = GetComponent<Rigidbody2D>();
        groundDetector = GetComponent<GroundDetector>();
    }
    public override bool IsExecuteOK()
    {
        bool isOK = false;
        if (groundDetector.isDetected == false &&
           (manager.oldPlayerState == PlayerState.Jump ||
            manager.oldPlayerState == PlayerState.Idle ||
            manager.oldPlayerState == PlayerState.Run  ||
            manager.oldPlayerState == PlayerState.DownJump) &&
            rb.velocity.y < 0)
        {
            isOK = true;
        }
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
                fallPositionMark = rb.position.y;
                state = State.OnAction;
                break;
            case State.Casting:
                break;
            case State.OnAction:
                if (groundDetector.isDetected)
                {
                    if(fallPositionMark - rb.position.y > fallingToCrouchDistance)
                    {
                        nextPlayerState = PlayerState.CrouchFromFall;
                    }
                    else
                    {
                        nextPlayerState = PlayerState.Idle;
                    }
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