using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Detect singel ground & multiple grounds
/// remember latest detected ground.
/// able to ignore latest detected ground
/// </summary>
public class GroundDetector : MonoBehaviour
{
    Vector2 center;
    [HideInInspector] public Vector2 size;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask groundLimitLayer;
    private Collider2D currentGroundCol;
    public Collider2D lastGroundCol;
    private Collider2D passingGroundCol;
    public bool isDetected;
    public bool doDownJumpCheck;
    public bool downJumpAvailable;
    public float downJumpCheckHeight;
    public bool isIgnoringGround;
    public Coroutine passingGroundCoroutine;

    // Components
    Rigidbody2D rb;
    CapsuleCollider2D col;

    //============================================================================
    //*************************** Public Methods *********************************
    //============================================================================

    public void IgnoreCurrentGroundUntilPassedIt()
    {
        passingGroundCol = lastGroundCol;
        if (passingGroundCol != null)
        {
            isIgnoringGround = true;
            StartCoroutine(E_IgnorePassingGroundUntilPassedIt(passingGroundCol));
        }
    }

    public void StopIgnoreGrounds()
    {
        StopAllCoroutines();
    }


    //============================================================================
    //*************************** Public Methods *********************************
    //============================================================================

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        size.x = col.size.x /2;
        size.y = 0.005f;
    }

    void Update()
    {
        center.x = rb.position.x + col.offset.x;
        center.y = rb.position.y + col.offset.y - col.size.y/2 -  (size.y * 2);
        currentGroundCol = Physics2D.OverlapBox(center, size, 0, groundLayer | groundLimitLayer);
        if (currentGroundCol == null)
            isDetected = false;
        else
        {
            lastGroundCol = currentGroundCol;
            if (currentGroundCol != passingGroundCol)
                isDetected = true;
        }

        if (doDownJumpCheck)
        {
             List<Collider2D> groundsCols = Physics2D.OverlapBoxAll(new Vector2(center.x, center.y - downJumpCheckHeight / 2),
                                                    new Vector2(col.size.x, downJumpCheckHeight),
                                                    0,
                                                    groundLayer | groundLimitLayer).ToList();
            groundsCols.OrderBy(x=> x.transform.position.y);

            int groundNum = groundsCols.Count;
            if (groundNum > 1 &&
                ((1 << groundsCols[0].gameObject.layer) == groundLayer))
            {
                downJumpAvailable = true;
            }
            else
                downJumpAvailable = false;
            
        }
        else
            downJumpAvailable = false;
    }

    IEnumerator E_IgnorePassingGroundUntilPassedIt(Collider2D groundCol)
    {
        Physics2D.IgnoreCollision(col,groundCol,true);
        float passingGroundColCenterY = groundCol.transform.position.y + col.offset.y;

        // wait passing start
        yield return new WaitUntil(() =>
        {
            return rb.position.y + col.offset.y - col.size.y / 2 < passingGroundColCenterY - size.y;
        });

        // ignoring
        yield return new WaitUntil(() =>
        {
            bool isPassed = false;

            if (groundCol != null)
            {
                passingGroundColCenterY = groundCol.transform.position.y + col.offset.y;
                if ((rb.position.y + col.offset.y + col.size.y / 2 < passingGroundColCenterY - size.y) ||
                    (rb.position.y + col.offset.y - col.size.y / 2 > passingGroundColCenterY + size.y))
                {
                    isPassed = true;
                }
            }
            else
                isPassed = true;
            return isPassed;
        });

        Physics2D.IgnoreCollision(col, groundCol, false);
        isIgnoringGround = false;
        passingGroundCol = null;
    }

    private void OnDrawGizmosSelected()
    {   
        if (doDownJumpCheck)
        {
            Gizmos.color = new Color(0, 1f, 0.4f, 1);
            Gizmos.DrawCube(new Vector3(center.x, center.y - downJumpCheckHeight / 2, 0), new Vector3(size.x, downJumpCheckHeight, 0f));
        }
        Gizmos.color = Color.green;
        Gizmos.DrawCube(new Vector3(center.x, center.y, 0), new Vector3(size.x, size.y, 0f));

        Gizmos.color = Color.black;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CapsuleCollider2D>();
        Gizmos.DrawLine(new Vector3(rb.position.x + col.offset.x - size.x / 2, rb.position.y + col.offset.y + col.size.y / 2, 0f),
                        new Vector3(rb.position.x + col.offset.x + size.x / 2, rb.position.y + col.offset.y + col.size.y / 2, 0f));
        Gizmos.DrawLine(new Vector3(rb.position.x + col.offset.x - size.x / 2, rb.position.y + col.offset.y - col.size.y / 2, 0f),
                        new Vector3(rb.position.x + col.offset.x + size.x / 2, rb.position.y + col.offset.y - col.size.y / 2, 0f));

        if (passingGroundCol != null)
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawSphere(new Vector3(passingGroundCol.transform.position.x + passingGroundCol.offset.x,
                                          passingGroundCol.transform.position.y + passingGroundCol.offset.y, 0), 0.01f);

            BoxCollider2D boxCol;
            if (passingGroundCol.TryGetComponent(out boxCol))
            {
                Gizmos.DrawCube(new Vector3(passingGroundCol.transform.position.x + passingGroundCol.offset.x,
                                          passingGroundCol.transform.position.y + passingGroundCol.offset.y, 0),
                                boxCol.size);
            }
        }
        
    }
}
