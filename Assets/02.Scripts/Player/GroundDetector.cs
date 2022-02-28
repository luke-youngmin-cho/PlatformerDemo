using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetector : MonoBehaviour
{
    Rigidbody2D rb;
    CapsuleCollider2D col;
    Vector2 center;
    [HideInInspector] public Vector2 size;
    [SerializeField] LayerMask groundLayer;
    private Collider2D currentGroundCol;
    private Collider2D passingGroundCol;

    public float passingGroundColCenterY;
    public bool isDetected;
    public bool doDownJumpCheck;
    public bool downJumpAvailable;
    public float downJumpCheckHeight;

    public bool isIgnoringGround;
    
    private void Awake()
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
        currentGroundCol = Physics2D.OverlapBox(center, size, 0, groundLayer);
        if (currentGroundCol == null)
            isDetected = false;
        else if (currentGroundCol != passingGroundCol)
            isDetected = true;

        if (doDownJumpCheck)
        {
            int groundNum = Physics2D.OverlapBoxAll(new Vector2(center.x, center.y - downJumpCheckHeight / 2),
                                                    new Vector2(size.x, downJumpCheckHeight),
                                                    0,
                                                    groundLayer).Length;
            downJumpAvailable = groundNum > 1 ? true : false;
            //Debug.Log(groundNum);
        }
        else
            downJumpAvailable = false;
    }
    public void IgnoreCurrentGroundUntilPassedIt()
    {
        passingGroundCol = currentGroundCol;
        StartCoroutine(E_IgnoreCurrentGroundUntilPassedIt(currentGroundCol));
    }
    IEnumerator E_IgnoreCurrentGroundUntilPassedIt(Collider2D passingGround)
    {
        passingGroundCol = passingGround;
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Ground"), true);
        isIgnoringGround = true;
        yield return new WaitUntil(() =>
        {
            bool isPassed = false;
            passingGroundColCenterY = passingGround.transform.position.y + col.offset.y;
            if (passingGround == null ||
              (rb.position.y + col.offset.y + col.size.y / 2 < passingGroundColCenterY - size.y) ||
              (rb.position.y + col.offset.y - col.size.y / 2 < passingGroundColCenterY + size.y))
            {
                isPassed = true;
            }
            return isPassed;
        });
        Debug.Log("Ignoring ground finished");
        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("Ground"), false);
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
