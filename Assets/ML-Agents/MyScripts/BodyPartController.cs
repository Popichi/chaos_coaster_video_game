using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents.Sensors;

public class BodyPartController : MonoBehaviour
{
    public List<IsBodyPart> bodyParts;
    public Transform root;

    private void ResetAllBodyParts()
    {
        foreach(var a in bodyParts)
        {
            a.ResetLimb();
            a.groundContact.reset();
        }
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
    public void CanTouchFloor(bool b)
    {
        b = !b;
        foreach (var a in bodyParts)
        {
            a.groundContact.agentDoneOnGroundContact = b;
        }
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
                r.material = a.skinned.GetComponent<Material>();
                if (a.skinned)
                {
                    m.mesh = a.skinned.sharedMesh;
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
            }
               
            


        }
    }
    public void Awake()
    {
       
    }
    // Start is called before the first frame update


    // Update is called once per frame

}
