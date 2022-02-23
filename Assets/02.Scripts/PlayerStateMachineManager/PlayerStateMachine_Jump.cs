using UnityEngine;
class PlayerStateMachine_Jump : PlayerStateMachine
{

    [HideInInspector] public PlayerGroundDetector groundDetector;
    //state
    JumpState jumpState = JumpState.Idle;

    public override void Awake()
    {
        animationName = "Jump";
        base.Awake();
        groundDetector = GetComponent<PlayerGroundDetector>();
    }
    public override PlayerState UpdateWorkflow()
    {
        PlayerState nextState = machineType;
        switch (jumpState)
        {
            case JumpState.PrepareToJump:
                playerAnimation.ChangeAnimationState(animationName);
                jumpState = JumpState.Jumping;
                break;
            case JumpState.Jumping:
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                rb.AddForce(new Vector2(0f, player.stats.jumpForce));
                jumpState = JumpState.InFlight;
                break;
            case JumpState.InFlight:
                if ((playerAnimation.animationElapsedTime > 0.1f) &&
                    groundDetector.isGrounded)
                {
                    nextState = PlayerState.Idle;
                }
                playerAnimation.animationElapsedTime += Time.deltaTime;
                break;
            default:
                break;
        }
        return nextState;
    }
    public override void PrepareWorkflow()
    {
        jumpState = JumpState.PrepareToJump;
    }
    public enum JumpState
    {
        Idle,
        PrepareToJump,
        Jumping,
        InFlight,
    }
}