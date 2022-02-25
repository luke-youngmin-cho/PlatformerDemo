using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// this class is for 2D player basic movement & animation
/// </summary>
public class PlayerController : MonoBehaviour
{
    public bool controlEnabled = true;

    [Header("Parameters")]
    public float dashMultiplier = 2f;
    public float downJumpForce = 50;
    public float downJumpIgnoreTime = 0.3f;
    public Vector2 knockBackForce;
    private Vector2 playerSize;
    public Vector2 colliderOffsetForCrouching;
    public Vector2 colliderSizeForCrouching;
    private Vector2 colliderOffsetOriginal;
    private Vector2 colliderSizeOriginal;
    public Vector2 attackBoxCastCenter =  new Vector2(0.15f,0f);
    public Vector2 attackBoxCastSize = new Vector2(0.5f,0.5f);
    public float attackBoxCastLength = 0;
    public Vector2 dashAttackBoxCastCenter = new Vector2(0.15f, 0f);
    public Vector2 dashAttackBoxCastSize = new Vector2(0.5f, 0.5f);
    public float dashAttackBoxCastLength = 3;

    // audio
    // todo -> add some sounds

    [Header("States")]
    public PlayerState newPlayerState = PlayerState.Idle;
    public PlayerState oldPlayerState = PlayerState.Idle;
    private JumpState jumpState = JumpState.Idle;
    private FallState fallState = FallState.Idle;
    private DashState dashState = DashState.Idle;
    private AttackState attackState = AttackState.Idle;
    private DashAttackState dashAttackState = DashAttackState.Idle;
    private CrouchFromFallState crouchFromFallState = CrouchFromFallState.Idle;
    private CrouchState crouchState = CrouchState.Idle;
    private StandUpState standUpState = StandUpState.Idle;
    private EdgeGrabState edgeGrabState = EdgeGrabState.Idle;
    private EdgeClimbState edgeClimbState = EdgeClimbState.Idle;
    private WallSlideState wallSlideState = WallSlideState.Idle;
    private LadderState ladderState = LadderState.Idle;
    private HurtState hurtState = HurtState.Idle;
    private DieState dieState = DieState.Idle;
    private DownJumpState downJumpState = DownJumpState.Idle;
    public bool isAttacking
    {
        get
        {
            bool isIt = (oldPlayerState == PlayerState.Attack) ||
                        (oldPlayerState == PlayerState.DashAttack);
            return isIt;
        }
    }
    
    private Coroutine controlDisableCoroutine = null;
    public float disableTimeWhenHurt = 0.5f;

    //[Header("Detections")]
    private GroundDetector groundDetector;
    private PlayerEdgeDetector edgeDetector;
    private PlayerLadderDetector ladderDetector;
    private WallSlideDetector wallSlideDetector;
    private TargetCaster targetCaster;

    [Header("Animations")]
    public float fallingToCrouchDistance = 1.5f;
    private float fallPositionMark;
    string currentAnimationName;
    private float attackTime;
    private float dashAttackTime;
    private float dashTime;
    private float crouchFromFallTime;
    private float crouchTime;
    private float standUpTime;
    private float edgeGrabTime;
    private float edgeClimbTime;
    private float hurtTime;
    private float dieTime;
    private float animationTimer;

    // components
    private Player player;
    internal Animator animator;
    Rigidbody2D rb;
    CapsuleCollider2D col;
    //state machines
    List<PlayerStateMachine_Skill> stateMachines = new List<PlayerStateMachine_Skill>();
    PlayerStateMachine_Skill_DoubleAttack stateMachine_Skill_DoubleAttack;

    // kinematics
    Vector2 move;
    Vector2 targetVelocity;
    float gravityScaleInit;
    
    // direction
    int _direction; // +1 : right -1 : left
    public int direction
    {
        set
        {
            if (value < 0)
            {
                _direction = -1;
                transform.eulerAngles = new Vector3(0f, 180f, 0f);
            }
            else if(value > 0)
            {
                _direction = 1;
                transform.eulerAngles = Vector3.zero;
            }
        }
        get { return _direction; }
    }
    public int directionInit;
    
