using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour
{
    VacuumBreather.ControlledObject controlledObject;
    public ShootRocket manager;
    Rigidbody rb;
    public float power = 10;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        controlledObject = GetComponent<VacuumBreather.ControlledObject>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.AddForce(transform.forward * power);
        if(manager != null)
        {
            rb.velocity += manager.speed * Time.deltaTime;
            var a = (manager.target.position - transform.position).normalized;
            controlledObject.DesiredOrientation = Quaternion.LookRotation(a);
        }
       
        
    }
}
