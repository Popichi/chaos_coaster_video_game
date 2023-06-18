using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
public class GetMovement : MonoBehaviour
{

    public float  mag;
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
       maRot.AddSample( new Vector3(calcAngle(Vector3.right, deltaRotation), calcAngle(Vector3.up, deltaRotation), calcAngle(Vector3.forward, deltaRotation)));
        lastRotation = transform.rotation;

       maSpeed.AddSample((lastPos - transform.position)/ Time.deltaTime);
        lastPos = transform.position;

        mag = math.length(maSpeed.GetAverage());
    }

    public Vector3 GetRotationSpeed()
    {
        rotationSpeed = maRot.GetAverage();
        if (!ContainsNaNOrInfinity(rotationSpeed))
        {
            return rotationSpeed;

        }
        else
        {
            return Vector3.zero;
        }
        
    }
    //chatgpt4 code
    public class MovingAverage
    {
        private Queue<float3> samples;
        private int windowSize;
        private float3 sum;

        public MovingAverage(int windowSize)
        {
            this.windowSize = windowSize;
            this.samples = new Queue<float3>(windowSize);
            this.sum = 0;
        }

        public void AddSample(float3 sample)
        {
            sum += sample;
            samples.Enqueue(sample);

            if (samples.Count > windowSize)
            {
                sum -= samples.Dequeue();
            }
        }

        public float3 GetAverage()
        {
            return sum / samples.Count;
        }
    }
    public  bool ContainsNaNOrInfinity( Vector3 vector)
    {
        return float.IsNaN(vector.x) || float.IsNaN(vector.y) || float.IsNaN(vector.z)
            || float.IsInfinity(vector.x) || float.IsInfinity(vector.y) || float.IsInfinity(vector.z);
    }
    public Vector3 GetSpeed()
    {
        if(maSpeed != null)
        {
            speed = maSpeed.GetAverage();
            if (!ContainsNaNOrInfinity(speed))
            {
                return speed;
            }
            else
            {
                return Vector3.zero;
            }
        }

        return Vector3.zero;

    }
    MovingAverage maRot;
    MovingAverage maSpeed;
    // Start is called before the first frame update
    void Awake()
    {
        lastPos = transform.position;
        lastRotation = transform.rotation;
        maRot = new MovingAverage(10);  // window size of 10
        maSpeed = new MovingAverage(10);
        // Then, when you have a new sample:

    }

    // Update is called once per frame
 
}