    void Awake()
    {
        player = GetComponent<Player>();
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        colliderSizeOriginal = col.size;
        colliderOffsetOriginal = col.offset;
        playerSize = col.size;
        gravityScaleInit = rb.gravityScale;
        // animations time
        attackTime = GetAnimationTime("Attack");
        dashAttackTime = GetAnimationTime("DashAttack");
        dashTime = GetAnimationTime("Dash");
        crouchFromFallTime = GetAnimationTime("CrouchFromFall");
        crouchTime = GetAnimationTime("CrouchTime");
        standUpTime = GetAnimationTime("standUpTime");
        edgeGrabTime = GetAnimationTime("EdgeGrab");
        edgeClimbTime = GetAnimationTime("EdgeClimb");
        hurtTime = GetAnimationTime("Hurt");
        dieTime = GetAnimationTime("Die");
        direction = directionInit;
        groundDetector = GetComponent<GroundDetector>();
        edgeDetector = GetComponent<PlayerEdgeDetector>();
        ladderDetector = GetComponent<PlayerLadderDetector>();
        wallSlideDetector = GetComponent<WallSlideDetector>();
        targetCaster = GetComponent<TargetCaster>();
        // state machines
        stateMachine_Skill_DoubleAttack = GetComponent<PlayerStateMachine_Skill_DoubleAttack>();
        stateMachines.Add(stateMachine_Skill_DoubleAttack);
    }
    
    void Update()
    {
        if (controlEnabled)
        {
            float h = Input.GetAxis("Horizontal");

            //direction
            if (IsChangeDirectionPossible())
            {
                if (h < 0) direction = -1;
                else if (h > 0) direction = 1;
            }
           
            // horizontal movement
            if (IsHorizontalMovePossible())
            {
                if (groundDetector.isGrounded &&
                    jumpState == JumpState.Idle)
                {
                    if ((-0.1f > h) | (0.1f < h))
                    {
                        ChangePlayerState(PlayerState.Run);
                        move.x = h;
                    }
                    else
                    {
                        move.x = 0;
                        ChangePlayerState(PlayerState.Idle);
                    }
                }
                else if (groundDetector.isGrounded == false &&
                         jumpState != JumpState.Idle)
                {
                     if ((-0.1f > h) | (0.1f < h))
                     {
                         move.x = h;
                     }
                }
            }
            // crouch
            if (IsCrouchPossible() && Input.GetKey(KeyCode.DownArrow))
                ChangePlayerState(PlayerState.Crouch);
            // dash
            if (IsDashPossible() && Input.GetKey(KeyCode.LeftShift))
                ChangePlayerState(PlayerState.Dash);
            // jump
            if (IsJumpPossible() && Input.GetKey(KeyCode.LeftAlt))
            {
                ChangePlayerState(PlayerState.Jump);
                move.x = h;
            }
            // attack
            if (IsAttackPossible() && Input.GetKey(KeyCode.A))
            {
                if (newPlayerState == PlayerState.Dash)
                    ChangePlayerState(PlayerState.DashAttack);
                else
                    ChangePlayerState(PlayerState.Attack);
            }
            // Skill - DoubleAttack
            if (stateMachine_Skill_DoubleAttack.IsSkillPossible() && Input.GetKey(KeyCode.LeftControl))
            {
                ChangePlayerState(PlayerState.Skill_DoubleAttack);
            }
            // fall
            if (IsFallPossible())
                ChangePlayerState(PlayerState.Fall);            
            // Ladder up
            if (IsLadderUpPossible() && Input.GetKey(KeyCode.UpArrow))
                ChangePlayerState(PlayerState.Ladder);
            // Ladder down
            if (IsLadderDownPossible() && Input.GetKey(KeyCode.DownArrow))
            {
                ChangePlayerState(PlayerState.Ladder);
            }
            // Ladder Jump
            if (oldPlayerState == PlayerState.Ladder && Input.GetKeyDown(KeyCode.LeftAlt) && (Mathf.Abs(h) > 0.1f))
            {
                ChangePlayerState(PlayerState.Jump);
                rb.velocity = Vector2.zero;
                move = new Vector2(h, 0);
            }
            // EdgeGrab
            if (IsEdgeGrabPossible() && Input.GetKey(KeyCode.UpArrow))
                ChangePlayerState(PlayerState.EdgeGrab);
            // Edge Climb
            if(oldPlayerState == PlayerState.EdgeGrab && Input.GetKeyDown(KeyCode.UpArrow))
                ChangePlayerState(PlayerState.EdgeClimb);
            // wallslide
            if (IsWallSlidePossible() && Input.GetKeyDown(KeyCode.DownArrow))
                ChangePlayerState(PlayerState.WallSlide);
            // wall Jump
            if ((oldPlayerState == PlayerState.WallSlide || oldPlayerState == PlayerState.EdgeGrab) &&
                Input.GetKeyDown(KeyCode.LeftAlt))
            {   
                ChangePlayerState(PlayerState.Jump);
                rb.velocity = Vector2.zero;
                move = new Vector2(-direction, 0);
                direction = -direction;
            }   
        }
        else
        {
            move.x = 0;
        }
        UpdatePlayerState();
    }
    private void FixedUpdate()
    {
        FixedUpdateMovement();
    }

