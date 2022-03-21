using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Check up / down ladder is possible
/// </summary>
public class LadderDetector : MonoBehaviour
{
    [HideInInspector] public bool isGoUpPossible;
    [HideInInspector] public bool isGoDownPossible;
    [HideInInspector] public bool isAtFeet;
    [HideInInspector] public Vector2 ladderTopPos;
    float ladderPosX;
    float playerSizeY;
    float colliderOffsetY;
    public LayerMask layer;
    float playerLadderMoveYOffset = 0.25f;
    float playerLadderAtFeetStartYOffset = 0.33f;
    float playerLadderStartYOffset = 0.03f;

    // Components
    Rigidbody2D rb;


    //============================================================================
    //*************************** Public Methods *********************************
    //============================================================================

    public Vector2 GetLadderStartPosWhenIsAtFeet()
    {
        Vector2 startPos = ladderTopPos - new Vector2(0f, playerSizeY * playerLadderAtFeetStartYOffset);
        return startPos;
    }

    public Vector2 GetLadderStartPosWhenIsAboveHead()
    {
        Vector2 startPos = new Vector2(ladderPosX, rb.position.y);
        return startPos;
    }

    public Vector2 GetLadderstartPosOnGround()
    {
        Vector2 startPos = new Vector2(ladderPosX, rb.position.y + playerLadderStartYOffset);
        return startPos;
    }


    //============================================================================
    //*************************** Private Methods ********************************
    //============================================================================

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerSizeY = GetComponent<CapsuleCollider2D>().size.y;
        colliderOffsetY = GetComponent<CapsuleCollider2D>().offset.y;
    }

    void Update()
    {
        // at ground or above
        Collider2D ladderCol = Physics2D.OverlapCircle(new Vector2(rb.position.x, rb.position.y + colliderOffsetY + (playerSizeY * playerLadderMoveYOffset)), 0.01f, layer);
        if (ladderCol != null)
        {
            BoxCollider2D ladderBoxCol = (BoxCollider2D)ladderCol;
            ladderPosX = ladderBoxCol.transform.position.x + ladderBoxCol.offset.x;
            ladderTopPos = new Vector2(ladderPosX,
                                       ladderBoxCol.transform.position.y + ladderBoxCol.offset.y + ladderBoxCol.size.y/2);
            
            isGoUpPossible = true;
        }
        else
        {
            isGoUpPossible = false;
        }
        isGoDownPossible = Physics2D.OverlapCircle(new Vector2(rb.position.x, rb.position.y + colliderOffsetY - (playerSizeY * playerLadderMoveYOffset)), 0.01f, layer);

        // at feet
        if (isGoUpPossible == false && isGoDownPossible == false)
        {
            ladderCol = Physics2D.OverlapCircle(new Vector2(rb.position.x, rb.position.y + colliderOffsetY - (playerSizeY * (1 + playerLadderMoveYOffset))), 0.01f, layer);
            if (ladderCol != null)
            {
                BoxCollider2D ladderBoxCol = (BoxCollider2D)ladderCol;
                ladderPosX = ladderBoxCol.transform.position.x + ladderBoxCol.offset.x;
                ladderTopPos = new Vector2(ladderPosX,
                                           ladderBoxCol.transform.position.y + ladderBoxCol.offset.y + ladderBoxCol.size.y / 2);
                isAtFeet = true;
            }
            else
                isAtFeet = false;
        }
        else
            isAtFeet = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (rb == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(new Vector3(rb.position.x, rb.position.y + colliderOffsetY + (playerSizeY * playerLadderMoveYOffset), 0f), 0.01f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(new Vector3(rb.position.x, rb.position.y + colliderOffsetY - (playerSizeY * playerLadderMoveYOffset), 0f), 0.01f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(new Vector3(rb.position.x, rb.position.y + colliderOffsetY - (playerSizeY * (1+ playerLadderMoveYOffset)), 0f), 0.01f);
    }
}
