using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
    public st_Stats stats;
    public List<st_Skill> skills;
    public bool isDead;
    private int _hp;
    public int hp
    {
        set
        {
            int tmpValue = value;
            if (tmpValue > 0)
            {
                isDead = false;
            }
            else
            {
                tmpValue = 0;
                isDead = true;
            }
                
            _hp = tmpValue;
            stats.hp = _hp;
            hpSlider.value = (float)_hp / stats.hpMax;
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
            stats.mp = _mp;
            mpSlider.value = (float)_mp / stats.mpMax;
            mpText.text = _mp.ToString();
        }
        get { return _mp; }
    }
    public Slider mpSlider;
    public Text mpText;
    private int _exp;
    public int exp
    {
        set
        {
            int tmpValue = value;
            if (tmpValue < 0)
                tmpValue = 0;
            _exp = tmpValue;
            stats.EXP = _exp;
            expSlider.value = (float)_exp / expMax;
        }
        get { return _exp; }
    }
    public Slider expSlider;
    public Text expText;
    public int expMax;

    [HideInInspector]public bool invincible;
    float invincibleTime = 1f;
    Coroutine invincibleCoroutine = null;

    // components
    private PlayerStateMachineManager controller;
    private Transform tr;
    private CapsuleCollider2D col;
    private void Awake()
    {
        stats = PlayerDataManager.instance.currentPlayerData.stats;
        skills = PlayerDataManager.instance.currentPlayerData.skills;
        controller = GetComponent<PlayerStateMachineManager>();
        tr = GetComponent<Transform>();
        col = GetComponent<CapsuleCollider2D>();
        hp = stats.hp;
        mp = stats.mp;
        exp = stats.EXP;
        expMax = stats.Level * (int)(100f *Mathf.Exp(3.0f));
        InvokeRepeating("SaveData", 2, 1);
    }
    
    public void Hurt(int damage, bool isCriticalHit)
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

        // damage Pop up 
        if(isDead == false)
            DamagePopUp.Create(tr.position + new Vector3(0f, col.size.y / 2, 0f), damage, gameObject.layer, isCriticalHit);
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
    public int CalcDamage(out bool isCriticalHit)
    {
        isCriticalHit = false;
        int damage = (stats.attack * 100 + stats.STR * 20) / 2;
        int randomValue = Random.Range(0, 100);
        if (randomValue < stats.criticalRate)
        {
            isCriticalHit = true;
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

    private void SaveData()
    {
        PlayerDataManager.instance.SavePlayerData(this);
        Debug.Log($"Save Data {stats.hp}");
    }
}