    #region player states
    void ChangePlayerState(PlayerState newState)
    {
        if (oldPlayerState == newState) return;
        newPlayerState = newState;
        ResetAllMotion();
        switch (newPlayerState)
        {
            case PlayerState.Idle:
                ChangeAnimationState("Idle");
                break;
            case PlayerState.Run:
                ChangeAnimationState("Run");
                break;
            case PlayerState.Dash:
                Dash();
                break;
            case PlayerState.Jump:
                Jump();
                break;
            case PlayerState.Fall:
                Fall();
                break;
            case PlayerState.CrouchFromFall:
                CrouchFromFall();
                break;
            case PlayerState.Crouch:
                Crouch();
                break;
            case PlayerState.StandUp:
                StandUp();
                break;
            case PlayerState.Attack:
                Attack();
                break;
            case PlayerState.DashAttack:
                DashAttack();
                break;
            case PlayerState.EdgeGrab:
                EdgeGrab();
                break;
            case PlayerState.EdgeClimb:
                EdgeClimb();
                break;
            case PlayerState.WallSlide:
                WallSlide();
                break;
            case PlayerState.Ladder:
                Ladder();
                break;
            case PlayerState.Hurt:
                Hurt();
                break;
            case PlayerState.Die:
                Die();
                break;
            case PlayerState.DownJump:
                DownJump();
                break;
            case PlayerState.Skill_DoubleAttack:
                stateMachine_Skill_DoubleAttack.ActiveSkill();
                break;
            default:
                break;
        }
        animationTimer = 0;
        oldPlayerState = newPlayerState;
    }
    void UpdatePlayerState()
    {
        switch (newPlayerState)
        {
            case PlayerState.Idle:
                break;
            case PlayerState.Run:
                break;
            case PlayerState.Dash:
                UpdateDashState();
                break;
            case PlayerState.Jump:
                UpdateJumpState();
                break;
            case PlayerState.Fall:
                UpdateFallState();
                break;
            case PlayerState.CrouchFromFall:
                UpdateCrouchFromFallState();
                break;
            case PlayerState.Crouch:
                UpdateCrouchState();
                break;
            case PlayerState.StandUp:
                UpdateStandUpState();
                break;
            case PlayerState.Attack:
                UpdateAttackState();
                break;
            case PlayerState.DashAttack:
                UpdateDashAttackState();
                break;
            case PlayerState.EdgeGrab:
                UpdateEdgeGrabState();
                break;
            case PlayerState.EdgeClimb:
                UpdateEdgeClimbState();
                break;
            case PlayerState.WallSlide:
                UpdateWallSlideState();
                break;
            case PlayerState.Ladder:
                UpdateLadderState();
                break;
            case PlayerState.Hurt:
                UpdateHurtState();
                break;
            case PlayerState.Die:
                UpdateDieState();
                break;
            case PlayerState.DownJump:
                UpdateDownJumpState();
                break;
            case PlayerState.Skill_DoubleAttack:
                stateMachine_Skill_DoubleAttack.UpdateSkillState();
                if (stateMachine_Skill_DoubleAttack.skillState == PlayerStateMachine_Skill.SkillState.Finished)
                    ChangePlayerState(PlayerState.Idle);
                break;
            default:
                break;
        }
    }
    #endregion

