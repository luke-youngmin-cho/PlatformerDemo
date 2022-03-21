using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player info & stats & skill list data
/// </summary>
public class PlayerData
{
    public string nickName;
    public st_Stats stats;
    public List<st_SkillStats> skillstatsList;

    //============================================================================
    //*************************** Public Methods *********************************
    //============================================================================

    public PlayerData()
    {
        skillstatsList = new List<st_SkillStats>();
    }

    public void OverwriteStats(st_Stats newStats)
    {
        stats = newStats;
    }

    public void OverwriteSkillStatsList(List<st_SkillStats> newSkillStatsList)
    {
        skillstatsList = newSkillStatsList;
    }
}
[System.Serializable]
public struct st_Stats
{
    public int Level;
    public int EXP;

    public int STR;
    public int DEX;
    public int CON;
    public int WIS;
    public int INT;
    public int RES;

    public int hpMax;
    public int mpMax;
    public int hp;
    public int mp;

    public int attack;
    public int criticalRate; // %
    public int criticalDamage; // %
    public int defence;
    public int magicDefence;
    public int defencePen;
    public int magicDefencePen;
    public float moveSpeed;
    public float jumpForce;

    public float statPoint;
    public float skillPoint;
}

[System.Serializable]
public struct st_SkillStats
{
    public PlayerState state;
    public int level;
    public int hpRequired;
    public int mpRequired;
}
