using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainProjectile : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy")) //look into using layers for efficiency, also for other projectiles
        {

            collision.gameObject.GetComponent<Enemy>().TakeDamage(10);
        }


    }
}
