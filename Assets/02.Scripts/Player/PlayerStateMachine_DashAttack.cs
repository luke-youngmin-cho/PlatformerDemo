using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine_DashAttack : PlayerStateMachine
{
    TargetCaster targetCaster;
    CapsuleCollider2D col;

    public Vector2 attackBoxCastCenter = new Vector2(0.6f, 0.1f);
    public Vector2 attackBoxCastSize = new Vector2(1f , 0.6f);
    public float attackBoxCastLength;
    public float dashSpeedMultiplier_Casting = 0.5f;
    public float dashSpeedMultiplier_Brandish = 2f;
    override public void Awake()
    {
        base.Awake();
        playerStateType = PlayerState.DashAttack;
        machineType = MachineType.ActiveSkill;
        targetCaster = GetComponent<TargetCaster>();
        col = GetComponent<CapsuleCollider2D>();
    }

    override public bool IsExecuteOK()
    {
        bool isOK = false;
        if (manager.onActiveSkill == false &&
            (manager.oldPlayerState == PlayerState.Idle ||
             manager.oldPlayerState == PlayerState.Jump ||
             manager.oldPlayerState == PlayerState.Fall ||
             manager.oldPlayerState == PlayerState.Run  ||
             manager.oldPlayerState == PlayerState.Dash ))
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
                targetCaster.BoxCastAll(animationName, new Vector2((col.offset.x + attackBoxCastCenter.x) * manager.direction, col.offset.y + attackBoxCastCenter.y),
                                        attackBoxCastSize, new Vector2(manager.direction, 0), attackBoxCastLength);
                state = State.Casting;
                break;
            case State.Casting:
                if(elapsedTime < (animationTime * animationManager.speed) * 1 / 4)
                {
                    manager.move.x = manager.direction * dashSpeedMultiplier_Casting;
                }
                else if (elapsedTime < (animationTime * animationManager.speed) * 2 / 4)
                {
                    manager.move.x = manager.direction * dashSpeedMultiplier_Brandish;
                }
                else if (elapsedTime < (animationTime * animationManager.speed) * 3 / 4)
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