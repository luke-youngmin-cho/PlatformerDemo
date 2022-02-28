using UnityEngine;
public class PlayerStateMachine_Crouch: PlayerStateMachine
{
    CapsuleCollider2D col;
    GroundDetector groundDetector;
    public Vector2 colliderOffsetForCrouching = new Vector2(0f, -0.15f);
    public Vector2 colliderSizeForCrouching   = new Vector2(0.13f, 0.13f);
    Vector2 colliderOffsetOriginal;
    Vector2 colliderSizeOriginal;

    public override void Awake()
    {
        base.Awake();
        col = GetComponent<CapsuleCollider2D>();
        groundDetector = GetComponent<GroundDetector>();
        colliderOffsetOriginal = col.offset;
        colliderSizeOriginal = col.size;
    }
    public override bool IsExecuteOK()
    {
        bool isOK = false;
        if (groundDetector.isDetected &&
           (manager.oldPlayerState == PlayerState.Idle ||
            manager.oldPlayerState == PlayerState.Run))
            isOK = true;
        return isOK;
    }
    public override void Execute()
    {
        base.Execute();
        col.offset = colliderOffsetForCrouching;
        col.size = colliderSizeForCrouching;
        groundDetector.doDownJumpCheck = true;
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
                if (Input.GetKey(KeyCode.DownArrow) == false)
                {
                    groundDetector.doDownJumpCheck = false;
                    nextPlayerState = PlayerState.StandUp;
                    state = State.Finished;
                }
                // downjump
                if (groundDetector.downJumpAvailable &&
                    Input.GetKey(KeyCode.LeftAlt))
                {
                    Debug.Log("Do Downjump!");
                    groundDetector.doDownJumpCheck = false;
                    nextPlayerState = PlayerState.DownJump;
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
    public override void ResetMachine()
    {
        base.ResetMachine();
        colliderOffsetForCrouching = colliderOffsetOriginal;
        colliderSizeForCrouching = colliderSizeOriginal;
        groundDetector.doDownJumpCheck = false;
    }
}