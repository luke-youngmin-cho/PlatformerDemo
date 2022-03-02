using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerData
{
    public string nickName;
    public st_Stats stats;
    public List<st_Skill> skills;
}
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
}
public struct st_Skill
{
    public PlayerState state;
    public int level;
    public int hpRequired;
    public int mpRequired;
}
