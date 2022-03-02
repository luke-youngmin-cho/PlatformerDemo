using UnityEngine;
public class PlayerStateMachine_Jump : PlayerStateMachine
{
    Rigidbody2D rb;
    GroundDetector groundDetector;
    Player player;
    public override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        groundDetector = GetComponent<GroundDetector>();
        player = GetComponent<Player>();
    }
    public override bool IsExecuteOK()
    {
        bool isOK = false;
        if (groundDetector.isDetected &&
           Input.GetKey(KeyCode.DownArrow) == false &&
           (manager.oldPlayerState == PlayerState.Idle ||
            manager.oldPlayerState == PlayerState.Run))
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
                rb.velocity = new Vector2(rb.velocity.x, 0.1f);
                rb.AddForce(new Vector2(0f, player.stats.jumpForce), ForceMode2D.Impulse);
                state = State.Casting;
                break;
            case State.Casting:
                if (groundDetector.isDetected == false)
                    state = State.OnAction;
                break;
            case State.OnAction:
                if (rb.velocity.y <= 0)
                {
                    nextPlayerState = PlayerState.Fall;
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