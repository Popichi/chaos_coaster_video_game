using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainProjectile : MonoBehaviour
{
    public int damage;
    private int damageCounter;
    

    // Start is called before the first frame update
    void Start()
    {
        damageCounter = 2;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(UnityEngine.Collision collision)
    {

        if (damageCounter > 0 && collision.gameObject.CompareTag("agent"))
        {
            GameObject current = collision.gameObject.transform.parent.gameObject;
            while(current.GetComponent<Enemy>() == null)
            {
                current = current.transform.parent.gameObject;
            }
            current.GetComponent<Enemy>().TakeDamage(damage);
            Debug.Log("Enemy Took damage finally");
            damageCounter--;
        }
    }
}