    #region action states
    void UpdateDashState()
    {
        switch (dashState)
        {
            case DashState.PrepareToDash:
                ChangeAnimationState("Dash");
                dashState = DashState.Dashing;
                break;
            case DashState.Dashing:
                if(animationTimer < (dashTime * animator.speed * 0.5f))
                {
                    move.x = direction * dashMultiplier;
                }
                else if(animationTimer < (dashTime * animator.speed * 0.8f))
                {
                    move.x = direction;
                }
                else if(animationTimer < (dashTime * animator.speed))
                {
                    move.x = direction / 4;
                }
                else
                {
                    move.x = 0;
                    dashState = DashState.Stop;
                }
                animationTimer += Time.deltaTime;
                break;
            case DashState.Stop:
                ChangePlayerState(PlayerState.Idle);
                break;
            default:
                break;
        }
    }
    void UpdateJumpState()
    {
        switch (jumpState)
        {
            case JumpState.PrepareToJump:
                ChangeAnimationState("Jump");
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                rb.AddForce(new Vector2(0f, player.stats.jumpForce), ForceMode2D.Impulse);
                jumpState = JumpState.Jumping;
                break;
            case JumpState.Jumping:
                if(groundDetector.isGrounded == false)
                    jumpState = JumpState.InFlight;
                break;
            case JumpState.InFlight:
                if(rb.velocity.y <= 0)
                    ChangePlayerState(PlayerState.Fall);
                break;
        }
    }
    void UpdateFallState()
    {
        switch (fallState)
        {
            case FallState.PrepareToFall:
                ChangeAnimationState("Fall");
                fallPositionMark = rb.position.y;
                fallState = FallState.Falling;
                break;
            case FallState.Falling:
                
                if (groundDetector.isGrounded)
                {
                    if (fallPositionMark - rb.position.y > fallingToCrouchDistance)
                    {
                        ChangePlayerState(PlayerState.CrouchFromFall);
                    }
                    else
                    {
                        ChangePlayerState(PlayerState.Idle);
                    }
                }
                break;
        }
    }
    void UpdateCrouchFromFallState()
    {
        switch (crouchFromFallState)
        {
            case CrouchFromFallState.PrepareToCrouch:
                col.offset = colliderOffsetForCrouching;
                col.size = colliderSizeForCrouching;
                ChangeAnimationState("Crouch");
                crouchFromFallState = CrouchFromFallState.Crouching;
                break;
            case CrouchFromFallState.Crouching:
                if(animationTimer > (crouchFromFallTime * animator.speed))
                {
                    ChangePlayerState(PlayerState.Idle);
                    col.offset = colliderOffsetOriginal;
                    col.size = colliderSizeOriginal;
                }
                animationTimer += Time.deltaTime;
                break;
        }
    }
    void UpdateCrouchState()
    {
        switch (crouchState)
        {
            case CrouchState.PrepareToCrouch:
                col.offset = colliderOffsetForCrouching;
                col.size = colliderSizeForCrouching;
                ChangeAnimationState("Crouch");
                groundDetector.doDownJumpCheck = true;
                crouchState = CrouchState.Crouching;
                break;
            case CrouchState.Crouching:
                if (animationTimer > (crouchTime * animator.speed))
                {
                    crouchState = CrouchState.Crouched;
                }
                animationTimer += Time.deltaTime;
                break;
            case CrouchState.Crouched:
                break;
        }
        if (IsDownJumpPossible() && Input.GetKey(KeyCode.LeftAlt))
        {
            col.offset = colliderOffsetOriginal;
            col.size = colliderSizeOriginal;
            ChangePlayerState(PlayerState.DownJump);
            groundDetector.doDownJumpCheck = false;
        }
        if (Input.GetKey(KeyCode.DownArrow) == false)
        {
            ChangePlayerState(PlayerState.StandUp);
            groundDetector.doDownJumpCheck = false;
        }
    }
    void UpdateStandUpState()
    {
        switch (standUpState)
        {
            case StandUpState.PrepareToStandUp:
                col.offset = colliderOffsetOriginal;
                col.size = colliderSizeOriginal;
                ChangeAnimationState("StandUp");
                standUpState = StandUpState.StandingUp;
                break;
            case StandUpState.StandingUp:
                if (animationTimer > (standUpTime * animator.speed))
                {
                    ChangePlayerState(PlayerState.Idle);
                }
                animationTimer += Time.deltaTime;
                break;
        }
    }
    void UpdateAttackState()
    {
        switch (attackState)
        {   
            case AttackState.PrepareToAttack:
                ChangeAnimationState("Attack");
                targetCaster.BoxCast("Attack",new Vector2((col.offset.x + attackBoxCastCenter.x) * direction, col.offset.y + attackBoxCastCenter.y),
                                        attackBoxCastSize, new Vector2(direction, 0), attackBoxCastLength);
                attackState = AttackState.AttackCasting;
                break;
            case AttackState.AttackCasting:
                if (animationTimer > (attackTime * animator.speed) / 2)
                {
                    foreach (GameObject target in targetCaster.targetsDictionary["Attack"])
                    {
                        bool isCriticalHit;
                        int damage = player.CalcDamage(out isCriticalHit);
                        Enemy enemy = target.GetComponent<Enemy>();
                        EnemyController enemyController = target.GetComponent<EnemyController>();
                        if(enemy.isDead == false)
                        {
                            enemy.Hurt(damage, isCriticalHit);
                            enemyController.direction = -_direction;
                            enemyController.KnockBack(_direction);
                        }   
                    }
                    attackState = AttackState.Attacking;
                }
                animationTimer += Time.deltaTime;
                break;
            case AttackState.Attacking:
                if(animationTimer > (attackTime * animator.speed))
                {
                    attackState = AttackState.Attacked;
                }
                animationTimer += Time.deltaTime;
                break;
            case AttackState.Attacked:
                ChangePlayerState(PlayerState.Idle);
                break;
            default:
                break;
        }
    }
    void UpdateDashAttackState()
    {
        switch (dashAttackState)
        {
            case DashAttackState.PrepareToAttack:
                ChangeAnimationState("DashAttack");
                targetCaster.BoxCastAll("Attack",new Vector2((col.offset.x + dashAttackBoxCastCenter.x) * direction, col.offset.y + dashAttackBoxCastCenter.y),
                                        dashAttackBoxCastSize, new Vector2(direction, 0), dashAttackBoxCastLength);
                dashAttackState = DashAttackState.AttackCasting;
                break;
            case DashAttackState.AttackCasting:
                if (animationTimer < (dashAttackTime * animator.speed / 4))
                {
                    move.x = direction/2;
                }
                else if (animationTimer < (dashAttackTime * animator.speed / 3))
                {
                    move.x = direction * dashMultiplier * 2;
                }
                else
                {
                    foreach (GameObject target in targetCaster.targetsDictionary["Attack"])
                    {
                        bool isCriticalHit;
                        int damage = player.CalcDamage(out isCriticalHit);
                        Enemy enemy = target.GetComponent<Enemy>();
                        EnemyController enemyController = target.GetComponent<EnemyController>();
                        if (enemy.isDead == false)
                        {
                            enemy.Hurt(damage, isCriticalHit);
                            enemyController.direction = -_direction;
                            enemyController.KnockBack(_direction, 0.3f);
                        }
                    }
                    dashAttackState = DashAttackState.Attacking;
                }
                animationTimer += Time.deltaTime;
                break;
            case DashAttackState.Attacking:
                if (animationTimer < (dashAttackTime * animator.speed))
                {
                    move.x = direction / 2;
                }
                else
                {
                    move.x = 0;
                    dashAttackState = DashAttackState.Attacked;
                }
                animationTimer += Time.deltaTime;
                break;
            case DashAttackState.Attacked:
                ChangePlayerState(PlayerState.Idle);
                break;
            default:
                break;
        }
    }
    void UpdateEdgeGrabState()
    {
        switch (edgeGrabState)
        {
            case EdgeGrabState.Idle:
                break;
            case EdgeGrabState.PrepareToGrab:
                ChangeAnimationState("EdgeGrab");
                move = Vector2.zero;
                rb.velocity = move;
                rb.gravityScale = 0;
                rb.position = edgeDetector.targetPlayerPos;
                edgeGrabState = EdgeGrabState.Grabbing;
                break;
            case EdgeGrabState.Grabbing:
                if (animationTimer > (edgeGrabTime*animator.speed))
                {
                    ChangeAnimationState("EdgeGrabIdle");
                    edgeGrabState = EdgeGrabState.Grabbed;
                }
                animationTimer += Time.deltaTime;
                break;
            case EdgeGrabState.Grabbed:
                // stay grab idle
                break;
            default:
                break;
        }
    }
    void UpdateEdgeClimbState()
    {
        switch (edgeClimbState)
        {
            case EdgeClimbState.PrepareToClimb:
                ChangeAnimationState("EdgeClimb");
                move = Vector2.zero;
                rb.velocity = move;
                rb.bodyType = RigidbodyType2D.Kinematic;
                edgeClimbState = EdgeClimbState.Climbing;
                break;
            case EdgeClimbState.Climbing:
                if(animationTimer > edgeClimbTime)
                {
                    ChangePlayerState(PlayerState.Idle);
                }
                else
                {
                    float deltaPos = Time.deltaTime / edgeClimbTime;
                    if (rb.position.y < edgeDetector.targetPlayerPos.y + col.size.y)
                        rb.position += new Vector2(0f, deltaPos);
                    else if (Mathf.Abs(rb.position.x - edgeDetector.targetPlayerPos.x) < col.size.x )
                        rb.position += new Vector2(deltaPos * direction, 0f);
                }
                animationTimer += Time.deltaTime;
                break;
            default:
                break;
        }
    }
    void UpdateWallSlideState()
    {
        switch (wallSlideState)
        {
            case WallSlideState.PrepareToWallSlide:
                ChangeAnimationState("WallSlide");
                wallSlideState = WallSlideState.WallSliding;
                break;
            case WallSlideState.WallSliding:
                if(groundDetector.isGrounded)
                {
                    ChangePlayerState(PlayerState.CrouchFromFall);
                }
                else if(wallSlideDetector.isDetected == false)
                {
                    ChangePlayerState(PlayerState.Fall);
                }
                break;
            default:
                break;
        }
    }
    void UpdateLadderState()
    {
        switch (ladderState)
        {
            case LadderState.Idle:
                break;
            case LadderState.PrepareToLadder:
                ChangeAnimationState("Ladder");
                animator.speed = 0;
                rb.bodyType = RigidbodyType2D.Kinematic;
                move = Vector2.zero;
                rb.velocity = Vector2.zero;

                // ground
                if (groundDetector.isGrounded)
                {
                    // at feet
                    if (ladderDetector.isAtFeet)
                        rb.position = ladderDetector.GetLadderStartPosWhenIsAtFeet();
                    else
                        rb.position = ladderDetector.GetLadderstartPosOnGround();
                }
                // above head
                else
                    rb.position = ladderDetector.GetLadderStartPosWhenIsAboveHead();

                ladderState = LadderState.OnLadder;
                break;
            case LadderState.OnLadder:
                float v = Input.GetAxis("Vertical");
                animator.speed = Mathf.Abs(v);
                rb.position += Vector2.up * v * Time.deltaTime;
                // Reached to the top of the ladder
                if(ladderDetector.isGoDownPossible && (ladderDetector.isGoUpPossible == false))
                {
                    rb.position = ladderDetector.ladderTopPos + new Vector2(0f, playerSize.y / 1.9f);
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    animator.speed = 1f;
                    ChangePlayerState(PlayerState.Idle);
                }
                // Reached to the bottom of the ladder
                else if((ladderDetector.isGoUpPossible == false) && (ladderDetector.isGoDownPossible == false) ||
                        groundDetector.isGrounded)
                {
                    rb.bodyType = RigidbodyType2D.Dynamic;
                    animator.speed = 1f;
                    ChangePlayerState(PlayerState.Idle);
                }
                break;
            default:
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
                if(animationTimer > (hurtTime * animator.speed))
                {
                    hurtState = HurtState.Hurted;
                }
                animationTimer += Time.deltaTime;
                break;
            case HurtState.Hurted:
                ChangePlayerState(PlayerState.Idle);
                break;
            default:
                break;
        }
    }
    void UpdateDieState()
    {
        switch (dieState)
        {
            case DieState.PrepareToDie:
                ChangeAnimationState("Die");
                dieState = DieState.Dying;
                break;
            case DieState.Dying:
                if(animationTimer > (dieTime * animator.speed))
                {
                    dieState = DieState.Dead;
                }
                animationTimer += Time.deltaTime;
                break;
            case DieState.Dead:
                // todo -> Dead event
                break;
            default:
                break;
        }
    }
    void UpdateDownJumpState()
    {
        switch (downJumpState)
        {
            case DownJumpState.PrepareToDownJump:
                ChangeAnimationState("Jump");
                downJumpState = DownJumpState.DownJumping;
                break;
            case DownJumpState.DownJumping:
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                rb.AddForce(new Vector2(0f, downJumpForce));
                StartCoroutine(E_IgnoreLayerCollisionForSeconds(downJumpIgnoreTime));
                downJumpState = DownJumpState.InFlight;
                break;
            case DownJumpState.InFlight:
                if(animationTimer > 0.5f) 
                {
                    ChangePlayerState(PlayerState.Idle);
                }
                animationTimer += Time.deltaTime;
                break;
            default:
                break;
        }
    }
    IEnumerator E_IgnoreLayerCollisionForSeconds(float time)
    {
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Ground"), true);
        yield return new WaitForSeconds(time);
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Ground"), false);
    }
    #endregion

