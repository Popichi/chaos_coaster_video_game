using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainProjectile : MonoBehaviour
{
    public int damage = 20;
    private int damageCounter = 2;
    

    // Start is called before the first frame update
    void Start()
    {
        //dont write in start bc it overwrites what is set in the inspector
    }

    // Update is called once per frame


    private void OnCollisionEnter(UnityEngine.Collision collision)
    {

        if (damageCounter > 0 && collision.gameObject.CompareTag("agent"))
        {
            Enemy e;
            e = collision.gameObject.GetComponent<Enemy>();
            if(!e)
            e = collision.gameObject.GetComponentInParent<Enemy>();
            //if e != null
            if (e)
            {
                e.TakeDamage(damage);
                Debug.Log("Enemy Took damage finally");
                damageCounter--;
            }
            else
            {
                Debug.Log("No script attached");
            }
            
        }
    }
}
