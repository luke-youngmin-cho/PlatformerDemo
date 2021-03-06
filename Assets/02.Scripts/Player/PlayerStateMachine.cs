using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player's every single of behavior class should be derived from this class
/// Workflow : Idle -> Prepare -> Casting -> OnAction -> Finished
/// </summary>
public class PlayerStateMachine : MonoBehaviour
{
    public bool isReady 
    { 
        get
        { 
            return state == State.Idle; 
        } 
    }
    public bool isStarted
    {
        get 
        {
            return state >= State.Prepare; 
        } 
    }
    public MachineType machineType;
    public KeyCode keyCode; // registered short cut.
    public State state;
    private PlayerState _playerStateType;
    public PlayerState playerStateType
    {
        set
        {
            _playerStateType = value;
            animationName = _playerStateType.ToString();
            animationTime = animationManager.GetAnimationTime(animationName);
        }
        get 
        { 
            return _playerStateType; 
        }
    }
    [HideInInspector] public PlayerState nextPlayerState = PlayerState.Idle; // normally state will change to idle when machine finished
    [HideInInspector] public string animationName;
    [HideInInspector] public float animationTime;
    [HideInInspector] public float elapsedTime;
    [HideInInspector] public PlayerStateMachineManager manager;
    [HideInInspector] public AnimationManager animationManager;

    // player data
    [HideInInspector] public Player player;
    [HideInInspector] public int level = 1;
    [HideInInspector] public int hpRequired;
    [HideInInspector] public int mpRequired;
     
    public virtual void Awake()
    {
        manager = gameObject.GetComponent<PlayerStateMachineManager>();
        animationManager = gameObject.GetComponent<AnimationManager>();
        player = GetComponent<Player>();
        machineType = MachineType.BasicSkill;
        animationName = _playerStateType.ToString();
        animationTime = animationManager.GetAnimationTime(animationName);
    }

    public virtual bool IsExecuteOK()
    {
        bool isOK = true;
        return isOK;
    }

    public virtual void UpdateWorkflow()
    {
        switch (state)
        {
            case State.Idle:
                break; 
            case State.Prepare:
                animationManager.ChangeAnimationState(animationName);
                state = State.OnAction;
                break;
            case State.Casting:
                break;
            case State.OnAction:
                if(elapsedTime > animationTime)
                {
                    nextPlayerState = PlayerState.Idle;
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

    public virtual void Execute()
    {
        state = State.Prepare;
        st_Stats tmpStats = player.stats;
        if(player.stats.hpMax > hpRequired)
        {
            tmpStats.hp -= hpRequired;
        }
        if(player.stats.mpMax > mpRequired)
        {
            tmpStats.mp -= mpRequired;
        }
        player.stats = tmpStats;
    }

    public virtual void ResetMachine()
    {
        state = State.Idle;
        elapsedTime = 0;
    }

    public enum State
    {
        Idle,
        Prepare,
        Casting,
        OnAction,
        Finished,
    }
}

public enum MachineType
{
    BasicSkill,
    ActiveSkill,
    PassiveSkill,
    ActiveSkillInterruptible,
}