using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketController : MonoBehaviour
{
    int health = 50;
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
            rb.velocity += manager.speed * Time.fixedDeltaTime;
            var a = (manager.target.position - transform.position).normalized;
            controlledObject.DesiredOrientation = Quaternion.LookRotation(a);
        }
        Debug.DrawRay(transform.position, manager.speed);
        
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            --health;
            if(health<= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public int damage = 20;
    private int damageCounter = 1;


    // Start is called before the first frame update


    // Update is called once per frame

    public string target = "Player";
    private void OnCollisionEnter(UnityEngine.Collision collision)
    {

        if (damageCounter > 0 && collision.gameObject.CompareTag(target))
        {
            ITakeDamage e = collision.gameObject.GetComponent<ITakeDamage>(); 
            //if e != null
            if (e != null)
            {
                e.TakeDamage(damage);
                Debug.Log("Player Took damage finally");
                damageCounter--;
            }
            else
            {
                Debug.Log("No Enemy script attached");
            }
            
        }
    }
}


