using UnityEngine;
public class PlayerStateMachine_CrouchFromFall : PlayerStateMachine
{
    CapsuleCollider2D col;
    public Vector2 colliderOffsetForCrouching = new Vector2(0f, -0.15f);
    public Vector2 colliderSizeForCrouching = new Vector2(0.13f, 0.13f);
    Vector2 colliderOffsetOriginal;
    Vector2 colliderSizeOriginal;

    public override void Awake()
    {
        base.Awake();
        col = GetComponent<CapsuleCollider2D>();
        colliderOffsetOriginal = col.offset;
        colliderSizeOriginal = col.size;
    }
    public override void UpdateWorkflow()
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Prepare:
                animationManager.ChangeAnimationState(animationName);
                col.offset = colliderOffsetForCrouching;
                col.size = colliderSizeForCrouching;
                state = State.OnAction;
                break;
            case State.Casting:
                break;
            case State.OnAction:
                if (elapsedTime > (animationTime * animationManager.speed))
                {
                    col.offset = colliderOffsetOriginal;
                    col.size = colliderSizeOriginal;
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