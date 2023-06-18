using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgentsExamples;
using Unity.MLAgents.Sensors;
using BodyPart = Unity.MLAgentsExamples.BodyPart;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Animations;
public enum EnemyState
{
    training,
    playing,
    dead
}
public interface IState
{
    public EnemyState GetState();
}
public class SpiderAgent : Agent, IReward, Iid, IState
{
    public EnemyState state;
    public bool isTraining = true;
    public GameObject mainBody;
    public GameObject mainHead;
    const float minWSpeed = 0.1f;
    const float maxWSpeed = 20;
    [Header("Walk Speed")]
    [Range(minWSpeed, maxWSpeed)]
    [SerializeField]
    //The walking speed to try and achieve
    private float m_TargetWalkingSpeed = maxWSpeed;

    public float MTargetWalkingSpeed // property
    {
        get { return m_TargetWalkingSpeed; }
        set { m_TargetWalkingSpeed = Mathf.Clamp(value, minWSpeed, maxWSpeed); }
    }

    const float m_maxWalkingSpeed = maxWSpeed; //The max walking speed

    //Should the agent sample a new goal velocity each episode?
    //If true, walkSpeed will be randomly set between zero and m_maxWalkingSpeed in OnEpisodeBegin()
    //If false, the goal velocity will be walkingSpeed
    public bool randomizeWalkSpeedEachEpisode;

    //The direction an agent will walk during training.
    private Vector3 m_WorldDirToWalk = Vector3.right;

    [Header("Target To Walk Towards")] public Transform target; //Target the agent will walk towards during training.

    public List<IsBodyPart> bodyparts;
    public Transform rootPrefab;
    //This will be used as a stabilized model space reference point for observations
    //Because ragdolls can move erratically during training, using a stabilized reference transform improves learning
    OrientationCubeController m_OrientationCube;

    //The indicator graphic gameobject that points towards the target
    DirectionIndicator m_DirectionIndicator;
    JointDriveController m_JdController;
    EnvironmentParameters m_ResetParams;
    public static int id = 0;
    public int myID;
    public Transform Player;
    public WaveSpawner waveSpawner;
    public override void Initialize()
    {
        
        if(state == EnemyState.playing)
        {
            waveSpawner = FindAnyObjectByType<WaveSpawner>();
            getSpeed = waveSpawner.getMovement;
            trainingGround = waveSpawner.TrainingGround.gameObject;
            movingPlattform = waveSpawner.mapMoving.gameObject;

        }


        mainBodyRigidBody = mainBody.GetComponent<Rigidbody>();
        if (state == EnemyState.training)
        {
            covers = new List<GameObject>();
            for (int i = 0; i < numberCovers; ++i)
            {
                covers.Add(Instantiate(coverPrefab));
                covers[i].transform.parent = trainingGround.transform;
            }
        }
        if(state == EnemyState.playing)
        {
            gameObject.GetComponentInChildren<ShootRocket>().enabled = true;
            MaxStep = 0;
            Player = FindFirstObjectByType<PlayerController>().transform;
            PositionConstraint positionConstraint = target.gameObject.AddComponent<PositionConstraint>(); // Add the PositionConstraint component

            ConstraintSource source = new ConstraintSource();
            source.sourceTransform = Player; // Use the sourceObject's transform as the source
            source.weight = 1.0f; // Set the weight

            positionConstraint.AddSource(source); // Add the source to the PositionConstraint
            positionConstraint.constraintActive = true; // Activate the constraint
            if (!debug)
                Destroy(target.GetComponent<Renderer>());
            Destroy(target.GetComponent<Collider>());
        }

        myID = id++;
        m_OrientationCube = rootPrefab.GetComponentInChildren<OrientationCubeController>();
        m_DirectionIndicator = rootPrefab.GetComponentInChildren<DirectionIndicator>();
        bodyparts = rootPrefab.GetComponentsInChildren<IsBodyPart>().ToList();
        //Setup each body part
        m_JdController = GetComponent<JointDriveController>();
        foreach(var a in bodyparts)
        {
            a.id = myID;
            m_JdController.SetupBodyPart(a.transform);
        }
       

        m_ResetParams = Academy.Instance.EnvironmentParameters;

        //   SetResetParameters();
    }

