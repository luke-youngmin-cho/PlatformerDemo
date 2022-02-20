using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// this class is for 2D player basic movement & animation
/// </summary>
public class PlayerController : MonoBehaviour
{
    public bool controlEnabled = true;

    // parameters
    public float maxSpeed = 2;
    public float dashMultiplier = 2f;
    public float jumpForce = 150; // mass 1 standard

    // audio
    // todo -> add some sounds

    // states
    public PlayerState newPlayerState = PlayerState.Idle;
    public PlayerState oldPlayerState = PlayerState.Idle;
    public JumpState jumpState = JumpState.Idle;
    public FallState fallState = FallState.Idle;
    public DashState dashState = DashState.Idle;
    public AttackState attackState = AttackState.Idle;
    public DashAttackState dashAttackState = DashAttackState.Idle;

    // detections
    bool isGrounded;
    public Transform groundDetectPoint;
    public float groundMinDistance;
    public GameObject attackSensor;
    public GameObject dashAttackSensor;

    // animation
    internal Animator animator;
    string currentAnimationName;
    private float attackTime;
    private float dashAttackTime;
    private float dashTime;
    private float animationTimer;
    public float animationTimeOffset = 3.5f;

    // kinematics
    Rigidbody2D rb;
    Vector2 move;
    Vector2 targetVelocity;
    
