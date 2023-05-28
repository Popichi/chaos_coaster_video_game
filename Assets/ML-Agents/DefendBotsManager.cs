using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefendBotsManager: MonoBehaviour
{
    public float dotDelta = 0.01f;
    public float targetSpawnSize;
    public float delta;
    public float legStrenght;
    public float playFieldSize;
    public float maxGoalSpeed;
    public Quaternion randomQuaternion1;
    public Quaternion randomQuaternion2;
    public float randomFloat;
    public float randomFloat2;
    public float randomFloat3;
    public float min;
    public float max;
    public bool locked;
    public float maxVelocity = 100;
    public float[] powers;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!locked)
        {
            randomQuaternion1 = Random.rotation;
            randomQuaternion2 = Random.rotation;
            randomFloat = Random.Range(min, max);
            randomFloat2 = Random.Range(-180, 180);
            randomFloat3 = Random.Range(-180, 180);
        }
    }
}
