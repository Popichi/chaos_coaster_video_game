using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float timeExplosion;
    float damage;
    float radius;
    float force;
    public GameObject explosionVFX;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (timeExplosion >= 0f)
        {
            timeExplosion -= Time.deltaTime;
        } else
        {
            Explode();
        }
    }

    public void SetValues(float _damage, float _radius, float _force, float _timer)
    {
        damage = _damage;
        radius = _radius;
        force = _force;
        timeExplosion = _timer;
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
                nearbyRb.AddExplosionForce(force, transform.position, radius, 7f, ForceMode.Impulse);
            }

            if (nearbyObject.gameObject.CompareTag("agent"))
            {
                
                
                Enemy e = nearbyObject.GetComponentInParent<Enemy>();
                if (e)
                {
                    e.TakeDamage(damage);
                    Debug.Log("Enemy Took damage finally");
                    
                }
                else
                {
                    Debug.Log("No Enemy script attached");
                }
                IsBodyPart takeDamage = nearbyObject.gameObject.GetComponent<IsBodyPart>();
                if (takeDamage)
                {
                    takeDamage.TakeDamage(damage);
                }
            }
        }
        GameObject ps = Instantiate(explosionVFX, transform.position, Quaternion.identity);
        Destroy(ps, 1f);

        Destroy(gameObject);

    }
}
