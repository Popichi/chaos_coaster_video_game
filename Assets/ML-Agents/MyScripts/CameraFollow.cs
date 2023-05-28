using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform target1;
    public Transform target2;
    public float smoothSpeed = 0.125f;
    public float minDistance = 10f;
    public float maxDistance = 20f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void FixedUpdate()
    {
        Vector3 midpoint = (target1.position + target2.position) / 2f;

        float distance = Vector3.Distance(target1.position, target2.position);
        float cameraDistance = Mathf.Clamp(distance, minDistance, maxDistance);
        float frustumHeight = 2.0f * cameraDistance * Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float frustumWidth = frustumHeight * cam.aspect;

        Vector3 offset = new Vector3(0, frustumHeight, -cameraDistance);
        Vector3 desiredPosition = midpoint + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(midpoint);
    }

}




