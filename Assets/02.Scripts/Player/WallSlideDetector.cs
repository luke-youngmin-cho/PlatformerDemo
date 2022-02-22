using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallSlideDetector : MonoBehaviour
{
    public bool isDetected;
    float detectionOffset;
    Rigidbody2D rb;
    PlayerController controller;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<PlayerController>();
        detectionOffset = GetComponent<CapsuleCollider2D>().size.y / 4;
    }
    void Update()
    {
        isDetected = Physics2D.OverlapCircle(rb.position + new Vector2(detectionOffset * controller.direction, 0),0.01f);
    }
    private void OnDrawGizmosSelected()
    {
        if (rb == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(new Vector3(detectionOffset * controller.direction + rb.position.x, rb.position.y, 0), 0.01f);
    }
}
