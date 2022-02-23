using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetCaster : MonoBehaviour
{
    Rigidbody2D rb;
    public List<GameObject> targets;

    // for gizmos
    private Vector3 rangeCenterForGizmos;
    private Vector3 rangeSizeForGizmos;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void BoxCast(Vector2 center, Vector2 size, Vector2 direction, float distance, int layer)
    {
        targets.Clear();
        RaycastHit2D hit = Physics2D.BoxCast(rb.position + center, size, 0f, direction, distance, layer);
        if(hit.collider != null)
        {
            targets.Add(hit.collider.gameObject);
            Debug.Log(hit.collider.gameObject.name);
        }

        // gizmos
        rangeCenterForGizmos = new Vector3(rb.position.x + center.x + (distance *direction.x / 2), (rb.position.y + center.y), 0f);
        rangeSizeForGizmos = new Vector3(size.x + distance, size.y, 0);
        Debug.Log(rangeSizeForGizmos);
    }
    public void BoxCastAll(Vector2 center, Vector2 size, Vector2 direction, float distance, int layer)
    {
        targets.Clear();
        RaycastHit2D[] hits = Physics2D.BoxCastAll(rb.position + center, size, 0f, direction,distance,layer);
        foreach (RaycastHit2D hit in hits)
        {
            targets.Add(hit.collider.gameObject);
            Debug.Log(hit.collider.gameObject.name);
        }

        // gizmos
        rangeCenterForGizmos = new Vector3(rb.position.x + center.x + (distance * direction.x / 2), (rb.position.y + center.y), 0f); rangeCenterForGizmos = new Vector3(rb.position.x + ((center.x + distance * direction.x) / 2), (rb.position.y + center.y), 0f);
        rangeSizeForGizmos = new Vector3(size.x + distance, size.y, 0);

    }
    public void CircleCastAll(Vector2 center, float radius, Vector2 direction, float distance, int layer)
    {
        targets.Clear();
        RaycastHit2D[] hits = Physics2D.CircleCastAll(center, radius, direction,distance, layer);
        foreach (RaycastHit2D hit in hits)
        {
            targets.Add(hit.collider.gameObject);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(rangeCenterForGizmos, rangeSizeForGizmos);
        Gizmos.color = Color.cyan;
        foreach (GameObject target in targets)
        {
            BoxCollider2D targetBoxCol;
            bool boxColExist = target.TryGetComponent<BoxCollider2D>(out targetBoxCol);
            if (boxColExist)
            {
                Gizmos.DrawWireCube(targetBoxCol.transform.position + new Vector3(targetBoxCol.offset.x, targetBoxCol.offset.y, 0f),
                                new Vector3(targetBoxCol.size.x, targetBoxCol.size.y, 0));
            }
        }
    }
}
