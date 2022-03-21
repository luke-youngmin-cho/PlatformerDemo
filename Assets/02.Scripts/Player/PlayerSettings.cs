using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// temporary class. 
/// </summary>
public class PlayerSettings
{
    public static st_Stats basicStats
    {
        get
        {
            return new st_Stats
            {
                Level = 1,
                EXP = 0,

                STR = 10,
                DEX = 10,
                CON = 10,
                WIS = 10,
                INT = 10,
                RES = 10,

                hpMax = 10000,
                mpMax = 100,
                hp = 10000,
                mp = 100,
                attack = 1,
                criticalRate = 50,
                criticalDamage = 150,
                defence = 0,
                magicDefence = 0,
                defencePen = 0,
                magicDefencePen = 0,
                moveSpeed = 1f,
                jumpForce = 3f,

                statPoint = 5,
                skillPoint = 5,
            };
        }
    }
    
    public static List<st_SkillStats> basicSkills
    {
        get
        {
            List<st_SkillStats> skillDatas = new List<st_SkillStats>();
            // Double Attack
            st_SkillStats doubleAttack = new st_SkillStats {
                state = PlayerState.DoubleAttack,
                level = 1,
                hpRequired = 0,
                mpRequired = 10
            };
            skillDatas.Add(doubleAttack);
            st_SkillStats dashAttack = new st_SkillStats
            {
                state = PlayerState.DashAttack,
                level = 1,
                hpRequired = 0,
                mpRequired = 20
            };
            skillDatas.Add(dashAttack);
            return skillDatas;
        }
    }
}