    /// <summary>
    /// Loop over body parts and reset them to initial conditions.
    /// </summary>
    /// 
    Transform rootRoot;

    public int numberCovers;
    public GameObject coverPrefab;
    List<GameObject> covers;
    void rotate2()
    {
        Quaternion rot = Quaternion.Euler(0, Random.value * 360, 0);//Random.rotation;
        rootRoot = rootPrefab.transform.parent;
        rootRoot.rotation *= rot;
        rootPrefab.transform.parent = null;
        rootRoot.rotation = Quaternion.identity;
        rootPrefab.parent = rootRoot;
    }
    void rotate()
    {
        Quaternion rot = Quaternion.Euler(movingPlattform.transform.up * 360);//Random.rotation;

        mainBody.transform.rotation *= rot;
        float off = 14;
        mainBody.transform.position += new Vector3(Random.value * off - off / 2, 0, Random.value * off - off / 2);

    }
    public void ResetPose()
    {
        foreach (var bodyPart in m_JdController.bodyPartsDict.Values)
        {
            bodyPart.Reset(bodyPart);
        }
    }
    public override void OnEpisodeBegin()
    {
        //test
        TargetController targetController = target.GetComponent<TargetController>();
        if (state == EnemyState.training)
        {
            for (int i = 0; i < numberCovers; ++i)
            {
                covers[i].active = true;
                Rigidbody r = covers[i].GetComponent<Rigidbody>();
                r.velocity = Vector3.zero;
                r.angularVelocity = Vector3.zero;
                r.transform.rotation = Quaternion.Euler(0, Random.value * 360, 0);
                covers[i].transform.position = targetController.getRandom.randomPosOnGrid(2);
            }
            targetController.MoveTargetToRandomPosition();
        }

        //Reset all of the body parts
        ResetPose();

        
        //Random start rotation to help generalize
        if (state == EnemyState.training)
        {

            rotate();
            //mainBody.transform.rotation = rot;
            foreach (var a in bodyparts)
            {
               //a.transform.rotation *= rot;
                //a.transform.position += Vector3.up; 
            }
        }
       

        UpdateOrientationObjects();

        //Set our goal walking speed
        MTargetWalkingSpeed =
            randomizeWalkSpeedEachEpisode ? Random.Range(0.1f, m_maxWalkingSpeed) : MTargetWalkingSpeed;

        SetResetParameters();

        nearestDistance = Vector3.Distance(mainBody.transform.position, target.transform.position);
        startDistance = nearestDistance;
        startY = Mathf.Abs(target.transform.position.y - mainBody.transform.position.y);
    }