    // direction
    int _direction; // +1 : right -1 : left
    int direction
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
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
        // animations time
        attackTime = GetAnimationTime("Attack");
        dashAttackTime = GetAnimationTime("DashAttack");
        dashTime = GetAnimationTime("Dash");
        direction = directionInit;
    }
    
    void Update()
    {
        if (controlEnabled)
        {
            DetectGround();

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
                if (isGrounded &&
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
                else if (isGrounded == false &&
                         jumpState != JumpState.Idle)
                {
                     if ((-0.1f > h) | (0.1f < h))
                     {
                         move.x = h;
                     }
                }
            }
            // dash
            if (IsDashPossible() && Input.GetKeyDown(KeyCode.LeftShift))
                ChangePlayerState(PlayerState.Dash);
            // jump
            if (IsJumpPossible() && Input.GetKeyDown(KeyCode.LeftAlt))
                ChangePlayerState(PlayerState.Jump);

            // attack
            if (IsAttackPossible() && Input.GetKeyDown(KeyCode.LeftControl))
            {
                if (newPlayerState == PlayerState.Dash)
                    ChangePlayerState(PlayerState.DashAttack);
                else
                    ChangePlayerState(PlayerState.Attack);
            }

            // fall
            if (IsFallPossible())
            {
                ChangePlayerState(PlayerState.Fall);
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
        //Debug.Log(newPlayerState);
    }
    void DetectGround()
    {    
        Vector2 origin = groundDetectPoint.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, groundMinDistance);
        Collider2D hitCol = hit.collider;
        if (hitCol != null)
        {
            GameObject hitGO = hitCol.gameObject;
            if (hitGO.layer == LayerMask.NameToLayer("Ground"))
            {
                isGrounded = true;
                //Debug.Log("Grounded");
                fallState = FallState.Idle;
            }
        }
        else
        {
            isGrounded = false;
            //Debug.Log("not Grounded");
        }
    }
    void UpdateDashState()
    {
        switch (dashState)
        {
            case DashState.PrepareToDash:
                ChangeAnimationState("Dash");
                dashState = DashState.Dashing;
                break;
            case DashState.Dashing:
                if(animationTimer < (dashTime * animationTimeOffset * 0.5f))
                {
                    move.x = direction * dashMultiplier;
                }
                else if(animationTimer < (dashTime * animationTimeOffset * 0.8f))
                {
                    move.x = direction;
                }
                else if(animationTimer < (dashTime * animationTimeOffset))
                {
                    move.x = direction / 4;
                }
                else
                {
                    move.x = 0;
                    dashState = DashState.Stop;
                }
                animationTimer += Time.fixedDeltaTime;
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
                jumpState = JumpState.Jumping;
                break;
            case JumpState.Jumping:
                rb.AddForce(new Vector2(0f,jumpForce));
                jumpState = JumpState.InFlight;
                break;
            case JumpState.InFlight:
                if ((animationTimer > 0.1f) &&
                    isGrounded)
                {
                    ChangePlayerState(PlayerState.Idle);
                }
                animationTimer += Time.fixedDeltaTime;
                break;
        }
    }
    void UpdateFallState()
    {
        switch (fallState)
        {
            case FallState.PrepareToFall:
                ChangeAnimationState("Fall");
                fallState = FallState.Falling;
                break;
            case FallState.Falling:
                if (isGrounded)
                    ChangePlayerState(PlayerState.Idle);
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
                if(animationTimer > (attackTime * animationTimeOffset) / 2)
                {
                    attackSensor.SetActive(true);
                    attackState = AttackState.Attacking;
                }
                animationTimer += Time.fixedDeltaTime;
                break;
            case AttackState.Attacking:
                if(animationTimer > (attackTime * animationTimeOffset))
                {
                    attackState = AttackState.Attacked;
                }
                animationTimer += Time.fixedDeltaTime;
                break;
            case AttackState.Attacked:
                attackSensor.SetActive(false);
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
                dashAttackState = DashAttackState.AttackCasting;
                break;
            case DashAttackState.AttackCasting:
                if (animationTimer < (dashAttackTime * animationTimeOffset / 4))
                {
                    move.x = direction/2;
                }
                else if (animationTimer < (dashAttackTime * animationTimeOffset / 3))
                {
                    move.x = direction * dashMultiplier * 2;
                }
                else
                {
                    dashAttackSensor.SetActive(true);
                    dashAttackState = DashAttackState.Attacking;
                }
                animationTimer += Time.fixedDeltaTime;
                break;
            case DashAttackState.Attacking:
                if (animationTimer < (dashAttackTime * animationTimeOffset))
                {
                    move.x = direction / 2;
                }
                else
                {
                    move.x = 0;
                    dashAttackState = DashAttackState.Attacked;
                }
                animationTimer += Time.fixedDeltaTime;
                break;
            case DashAttackState.Attacked:
                dashAttackSensor.SetActive(false);
                ChangePlayerState(PlayerState.Idle);
                break;
            default:
                break;
        }
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
            case PlayerState.Attack:
                UpdateAttackState();
                break;
            case PlayerState.DashAttack:
                UpdateDashAttackState();
                break;
            default:
                break;
        }
    }
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
            case PlayerState.Attack:
                Attack();
                break;
            case PlayerState.DashAttack:
                DashAttack();
                break;
            default:
                break;
        }
        animationTimer = 0;
        oldPlayerState = newPlayerState;
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
    }
    void FixedUpdateMovement()
    {
        ComputeVelocity();
        rb.position += targetVelocity * Time.fixedDeltaTime;
    }
    void ComputeVelocity()
    {
        Vector2 velocity = new Vector2(move.x * maxSpeed, move.y);
        targetVelocity = velocity;
    }
    void ChangeAnimationState(string newAnimationName)
    {
        if (currentAnimationName == newAnimationName) return;

        animator.Play(newAnimationName);
        currentAnimationName = newAnimationName;
    }
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
    void Attack()
    {
        attackState = AttackState.PrepareToAttack;
    }
    void DashAttack()
    {
        dashAttackState = DashAttackState.PrepareToAttack;
    }
    bool IsChangeDirectionPossible()
    {
        bool isOK = false;
        if ((attackState == AttackState.Idle) &&
           (dashAttackState == DashAttackState.Idle) &&
           (dashState == DashState.Idle))
        {
            isOK = true;
        }
        return isOK;
    }
    bool IsHorizontalMovePossible()
    {
        bool isOK = false;
        if((attackState == AttackState.Idle) &&
           (dashAttackState == DashAttackState.Idle) &&
           (dashState == DashState.Idle) &&
           (jumpState == JumpState.Idle) &&
           (fallState == FallState.Idle))
        {
            isOK = true;
        }
        return isOK;
    }
    bool IsDashPossible()
    {
        bool isOK = false;
        if (attackState == AttackState.Idle &&
            dashAttackState == DashAttackState.Idle)
            isOK = true;
        return isOK;
    }
    bool IsJumpPossible()
    {
        bool isOK = false;
        if (isGrounded &&
            jumpState == JumpState.Idle &&
            dashAttackState == DashAttackState.Idle &&
            attackState == AttackState.Idle)
            isOK = true;
        return isOK;
    }
    bool IsFallPossible()
    {
        bool isOK = false;
        if(isGrounded == false && 
          (newPlayerState == PlayerState.Jump ||
           newPlayerState == PlayerState.Idle ||
           newPlayerState == PlayerState.Run) &&
           rb.velocity.y < 0)
        {
            isOK = true;
        }
        return isOK;
    }
    bool IsAttackPossible()
    {
        bool isOK = true;
        return isOK;
    }
    
    float GetAnimationTime(string name)
    {
        float time = 0;
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
    public enum PlayerState
    {
        Idle,
        Run,
        Dash,
        Jump,
        Fall,
        Attack,
        DashAttack,
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
}