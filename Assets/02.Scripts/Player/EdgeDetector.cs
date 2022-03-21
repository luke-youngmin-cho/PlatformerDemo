using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Edge detection rising edge & falling edge
/// </summary>
[RequireComponent(typeof(PlayerStateMachineManager))]
public class EdgeDetector : MonoBehaviour
{   
    public float topX, topY, bottomX, bottomY;
    bool topOn, bottomOn;
    bool detectingFallingEdge;
    bool detectingRisingEdge;
    [HideInInspector] public bool isDetected;
    [HideInInspector] public Vector2 targetPlayerPos;

    Rigidbody2D rb;
    PlayerStateMachineManager controller;
    public LayerMask layer;


    //============================================================================
    //*************************** Private Methods ********************************
    //============================================================================

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<PlayerStateMachineManager>();
    }

    private void Update()
    {
        topOn = Physics2D.OverlapCircle(new Vector2(rb.position.x + topX * controller.direction, rb.position.y + topY), 0.01f, layer);
        bottomOn = Physics2D.OverlapCircle(new Vector2(rb.position.x + bottomX * controller.direction, rb.position.y + bottomY),0.01f, layer);

        // when falling edge is detected
        if (detectingFallingEdge && topOn)
        {
            isDetected = true;
        }
        // when rising edge is detected
        else if (detectingRisingEdge && (topOn == false))
        {
            isDetected = true;
            targetPlayerPos = new Vector2(rb.position.x + (topX * controller.direction/2), //compensation 
                                          rb.position.y);
        }
        else
            isDetected = false;

        // detecting falling edge
        if (bottomOn && (topOn == false) && rb.velocity.y < 0)
        {
            detectingFallingEdge = true;
            targetPlayerPos = new Vector2(rb.position.x + (topX * controller.direction / 2), //compensation
                                          rb.position.y);
        }   
        else
            detectingFallingEdge = false;

        // detecting rising edge
        if (topOn && bottomOn && rb.velocity.y > 0)
            detectingRisingEdge = true;
        else
            detectingRisingEdge = false;
        
    }

    private void OnDrawGizmosSelected()
    {
        if (rb == null) return;
        // top
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(rb.position.x + topX * controller.direction, rb.position.y+topY, 0f), 0.01f);
        // bottom
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(new Vector3(rb.position.x + bottomX * controller.direction, rb.position.y+ bottomY, 0f), 0.01f);
    }
}
