using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PlayerController))]
public class PlayerEdgeDetector : MonoBehaviour
{
    bool top, bottom;
    public float topX, topY, bottomX, bottomY;
    bool detectingFallingEdge;
    bool detectingRisingEdge;
    [HideInInspector] public bool isDetected;
    [HideInInspector] public float targetPlayerPosY;

    Transform tr;
    Rigidbody2D rb;
    PlayerController controller;
    public LayerMask layer;
    private void Awake()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<PlayerController>();
    }
    private void Update()
    {
        top = Physics2D.OverlapCircle(new Vector2(tr.position.x + topX * controller.direction, tr.position.y + topY), 0.01f, layer);
        bottom = Physics2D.OverlapCircle(new Vector2(tr.position.x + bottomX * controller.direction, tr.position.y + bottomY),0.01f, layer);

        // when falling edge is detected
        if (detectingFallingEdge && top)
        {
            isDetected = true;
        }
        // when rising edge is detected
        else if (detectingRisingEdge && (top == false))
        {
            isDetected = true;
            targetPlayerPosY = rb.position.y;
        }
        else
            isDetected = false;

        // detecting falling edge
        if (bottom && (top == false) && rb.velocity.y < 0)
        {
            detectingFallingEdge = true;
            targetPlayerPosY = rb.position.y;
        }   
        else
            detectingFallingEdge = false;
        // detecting rising edge
        if (top && bottom && rb.velocity.y > 0)
            detectingRisingEdge = true;
        else
            detectingRisingEdge = false;
        

        /*if (bottom && top == false)
        {
            isDetected = true;
            targetPlayerPosY = tr.position.y;
        }   
        else
            isDetected = false;*/

        //Debug.Log($"Edge detection : {isDetected}");
    }
    private void OnDrawGizmosSelected()
    {
        if (controller == null) return;
        // top
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(transform.position.x + topX * controller.direction, transform.position.y+topY, 0f), 0.01f);
        // bottom
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(new Vector3(transform.position.x + bottomX * controller.direction, transform.position.y+ bottomY, 0f), 0.01f);
    }
}