    /// <summary>
    /// Add relevant information on each body part to observations.
    /// </summary>
    /// 
    public void AddRot(VectorSensor s, Transform t, bool w = true)
    {
        if(w == true)
        {
            s.AddObservation(t.forward);
            s.AddObservation(t.up);
        }
        else
        {
            s.AddObservation(t.localRotation *Vector3.forward);
            s.AddObservation(t.localRotation * Vector3.up);
        }
      
    }
    public bool debug = true;
    public void DebugReward(float f, string message = "")
    {
        if (debug)
        {
            message = " " + message + " ";
            Debug.Log("Reward:" + message + f);
        }
        AddReward(f);
        

    }
    public void AddRot(VectorSensor s, Quaternion q, bool w = true)
    {
        
        {
            s.AddObservation(q * Vector3.forward);
            s.AddObservation(q * Vector3.up);
        }

    }
    public GameObject trainingGround;
    public void CollectObservationBodyPart(BodyPart bp, VectorSensor sensor)
    {
        
        //GROUND CHECK
        sensor.AddObservation(bp.groundContact.touchingGround); // Is this bp touching the ground

        //Get velocities in the context of our orientation cube's space
        //Note: You can get these velocities in world space as well but it may not train as well.
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(bp.rb.velocity));
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(bp.rb.angularVelocity));

        //Get position relative to hips in the context of our orientation cube's space
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(bp.rb.position - mainBody.transform.position));
        //AddRot(sensor, bp.rb.transform.localRotation);
        sensor.AddObservation(bp.rb.transform.localRotation);
        sensor.AddObservation(bp.currentStrength / m_JdController.maxJointForceLimit);
            
        
    }

    /// <summary>
    /// Loop over body parts to add them to observation.
    /// </summary>
    /// 
    public Vector3 forward = new Vector3(0, 0,1);
    public Vector3 up = new Vector3(0, 1, 0);
    public Vector3 Forward(Transform t)
    {
        return t.TransformDirection(forward);
    }
 
    public Vector3 UP(Transform t)
    {
        return t.TransformDirection(up);
    }
    public GameObject movingPlattform;
    public override void CollectObservations(VectorSensor sensor)
    {
        var cubeForward = m_OrientationCube.transform.forward;

        //velocity we want to match
        var velGoal = cubeForward * MTargetWalkingSpeed;
        //ragdoll's avg vel
        var avgVel = GetAvgVelocity();

        //current ragdoll velocity. normalized
        sensor.AddObservation(Vector3.Distance(velGoal, avgVel));
        //avg body vel relative to cube
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(avgVel));
        //vel goal relative to cube
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(velGoal));

        //rotation deltas
        //AddRot(sensor, Quaternion.FromToRotation(Forward(mainBody.transform), cubeForward));
       // AddRot(sensor,Quaternion.FromToRotation(Forward(mainHead.transform), cubeForward));

        sensor.AddObservation((Quaternion.FromToRotation(Forward(mainBody.transform), cubeForward)));
        sensor.AddObservation(Quaternion.FromToRotation(Forward(mainHead.transform), cubeForward));

        //430 sa rot



        //Position of target position relative to cube
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformPoint(target.transform.position));
        sensor.AddObservation(trainingGround.transform.InverseTransformPoint(mainBody.transform.position));
        foreach (var bodyPart in m_JdController.bodyPartsList)
        {
            CollectObservationBodyPart(bodyPart, sensor);
        }
        //AddRot(sensor, mainBody.transform.rotation);
        sensor.AddObservation(mainBody.transform.rotation);
        sensor.AddObservation(trainingGround.transform.InverseTransformPoint(target.transform.position));
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(getSpeed.GetSpeed()));
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(getSpeed.GetRotationSpeed()));
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(Vector3.down));
        sensor.AddObservation((Quaternion.FromToRotation(movingPlattform.transform.forward, cubeForward)));
        sensor.AddObservation(Quaternion.FromToRotation(movingPlattform.transform.up, m_OrientationCube.transform.up));

    }
    public GetMovement getSpeed;
    public bool showOutput = false;
    public override void OnActionReceived(ActionBuffers actionBuffers)

    {
        var bpDict = m_JdController.bodyPartsDict;
        var i = -1;

        var continuousActions = actionBuffers.ContinuousActions;
        foreach (var a in bodyparts)
        {
            Vector3 v = new Vector3();
            bool b = false;
            if (a.x)
            {
                v.x = continuousActions[++i];
                b = true;

            }
                
            if (a.y)
            {
                v.y = continuousActions[++i];
                b = true;
            }
               
            if (a.z)
            {
                v.z = continuousActions[++i];
                b = true;
            }
                
            
            if(b == true)
            {
                bpDict[a.transform].SetJointTargetRotation(v);
                bpDict[a.transform].SetJointStrength(continuousActions[i]);
                //Debug.Log(continuousActions[i - 1]);
            }
            if(showOutput)
            Debug.Log(i);
        }
       

        //update joint strength settings
       
    }

    //Update OrientationCube and DirectionIndicator
    void UpdateOrientationObjects()
    {
        m_WorldDirToWalk = target.position - mainBody.transform.position;
        m_OrientationCube.UpdateOrientation(mainBody.transform, target, false);
        if (m_DirectionIndicator)
        {
            m_DirectionIndicator.MatchOrientation(m_OrientationCube.transform);
        }
    }
    public enum RewardMode
    {
        Gaze,
        Nearest,
        Ndistance,
        JustGetit,
        MaxVelocity,
        AvgVelocity

    }
    public RewardMode RewardFunction;
    public float nearestDistance = 0;
    float nearestY = 0;
    float startDistance;
    float startY;
    public float velocityBonus = 0.1f;
    public void reward()
    {
        if (RewardMode.Ndistance == RewardFunction)
        {
            if(startDistance - nearestDistance != 0)
            {
                //AddReward(startDistance - nearestDistance);
            }
            if (Mathf.Abs(startY - nearestY) * 3 != 0)
            {
                //AddReward(Mathf.Abs(startY - nearestY) * 3);
            }
           
        }
    }
    public bool backCounterActivated = true;
    public int onBackCounter = 0;
    public int maxFramesOnBack = 140;
    public float backForce = 3;
    Rigidbody mainBodyRigidBody;
    void FixedUpdate()
    {
        UpdateOrientationObjects();
        if(state == EnemyState.training)
        {
            for (int i = 0; i < numberCovers; ++i)
            {
                if (covers[i].transform.position.y < -100)
                {
                    covers[i].active = false;
                }

            }
            if (StepCount == MaxStep - 2)
            {

                //reward();
                EndEpisode();

            }
            if (backCounterActivated)
            {
                if (onBackCounter > 100)
                {
                    onBackCounter = 0;
                    if(state == EnemyState.training)
                    {
                        EndEpisode();
                    }
                    if(state == EnemyState.playing)
                    {
                        mainBodyRigidBody.AddTorque(mainBody.transform.forward * backForce, ForceMode.VelocityChange);
                        mainBodyRigidBody.AddForce(-mainBody.transform.up * backForce, ForceMode.VelocityChange);

                    }
                   
                }
                if (Vector3.Dot(mainBody.transform.up, -movingPlattform.transform.up) > 0.5f)
                {
                    DebugReward(-0.3f, "Being upside down");
                    onBackCounter++;
                    //EndEpisode();
                }
            }
        }


        //Debug.Log(MaxStep +" " + StepCount);



      
        //AddReward(-0.001f);
        if (movingPlattform.transform.InverseTransformPoint(mainBody.transform.position).y < -30)
        {
            DebugReward(-100, "fell from plattform");
            if(state == EnemyState.training)
            {
                EndEpisode();
            }
            else
            {
                //Dead();
            }
         
        }
        
        if (RewardFunction == RewardMode.Nearest || RewardFunction == RewardMode.JustGetit)
            DebugReward(-1, "living penalty");
            if (RewardFunction == RewardMode.Ndistance)
        {
            float f = Vector3.Distance(mainBody.transform.position, target.transform.position);
            if (f < nearestDistance)
            {
                DebugReward(nearestDistance - f, "Get nearer");
                nearestDistance = f;
            }
            f = Mathf.Abs(target.transform.position.y -  mainBody.transform.position.y);
            //AddReward(30-f);
            if (f < nearestY)
            {
                //AddReward(nearestDistance - f);
                nearestY = f;
            }

        }
        float r = 1-Mathf.Clamp01(Vector3.Distance(target.position, mainBody.transform.position) / 35);
        //AddReward(r*0.1f);
        var cubeForward = m_OrientationCube.transform.forward;

        if (RewardFunction == RewardMode.MaxVelocity)
        { Vector3 vel = mainBody.GetComponent<Rigidbody>().velocity;
            DebugReward((Vector3.Dot((vel), cubeForward)* velocityBonus), "Velocity: " + vel);
        }

        // Set reward for this step according to mixture of the following elements.
        // a. Match target speed
        //This reward will approach 1 if it matches perfectly and approach zero as it deviates
        var matchSpeedReward = GetMatchingVelocityReward(cubeForward * MTargetWalkingSpeed, GetAvgVelocity());
        if (RewardFunction == RewardMode.AvgVelocity)
        {
            var g = GetAvgVelocity();
            var o = GetMatchingVelocityReward(cubeForward * MTargetWalkingSpeed, g);
            DebugReward(o, "AvgVelocity: " + g);
        }
        //Check for NaNs
        if (float.IsNaN(matchSpeedReward))
        {
            throw new ArgumentException(
                "NaN in moveTowardsTargetReward.\n" +
                $" cubeForward: {cubeForward}\n" +
                $" hips.velocity: {new Vector3()}\n" +
                $" maximumWalkingSpeed: {m_maxWalkingSpeed}"
            );
        }

        // b. Rotation alignment with target direction.
        //This reward will approach 1 if it faces the target direction perfectly and approach zero as it deviates
        var lookAtTargetReward = (Vector3.Dot(cubeForward, mainBody.transform.forward) + 1) * .5F;
        Debug.DrawRay(mainBody.transform.position, mainBody.transform.forward);
        //var lookAtTargetReward = Vector3.Dot(cubeForward, Forward(mainHead.transform)) ;
        //Debug.DrawRay(mainHead.transform.position, Forward(mainHead.transform)*5);
        var targetDir = (target.transform.position - mainHead.transform.position).normalized;
        var c = Vector3.Cross(targetDir, UP(mainHead.transform));
        c = Vector3.Cross(c, targetDir);
        var lookAtTargetReward2 = (Vector3.Dot(c, (mainHead.transform.forward)) + 1) * .5F;
        //Check for NaNs
        if (float.IsNaN(lookAtTargetReward))
        {
            throw new ArgumentException(
                "NaN in lookAtTargetReward.\n" +
                $" cubeForward: {cubeForward}\n" +
                $" head.forward: {Forward(mainHead.transform)}"
            );
        }
        if (RewardFunction == RewardMode.Nearest)
        {
            DebugReward(1 - Mathf.Clamp01(Vector3.Distance(mainBody.transform.position, target.transform.position) / 200), "Distance Bonus" );
        }
        else {

            if(RewardFunction == RewardMode.Gaze)
        {
                DebugReward(matchSpeedReward * lookAtTargetReward,"Gaze");
            }
        }
        
        //AddReward(1 - Mathf.Clamp01(Vector3.Distance(mainBody.transform.position, target.transform.position))/50);
        //AddReward(lookAtTargetReward * lookAtTargetReward2);
    }

    //Returns the average velocity of all of the body parts
    //Using the velocity of the hips only has shown to result in more erratic movement from the limbs, so...
    //...using the average helps prevent this erratic movement
    Vector3 GetAvgVelocity()
    {
        Vector3 velSum = Vector3.zero;

        //ALL RBS
        int numOfRb = 0;
        foreach (var item in m_JdController.bodyPartsList)
        {
            numOfRb++;
            velSum += item.rb.velocity;
        }

        var avgVel = velSum / numOfRb;
        return avgVel;
    }

    //normalized value of the difference in avg speed vs goal walking speed.
    public float GetMatchingVelocityReward(Vector3 velocityGoal, Vector3 actualVelocity)
    {
        //distance between our actual velocity and goal velocity
        var velDeltaMagnitude = Mathf.Clamp(Vector3.Distance(actualVelocity, velocityGoal), 0, MTargetWalkingSpeed);

        //return the value on a declining sigmoid shaped curvesens that decays from 1 to 0
        //This reward will approach 1 if it matches perfectly and approach zero as it deviates
        return Mathf.Pow(1 - Mathf.Pow(velDeltaMagnitude / MTargetWalkingSpeed, 2), 2);
    }

    /// <summary>
    /// Agent touched the target
    /// </summary>
    /// 
    public float reachTargetreward = 0;
    public float timeRewardMult = 1;
    public void TouchedTarget()
    {
        if(state == EnemyState.training)
        {
            float d = 0;
            if (MaxStep != 0)
            {
                d = reachTargetreward + ((MaxStep - StepCount) / (MaxStep * 1.0f)) * timeRewardMult;
            }
           
            if (RewardFunction == RewardMode.Nearest || RewardFunction == RewardMode.Ndistance || RewardFunction == RewardMode.JustGetit
                || RewardFunction == RewardMode.MaxVelocity)
            {
                DebugReward(d, "Touched: " + (MaxStep - StepCount));
            }
            else
            {
                DebugReward(1, "Standard touch reward");
            }
        }

    }

    public void SetTorsoMass()
    {
        
    }

    public void SetResetParameters()
    {
        SetTorsoMass();
    }

    public int GetID()
    {
        return myID;
    }

    public EnemyState GetState()
    {
        return state;
    }
}
