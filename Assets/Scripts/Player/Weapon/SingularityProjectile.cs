using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingularityProjectile : MonoBehaviour
{
    float damage;
    float radius;
    float force;
    public GameObject sphereRadius;
    public GameObject particleEffect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetValues(float _damage, float _radius, float _force)
    {
        damage = _damage;
        radius = _radius;
        force = _force;
    }

    public void Explode()
    {
        // Add some explosion effect

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider nearbyObject in colliders)
        {
            Rigidbody nearbyRb = nearbyObject.GetComponent<Rigidbody>();
            if (nearbyRb != null)
            {
                //Pushes objects towards it
                Vector3 otherPos = nearbyObject.transform.position;
                Vector3 forceDirection = -(otherPos - transform.position).normalized;
                nearbyRb.AddForce(forceDirection * force, ForceMode.Impulse);
            }

            Enemy enemyHealth = nearbyObject.GetComponent<Enemy>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(20);
            }
        }
        GameObject ps = Instantiate(particleEffect, transform.position, Quaternion.identity, this.transform.parent);
        Destroy(ps, 1);

        Destroy(gameObject);
    }

    private IEnumerator RadiusDetonation()
    {
        yield return new WaitForEndOfFrame();
            
    }
}
