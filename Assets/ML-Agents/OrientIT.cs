using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class OrientIT : MonoBehaviour
{
    public Transform up;
    public Transform forward;
    public Transform position;

    WaveSpawner waveSpawner;
    // Start is called before the first frame update
    void Start()
    {
        waveSpawner = FindAnyObjectByType<WaveSpawner>();
        up = waveSpawner.up;

        forward = transform.parent.transform;
        position = transform.parent.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 r = Vector3.Cross(forward.forward, up.up);
        Vector3 f = -Vector3.Cross(r, up.up);
       transform.rotation = Quaternion.LookRotation(f,up.up);
        transform.position = position.position;
    }
}


