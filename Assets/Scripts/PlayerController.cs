using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is the main class used to implement control of the player.
/// It is a superset of the AnimationController class, but is inlined to allow for any kind of customisation.
/// </summary>
public class PlayerController : MonoBehaviour
{
    /*public AudioClip jumpAudio;
    public AudioClip respawnAudio;
    public AudioClip ouchAudio;*/

    /// <summary>
    /// Max horizontal speed of the player.
    /// </summary>
    public float maxSpeed = 7;
    public float dashMultiplier = 1.5f;
    public PlayerState newPlayerState = PlayerState.Idle;
    public PlayerState oldPlayerstate = PlayerState.Idle;
    /// <summary>
    /// Initial jump velocity at the start of a jump.
    /// </summary>
    public float jumpTakeOffSpeed = 7;

    public DashState dashState = DashState.Idle;
    public JumpState jumpState = JumpState.Idle;
    public AttackState attackState = AttackState.Idle;
    public DashAttackState dashAttackState = DashAttackState.Idle;

    public GameObject attackSensor;
    public GameObject dashAttackSensor;
    private float attackTime;
    private float dashAttackTime;
    private float dashTime;
    private float animationTimer;
    public float animationTimeOffset;
    /*internal new*/
    public Collider2D collider2d;
    /*internal new*/
    //public AudioSource audioSource;
    public bool controlEnabled = true;

    Vector2 move;
    internal Animator animator;

    Rigidbody2D rb;
    Vector2 targetVelocity;
    public float gravityModifier;
    public float jumpModifier;
    bool isGrounded;
    int direction; // +1 : right -1 : left
    public float groundMinDistance;
    public Transform groundDetectPoint;
    string currentAnimationName;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //audioSource = GetComponent<AudioSource>();
        collider2d = GetComponent<Collider2D>();
        animator = GetComponentInChildren<Animator>();
        // animations time
        attackTime = GetAnimationTime("Attack");
        dashAttackTime = GetAnimationTime("DashAttack");
        dashTime = GetAnimationTime("Dash");
    }
    
    void Update()
    {
        if (controlEnabled)
        {
            DetectGround();

            // horizontal movement
            if (IsHorizontalMovePossible())
            {
                float h = Input.GetAxis("Horizontal");
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

            if (IsAttackPossible() && Input.GetKeyDown(KeyCode.LeftControl))
            {
                if (newPlayerState == PlayerState.Dash)
                    ChangePlayerState(PlayerState.DashAttack);
                else
                    ChangePlayerState(PlayerState.Attack);
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
        Debug.Log(newPlayerState);
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
                Debug.Log("Grounded");
            }
        }
        else
        {
            isGrounded = false;
            Debug.Log("not Grounded");
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
                if(animationTimer < (dashTime * animationTimeOffset / 1.5f))
                {
                    move.x = direction * dashMultiplier;
                }
                else if(animationTimer < (dashTime * animationTimeOffset / 1.2f))
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
                if (!isGrounded)
                {   
                    jumpState = JumpState.InFlight;
                }
                break;
            case JumpState.InFlight:
                if (rb.velocity.y < 0)
                {
                    ChangeAnimationState("Fall");
                }
                if (isGrounded)
                {
                    jumpState = JumpState.Landed;
                }
                break;
            case JumpState.Landed:
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
        Debug.Log($"{dashAttackState} -{animationTimer} - {move.x} ");
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
        if (oldPlayerstate == newState) return;
        ResetAllMotion();
        newPlayerState = newState;
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
        oldPlayerstate = newPlayerState;
    }
    void ResetAllMotion()
    {
        move = Vector2.zero;
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
        if ((jumpState == JumpState.Jumping) && isGrounded)
        {
            move.y = jumpTakeOffSpeed * jumpModifier;
        }
        else
        {
            move.y = 0;
        }

        // direction
        if (move.x > 0.01f)
        {
            transform.eulerAngles = Vector3.zero;
            direction = 1;
        }   
        else if (move.x < -0.01f)
        {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
            direction = -1;
        }

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
    void Attack()
    {
        attackState = AttackState.PrepareToAttack;
    }
    void DashAttack()
    {
        dashAttackState = DashAttackState.PrepareToAttack;
    }
    bool IsHorizontalMovePossible()
    {
        bool isOK = false;
        if((attackState == AttackState.Idle) &&
           (dashAttackState == DashAttackState.Idle) &&
           (dashState == DashState.Idle))
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
        Landed
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