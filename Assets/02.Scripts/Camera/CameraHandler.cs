using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public Vector3 offset;    
    [Range(1,10)]
    public float smoothness;
    private Transform tr;

    Camera cam;
    public BoxCollider2D boundingShape;
    float boundingShapeXMin;
    float boundingShapeXMax;
    float boundingShapeYMin;
    float boundingShapeYMax;
    private void Awake()
    {
        cam = GetComponent<Camera>();
        tr = GetComponent<Transform>();
        boundingShapeXMin = boundingShape.transform.position.x + boundingShape.offset.x - boundingShape.size.x / 2;
        boundingShapeXMax = boundingShape.transform.position.x + boundingShape.offset.x + boundingShape.size.x / 2;
        boundingShapeYMin = boundingShape.transform.position.y + boundingShape.offset.y - boundingShape.size.y / 2;
        boundingShapeYMax = boundingShape.transform.position.y + boundingShape.offset.y + boundingShape.size.y / 2;
    }
    private void FixedUpdate()
    {
        Follow();
    }
    void Follow()
    {
        if (Player.instance == null) return;

        Transform target = Player.instance.transform;

        Vector3 targetPos = new Vector3(target.position.x, target.position.y, tr.position.z) + offset;
        Vector3 smoothPos = Vector3.Lerp(tr.position, targetPos, smoothness * Time.fixedDeltaTime);

        Vector2 camWorldPosLeftBottom = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector2 camWorldPosRightTop = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));
        Vector2 camWorldPosSize = new Vector2(camWorldPosRightTop.x - camWorldPosLeftBottom.x,
                                              camWorldPosRightTop.y - camWorldPosLeftBottom.y);

        if (smoothPos.x - camWorldPosSize.x / 2 < boundingShapeXMin)
            smoothPos.x = boundingShapeXMin + camWorldPosSize.x / 2;
        else if (smoothPos.x + camWorldPosSize.x / 2 > boundingShapeXMax)
            smoothPos.x = boundingShapeXMax - camWorldPosSize.x / 2;

        if (smoothPos.y - camWorldPosSize.y / 2 < boundingShapeYMin)
            smoothPos.y = boundingShapeYMin + camWorldPosSize.y / 2;
        else if (smoothPos.y + camWorldPosSize.y / 2 > boundingShapeYMax)
            smoothPos.y = boundingShapeYMax - camWorldPosSize.y / 2;

        tr.position = smoothPos;
    }
    private void OnDrawGizmosSelected()
    {
        Camera cam = GetComponent<Camera>();
        Vector3 p = cam.ViewportToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 q = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));
        Vector3 center = cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, cam.nearClipPlane));
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(center, new Vector2(q.x - p.x, q.y - p.y));

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(boundingShape.transform.position + (Vector3)boundingShape.offset, (boundingShape.size));
    }
}
