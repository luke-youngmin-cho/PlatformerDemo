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

                hp = 100000,
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
struct st_Ability
{



}
