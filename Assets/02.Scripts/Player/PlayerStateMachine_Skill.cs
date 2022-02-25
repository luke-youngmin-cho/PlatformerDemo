using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine_Skill : MonoBehaviour
{
    public bool isBusy;
    public SkillState skillState;
    [HideInInspector] public float skillTime;
    [HideInInspector] public float elapsedSkillTime;
    public virtual bool IsSkillPossible()
    {
        return true;
    }
    public virtual void UpdateSkillState()
    {
        switch (skillState)
        {
            case SkillState.Idle:
                break; 
            case SkillState.PrepareToSkill:
                break;
            case SkillState.Casting:
                break;
            case SkillState.Using:
                break;
            case SkillState.Finished:
                break;
            default:
                break;
        }
    }
    public virtual void ActiveSkill()
    {
        isBusy = true;
        skillState = SkillState.PrepareToSkill;
    }
    public virtual void ResetState()
    {
        isBusy = false;
        skillState = SkillState.Idle;
    }
    public enum SkillState
    {
        Idle,
        PrepareToSkill,
        Casting,
        Using,
        Finished,
    }
}
