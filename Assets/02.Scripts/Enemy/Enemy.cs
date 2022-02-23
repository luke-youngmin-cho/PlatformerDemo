using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Enemy : MonoBehaviour
{
    st_Stats stats;
    private int _hp;
    public int hp
    {
        set
        {
            _hp = value;
            hpSlider.value = (float)_hp / stats.hp;
        }
        get { return _hp; }
    }
    [SerializeField] Slider hpSlider;
    private void Awake()
    {
        stats = PlayerSettings.basicStats;
    }
    
    private int CalcDamage()
    {
        int damage = (stats.attack * 100 + stats.STR * 20) / 2;
        int randomValue = Random.Range(0, 100);
        if (randomValue < stats.criticalRate)
        {
            damage *= stats.criticalDamage;
            damage /= 100;
        }
        return damage;
    }
    public void Hurt(int damage)
    {
        hp -= damage;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log($"Something hit {collision.name}");
        GameObject go = collision.gameObject;
        if (go == null) return;

        if (go.layer == LayerMask.NameToLayer("Player"))
        {
            go.GetComponent<Player>().Hurt(CalcDamage());
        }
    }

    
}
