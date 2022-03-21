using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy's projectile behavior 
/// </summary>
public class EnemyProjectile : MonoBehaviour
{
    public LayerMask targetLayer;
    public float speed;
    int _direction;
    int _damage;
    bool _isCritical;
    bool setupFinished;
    Transform tr;

    //============================================================================
    //*************************** Public Methods *********************************
    //============================================================================

    public void Setup(int direction, int damage, bool isCritical)
    {
        _direction = direction;
        _damage = damage;
        _isCritical = isCritical;
        setupFinished = true;
    }


    //============================================================================
    //*************************** Private Methods ********************************
    //============================================================================

    private void Awake()
    {
        tr = GetComponent<Transform>();
    }
    
    void Update()
    {
        if(setupFinished)
            tr.position += new Vector3(_direction * speed * Time.deltaTime, 0f, 0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (setupFinished)
        {
            GameObject go = collision.gameObject;
            if (go == null) return;


            if (go.layer == LayerMask.NameToLayer("Player"))
            {
                Player player = go.GetComponent<Player>();
                if(player.isDead ==false)
                {
                    go.GetComponent<PlayerStateMachineManager>().KnockBack(_direction);
                    player.Hurt(_damage, _isCritical);
                }
                
            }
        }
    }
}