    public void ChangeAnimationState(string newAnimationName)
    {
        if (currentAnimationName == newAnimationName) return;

        animator.Play(newAnimationName);
        currentAnimationName = newAnimationName;
    }
    void ResetAllMotion()
    {
        if(newPlayerState != PlayerState.Jump && 
           newPlayerState != PlayerState.Fall &&
           oldPlayerState != PlayerState.Jump &&
           oldPlayerState != PlayerState.Fall)
        {
            move = Vector2.zero;
        }
        
        dashState = DashState.Idle;
        jumpState = JumpState.Idle;
        attackState = AttackState.Idle;
        dashAttackState = DashAttackState.Idle;
        fallState = FallState.Idle;
        crouchFromFallState = CrouchFromFallState.Idle;
        crouchState = CrouchState.Idle;
        standUpState = StandUpState.Idle;
        edgeGrabState = EdgeGrabState.Idle;
        edgeClimbState = EdgeClimbState.Idle;
        wallSlideState = WallSlideState.Idle;
        ladderState = LadderState.Idle;
        hurtState = HurtState.Idle;
        dieState = DieState.Idle;
        stateMachine_Skill_DoubleAttack.ResetState();

        if(rb.bodyType == RigidbodyType2D.Kinematic)
            rb.bodyType = RigidbodyType2D.Dynamic;
        if(animator.speed < 1f)
            animator.speed = 1f;
        rb.gravityScale = 1;
    }    
    void ComputeVelocity()
    {
        Vector2 velocity = new Vector2(move.x * player.stats.moveSpeed, move.y);
        targetVelocity = velocity;
    }
    void FixedUpdateMovement()
    {
        ComputeVelocity();
        rb.position += targetVelocity * Time.fixedDeltaTime;
    }

