using UnityEngine;
public class PlayerStateMachine_EdgeGrab : PlayerStateMachine
{
    Rigidbody2D rb;
    EdgeDetector edgeDetector;
    WallSlideDetector wallSlideDetector;
    public override void Awake()
    {
        base.Awake();
        playerStateType = PlayerState.EdgeGrab;
        rb = GetComponent<Rigidbody2D>();
        edgeDetector = GetComponent<EdgeDetector>();
        wallSlideDetector = GetComponent<WallSlideDetector>();
    }
    public override bool IsExecuteOK()
    {
        bool isOK = false;
        if (edgeDetector.isDetected == true &&
            (manager.oldPlayerState == PlayerState.Idle ||
             manager.oldPlayerState == PlayerState.Jump ||
             manager.oldPlayerState == PlayerState.Fall ||
             manager.oldPlayerState == PlayerState.Run ||
             manager.oldPlayerState == PlayerState.Dash))
            isOK = true;
        return isOK;
    }
    public override void Execute()
    {
        base.Execute();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }
    public override void UpdateWorkflow()
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Prepare:
                animationManager.ChangeAnimationState(animationName);
                manager.move = Vector2.zero;
                rb.velocity = Vector2.zero;
                rb.position = edgeDetector.targetPlayerPos;
                state = State.Casting;
                break;
            case State.Casting:
                if (elapsedTime > animationTime * animationManager.speed)
                {
                    animationManager.ChangeAnimationState("EdgeGrabIdle");
                    state = State.OnAction;
                }
                elapsedTime += Time.deltaTime;
                break;
            case State.OnAction:
                // climb
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    nextPlayerState = PlayerState.EdgeClimb;
                    state = State.Finished;
                }
                // wall jump
                if (Input.GetKeyDown(KeyCode.LeftAlt) &&
                    (manager.direction == 1 && Input.GetKey(KeyCode.LeftArrow) ||
                    (manager.direction == -1 && Input.GetKey(KeyCode.RightArrow))))
                {
                    nextPlayerState = PlayerState.Jump;
                    state = State.Finished;
                }
                // wall slide
                if(wallSlideDetector.isDetected &&
                   Input.GetKeyDown(KeyCode.DownArrow))
                {
                    nextPlayerState = PlayerState.WallSlide;
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
        rb.bodyType = RigidbodyType2D.Dynamic;
    }
}