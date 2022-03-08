using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine_DoubleAttack : PlayerStateMachine
{
    TargetCaster targetCaster;
    CapsuleCollider2D col;

    public Vector2 attackBoxCastCenter = new Vector2(0.2f, 0.1f);
    public Vector2 attackBoxCastSize = new Vector2(0.6f,0.6f);
    public float attackBoxCastLength;
    override public void Awake()
    {
        base.Awake();
        playerStateType = PlayerState.DoubleAttack;
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
             manager.oldPlayerState == PlayerState.Dash))
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
                Debug.Log("DoubleAttack");
                animationManager.ChangeAnimationState(animationName);
                targetCaster.BoxCast(animationName,new Vector2((col.offset.x + attackBoxCastCenter.x) * manager.direction, col.offset.y + attackBoxCastCenter.y),
                                        attackBoxCastSize, new Vector2(manager.direction, 0), attackBoxCastLength);
                state = State.Casting;
                break;
            case State.Casting:
                if (elapsedTime > (animationTime * animationManager.speed) / 3)
                {
                    foreach (GameObject target in targetCaster.targetsDictionary[animationName])
                    {   
                        List<int> damages = new List<int>();
                        List<bool> isCriticalHits = new List<bool>();
                        bool isCriticalHit;
                        
                        // double damages.
                        damages.Add(player.CalcDamage(out isCriticalHit));
                        isCriticalHits.Add(isCriticalHit);
                        damages.Add(player.CalcDamage(out isCriticalHit));
                        isCriticalHits.Add(isCriticalHit);

                        Enemy enemy = target.GetComponent<Enemy>();
                        EnemyController enemyController = target.GetComponent<EnemyController>();
                        if (enemy.isDead == false)
                        {
                            enemy.Hurt(damages, isCriticalHits, (animationTime * animationManager.speed) / 3);
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