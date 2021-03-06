using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gizmos & casted target dictionary
/// </summary>
public class TargetCaster : MonoBehaviour
{
    Rigidbody2D rb;
    public Dictionary<string, List<GameObject>> targetsDictionary = new Dictionary<string, List<GameObject>>();
    public LayerMask targetLayer;
    
    // Gizmos
    private Vector3 rangeCenterForGizmos;
    private Vector3 rangeSizeForGizmos;
    private string currentCategory;
    

    //============================================================================
    //*************************** Public Methods *********************************
    //============================================================================

    public void BoxCast(string category, Vector2 center, Vector2 size, Vector2 direction, float distance)
    {
        if(targetsDictionary.ContainsKey(category) == false)
        {
            List<GameObject> targets = new List<GameObject>();
            targetsDictionary.Add(category, targets);
        }

        targetsDictionary[category].Clear();
        
        RaycastHit2D hit = Physics2D.BoxCast(rb.position + center, size, 0f, direction, distance, targetLayer);
        if(hit.collider != null)
        {
            targetsDictionary[category].Add(hit.collider.gameObject);
        }

        // gizmos
        rangeCenterForGizmos = new Vector3(rb.position.x + center.x + (distance *direction.x / 2), (rb.position.y + center.y), 0f);
        rangeSizeForGizmos = new Vector3(size.x + distance, size.y, 0);
        currentCategory = category;
    }

    public void BoxCastAll(string category, Vector2 center, Vector2 size, Vector2 direction, float distance)
    {
        if (targetsDictionary.ContainsKey(category) == false)
        {
            List<GameObject> targets = new List<GameObject>();
            targetsDictionary.Add(category, targets);
        }

        targetsDictionary[category].Clear();

        RaycastHit2D[] hits = Physics2D.BoxCastAll(rb.position + center, size, 0f, direction,distance, targetLayer);
        foreach (RaycastHit2D hit in hits)
        {
            targetsDictionary[category].Add(hit.collider.gameObject);
        }

        // gizmos
        rangeCenterForGizmos = new Vector3(rb.position.x + center.x + (distance * direction.x / 2), (rb.position.y + center.y), 0f); rangeCenterForGizmos = new Vector3(rb.position.x + ((center.x + distance * direction.x) / 2), (rb.position.y + center.y), 0f);
        rangeSizeForGizmos = new Vector3(size.x + distance, size.y, 0);
        currentCategory = category;

    }


    //============================================================================
    //*************************** Private Methods ********************************
    //============================================================================
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(rangeCenterForGizmos, rangeSizeForGizmos);
        Gizmos.color = Color.cyan;

        if(targetsDictionary.Count > 0)
        {
            if (targetsDictionary.ContainsKey(currentCategory))
            {
                foreach (GameObject target in targetsDictionary[currentCategory])
                {
                    if (target != null)
                    {
                        BoxCollider2D targetBoxCol = target.GetComponent<BoxCollider2D>();
                        if (targetBoxCol != null)
                        {
                            Gizmos.DrawWireCube(targetBoxCol.transform.position + new Vector3(targetBoxCol.offset.x, targetBoxCol.offset.y, 0f),
                                            new Vector3(targetBoxCol.size.x, targetBoxCol.size.y, 0));
                        }
                    }
                }
            }
        }
    }

    [System.Serializable]
    public struct st_BoxCastElements
    {
        public Vector2 center;
        public Vector2 size;
        public Vector2 direction;
        public float distance;
    }
}
