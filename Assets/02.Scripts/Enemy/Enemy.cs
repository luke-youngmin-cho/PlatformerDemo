using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    int damage = 20;
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log($"Something hit {collision.name}");
        GameObject go = collision.gameObject;
        if (go == null) return;

        if (go.layer == LayerMask.NameToLayer("Player"))
        {
            go.GetComponent<Player>().Hurt(damage);
        }
    }
}
