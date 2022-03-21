using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillAssets : MonoBehaviour
{
    private static SkillAssets _instance;
    public static SkillAssets instance
    {
        get
        {
            if (_instance == null)
                _instance = Instantiate(Resources.Load<SkillAssets>("Assets/SkillAssets"));
            return _instance;
        }
    }
    public List<Skill> skills = new List<Skill>();

    public Skill GetSkillByState(PlayerState state)
    {
        Skill tmpSkill = null;
        foreach (var skill in skills)
        {
            if(skill.playerState == state)
            {
                tmpSkill = skill;
                break;
            }                
        }
        return tmpSkill;
    }
    public MachineType GetMachineTypeByState(PlayerState state)
    {
        MachineType tmpMachineType = MachineType.BasicSkill;
        foreach (var skill in skills)
        {
            if(skill.playerState == state)
            {
                tmpMachineType = skill.machineType;
                break;
            }
        }
        return tmpMachineType;
    }
}
