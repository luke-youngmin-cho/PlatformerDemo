using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Player : MonoBehaviour
{
    public static Player instance;
    public bool isReady = false;
    private st_Stats _stats;
    public st_Stats stats
    {
        set { 
            _stats = value;
            PlayerDataManager.instance.data.OverwriteStats(stats);
            PlayerDataManager.instance.SaveData();
        }
        get { return _stats; }
    }
    private List<st_SkillStats> _skillStatsList;
    public List<st_SkillStats> skillStatsList
    {
        set{
            _skillStatsList = value;
            PlayerDataManager.instance.data.OverwriteSkillStatsList(_skillStatsList);
            PlayerDataManager.instance.SaveData();
        }
        get{ return _skillStatsList; }
    }
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
            st_Stats tmpStats = stats;
            tmpStats.hp = _hp;
            stats = tmpStats;
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
            st_Stats tmpStats = stats;
            tmpStats.mp = _mp;
            stats = tmpStats;
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
            st_Stats tmpStats = stats;
            tmpStats.EXP = _exp;
            stats = tmpStats;
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
    private PlayerStateMachineManager machineManager;
    private Transform tr;
    private CapsuleCollider2D col;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        machineManager = GetComponent<PlayerStateMachineManager>();
        tr = GetComponent<Transform>();
        col = GetComponent<CapsuleCollider2D>();
        
    }
    private void Start()
    {
        StartCoroutine(E_Start());
    }
    IEnumerator E_Start()
    {
        yield return new WaitUntil(() => PlayerDataManager.instance.isApplied);
        
        hp =  stats.hp;
        mp =  stats.mp;
        exp = stats.EXP;
        expMax = stats.Level * (int)(100f * Mathf.Exp(3.0f));
        AddAllSkillMachines();
        isReady = true;
    }
    public void AddAllSkillMachines()
    {
        foreach (var skillStats in skillStatsList)
        {
            if(SkillAssets.instance.GetMachineTypeByState(skillStats.state) != MachineType.BasicSkill)
                machineManager.AddStateMachineComponentByState(skillStats.state);
        }
    }
    public void SkillLevelUp(PlayerState state)
    {
        if(stats.skillPoint > 0)
        {
            for (int i = 0; i < skillStatsList.Count; i++)
            {
                if (skillStatsList[i].state == state)
                {
                    skillStatsList[i] = new st_SkillStats
                    {
                        state = skillStatsList[i].state,
                        hpRequired = skillStatsList[i].hpRequired,
                        mpRequired = skillStatsList[i].mpRequired,
                        level = skillStatsList[i].level + 1,
                    };
                    st_Stats tmpStats = stats;
                    tmpStats.skillPoint--;
                    stats = tmpStats;
                    break;
                }
            }
        }
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
            machineManager.TryHurt();
        }   
        else
        {
            invincible = true;
            machineManager.TryDie();
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


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision == null) return;
        GameObject go = collision.gameObject;

        //Debug.Log($"something detected : {go.name}");
        if (go.layer == LayerMask.NameToLayer("Item"))
        {
            if (DataManager.isApplied)
            {
                // Pick Up
                if (ShortCutsView.instance.TryGetShortCut("BasicKey_PickUp", out ShortCut pickUpShortCut))
                {
                    if (Input.GetKey(pickUpShortCut._keyCode))
                    {
                        ItemController itemController = null;
                        if (go.TryGetComponent(out itemController))
                        {
                            itemController.PickUp(this);
                        }
                    }
                }
            }
        }

        if (go.layer == LayerMask.NameToLayer("Money"))
        {
            if (DataManager.isApplied)
            {
                // Pick Up
                if (ShortCutsView.instance.TryGetShortCut("BasicKey_PickUp", out ShortCut pickUpShortCut))
                {
                    if (Input.GetKey(pickUpShortCut._keyCode))
                    {
                        MoneyController moneyController = null;
                        if (go.TryGetComponent(out moneyController))
                        {
                            moneyController.PickUp(this);
                        }
                    }
                }
            }
        }
    }
}
