using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerStateMachineManager : MonoBehaviour
{
    private Player player;
    private Rigidbody2D rb;

    // state machines
    Dictionary<PlayerState, PlayerStateMachine> stateMachineDictionary = new Dictionary<PlayerState, PlayerStateMachine>();
    PlayerState newState;
    PlayerState oldState;
    public delegate PlayerState MachineUpdateWorkflow();
    public MachineUpdateWorkflow machineUpdateWorkflow;
    public delegate void MachineFixedUpdateWorkflow();
    public MachineFixedUpdateWorkflow machineFixedUpdateWorkflow;
    private void Awake()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        RefreshStateMachineDictionary();
    }
    private void Update()
    {
        UpdateMachineWorkflow();
    }
    private void FixedUpdate()
    {
        machineFixedUpdateWorkflow();
    }
    private void RefreshStateMachineDictionary()
    {
        PlayerStateMachine[] tmpMachineArray = GetComponents<PlayerStateMachine>();
        for (int i = 0; i < tmpMachineArray.Length; i++)
        {
            stateMachineDictionary.Add(tmpMachineArray[i].machineType, tmpMachineArray[i]);
        }
        machineUpdateWorkflow = stateMachineDictionary[PlayerState.Idle].UpdateWorkflow;
        machineFixedUpdateWorkflow = stateMachineDictionary[PlayerState.Idle].FixedUpdateWorkflow;
    }
    private void UpdateMachineWorkflow()
    {
        newState = machineUpdateWorkflow();
        if (newState != oldState)
        {
            ChangePlayerState(newState);
        }
        oldState = newState;
    }
    public void ChangePlayerState(PlayerState stateToChange)
    {
        newState = stateToChange;
        PlayerStateMachine machineToChange = stateMachineDictionary[newState];
        machineUpdateWorkflow = machineToChange.UpdateWorkflow;
        machineFixedUpdateWorkflow = machineToChange.FixedUpdateWorkflow;
        machineToChange.PrepareWorkflow();
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
    Ladder,
    Hurt,
    Die,
    DownJump,
}