using UnityEngine;
public class PlayerStateMachine_StandUp : PlayerStateMachine
{
    CapsuleCollider2D col;
    Vector2 colliderOffsetOriginal;
    Vector2 colliderSizeOriginal;

    public override void Awake()
    {
        base.Awake();
        col = GetComponent<CapsuleCollider2D>();
        colliderOffsetOriginal = col.offset;
        colliderSizeOriginal = col.size;
    }
    public override bool IsExecuteOK()
    {
        bool isOk = false;
        if(manager.oldPlayerState == PlayerState.Crouch)
            isOk = true;
        return isOk;

    }
    public override void UpdateWorkflow()
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Prepare:
                animationManager.ChangeAnimationState(animationName);
                col.offset = colliderOffsetOriginal;
                col.size = colliderSizeOriginal;
                state = State.OnAction;
                break;
            case State.Casting:
                break;
            case State.OnAction:
                if (elapsedTime > animationTime * animationManager.speed)
                {
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