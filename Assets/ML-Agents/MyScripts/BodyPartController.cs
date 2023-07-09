using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Policies;
using Unity.Barracuda;

public class BodyPartController : MonoBehaviour
{
    public List<IsBodyPart> bodyParts;
    public Transform root;
    public NNModel runNormal;
    public NNModel runLimbsOff;
    public NNModel exorcist;
    private BehaviorParameters behaviorParameters;
    private void ResetAllBodyParts()
    {
        foreach(var a in bodyParts)
        {
            a.ResetLimb();
            a.groundContact.reset();
        }
    }
    public bool normal = true;
    public static void MakeIgnore()
    {
        IsBodyPart[] bodyParts = FindObjectsByType<IsBodyPart>(FindObjectsSortMode.None);



            foreach (var b in bodyParts)
            {
            Collider c = b.GetComponent<Collider>();
            if (!c)
                c = b.GetComponentInChildren<Collider>();
                foreach (var bb in bodyParts)
                {
                   
                        if (b != bb)
                        {
                            Collider col = bb.gameObject.GetComponentInChildren<Collider>();
                            if (col)
                                Physics.IgnoreCollision(c, col, true);
                            return;
                        }
                    }
                   
                    
                
            }
              
        
    }
    public void SwitchModelToNormal()
    {
        normal = true;
       
        if(behaviorParameters.Model != runNormal)
        behaviorParameters.Model = runNormal;
    }
    public void SwitchModelToExo()
    {
        normal = false;
        if (behaviorParameters.Model != exorcist)
            behaviorParameters.Model = exorcist;
    }

        public void SwitchModelToLimb()
    {
        normal = false;
        if(behaviorParameters.Model != runLimbsOff)
        behaviorParameters.Model = runLimbsOff;
    }
    public void SetBodies(List<IsBodyPart> a)
    {
        bodyParts = a;
        setVisuals();
    }
    public void GetObs(VectorSensor sensor)
    {
        foreach (var a in bodyParts)
        {
            if (a)
            {
                sensor.AddObservation(a.detached);
            }
            else
            {
                sensor.AddObservation(0);
            }
           
        }

    }
    public Rigidbody setToRB;
    public void CanTouchFloor(bool b)
    {
        b = !b;
        foreach (var a in bodyParts)
        {
            a.groundContact.agentDoneOnGroundContact = b;
        }
    }
    Material material;
    public void SetLimbOff()
    {

    }
    public void setVisuals()
    {
        foreach (var a in bodyParts)
        {

            if (a.sliceable)
            {
                a.visual = new GameObject();
                MeshFilter m = a.visual.AddComponent<MeshFilter>();
                var r = a.visual.AddComponent<MeshRenderer>();
                
                if (a.skinned)
                {
                    material = a.skinned.material;
                    m.mesh = a.skinned.sharedMesh;
                    r.material = material;
                }
                else
                {
                    m.mesh = GetComponent<MeshFilter>().mesh;
                }

                a.visual.transform.parent = root;
                a.visual.transform.localPosition = Vector3.zero;
                a.visual.transform.localRotation = Quaternion.identity;
                a.visual.transform.localScale = Vector3.one;
                a.visual.transform.parent = a.transform;
                a.visual.active = false;
            }
               
            


        }
    }
    public void Awake()
    {
        //setToRB = GameObject.FindWithTag("Map").GetComponent<Rigidbody>();
        behaviorParameters = GetComponent<BehaviorParameters>();
    }
    // Start is called before the first frame update


    // Update is called once per frame

}
