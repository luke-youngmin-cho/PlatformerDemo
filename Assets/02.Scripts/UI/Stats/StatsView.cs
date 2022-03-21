using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Show player stats.
/// </summary>
public class StatsView : MonoBehaviour
{
    [SerializeField] Text nickName;
    [SerializeField] Text level;
    [SerializeField] Text damage;
    [SerializeField] Text hp;
    [SerializeField] Text mp;
    [SerializeField] Text attack;
    [SerializeField] Text critical;
    [SerializeField] Text criticalRate;
    [SerializeField] Text defencePen;
    [SerializeField] Text magicDefencePen;
    [SerializeField] Text defence;
    [SerializeField] Text magicDefence;
    [SerializeField] Text statPoint;
    [SerializeField] Text STR;
    [SerializeField] Text DEX;
    [SerializeField] Text CON;
    [SerializeField] Text WIS;
    [SerializeField] Text INT;
    [SerializeField] Text RES;


    private void Update()
    {
        Player player = Player.instance;
        if (player != null && player.isReady)
        {
            nickName.text = PlayerDataManager.instance.data.nickName;
            level.text = player.stats.Level.ToString();
            damage.text = "Not implemented yet";
            hp.text = player.stats.hp.ToString() + "/" + player.stats.hpMax.ToString();
            mp.text = player.stats.mp.ToString() + "/" + player.stats.mpMax.ToString();
            attack.text = player.stats.attack.ToString();
            critical.text = player.stats.criticalDamage.ToString();
            criticalRate.text = player.stats.criticalRate.ToString();
            defencePen.text = player.stats.defencePen.ToString();
            magicDefencePen.text = player.stats.magicDefencePen.ToString();
            defence.text = player.stats.defence.ToString();
            magicDefence.text = player.stats.magicDefence.ToString();
            statPoint.text = player.stats.statPoint.ToString();
            STR.text = player.stats.STR.ToString();
            DEX.text = player.stats.DEX.ToString();
            CON.text = player.stats.CON.ToString();
            WIS.text = player.stats.WIS.ToString();
            INT.text = player.stats.INT.ToString();
            RES.text = player.stats.RES.ToString();
        }
    }
}
