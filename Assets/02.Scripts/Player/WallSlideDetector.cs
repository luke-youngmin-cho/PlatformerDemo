using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSlideDetector : MonoBehaviour
{
    public bool isDetected;
    public LayerMask layer;
    public Vector2 top,bottom;
    Rigidbody2D rb;
    PlayerStateMachineManager controller;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<PlayerStateMachineManager>();
    }
    void Update()
    {
        bool topDetected = Physics2D.OverlapCircle(rb.position + new Vector2(controller.direction * top.x, top.y),0.01f,layer);
        bool bottomDetected = Physics2D.OverlapCircle(rb.position + new Vector2(controller.direction * bottom.x, bottom.y), 0.01f, layer);
        isDetected = topDetected && bottomDetected;
    }
    private void OnDrawGizmosSelected()
    {
        if (rb == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(new Vector3(controller.direction * top.x + rb.position.x, rb.position.y + top.y, 0), 0.01f);
        Gizmos.DrawSphere(new Vector3(controller.direction * bottom.x + rb.position.x, rb.position.y - bottom.y, 0), 0.01f);
    }
}
