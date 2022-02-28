using UnityEngine;
public class PlayerStateMachine_EdgeClimb : PlayerStateMachine
{
    Rigidbody2D rb;
    CapsuleCollider2D col;
    PlayerEdgeDetector edgeDetector;
    public override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        edgeDetector = GetComponent<PlayerEdgeDetector>();
    }
    public override bool IsExecuteOK()
    {
        bool isOK = false;
        if (manager.oldPlayerState == PlayerState.EdgeGrab)
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
                manager.move = Vector2.zero;
                rb.velocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
                state = State.OnAction;
                break;
            case State.Casting:
                break;
            case State.OnAction:
                if (elapsedTime > animationTime * animationManager.speed)
                {
                    state = State.Finished;
                }
                else
                {
                    float deltaPos = Time.deltaTime / animationTime;
                    if (rb.position.y < edgeDetector.targetPlayerPos.y + col.size.y)
                        rb.position += new Vector2(0f, deltaPos);
                    else if (Mathf.Abs(rb.position.x - edgeDetector.targetPlayerPos.x) < col.size.x)
                        rb.position += new Vector2(deltaPos * manager.direction, 0f);
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
    public override void ResetMachine()
    {
        base.ResetMachine();
        rb.bodyType = RigidbodyType2D.Dynamic;
    }
}