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
        }
    }

    public void GetObs(VectorSensor sensor)
    {
        foreach (var a in bodyParts)
        {
            sensor.AddObservation(a.detached);
        }

    }
    // Start is called before the first frame update


    // Update is called once per frame

}
