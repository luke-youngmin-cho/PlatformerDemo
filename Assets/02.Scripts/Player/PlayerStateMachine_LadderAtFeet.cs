using UnityEngine;
public class PlayerStateMachine_LadderAtFeet : PlayerStateMachine
{
    Rigidbody2D rb;
    CapsuleCollider2D col;
    PlayerLadderDetector ladderDetector;
    GroundDetector groundDetector;
    public override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        ladderDetector = GetComponent<PlayerLadderDetector>();
        groundDetector = GetComponent<GroundDetector>();
    }
    public override bool IsExecuteOK()
    {
        bool isOK = false;
        if (ladderDetector.isAtFeet &&
           (manager.oldPlayerState == PlayerState.Idle ||
            manager.oldPlayerState == PlayerState.Jump ||
            manager.oldPlayerState == PlayerState.Fall ||
            manager.oldPlayerState == PlayerState.Run  ||
            manager.oldPlayerState == PlayerState.Dash))
            isOK = true;
        return isOK;
    }
    public override void Execute()
    {
        base.Execute();
        rb.bodyType = RigidbodyType2D.Kinematic;
        animationManager.speed = 0;
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
                rb.position = ladderDetector.GetLadderStartPosWhenIsAtFeet();

                state = State.OnAction;
                break;
            case State.Casting:
                break;
            case State.OnAction:
                float v = Input.GetAxis("Vertical");
                animationManager.speed = Mathf.Abs(v);
                rb.position += Vector2.up * v * Time.deltaTime;
                // Reached to the top of the ladder
                if (ladderDetector.isGoDownPossible && (ladderDetector.isGoUpPossible == false))
                {
                    rb.position = ladderDetector.ladderTopPos + new Vector2(0f, col.size.y / 1.9f);
                    nextPlayerState = PlayerState.Idle;
                    state = State.Finished;
                }
                // Reached to the bottom of the ladder
                else if ((ladderDetector.isGoUpPossible == false) && (ladderDetector.isGoDownPossible == false) ||
                        groundDetector.isDetected)
                {
                    nextPlayerState= PlayerState.Idle;
                    state = State.Finished;
                }
                // Ladder Jump
                if (Input.GetKey(KeyCode.LeftAlt))
                {
                    if (Input.GetKey(KeyCode.LeftArrow))
                    {
                        manager.direction = -1;
                        nextPlayerState = PlayerState.Jump;
                        state = State.Finished;
                    }
                    if (Input.GetKey(KeyCode.RightArrow))
                    {
                        manager.direction = 1;
                        nextPlayerState = PlayerState.Jump;
                        state = State.Finished;
                    }
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
        animationManager.speed = 1f;
    }
}