    #region moethods to start action
    void Dash()
    {
        dashState = DashState.PrepareToDash;
    }    
    void Jump()
    {
        jumpState = JumpState.PrepareToJump;
    }
    void Fall()
    {
        fallState = FallState.PrepareToFall;
    }
    void CrouchFromFall()
    {
        crouchFromFallState = CrouchFromFallState.PrepareToCrouch;
    }
    void Crouch()
    {
        crouchState = CrouchState.PrepareToCrouch;
    }
    void StandUp()
    {
        standUpState = StandUpState.PrepareToStandUp;
    }
    void EdgeGrab()
    {
        edgeGrabState = EdgeGrabState.PrepareToGrab;
    }
    void EdgeClimb()
    {
        edgeClimbState = EdgeClimbState.PrepareToClimb;
    }
    void WallSlide()
    {
        wallSlideState = WallSlideState.PrepareToWallSlide;
    }
    void Attack()
    {
        attackState = AttackState.PrepareToAttack;
    }
    void DashAttack()
    {
        dashAttackState = DashAttackState.PrepareToAttack;
    }
    void Ladder()
    {
        ladderState = LadderState.PrepareToLadder;
    }
    void Hurt()
    {
        hurtState = HurtState.PrepareToHurt;
    }
    void Die()
    {
        dieState = DieState.PrepareToDie;
    }
    void DownJump()
    {
        downJumpState = DownJumpState.PrepareToDownJump;
    }
    #endregion

