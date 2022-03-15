using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Enemy : MonoBehaviour
{
    public st_Stats stats;

    [Header("Target")]
    public LayerMask targetLayer;
    public float sightRange;
    public float releaseTargetTime;
    private Coroutine releaseTargetCoroutine;
    [HideInInspector] public GameObject target;

    [HideInInspector]public bool isDead;
    private int _hp;
    public int hp
    {
        set
        {
            _hp = value;
            if (_hp > 0)
            {
                isDead = false;
                col.enabled = true;
            }   
            else
            {
                isDead = true;
                col.enabled = false;
                _hp = 0;
            }   
            hpBar.value = (float)_hp / stats.hpMax;
        }
        get { return _hp; }
    }
    [SerializeField] Slider hpBar;

    [Header("Animation")]
    public float hpBarShowTime;
    Coroutine hpBarShowCoroutine;
    // components
    Transform tr;
    EnemyController controller;
    CapsuleCollider2D col;
    SpriteRenderer spriteRenderer;
    Color originColor;
    private void Awake()
    {
        tr = GetComponent<Transform>();
        controller = GetComponent<EnemyController>();
        col = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        stats = EnemySettings.GetStats(this.GetType().Name);
        
        originColor = spriteRenderer.color;
    }
    private void OnEnable()
    {
        spriteRenderer.color = originColor;
        hp = stats.hpMax;
        controller.moveAIState = EnemyController.MoveAIState.DecideRandomBehavior;
    }
    private void OnDisable()
    {
        ObjectPool.ReturnToPool(gameObject);
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
    public int CalcDamage(out bool isCritical)
    {
        isCritical = false;
        int damage = (stats.attack * 100 + stats.STR * 20) / 2;
        int randomValue = Random.Range(0, 100);
        if (randomValue < stats.criticalRate)
        {
            isCritical = true;
            damage *= stats.criticalDamage;
            damage /= 100;
        }
        return damage;
    }
    public void Hurt(int damage, bool isCriticalHit)
    {
        ShowHPBarForSeconds();
        DamagePopUp.Create(tr.position + new Vector3(0f, col.size.y / 2, 0f), damage, gameObject.layer, isCriticalHit);

        FindWhoHurtThis();
        hp -= damage;
        controller.TryHurt();
        if (_hp <= 0)
        {
            controller.TryDie();
            StartCoroutine(E_FadeOutWhenDead());
        }
    }
    public void Hurt(List<int> damages, List<bool> isCriticalHit, float timeGap)
    {
        FindWhoHurtThis();
        StartCoroutine(E_Hurt(damages,isCriticalHit,timeGap));
    }
    IEnumerator E_Hurt(List<int> damages, List<bool> isCriticalHits, float timeGap)
    {
        ShowHPBarForSeconds();
        for (int i = 0; i < damages.Count; i++)
        {
            DamagePopUp.Create(tr.position + new Vector3(0f, col.size.y / 2, 0f), damages[i], gameObject.layer, isCriticalHits[i]);
            hp -= damages[i];
            controller.TryHurt();
            yield return new WaitForSeconds(timeGap);
        }
        if (hp <= 0)
        {
            controller.TryDie();
            StartCoroutine(E_FadeOutWhenDead());
        }
    }

    private void FindWhoHurtThis()
    {
        Collider2D targetCol = Physics2D.OverlapCircle(new Vector2(tr.position.x, tr.position.y), sightRange,targetLayer);
        if (targetCol != null)
        {
            target = targetCol.gameObject;
            if (releaseTargetCoroutine != null)
                StopCoroutine(releaseTargetCoroutine);
            releaseTargetCoroutine = StartCoroutine(E_ReleaseTarget());
        }
    }
    IEnumerator E_ReleaseTarget()
    {
        yield return new WaitForSeconds(releaseTargetTime);
        target = null;
        releaseTargetCoroutine = null;
        FindWhoHurtThis();
    }
    IEnumerator E_FadeOutWhenDead()
    {
        float elapsedTime = 0;
        Color originColor = spriteRenderer.color;
        while (elapsedTime < 1f)
        {
            spriteRenderer.color = new Color(originColor.r, originColor.g, originColor.b, 1f - elapsedTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
    }
    void ShowHPBarForSeconds()
    {
        if (hpBarShowCoroutine != null)
            StopCoroutine(E_ShowHPBarForSeconds());
        hpBarShowCoroutine = StartCoroutine(E_ShowHPBarForSeconds());
    }
    IEnumerator E_ShowHPBarForSeconds()
    {
        float elapsedTime = 0;
        while(elapsedTime < hpBarShowTime)
        {
            hpBar.gameObject.SetActive(true);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        hpBar.gameObject.SetActive(false);
        hpBarShowCoroutine = null;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        GameObject go = collision.gameObject;
        if (go == null) return;

        if (go.layer == LayerMask.NameToLayer("Player"))
        {
            Player player = go.GetComponent<Player>();
            if (isDead == false && 
                player.isDead == false)
            {
                bool isCritical;
                int damage = CalcDamage(out isCritical);
                go.GetComponent<PlayerStateMachineManager>().KnockBack();
                player.Hurt(damage, isCritical);
            }
        }
    }

    
}
