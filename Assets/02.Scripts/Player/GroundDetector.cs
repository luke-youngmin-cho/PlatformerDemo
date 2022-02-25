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
    public bool isGrounded;
    public bool doDownJumpCheck;
    public bool downJumpAvailable;
    public float downJumpCheckHeight;
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
        isGrounded = Physics2D.OverlapBox(center, size, 0, groundLayer);
        if (doDownJumpCheck)
        {
            int groundNum = Physics2D.OverlapBoxAll(new Vector2(center.x, center.y - downJumpCheckHeight / 2),
                                                    new Vector2(size.x, downJumpCheckHeight),
                                                    0,
                                                    groundLayer).Length;
            downJumpAvailable = groundNum > 1 ? true : false;
            Debug.Log(groundNum);
        }
        else
            downJumpAvailable = false;
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

    }
}
