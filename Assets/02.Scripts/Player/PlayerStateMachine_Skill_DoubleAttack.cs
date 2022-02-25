using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine_Skill_DoubleAttack : PlayerStateMachine_Skill
{
    TargetCaster targetCaster;
    PlayerController controller;
    Player player;
    CapsuleCollider2D col;
    Animator animator;

    public Vector2 attackBoxCastCenter;
    public Vector2 attackBoxCastSize;
    public float attackBoxCastLength;
    private void Awake()
    {
        targetCaster = GetComponent<TargetCaster>();
        controller = GetComponent<PlayerController>();
        player = GetComponent<Player>();
        col = GetComponent<CapsuleCollider2D>();
        animator = GetComponentInChildren<Animator>();
        skillTime = controller.GetAnimationTime("Skill_DoubleAttack");
    }

    override public bool IsSkillPossible()
    {
        bool isOK = false;
        if (controller.isAttacking == false &&
            controller.oldPlayerState != PlayerController.PlayerState.EdgeGrab)
            isOK = true;
        return isOK;
    }
    override public void UpdateSkillState()
    {
        switch (skillState)
        {
            case SkillState.Idle:
                break;
            case SkillState.PrepareToSkill:
                controller.ChangeAnimationState("Skill_DoubleAttack");
                targetCaster.BoxCast("Skill_DoubleAttack",new Vector2((col.offset.x + attackBoxCastCenter.x) * controller.direction, col.offset.y + attackBoxCastCenter.y),
                                        attackBoxCastSize, new Vector2(controller.direction, 0), attackBoxCastLength);
                skillState = SkillState.Casting;
                break;
            case SkillState.Casting:
                if (elapsedSkillTime > (skillTime * animator.speed) / 3)
                {
                    foreach (GameObject target in targetCaster.targetsDictionary["Skill_DoubleAttack"])
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
                            enemy.Hurt(damages, isCriticalHits, (skillTime * animator.speed) / 3);
                            enemyController.direction = -controller.direction;
                            enemyController.KnockBack(controller.direction);
                        }
                    }
                    skillState = SkillState.Using;
                }
                elapsedSkillTime += Time.deltaTime;
                break;
            case SkillState.Using:
                if (elapsedSkillTime > (skillTime * animator.speed))
                {
                    skillState = SkillState.Finished;
                }
                elapsedSkillTime += Time.deltaTime;
                break;
            case SkillState.Finished:
                break;
            default:
                break;
        }
    }
    override public void ResetState()
    {
        isBusy = false;
        elapsedSkillTime = 0;
        skillState = SkillState.Idle;
    }
}