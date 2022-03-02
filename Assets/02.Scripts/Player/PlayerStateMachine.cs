using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Player's every single behavior state machine should be derived from this class
public class PlayerStateMachine : MonoBehaviour
{
    public bool isReady { get { return state == State.Idle; } }
    public bool isStarted {  get { return state >= State.Prepare; } }
    public bool isActiveSkill = true;
    public KeyCode keyCode;
    public State state;
    public PlayerState playerStateType;
    [HideInInspector] public PlayerState nextPlayerState = PlayerState.Idle; // normally state will change to idle when machine finished
    public string animationName;
    [HideInInspector] public float animationTime;
    [HideInInspector] public float elapsedTime;
    [HideInInspector] public PlayerStateMachineManager manager;
    [HideInInspector] public AnimationManager animationManager;

    // player data
    public Player player;
    public int level = 1;
    public int hpRequired;
    public int mpRequired;
     
    public virtual void Awake()
    {
        manager = gameObject.GetComponent<PlayerStateMachineManager>();
        animationManager = gameObject.GetComponent<AnimationManager>();
        player = GetComponent<Player>();
        animationName = playerStateType.ToString();
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
        if(player.stats.hpMax > hpRequired)
            player.stats.hpMax -= hpRequired;
        if(player.stats.mpMax > mpRequired)
            player.stats.mpMax -= mpRequired;
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
