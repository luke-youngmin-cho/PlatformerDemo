using UnityEngine;
class PlayerStateMachine : MonoBehaviour
{
    public PlayerState machineType;
    [HideInInspector] public Player player;
    [HideInInspector] public Animator animator;
    [HideInInspector] public PlayerAnimation playerAnimation;
    // physics
    [HideInInspector] public Rigidbody2D rb;

    // animation
    public string animationName; // must initiate this field at the top of Awake method. (beforer calling [playerAnimation.RegisterAnimationRequired(animationName);])
    public virtual void Awake()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerAnimation = GetComponent<PlayerAnimation>();
        playerAnimation.RegisterAnimationRequired(animationName);
    }
    
    public virtual PlayerState UpdateWorkflow()
    {
        PlayerState nextState = machineType;
        //-----------------------------------
        // Do update workflow
        //-----------------------------------
        
        return nextState;
    }
    public virtual void FixedUpdateWorkflow()
    {
        //-----------------------------------
        // Do fixed update workflow
        //-----------------------------------
    }
    public virtual void PrepareWorkflow()
    {
        // Change machine state from idle to prepare step.
    }
    
}