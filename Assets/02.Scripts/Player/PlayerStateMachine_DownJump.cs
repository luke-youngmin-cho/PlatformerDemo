using System.Collections;
using UnityEngine;
public class PlayerStateMachine_DownJump : PlayerStateMachine
{
    Rigidbody2D rb;
    CapsuleCollider2D col;
    GroundDetector groundDetector;
    Player player;
    public float downJumpForce = 0.3f;
    public float downJumpIgnoreTime = 0.3f;
    public override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        groundDetector = GetComponent<GroundDetector>();
        col = GetComponent<CapsuleCollider2D>();
        player = GetComponent<Player>();
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
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                rb.AddForce(new Vector2(0f, downJumpForce), ForceMode2D.Impulse);
                state = State.OnAction;
                break;
            case State.OnAction:
                if (groundDetector.isIgnoringGround &&
                    rb.position.y < groundDetector.passingGroundColCenterY - col.size.y/2)
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