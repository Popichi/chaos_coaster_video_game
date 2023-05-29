using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseShotProjectile : MonoBehaviour
{
    public float timeDetonation;
    Rigidbody rb;
    GameObject other;

    bool collided;
    Vector3 collisionDirection;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody>();
        collided = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        other = collision.gameObject;
        if (!other.CompareTag("Player")) //look into using layers for efficiency, also for other projectiles
        {

            collided = true;
            Vector3 otherPos = other.transform.position;
            collisionDirection = otherPos - this.transform.position;
            //rb.velocity = new Vector3();
            SphereCollider sCollider = gameObject.GetComponent<SphereCollider>();
            sCollider.isTrigger = true;
            this.transform.parent = other.transform;
            rb.isKinematic = true;
            Invoke(nameof(Detonate), timeDetonation);
        }


    }


    private void Detonate()
    {
        Rigidbody targetRB = other.GetComponent<Rigidbody>();
        if (targetRB != null)
            targetRB.AddForce(collisionDirection.normalized * 3f, ForceMode.Impulse);
        Destroy(gameObject, 0.5f);
    }
}