    #region methods to check specific action is possible
    bool IsChangeDirectionPossible()
    {
        bool isOK = false;
        if (oldPlayerState == PlayerState.Idle ||
            oldPlayerState == PlayerState.Run ||
            oldPlayerState == PlayerState.Jump ||
            oldPlayerState == PlayerState.Fall ||
            oldPlayerState == PlayerState.Crouch ||
            oldPlayerState == PlayerState.CrouchFromFall ||
            oldPlayerState == PlayerState.StandUp)
        {
            isOK = true;
        }
        return isOK;
    }
    bool IsHorizontalMovePossible()
    {
        bool isOK = false;
        if(oldPlayerState == PlayerState.Idle ||
           oldPlayerState == PlayerState.Run  )
        {
            isOK = true;
        }
        return isOK;
    }
    bool IsDashPossible()
    {
        bool isOK = false;
        if (oldPlayerState == PlayerState.Idle ||
            oldPlayerState == PlayerState.Run  ||
            oldPlayerState == PlayerState.Jump ||
            oldPlayerState == PlayerState.Fall)
            isOK = true;
        return isOK;
    }
    bool IsJumpPossible()
    {
        bool isOK = false;
        if (groundDetector.isGrounded &&
            (oldPlayerState == PlayerState.Idle ||
             oldPlayerState == PlayerState.Run ))
        {
            isOK = true;
        }   
        return isOK;
    }
    bool IsFallPossible()
    {
        bool isOK = false;
        if(groundDetector.isGrounded == false && 
          (oldPlayerState == PlayerState.Jump ||
           oldPlayerState == PlayerState.Idle ||
           oldPlayerState == PlayerState.Run ) &&
           rb.velocity.y < 0 )
        {
            isOK = true;
        }
        return isOK;
    }
    bool IsCrouchPossible()
    {
        bool isOK = false;
        if (groundDetector.isGrounded &&
            (oldPlayerState == PlayerState.Idle ||
             oldPlayerState == PlayerState.Run))
            isOK = true;
        return isOK;
    }
    bool IsAttackPossible()
    {
        bool isOK = false;
        if (isAttacking == false &&
            oldPlayerState != PlayerState.EdgeGrab)
            isOK = true;
        return isOK;
    }
    bool IsEdgeGrabPossible()
    {
        bool isOK = false;
        if (edgeDetector.isDetected &&
            oldPlayerState != PlayerState.Attack &&
            oldPlayerState != PlayerState.DashAttack &&
            oldPlayerState != PlayerState.CrouchFromFall)
            isOK = true;
        return isOK;
    }
    bool IsEdgeClimbPossible()
    {
        bool isOK = false;
        if (oldPlayerState == PlayerState.EdgeGrab)
            isOK = true;
        return isOK;
    }
    bool IsWallSlidePossible()
    {
        bool isOK = false;
        if (oldPlayerState == PlayerState.EdgeGrab &&
           wallSlideDetector.isDetected)
        {
            isOK = true;
        }
        return isOK;
    }
    bool IsLadderUpPossible()
    {
        bool isOK = false;
        if (ladderDetector.isGoUpPossible &&
            oldPlayerState != PlayerState.Attack &&
            oldPlayerState != PlayerState.DashAttack )
            isOK = true;
        return isOK;
    }
    bool IsLadderDownPossible()
    {
        bool isOK = false;
        if (ladderDetector.isAtFeet &&
            oldPlayerState != PlayerState.Attack &&
            oldPlayerState != PlayerState.DashAttack)
            isOK = true;
        return isOK;
    }
    bool IsHurtPossible()
    {
        bool isOK = false;
        if (oldPlayerState != PlayerState.Attack &&
            oldPlayerState != PlayerState.DashAttack &&
            IsActiveSkill() == false)
            isOK = true;
        return isOK;
    }
    bool IsDownJumpPossible()
    {
        bool isOK = false;
        if (oldPlayerState == PlayerState.Crouch &&
           groundDetector.downJumpAvailable)
            isOK = true;
        return isOK;
    }
    bool IsActiveSkill()
    {
        bool isActive = false;
        foreach (PlayerStateMachine_Skill machine in stateMachines)
        {
            if (machine.isBusy)
            {
                isActive = true;
                break;
            }   
        }
        return isActive;
    }
    #endregion
    public float GetAnimationTime(string name)
    {
        float time = 0;
        if (animator == null) 
            animator = GetComponentInChildren<Animator>();

        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++)
        {
            if(ac.animationClips[i].name == name)
            {
                time = ac.animationClips[i].length;
            }
        }
        return time;
    }

    //==========================================================================
    // methods for external elements
    //==========================================================================
    public void TryDie()
    {
        // check die is possible or not if necessary
        ChangePlayerState(PlayerState.Die);
        controlEnabled = false;
        
    }
    public void TryHurt()
    {
        if (IsHurtPossible())
        {
            controlDisableCoroutine = StartCoroutine(E_DisableController(disableTimeWhenHurt));
            ChangePlayerState(PlayerState.Hurt);
        }   
    }
    public void KnockBack()
    {
        if (player.invincible) return;
        move = Vector2.zero;
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(knockBackForce.x * (-direction), knockBackForce.y), ForceMode2D.Impulse);
    }
    public void KnockBack(int knockBackDirection)
    {
        if (player.invincible) return;
        move = Vector2.zero;
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(knockBackForce.x * knockBackDirection, knockBackForce.y), ForceMode2D.Impulse);
    }
    public void KnockBack(int knockBackDirection, float time)
    {
        if (player.invincible) return;
        move = Vector2.zero;
        rb.velocity = Vector2.zero;
        StartCoroutine(E_KnockBack(knockBackDirection,time));
    }
    IEnumerator E_KnockBack(int knockBackDirection, float time)
    {
        float elapsedTime = 0f;

        while (elapsedTime < time)
        {
            rb.AddForce(new Vector2(knockBackForce.x * knockBackDirection, knockBackForce.y), ForceMode2D.Force);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    IEnumerator E_DisableController(float time)
    {
        controlEnabled = false;
        yield return new WaitForSeconds(time);
        controlEnabled = true;
        controlDisableCoroutine = null;
    }

    #region state enum types
    public enum PlayerState
    {
        Idle,
        Run,
        Dash,
        Jump,
        Fall,
        CrouchFromFall,
        Crouch,
        StandUp,
        Attack,
        DashAttack,
        EdgeGrab,
        EdgeClimb,
        WallSlide,
        Ladder,
        Hurt,
        Die,
        DownJump,
        // Skills
        Skill_DoubleAttack,
    }
    public enum DashState
    {
        Idle,
        PrepareToDash,
        Dashing,
        Stop,
    }
    public enum JumpState
    {
        Idle,
        PrepareToJump,
        Jumping,
        InFlight,
    }
    public enum FallState
    {
        Idle,
        PrepareToFall,
        Falling,
    }
    public enum CrouchFromFallState
    {
        Idle,
        PrepareToCrouch,
        Crouching
    }
    public enum CrouchState
    {
        Idle,
        PrepareToCrouch,
        Crouching,
        Crouched
    }
    public enum StandUpState
    {
        Idle,
        PrepareToStandUp,
        StandingUp,
    }
    public enum AttackState
    {
        Idle,
        PrepareToAttack,
        AttackCasting,
        Attacking,
        Attacked
    }
    public enum DashAttackState
    {
        Idle,
        PrepareToAttack,
        AttackCasting,
        Attacking,
        Attacked
    }    
    public enum EdgeGrabState
    {
        Idle,
        PrepareToGrab,
        Grabbing,
        Grabbed
    }
    public enum EdgeClimbState
    {
        Idle,
        PrepareToClimb,
        Climbing,
    }
    public enum WallSlideState
    {
        Idle,
        PrepareToWallSlide,
        WallSliding,
    }
    public enum LadderState
    {
        Idle,
        PrepareToLadder,
        OnLadder
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
    public enum DownJumpState
    {
        Idle,
        PrepareToDownJump,
        DownJumping,
        InFlight,
    }
    #endregion
}