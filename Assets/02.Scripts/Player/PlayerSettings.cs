using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettings : MonoBehaviour
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
            };
        }
    }
    public static List<st_Skill> basicSkills
    {
        get
        {
            List<st_Skill> skillDatas = new List<st_Skill>();
            // Double Attack
            st_Skill doubleAttack = new st_Skill {
                state = PlayerState.DoubleAttack,
                level = 1,
                hpRequired = 0,
                mpRequired = 10
            };
            skillDatas.Add(doubleAttack);
            return skillDatas;
        }
    }
}

