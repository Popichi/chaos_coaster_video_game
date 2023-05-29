using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Linq;
public class ARDDrone : Agent
{
    // Start is called before the first frame update
    ARDGunManager manager;
    void Init()
    {
    manager =    FindObjectOfType<ARDGunManager>();
    }
    public ParticleSystem[] particleSystems;
    float startParticleSpeed;

    public Rigidbody rb;
    float sigmoid(float value, float max)
    {
        float v = Mathf.Pow(1 - Mathf.Pow(value / max, 2), 2);
        return v;
    }
    void Start()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>();
        if (particleSystems != null)
        {
            startParticleSpeed = particleSystems[0].startSpeed;
        }
        rb = GetComponent<Rigidbody>();
        manager.SubscribeDrone(this);
    }
    public Vector3 deltaVector3(Vector3 v, float delta)
    {
        if (v.x < delta && v.x > -delta)
        {
            v.x = 0;
        }
        if (v.y < delta && v.y > -delta)
        {
            v.y = 0;
        }
        if (v.z < delta && v.z > -delta)
        {
            v.z = 0;
        }
        return v;

    }

    // Update is called once per frame

    public Transform[] jets;
    public void getObservations(Transform t, VectorSensor s )
    {
        s.AddObservation(t.InverseTransformDirection(rb.velocity));
        s.AddObservation(t.InverseTransformDirection(rb.angularVelocity));
        s.AddObservation(t.InverseTransformPoint(transform.position - t.position));
        s.AddObservation(t.InverseTransformDirection(transform.forward));
        s.AddObservation(t.InverseTransformDirection(transform.up));
    }
    int numberOfOtherAgents;
    public override void CollectObservations(VectorSensor sensor)
    {
        //global pos
        sensor.AddObservation(manager.root.transform.InverseTransformPoint(transform.position));
        //"global" rotation
        sensor.AddObservation(manager.root.transform.InverseTransformDirection(transform.forward));
        sensor.AddObservation(manager.root.transform.InverseTransformDirection(transform.up));
        //gravitation
        sensor.AddObservation(transform.InverseTransformDirection(Vector3.down));
        //relative to gaol
        sensor.AddObservation(transform.InverseTransformPoint(manager.target.transform.position - transform.position));

        sensor.AddObservation(transform.InverseTransformDirection(rb.velocity));
        sensor.AddObservation(transform.InverseTransformDirection(rb.angularVelocity));
        List<ARDDrone> drones = new List<ARDDrone>(manager.drones);

        drones = drones.OrderBy(t => Vector3.Distance(t.transform.position, transform.position)).ToList();
        for (int i = 0; i < manager.drones.Count; ++i)
        {
            if(drones[i] != this)
            {
                drones[i].getObservations(transform, sensor); 
            }
            
        }
        //for maintaining flexible size
        sensor.AddObservation(drones.Count);

    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        Transform v;
        var a = actions.ContinuousActions;
        Vector3 forceSum = Vector3.zero;
        Vector3 posSum = Vector3.zero;
        int n = 0;

        for (int i = 0; i < a.Length; i++)
        {
            v = jets[i];
            //a[i] = (a[i]+1)/ 2;
            if (a[i] > 0)
            {
                var u = particleSystems[i].main;
                u.startSpeedMultiplier = a[i] * startParticleSpeed;

                //AddReward(-0.1f);

                forceSum += v.up * manager.turbineStrength * a[i];
                posSum += jets[i].position;
                n++;
            }

            Debug.Log(forceSum);


        }
        if (n > 0)
        {
            forceSum = deltaVector3(forceSum, 2f);
            posSum = deltaVector3(posSum, 0.05f);
            rb.AddForceAtPosition(forceSum, posSum / n);
        }


    }
    

   
    public override void OnEpisodeBegin()
    {
        if (manager.isTraining)
        {
            rb.velocity = manager.RandomVectorWithLimitedAngle() * manager.GetStartVelocity();
            rb.angularVelocity = Random.rotation * Vector3.up * manager.startAngularVel;
        }


    }
}
