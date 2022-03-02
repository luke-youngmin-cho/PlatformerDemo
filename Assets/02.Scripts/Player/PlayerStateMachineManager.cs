using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// this class is for 2D player basic movement & animation
/// </summary>
public class PlayerStateMachineManager : MonoBehaviour
{
    public bool controlEnabled = true;

    [Header("Parameters")]
    public float moveInputOffset = 0.15f;
    public Vector2 knockBackForce;

    // audio
    // todo -> add some sounds

    [Header("States")]
    public PlayerState newPlayerState = PlayerState.Idle;
    public PlayerState oldPlayerState = PlayerState.Idle;
    public bool onActiveSkill { 
        get 
        {
            bool isOn = false;
            foreach (PlayerState playerState in activeSkillList)
            {
                if(newPlayerState == playerState)
                {
                    isOn = true;
                    break;
                }
            }
            return isOn;
        } 
    }
    private Coroutine controlDisableCoroutine = null;
    public float disableTimeWhenHurt = 0.5f;

    // components
    private Player player;
    Rigidbody2D rb;
    GroundDetector groundDetector;
    //state machines
    Dictionary<PlayerState, PlayerStateMachine> machineDictionaryOfPlayerState = new Dictionary<PlayerState, PlayerStateMachine>();
    Dictionary<KeyCode, PlayerStateMachine> machineDictionaryOfKeyCode = new Dictionary<KeyCode, PlayerStateMachine>();
    List<PlayerState> activeSkillList = new List<PlayerState>();
    // kinematics
    public Vector2 move;
    Vector2 targetVelocity;
    
    // direction
    int _direction; // +1 : right -1 : left

    //int debugcount; for debugging
    // input
    KeyCode keyInput;
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
        rb = GetComponent<Rigidbody2D>();
        groundDetector = GetComponent<GroundDetector>();
        direction = directionInit;        
    }
    private void Start()
    {
        // state machines
        PlayerStateMachine[] machines = GetComponents<PlayerStateMachine>();
        for (int i = 0; i < machines.Length; i++)
        {
            machineDictionaryOfPlayerState.Add(machines[i].playerStateType, machines[i]);
        }
        RefreshMachineDictionaryOfKeyCode();
    }
    // Call this method when user's short-key settings have been changed.
    void RefreshMachineDictionaryOfKeyCode()
    {
        machineDictionaryOfKeyCode.Clear();
        PlayerStateMachine[] machines = GetComponents<PlayerStateMachine>();
        for (int i = 0; i < machines.Length; i++)
        {
            if (machines[i].keyCode != KeyCode.None)
            {
                machineDictionaryOfKeyCode.Add(machines[i].keyCode, machines[i]);
                if(machines[i].isActiveSkill)
                    activeSkillList.Add(machines[i].playerStateType);
            }   
        }
    }
    void Update()
    {
        if (controlEnabled)
        {
            if (onActiveSkill == false)
            {
                float h = Input.GetAxis("Horizontal");
                //direction
                if (IsChangeDirectionPossible())
                {
                    if (h < 0) direction = -1;
                    else if (h > 0) direction = 1;
                }
                // horizontal move
                if (IsHorizontalMovePossible())
                {
                    if (Mathf.Abs(h) < moveInputOffset)
                        move.x = 0;
                    else
                        move.x = h;
                }

                // Basic arrow keys 
                //-----------------------------------------------------------------------
                // left arrow
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    if (machineDictionaryOfPlayerState[PlayerState.Run].IsExecuteOK())
                        ChangePlayerState(PlayerState.Run);
                }
                // right arrow
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    if (machineDictionaryOfPlayerState[PlayerState.Run].IsExecuteOK())
                        ChangePlayerState(PlayerState.Run);
                }
                // down arrow
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    //Debug.Log($"{newPlayerState}. {oldPlayerState}");
                    if (machineDictionaryOfPlayerState[PlayerState.Crouch].IsExecuteOK())
                        ChangePlayerState(PlayerState.Crouch);

                    if (machineDictionaryOfPlayerState[PlayerState.LadderAtFeet].IsExecuteOK())
                        ChangePlayerState(PlayerState.LadderAtFeet);

                    if (machineDictionaryOfPlayerState[PlayerState.WallSlide].IsExecuteOK())
                        ChangePlayerState(PlayerState.WallSlide);
                }
                // up arrow
                if (Input.GetKey(KeyCode.UpArrow))
                {
                    Debug.Log(machineDictionaryOfPlayerState[PlayerState.LadderGoUp].IsExecuteOK());
                    if (machineDictionaryOfPlayerState[PlayerState.LadderGoUp].IsExecuteOK())
                        ChangePlayerState(PlayerState.LadderGoUp);

                    if (machineDictionaryOfPlayerState[PlayerState.EdgeGrab].IsExecuteOK())
                        ChangePlayerState(PlayerState.EdgeGrab);
                }
                if (machineDictionaryOfPlayerState[PlayerState.Fall].IsExecuteOK())
                    ChangePlayerState(PlayerState.Fall);
            }
            else
            {
                if (groundDetector.isDetected)
                    move.x = 0;
            }

            // user - defined key input 
            //-----------------------------------------------
            PlayerStateMachine playerStateMachine;
            bool isOK = machineDictionaryOfKeyCode.TryGetValue(keyInput, out playerStateMachine);
            //Debug.Log($"{isOK}, { keyInput}");
            keyInput = KeyCode.None; // reset current key event input. 
            if (isOK &&
                playerStateMachine.isReady &&
                playerStateMachine.IsExecuteOK())
            {
                Debug.Log($"{ playerStateMachine.isReady}, { playerStateMachine.IsExecuteOK()}");
                ChangePlayerState(playerStateMachine.playerStateType);
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
    void ChangePlayerState(PlayerState stateToChange)
    {
        if (newPlayerState == stateToChange) return;
        newPlayerState = stateToChange;
        ResetMove();
        machineDictionaryOfPlayerState[oldPlayerState].ResetMachine();
        machineDictionaryOfPlayerState[newPlayerState].Execute();
    }
    void UpdatePlayerState()
    {
        PlayerStateMachine currentMachine = machineDictionaryOfPlayerState[newPlayerState]; // current machine
        currentMachine.UpdateWorkflow(); // machine workflow

        if (currentMachine.isStarted)
            oldPlayerState = newPlayerState;

        if (currentMachine.isReady)
            ChangePlayerState(currentMachine.nextPlayerState);
    }
    #endregion

    void ResetMove()
    {
        if(newPlayerState != PlayerState.Jump && 
           newPlayerState != PlayerState.Fall &&
           oldPlayerState != PlayerState.Jump &&
           oldPlayerState != PlayerState.Fall)
        {
            move = Vector2.zero;
        }
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
        if (machineDictionaryOfPlayerState[PlayerState.Hurt].IsExecuteOK())
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
    
    // User key input for keycode skills. 
    private void OnGUI()
    {
        Event e = Event.current;
        if(e.isKey && e.keyCode != KeyCode.None)
            keyInput = e.keyCode;
    }
}
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
    LadderGoUp,
    LadderAtFeet,
    Hurt,
    Die,
    DownJump,
    DoubleAttack,
}