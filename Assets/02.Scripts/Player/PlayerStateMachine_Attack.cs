using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine_Attack : PlayerStateMachine
{
    TargetCaster targetCaster;
    Player player;
    CapsuleCollider2D col;

    public Vector2 attackBoxCastCenter = new Vector2(0.2f, 0.1f);
    public Vector2 attackBoxCastSize = new Vector2(0.6f , 0.6f);
    public float attackBoxCastLength;
    override public void Awake()
    {
        base.Awake();
        targetCaster = GetComponent<TargetCaster>();
        player = GetComponent<Player>();
        col = GetComponent<CapsuleCollider2D>();
    }

    override public bool IsExecuteOK()
    {
        bool isOK = false;
        if (manager.onActiveSkill == false &&
            (manager.oldPlayerState == PlayerState.Idle ||
             manager.oldPlayerState == PlayerState.Jump ||
             manager.oldPlayerState == PlayerState.Fall ||
             manager.oldPlayerState == PlayerState.Run))
        {
            isOK = true;
        }
        return isOK;
    }
    override public void UpdateWorkflow()
    {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Prepare:
                animationManager.ChangeAnimationState(animationName);
                targetCaster.BoxCast(animationName, new Vector2((col.offset.x + attackBoxCastCenter.x) * manager.direction, col.offset.y + attackBoxCastCenter.y),
                                        attackBoxCastSize, new Vector2(manager.direction, 0), attackBoxCastLength);
                state = State.Casting;
                break;
            case State.Casting:
                if (elapsedTime > (animationTime * animationManager.speed) / 2)
                {
                    foreach (GameObject target in targetCaster.targetsDictionary[animationName])
                    {
                        bool isCriticalHit;
                        int damage = player.CalcDamage(out isCriticalHit);
                        Enemy enemy = target.GetComponent<Enemy>();
                        EnemyController enemyController = target.GetComponent<EnemyController>();
                        if (enemy.isDead == false)
                        {
                            enemy.Hurt(damage, isCriticalHit);
                            enemyController.direction = -manager.direction;
                            enemyController.KnockBack(manager.direction);
                        }
                    }
                    state = State.OnAction;
                }
                elapsedTime += Time.deltaTime;
                break;
            case State.OnAction:
                if (elapsedTime > (animationTime * animationManager.speed))
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