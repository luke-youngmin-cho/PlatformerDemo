using System.Collections;
using UnityEngine;
public class PlayerStateMachine_DownJump : PlayerStateMachine
{
    Rigidbody2D rb;
    CapsuleCollider2D col;
    GroundDetector groundDetector;
    public float downJumpForce = 0.3f;
    public float downJumpIgnoreTime = 0.3f;
    private float downJumpStartPosY;
    private float downJumpTolerance = 0.5f;
    public override void Awake()
    {
        base.Awake();
        playerStateType = PlayerState.DownJump;
        rb = GetComponent<Rigidbody2D>();
        groundDetector = GetComponent<GroundDetector>();
        col = GetComponent<CapsuleCollider2D>();
    }
    public override bool IsExecuteOK()
    {
        bool isOK = false;
        if (manager.oldPlayerState == PlayerState.Crouch &&
            groundDetector.downJumpAvailable)
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
                groundDetector.IgnoreCurrentGroundUntilPassedIt();
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                state = State.Casting;
                break;
            case State.Casting:
                downJumpStartPosY = rb.position.y;
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                rb.AddForce(new Vector2(0f, downJumpForce), ForceMode2D.Impulse);
                state = State.OnAction;
                break;
            case State.OnAction:
                if (groundDetector.isIgnoringGround &&
                    rb.position.y < downJumpStartPosY - downJumpTolerance)
                {
                    nextPlayerState = PlayerState.Fall;
                    state = State.Finished;
                }
                else
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