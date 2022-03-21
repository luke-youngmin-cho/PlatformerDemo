using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Temporary class. 
/// enemy datas should be managed with json or xml
/// </summary>
public class EnemySettings : MonoBehaviour
{
    public static st_Stats GetStats(string name)
    {
        st_Stats stats;
        switch (name)
        {
            case "Enemy_Snail":
                stats = new st_Stats
                {
                    STR = 0,
                    DEX = 0,
                    CON = 0,
                    WIS = 0,
                    INT = 0,
                    RES = 0,

                    hpMax = 100,
                    mpMax = 0,
                    attack = 1,
                    criticalRate = 0,
                    criticalDamage = 150,
                    defence = 0,
                    magicDefence = 0,
                    defencePen = 0,
                    magicDefencePen = 0,
                    moveSpeed = 0.5f,
                    jumpForce = 0,
                };
                break;
            case "Enemy_Nepenthes":
                stats = new st_Stats
                {
                    STR = 10,
                    DEX = 0,
                    CON = 0,
                    WIS = 0,
                    INT = 0,
                    RES = 0,

                    hpMax = 2000,
                    mpMax = 0,
                    attack = 10,
                    criticalRate = 10,
                    criticalDamage = 150,
                    defence = 0,
                    magicDefence = 0,
                    defencePen = 0,
                    magicDefencePen = 0,
                    moveSpeed = 0,
                    jumpForce = 0,
                };
                break;
            case "Enemy_DarkNepenthes":
                stats = new st_Stats
                {
                    STR = 0,
                    DEX = 0,
                    CON = 0,
                    WIS = 0,
                    INT = 10,
                    RES = 0,

                    hpMax = 2500,
                    mpMax = 0,
                    attack = 12,
                    criticalRate = 10,
                    criticalDamage = 150,
                    defence = 0,
                    magicDefence = 0,
                    defencePen = 0,
                    magicDefencePen = 0,
                    moveSpeed = 0.5f,
                    jumpForce = 0,
                };
                break;
            default:
                stats = new st_Stats
                {
                    STR = 0,
                    DEX = 0,
                    CON = 0,
                    WIS = 0,
                    INT = 0,
                    RES = 0,

                    hpMax = 10000,
                    mpMax = 0,
                    attack = 1,
                    criticalRate = 0,
                    criticalDamage = 150,
                    defence = 0,
                    magicDefence = 0,
                    defencePen = 0,
                    magicDefencePen = 0,
                    moveSpeed = 0.5f,
                    jumpForce = 0,
                };
                break;
        }
        return stats;
    }
}
