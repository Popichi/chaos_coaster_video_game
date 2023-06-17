using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunBullet : MonoBehaviour
{
    private float force;
    private float damage;
    public GameObject ps;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetValues(float _damage, float _force)
    {
        damage = _damage;
        force = _force;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player"))
        {
            GameObject impact = Instantiate(ps, transform.position, Quaternion.identity, transform.parent);
            Destroy(impact, 0.4f);
            Destroy(gameObject);
        }
    }
}
