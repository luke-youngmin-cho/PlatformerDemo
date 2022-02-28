using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public bool controlEnable = true;

    [Header("Parameters")]
    public bool attackEnabled;
    [ConditionalHide("attackEnabled", true)]
    public Vector2 attackRangeCenter;
    [ConditionalHide("attackEnabled", true)]
    public Vector2 attackRangeSize;
    public bool moveEnabled;
    [ConditionalHide("moveEnabled", true)]
    public Vector2 knockBackForce;
    public float hurtTimeOffset;    

    [Header("Components")]
    public Enemy enemy;
    GroundDetector groundDetector;
    TargetCaster targetCaster;

    [Header("States")]
    private EnemyState newState;
    private EnemyState oldState;
    private AttackState attackState;
    private HurtState hurtState;
    private DieState dieState;

    [Header("AI")]
    public MoveAIState moveAIState;
    float moveAIStateTime;
    float moveAIStateTimeElapsed;
    public bool autoFollowPlayer;
    [ConditionalHide("autoFollowPlayer", true)]
    public Vector2 autoFollowRangeCenter;
    [ConditionalHide("autoFollowPlayer", true)]
    public Vector2 autoFollowRangeSize;

    [Header("Kinematics")]
    Rigidbody2D rb;
    CapsuleCollider2D col;
    Vector2 move;
    Vector2 targetVelocity;
    int _direction; // +1 : right -1 : left

    public int direction
    {
        set
        {
            _direction = value;
            if(_direction < 0)
                transform.eulerAngles = Vector3.zero;
            else if(_direction > 0)
                transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
        get { return _direction; }
    }
    public int directionInit = -1;

    [Header("Animations")]
    Animator animator;
    string currentAnimationName;
    float animationTimer;
    float attackTime;
    float hurtTime;
    float dieTime;
    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        groundDetector = GetComponent<GroundDetector>();
        targetCaster = GetComponent<TargetCaster>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        animator = GetComponentInChildren<Animator>();
        
        direction = directionInit;
        attackTime = GetAnimationTime("Attack");
        hurtTime = GetAnimationTime("Hurt");
        dieTime = GetAnimationTime("Die");
    }
    private void Update()
    {
        if (controlEnable)
        {
            MoveAI();

            if (moveEnabled)
            {
                if (move.x < 0) direction = -1;
                else if (move.x > 0) direction = 1;

                if (IsHorizontalMovePossible())
                {
                    if (groundDetector.isDetected)
                    {
                        if (Mathf.Abs(move.x) > 0.1f)
                            ChangeEnemyState(EnemyState.Move);
                        else
                            ChangeEnemyState(EnemyState.Idle);
                    }
                }
            }
        }        
        UpdateEnemyState();
    }
    private void FixedUpdate()
    {
        FixedUpdateMovement();
    }
    void MoveAI()
    {
        if (oldState == EnemyState.Hurt && ((int)moveAIState < (int)MoveAIState.FollowTarget))
            moveAIState = MoveAIState.FollowTarget;

        // autofollow
        if (autoFollowPlayer)
        {
            Collider2D targetCol = Physics2D.OverlapBox(
            new Vector2(rb.position.x + autoFollowRangeCenter.x * direction, rb.position.y + autoFollowRangeCenter.y),
                        autoFollowRangeSize, 0, enemy.targetLayer);
            if (targetCol != null)
            {
                moveAIState = MoveAIState.FollowTarget;
            }
        }
        switch (moveAIState)
        {
            case MoveAIState.DecideRandomBehavior:
                moveAIStateTime = Random.Range(1f, 5f);
                moveAIStateTimeElapsed = 0;
                moveAIState = (MoveAIState)Random.Range(1, 4);
                break;
            case MoveAIState.TakeARest:
                if(moveAIStateTimeElapsed > moveAIStateTime)
                {
                    moveAIState = MoveAIState.DecideRandomBehavior;
                }
                moveAIStateTimeElapsed += Time.deltaTime;
                break;
            case MoveAIState.MoveRight:
                if (moveAIStateTimeElapsed > moveAIStateTime)
                    moveAIState = MoveAIState.DecideRandomBehavior;
                else
                    move.x = 1;
                moveAIStateTimeElapsed += Time.deltaTime;
                break;
            case MoveAIState.MoveLeft:
                if (moveAIStateTimeElapsed > moveAIStateTime)
                    moveAIState = MoveAIState.DecideRandomBehavior;
                else
                    move.x = -1;
                moveAIStateTimeElapsed += Time.deltaTime;
                break;
            case MoveAIState.FollowTarget:
                if(enemy.target != null)
                {
                    if (IsHorizontalMovePossible())
                    {
                        if (enemy.target.transform.position.x > rb.position.x + col.size.x)
                            move.x = 1;
                        else if (enemy.target.transform.position.x < rb.position.x - col.size.x)
                            move.x = -1;
                    }
                    //look at player
                    if (enemy.target.transform.position.x > rb.position.x)
                        direction = 1;
                    else
                        direction = -1;

                    // try attack if the target in range
                    if (attackEnabled && oldState != EnemyState.Attack)
                    {
                        targetCaster.BoxCast("AI",new Vector2((col.offset.x + attackRangeCenter.x) * direction, col.offset.y + attackRangeCenter.y),
                                          attackRangeSize, new Vector2(direction, 0), 0f);
                        foreach (GameObject target in targetCaster.targetsDictionary["AI"])
                        {
                            if(target != null)
                            {
                                ChangeEnemyState(EnemyState.Attack);
                                moveAIState = MoveAIState.AttackingTarget;
                            }
                        }
                    }
                }
                else
                {
                    moveAIState = MoveAIState.DecideRandomBehavior;
                }
                break;
            case MoveAIState.AttackingTarget:
                if (oldState == EnemyState.Idle)
                {
                    ChangeEnemyState(EnemyState.Idle);
                    moveAIState = MoveAIState.FollowTarget;
                }
                    
                break;
            default:
                break;
        }
    }
    void UpdateEnemyState()
    {
        switch (newState)
        {
            case EnemyState.Idle:
                break;
            case EnemyState.Move:
                break;
            case EnemyState.Attack:
                UpdateAttackState();
                break;
            case EnemyState.Hurt:
                UpdateHurtState();
                break;
            case EnemyState.Die:
                UpdateDieState();
                break;
        }
    }
    void UpdateAttackState()
    {
        switch (attackState)
        {
            case AttackState.PrepareToAttack:
                ChangeAnimationState("Attack");
                attackState = AttackState.AttackCasting;
                break;
            case AttackState.AttackCasting:
                if (animationTimer > (attackTime * animator.speed) / 2)
                {
                    targetCaster.BoxCast("Attack",new Vector2((col.offset.x + attackRangeCenter.x) * direction, col.offset.y + attackRangeCenter.y),
                                         attackRangeSize, new Vector2(direction, 0), 0f);
                    AttackBehavior(targetCaster.targetsDictionary["Attack"]);
                    attackState = AttackState.Attacking;
                }
                animationTimer += Time.deltaTime;
                break;
            case AttackState.Attacking:
                if (animationTimer > (attackTime * animator.speed))
                {
                    attackState = AttackState.Attacked;
                }
                animationTimer += Time.deltaTime;
                break;
            case AttackState.Attacked:
                ChangeEnemyState(EnemyState.Idle);
                break;
        }
    }
    void UpdateHurtState()
    {
        switch (hurtState)
        {
            case HurtState.PrepareToHurt:
                ChangeAnimationState("Hurt");
                hurtState = HurtState.Hurting;
                break;
            case HurtState.Hurting:
                if (animationTimer > (hurtTime * animator.speed + hurtTimeOffset))
                {
                    ChangeEnemyState(EnemyState.Idle);
                }
                animationTimer += Time.deltaTime;
                break;
        }
    }
    void UpdateDieState()
    {
        switch (dieState)
        {
            case DieState.PrepareToDie:
                ChangeAnimationState("Die");
                move = Vector2.zero;
                dieState = DieState.Dying;
                break;
            case DieState.Dying:
                if(animationTimer > dieTime * animator.speed)
                {
                    dieState = DieState.Dead;
                }
                animationTimer += Time.deltaTime;
                break;
            case DieState.Dead:
                // todo -> dead event
                break;
        }
    }
    void ChangeEnemyState(EnemyState stateToChange)
    {
        if (oldState == stateToChange) return;
        newState = stateToChange;
        ResetAllMotion();
        switch (newState)
        {
            case EnemyState.Idle:
                ChangeAnimationState("Idle");
                break;
            case EnemyState.Move:
                ChangeAnimationState("Move");
                break;
            case EnemyState.Attack:
                if (attackEnabled)
                    attackState = AttackState.PrepareToAttack;
                else
                    ChangeEnemyState(EnemyState.Idle);
                break;
            case EnemyState.Hurt:
                hurtState = HurtState.PrepareToHurt;
                break;
            case EnemyState.Die:
                dieState = DieState.PrepareToDie;
                break;
        }
        oldState = newState;
    }
    void ResetAllMotion()
    {
        move = Vector2.zero;
        animationTimer = 0;
        attackState = AttackState.Idle;        
        hurtState = HurtState.Idle;
        dieState = DieState.Idle;

        if (rb.bodyType == RigidbodyType2D.Kinematic)
            rb.bodyType = RigidbodyType2D.Dynamic;
        if (animator.speed < 1f)
            animator.speed = 1f;
        rb.gravityScale = 1;
    }
    void ChangeAnimationState(string newAnimationName)
    {
        if (currentAnimationName == newAnimationName) return;

        animator.Play(newAnimationName);
        currentAnimationName = newAnimationName;
    }
    bool IsHorizontalMovePossible()
    {
        bool isOK = false;
        if (oldState == EnemyState.Idle ||
            oldState == EnemyState.Move)
            isOK = true;
        return isOK;
    }
    bool IsHurtPossible()
    {
        bool isOK = false;
        if (oldState != EnemyState.Attack)
            isOK = true;
        return isOK;
    }
    virtual public void AttackBehavior(List<GameObject> castedTargets)
    {
        foreach (GameObject target in castedTargets)
        {
            Player player = target.GetComponent<Player>();
            if(player.isDead == false)
            {
                bool isCriticalHit;
                int damage = enemy.CalcDamage(out isCriticalHit);
                PlayerStateMachineManager playerController = target.GetComponent<PlayerStateMachineManager>();
                playerController.KnockBack();
                player.Hurt(damage, isCriticalHit);
            }
        }
    }
    public void TryHurt()
    {
        if (oldState == EnemyState.Hurt)
            animationTimer = 0; // keep hurting animation
        else if(IsHurtPossible())
            ChangeEnemyState(EnemyState.Hurt);
    }
    public void TryDie()
    {
        ChangeEnemyState(EnemyState.Die);
    }
    public void KnockBack(int knockBackDirection)
    {
        if (moveEnabled == false) return;
        move = Vector2.zero;
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(knockBackForce.x * (knockBackDirection), knockBackForce.y), ForceMode2D.Impulse);
    }
    public void KnockBack(int knockBackDirection,float time)
    {
        if (moveEnabled == false) return;
        move = Vector2.zero;
        rb.velocity = Vector2.zero;
        StartCoroutine(E_KnockBack(knockBackDirection,time));
    }
    IEnumerator E_KnockBack(int knockBackDirection,float time)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < time)
        {
            rb.AddForce(new Vector2(knockBackForce.x * knockBackDirection, knockBackForce.y), ForceMode2D.Force);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    void ComputeVelocity()
    {
        Vector2 velocity = new Vector2(move.x * enemy.stats.moveSpeed, move.y);
        targetVelocity = velocity;
    }
    void FixedUpdateMovement()
    {
        if (moveEnabled == false) return;
        ComputeVelocity();
        rb.position += targetVelocity * Time.fixedDeltaTime;
    }
    float GetAnimationTime(string name)
    {
        float time = 0;
        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++)
        {
            if (ac.animationClips[i].name == name)
            {
                time = ac.animationClips[i].length;
            }
        }
        return time;
    }
    public enum EnemyState
    {
        Idle,
        Move,
        Attack,
        Hurt,
        Die,
    }
    public enum AttackState
    {
        Idle,
        PrepareToAttack,
        AttackCasting,
        Attacking,
        Attacked
    }
    public enum HurtState
    {
        Idle,
        PrepareToHurt,
        Hurting,
        Hurted,
    }
    public enum DieState
    {
        Idle,
        PrepareToDie,
        Dying,
        Dead,
    }
    public enum MoveAIState
    {
        DecideRandomBehavior,
        TakeARest,
        MoveRight,
        MoveLeft,
        FollowTarget,
        AttackingTarget,
    }
    private void OnDrawGizmosSelected()
    {
        // attack range
        Gizmos.color = Color.red;
        CapsuleCollider2D capsuleCol = GetComponent<CapsuleCollider2D>();
        Gizmos.DrawWireCube(new Vector3(transform.position.x + (capsuleCol.offset.x + attackRangeCenter.x) * directionInit,
                                        transform.position.y + capsuleCol.offset.y + attackRangeCenter.y,
                                        0f),
                            new Vector3(attackRangeSize.x, attackRangeSize.y, 0f));
        // attack excution range
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(new Vector3(transform.position.x + autoFollowRangeCenter.x * directionInit,
                                        transform.position.y + autoFollowRangeCenter.y,
                                        0),
                            new Vector3(autoFollowRangeSize.x, autoFollowRangeSize.y, 0));
        
    }
}
