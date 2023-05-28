using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetMovement : MonoBehaviour
{
    Vector3 lastPos = Vector3.zero;
    public Vector3 speed=Vector3.zero;

    private Quaternion lastRotation = Quaternion.identity;
    public Vector3 rotationSpeed = Vector3.zero;

    float calcAngle(Vector3 axis, Quaternion delta)
    {
        float angleInDegrees;
        Vector3 rotationAxis;
        delta.ToAngleAxis(out angleInDegrees, out rotationAxis);
        return angleInDegrees / Time.deltaTime;

    }
    void Update()
    {
        Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(lastRotation);
        rotationSpeed = new Vector3(calcAngle(Vector3.right, deltaRotation), calcAngle(Vector3.up, deltaRotation), calcAngle(Vector3.forward, deltaRotation));
        lastRotation = transform.rotation;

        speed = (lastPos - transform.position)/ Time.deltaTime;
        lastPos = transform.position;
    }

    public Vector3 GetRotationSpeed()
    {
        return rotationSpeed;
    }
    public Vector3 GetSpeed()
    {
        return speed;
    }
    // Start is called before the first frame update
    void Awake()
    {
        lastPos = transform.position;
        lastRotation = transform.rotation;
    }

    // Update is called once per frame
 
}
