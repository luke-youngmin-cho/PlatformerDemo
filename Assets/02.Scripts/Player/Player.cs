using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
    public st_Stats stats;

    private int _hp;
    public int hp
    {
        set
        {
            int tmpValue = value;
            if (tmpValue < 0)
                tmpValue = 0;
            _hp = tmpValue;
            hpSlider.value = (float)_hp / stats.hp;
            hpText.text = _hp.ToString();
        }
        get { return _hp; }
    }
    public Slider hpSlider;
    public Text hpText;

    private int _mp;
    public int mp
    {
        set
        {
            int tmpValue = value;
            if (tmpValue < 0)
                tmpValue = 0;
            _mp = tmpValue;
            mpSlider.value = (float)_mp / stats.hp;
            mpText.text = _mp.ToString();
        }
        get { return _mp; }
    }
    public Slider mpSlider;
    public Text mpText;

    bool invincible;
    float invincibleTime = 1f;
    Coroutine invincibleCoroutine = null;

    private PlayerController controller;

    private void Awake()
    {
        stats = PlayerSettings.basicStats;
        controller = GetComponent<PlayerController>();
        hp = stats.hp;
        mp = stats.mp;
    }
    
    public void Hurt(int damage)
    {
        if (invincible) return;

        if (invincibleCoroutine != null)
            StopCoroutine(invincibleCoroutine);
        invincibleCoroutine = StartCoroutine(E_SetInvincible(invincibleTime));

        int tmpHP = _hp;
        tmpHP -= damage;
        if (tmpHP > 0)
        {
            controller.TryHurt();
        }   
        else
        {
            invincible = true;
            controller.TryDie();
        }
        hp = tmpHP;
    }
    public int CalcDamage()
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
    IEnumerator E_SetInvincible(float time)
    {
        invincible = true;
        yield return new WaitForSeconds(time);
        invincible = false;
        invincibleCoroutine = null;
    }
}
