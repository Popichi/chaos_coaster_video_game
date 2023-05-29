using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentDefendBots: Agent
{

    public GameObject visual;
    Rigidbody rb;
    public Transform[] jets;
    public Quaternion targetRotation;
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
                u.startSpeedMultiplier  = a[i]* startParticleSpeed;

                //AddReward(-0.1f);

                forceSum += v.up * manager.legStrenght*a[i];
                posSum += jets[i].position;
                n++;
            }
          
            
            
        }
        if (n > 0)
        {
            forceSum = deltaVector3(forceSum, 2f);
            posSum = deltaVector3(posSum, 0.05f);
            rb.AddForceAtPosition(forceSum, posSum / n);
        }
        
    
    }
    public Vector3 deltaVector3(Vector3 v, float delta)
    {
        if(v.x < delta && v.x > -delta)
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

    float toDegree = 57.2f;
    public float akkumulation;
    public float maxSpeed = 100f;
    public Transform Target;
    Vector3 lastAng;
    Vector3 lastAng2;
    Vector3 lastVel;
    Vector3 lastVel2;
    DefendBotsManager manager;
    public override void CollectObservations(VectorSensor sensor)
    {
        //
        sensor.AddObservation(targetLookDirection.normalized);
        var cubeForward =  (targetPosition - transform.localPosition).normalized;
        sensor.AddObservation(Quaternion.FromToRotation(targetLookDirection, transform.forward).normalized);
        //velocity we want to match
        var velGoal = cubeForward * goalSpeed;
  
        var avgVel = rb.velocity;

        sensor.AddObservation(rb.angularVelocity / rb.maxAngularVelocity);
        sensor.AddObservation(transform.localRotation.normalized);

        //Add pos of target relative to orientation cube
        sensor.AddObservation((targetPosition - transform.localPosition)/(manager.playFieldSize));
        sensor.AddObservation(targetPosition / (manager.playFieldSize / 2));
        sensor.AddObservation(transform.localPosition / (manager.playFieldSize / 2));
        sensor.AddObservation(lastVel2 / manager.maxVelocity);
        sensor.AddObservation(lastAng2 / rb.maxAngularVelocity);
        Vector3 angAcc = ((lastAng - lastAng2)) / (rb.maxAngularVelocity);
        //switch values
        Vector3 tmp = lastAng2;
        lastAng2 = lastAng;
        lastAng = tmp;
        sensor.AddObservation(angAcc);
        //----------------------
        Vector3 acc = ((lastVel - lastVel2)) / (manager.maxVelocity);
        //switch values
        tmp = lastVel2;
        lastVel2 = lastVel;
        lastVel = tmp;
        sensor.AddObservation(acc);
        //----------------------

        int there = -1;
        float dot = (Quaternion.Dot(targetRotation, transform.rotation) + 1) * 0.5f;
        if (1 - dot < delta)
        {
      
            there = 1;
        }
        


    }
    public int clockwise;
    
    public float newAbstand;
    public float delta = 5f;

    
    Quaternion lastRotation;
    Quaternion newRotation;
    public float goalSpeed;
    //-------------------------------
    
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var a = actionsOut.DiscreteActions;
        for(int i = 0; i< a.Length; i++)
        {
            a[i] = 0;
        }
        if (Input.GetKey("w"))
        {
            a[0] = 1;
            a[1] = 1;
            a[2] = 1;
            a[3] = 1;

        }

        if (Input.GetKey("d"))
        {
            a[4] = 1;
            a[5] = 1;
            a[6] = 1;
            a[7] = 1;
        }
        if (Input.GetKey("a"))
        {
            a[6] = 1;
            
            a[4] = 1;
            
        }




    }
    float nowDistance;
    float nowVelocity;
    float nowDot;
    float nowVelocityDot;
    private void FixedUpdate()
    {
       
        //AddReward(0.2f);
        targetPosition = Target.localPosition;
        Debug.DrawRay(transform.position, targetLookDirection, Color.red);
        nowVelocity = rb.velocity.magnitude;
        nowDistance = Vector3.Distance(targetPosition, transform.localPosition);
        Vector3 to = Vector3.Normalize(targetPosition - transform.localPosition);
        nowVelocityDot = (Vector3.Dot(to, rb.velocity.normalized) + 1) / 2f;
        float h = rb.velocity.magnitude;
        if (h > manager.maxVelocity)
        {
            rb.velocity = (rb.velocity / h) * manager.maxVelocity; 
        }
        
        
        if (up)
        {
            nowDot = (Vector3.Dot(transform.up, targetLookDirection) + 1) / 2f;
        }
        else
        {
            nowDot = (Vector3.Dot(transform.forward, targetLookDirection) + 1) / 2f;
        }
        
        delta = manager.delta;
        float reward = 1;
        float notSpinReward = 1 - (rb.angularVelocity.magnitude / rb.maxAngularVelocity);
        //reward *= notSpinReward;
        //AddReward((nowDot - lastDot + 1)/2f);
        //reward *= (nowDot - lastDot + 1) / 2f;
        if (to.Equals(Vector3.zero))
        {
           // reward *= 1;
        }
        else
        {
            //reward *= (nowVelocityDot);
        }
        
        if (nowDot > 1 - manager.dotDelta)
        {
            //AddReward(1);
            
        }


        reward *= 1 - (nowDistance / maxPlayFieldSize);
        reward *= nowDot;
        AddReward(reward);
        var directionToTarget = Vector3.Normalize(targetPosition - transform.localPosition);
        if (nowDistance < delta )
        {

        }

        else
        {   

            //reward *= (nowVelocityDot);
            if (nowVelocityDot > 1 - manager.dotDelta)
            {
                //AddReward(1f);
            }
            //AddReward(1 - (Vector3.Distance(directionToTarget * goalSpeed, rb.velocity)) / manager.maxGoalSpeed);
            //reward *= 1 - (Vector3.Distance(directionToTarget * goalSpeed, rb.velocity)) / (manager.maxGoalSpeed * 2);
        }

       
        float nowAngularSpeed = rb.angularVelocity.magnitude;
       //reward *= (1 - nowAngularSpeed / rb.maxAngularVelocity);
        
        lastAbstand = newAbstand;
        lastDistance = nowDistance;
        lastVelocityDot = nowVelocityDot;
        lastAng = rb.angularVelocity;
        lastVel = rb.velocity;
        // lastRotation = newRotation;
        lastDot = nowDot;
    }
    public float GetMatchingVelocityReward(Vector3 velocityGoal, Vector3 actualVelocity, float targetVelocity1)
    {
        //distance between our actual velocity and goal velocity
        var velDeltaMagnitude = Mathf.Clamp(Vector3.Distance(actualVelocity, velocityGoal), 0, targetVelocity1);

        //return the value on a declining sigmoid shaped curve that decays from 1 to 0
        //This reward will approach 1 if it matches perfectly and approach zero as it deviates
        return Mathf.Pow(1 - Mathf.Pow(velDeltaMagnitude / targetVelocity1, 2), 2);
    }
    public float lastAbstand;
    
    float sigmoid(float value, float max)
    {
        float v = Mathf.Pow(1 - Mathf.Pow(value /max, 2), 2);
        return v;
    }
    float lastDot;
    Vector3 lastVelocity;
    //Random Variables-----------------
    Vector3 targetPosition;
    float targetVelocity;
    public Vector3 targetLookDirection;
    public Vector3 targetVelocityVector;
    //---------------------------------------
    public float randomPosition;
    public float randomVelocity;
    public float randomAngularVelocity;
    public bool randomRotation;
    void setInit()
    {
        transform.localPosition = new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f) * manager.targetSpawnSize * randomPosition;
        if (randomRotation)
        {
            transform.rotation = Random.rotation;
        }
        else
        {
            transform.rotation = Quaternion.identity;
        }
       
        rb.velocity = new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f) * randomVelocity;
        rb.angularVelocity = new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f) * rb.maxAngularVelocity*randomAngularVelocity;
        
    }
    public bool up;
    void newPoint()
    {
        var k = Random.insideUnitCircle.normalized;
        if (up)
        {
            targetLookDirection = new Vector3(0, 1, 0);
            //if(targetLookDirection)
            targetRotation = Quaternion.LookRotation(Vector3.up, Vector3.up);
        }
        else
        {
            targetLookDirection = new Vector3(k.x, 0, k.y);
            //if(targetLookDirection)
            targetRotation = Quaternion.LookRotation(targetLookDirection, Vector3.up);
        }
        
        targetPosition = (new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f)) * manager.targetSpawnSize;
        Target.localPosition = targetPosition;
        goalSpeed = Random.value * (manager.maxGoalSpeed) + 0.1f;
        lastDot = (Vector3.Dot(transform.forward, targetLookDirection) + 1) / 2f;
        targetVelocityVector = Random.rotation * Vector3.forward;
    }

    public float maxForce;
    public int maxDuration;
    List<Vector3> randomForces;
    public void initCalculateRandomForces()
    {
        int steps = 1000;
        int currentStep = 0;
        while(currentStep < steps)
        {
            int r = Random.Range(0, maxDuration);
            if(currentStep + r > steps)
            {
                r = steps - currentStep;

            }
            Vector3 force = (new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f)) * maxForce;
            for(int i = 0; i < r; ++i)
            {
                randomForces.Add(force);
            }
            currentStep += r;
        }
    }
    public void addRandomForce()
    {

    }

    float lastDistance;
    float lastVelocityDot;
    public int numberMinObstacles;
    public int numberMaxObstacles;
    public GameObject barricadeObject;
    public float barricadeObjectSize;
    List<GameObject> barricades;
    
    public void rearrangeBarricadeObjects()
    {
        int number = Random.Range(numberMinObstacles, numberMaxObstacles + 1);
        int rest = numberMaxObstacles - number;
        for (int j = 0; j < number; ++j)
        {
            Vector3 pos = (new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f)) * manager.playFieldSize;
            while (Vector3.Distance(pos, rb.transform.position) < barricadeObjectSize + 2 || Vector3.Distance(pos, targetPosition) < barricadeObjectSize + 2)
            {
                pos = (new Vector3(Random.value - 0.5f, Random.value - 0.5f, Random.value - 0.5f)) * manager.playFieldSize;

            }
            barricades[j].transform.localPosition = pos;
        }
        for (int j = number; j < rest; ++j)
        {
            float t = manager.playFieldSize / 2;
            Vector3 pos = new Vector3(t, t, t);

            barricades[j].transform.localPosition = pos;
        }
    }
    Vector3 lastPositionError;
    Quaternion lastRotationError;
    public override void OnEpisodeBegin()
    {

        
  
        int i = (int)Academy.Instance.EnvironmentParameters.GetWithDefault("lecture", 0.1f);
       




        switch (i)
        {
            case 0:
                //setInit();
                delta = manager.delta;
                newPoint();
                //-----------------Baricades-----------
                rearrangeBarricadeObjects();
                //-------------------------------------------------
                lastPositionError = (targetPosition - transform.position) / manager.playFieldSize;
                lastRotationError = Quaternion.FromToRotation(targetLookDirection, transform.forward).normalized;
                lastRotation = Quaternion.identity;
                lastAng = rb.angularVelocity;
                lastAng2 = Vector3.zero;
                lastVel = rb.velocity;
                lastVel2 = Vector3.zero;
                break;
            case 1:
                
                rb.velocity = Vector3.zero;
                rb.angularVelocity = new Vector3(Random.Range(manager.min, manager.max), 0, 0);
                delta = 2;
                rb.rotation.SetEulerAngles(Random.Range(-180, 180), 0, 0);
                targetRotation = Quaternion.Euler(Random.Range(-180, 180), 0, 0);
                lastRotation = Quaternion.FromToRotation(transform.rotation.eulerAngles, targetRotation.eulerAngles);


                break;
            case 2:
                rb.velocity = Vector3.zero;
                rb.angularVelocity = new Vector3(Random.Range(0, 0), 0, 0);
                delta = 2;
                rb.rotation.SetEulerAngles(Random.Range(-180, 180), 0, 0);
                targetRotation = Quaternion.Euler(Random.Range(-180, 180), 0, 0);
                break;
            case 3:
                rb.velocity = Vector3.zero;
                rb.angularVelocity = new Vector3(Random.Range(0, 0), 0, 0);
                delta = 2;
                rb.rotation.SetEulerAngles(Random.Range(-180, 180), 0, 0);
                targetRotation = Quaternion.Euler(Random.Range(-180, 180), 0, 0);
                break;
        }
        visual.transform.localPosition = targetPosition;
        lastDistance = Vector3.Distance(targetPosition, transform.localPosition);
        
        lastDot = (Vector3.Dot(transform.forward, targetLookDirection) + 1) / 2f;
        lastVelocity = rb.velocity;
        Vector3 to = Vector3.Normalize(targetPosition - transform.localPosition);
        lastVelocityDot = (Vector3.Dot(to, rb.velocity.normalized) + 1) / 2f;
        //targetRotation = -160;
    }
    // Start is called before the first frame update
    float maxPlayFieldSize;
    public ParticleSystem[] particleSystems;
    float startParticleSpeed;
    void Start()
    {
        particleSystems = GetComponentsInChildren<ParticleSystem>();
        if(particleSystems != null)
        {
            startParticleSpeed = particleSystems[0].startSpeed;
        }

        manager = FindObjectOfType<DefendBotsManager>();
        List<Transform> t = new List<Transform>();
        foreach (Transform tr in transform)
        {
            if (tr.gameObject.active == true)
            {
                t.Add(tr);
            }
            
        }
        //Init Barricades----------------------
        barricades = new List<GameObject>();
        for (int j = 0; j < numberMaxObstacles; ++j)
        {
            GameObject g = GameObject.Instantiate(barricadeObject);
            barricades.Add(g);
            g.transform.parent = transform.parent;
        }
        //--------------------
        //jets = t.ToArray();
   
        rb = GetComponent<Rigidbody>();
        //rb.centerOfMass = new Vector3(0, -0.15f, 0);
        rb.maxAngularVelocity = 75;
        
        //maxPlayFieldSize = Mathf.Sqrt(3 * Mathf.Pow(manager.playFieldSize, 2));
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.gameObject.tag == "Obstacle")
        {
            AddReward(-Vector3.Magnitude(collision.impulse)/10f);
            setInit();
            EndEpisode();
        }
    }

    // Update is called once per frame

}
