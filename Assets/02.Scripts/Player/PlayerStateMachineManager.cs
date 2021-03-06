using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// this class is for 2D player basic movement & animation
/// </summary>
public class PlayerStateMachineManager : MonoBehaviour
{
    public static PlayerStateMachineManager instance;
    public bool isReady = false;
    public bool controlEnabled = true;

    [Header("Parameters")]
    public float moveInputOffset = 0.15f;
    public Vector2 knockBackForce;

    [Header("States")]
    public PlayerState newPlayerState = PlayerState.Idle;
    public PlayerState oldPlayerState = PlayerState.Idle;
    public bool onActiveSkill 
    { 
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
    public int direction
    {
        set
        {
            if (value < 0)
            {
                _direction = -1;
                transform.eulerAngles = new Vector3(0f, 180f, 0f);
            }
            else if (value > 0)
            {
                _direction = 1;
                transform.eulerAngles = Vector3.zero;
            }
        }
        get { return _direction; }
    }
    public int directionInit;

    // input
    private KeyCode _keyInput;
    public KeyCode keyInput
    {
        set 
        { 
            if (_keyInput == KeyCode.None) _keyInput = value; 
        }
    }

    //============================================================================
    //************************* Public Methods ***********************************
    //============================================================================

    public void RefreshMachineDictionaries()
    {
        RefreshMachineDictionaryOfPlayerState();
        RefreshMachineDictionaryOfKeyCode();
    }

    public void AddStateMachineComponentByState(PlayerState state)
    {
        if (TryGetStateMachineByState(state, out PlayerStateMachine existMachine))
            return;

        string stateName = state.ToString();
        string typeName = "PlayerStateMachine_" + stateName;
        System.Type type = System.Type.GetType(typeName);
        PlayerStateMachine stateMachine = null;
        if (type != null)
        {
            stateMachine = (PlayerStateMachine)gameObject.AddComponent(type);
            stateMachine.playerStateType = state;
            stateMachine.machineType = SkillAssets.instance.GetMachineTypeByState(state);
            st_SkillStats skillInfo = player.skillStatsList.Find(x => x.state == state);
            stateMachine.level = skillInfo.level;
            stateMachine.hpRequired = skillInfo.hpRequired;
            stateMachine.mpRequired = skillInfo.mpRequired;
        }
        else
            Debug.Log($"Failed to add player state machine : {typeName}");
        RefreshMachineDictionaryOfPlayerState();
    }

    public bool TryGetStateMachineByState(PlayerState state, out PlayerStateMachine stateMachine) =>
        machineDictionaryOfPlayerState.TryGetValue(state, out stateMachine);

    public bool TryGetStateMachineByKeyCode(KeyCode keyCode, out PlayerStateMachine stateMachine) =>
        machineDictionaryOfKeyCode.TryGetValue(keyCode, out stateMachine);

    public void TryDie()
    {
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
        StartCoroutine(E_KnockBack(knockBackDirection, time));
    }


    //============================================================================
    //************************* Private Methods **********************************
    //============================================================================

    void Awake()
    {
        instance = this; 
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        groundDetector = GetComponent<GroundDetector>();
        direction = directionInit;        
    }

    void Start()
    {
        StartCoroutine(E_Start());
    }

    IEnumerator E_Start()
    {
        yield return new WaitUntil(() => Player.instance.isReady);
        RefreshMachineDictionaries();
        isReady = true;
        Debug.Log("PlayerStateMachineManager is ready");
    }
    
    void RefreshMachineDictionaryOfPlayerState()
    {
        machineDictionaryOfPlayerState.Clear();
        PlayerStateMachine[] machines = GetComponents<PlayerStateMachine>();
        for (int i = 0; i < machines.Length; i++)
        {
            machineDictionaryOfPlayerState.Add(machines[i].playerStateType, machines[i]);
        }
    }

    void RefreshMachineDictionaryOfKeyCode()
    {
        machineDictionaryOfKeyCode.Clear();
        PlayerStateMachine[] machines = GetComponents<PlayerStateMachine>();
        for (int i = 0; i < machines.Length; i++)
        {
            if (machines[i].keyCode != KeyCode.None)
            {
                machineDictionaryOfKeyCode.Add(machines[i].keyCode, machines[i]);
                if(machines[i].machineType == MachineType.ActiveSkill)
                {
                    activeSkillList.Add(machines[i].playerStateType);
                    Debug.Log($"active skill list added {machines[i].playerStateType},{machines[i].machineType}");
                }   
            }   
        }
    }

    void Update()
    {
        if (isReady == false) return;
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
                    if (machineDictionaryOfPlayerState[PlayerState.LadderGoUp].IsExecuteOK())
                        ChangePlayerState(PlayerState.LadderGoUp);

                    if (machineDictionaryOfPlayerState[PlayerState.EdgeGrab].IsExecuteOK())
                        ChangePlayerState(PlayerState.EdgeGrab);
                }
                // Alt 
                if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                {
                    if (machineDictionaryOfPlayerState[PlayerState.Jump].IsExecuteOK())
                        ChangePlayerState(PlayerState.Jump);
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
            
            bool isOK = machineDictionaryOfKeyCode.TryGetValue(_keyInput, out playerStateMachine);
            //Debug.Log($"{isOK}, { keyInput}");
            _keyInput = KeyCode.None; // reset current key event input. 
            //Debug.Log($"{isOK}{ playerStateMachine.isReady}{playerStateMachine.IsExecuteOK()}");
            if (isOK &&
                playerStateMachine.isReady &&
                playerStateMachine.IsExecuteOK())
            {
                //Debug.Log($"{ playerStateMachine.isReady}, { playerStateMachine.IsExecuteOK()}");
                ChangePlayerState(playerStateMachine.playerStateType);
            }
        }
        else
        {
            move.x = 0;
        }
        UpdatePlayerState();
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
        if(newPlayerState == PlayerState.Crouch ||
           newPlayerState == PlayerState.CrouchFromFall)
            move.x = 0;
    }

    void FixedUpdate()
    {
        FixedUpdateMovement();
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
        if (oldPlayerState == PlayerState.Idle ||
           oldPlayerState == PlayerState.Run)
        {
            isOK = true;
        }
        return isOK;
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