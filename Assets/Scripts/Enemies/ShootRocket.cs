using System.Collections;
using UnityEngine;
using System.Collections.Generic;
public class ShootRocket : MonoBehaviour
{
    public GameObject projectilePrefab; // Assign this in the Inspector
    public float shootInterval = 1.0f; // X seconds
    public float pauseInterval = 2.0f; // Y seconds
    public int numberOfShots = 5; // Number of shots before pause
    public Vector3 shootDirection = Vector3.up; // Change this to the direction you want
    public Transform root;
    private Coroutine shootCoroutine;
    public Transform target;
    public float power = 5;
    GetMovement movement;
    
    private void OnEnable()
    {
        if (shootCoroutine == null)
        {
            shootCoroutine = StartCoroutine(ShootProjectile());
        }
    }
    public Vector3 speed;
    private void FixedUpdate()
    {
        speed = movement.GetSpeed();
    }
    private void OnDisable()
    {
        if (shootCoroutine != null)
        {
            StopCoroutine(shootCoroutine);
            shootCoroutine = null;
        }
    }

    private IEnumerator ShootProjectile()
    {
        while (true)
        {
            for (int i = 0; i < numberOfShots; i++)
            {
                GameObject projectile = Instantiate(projectilePrefab, root.position, Quaternion.identity * Quaternion.Euler(-90,0,0));
                projectile.GetComponentInChildren<RocketController>().manager = this;
                if(movement != null)
                {
                    projectile.GetComponentInChildren<Rigidbody>().velocity = root.TransformDirection(shootDirection) * power + movement.GetSpeed();
                }
                else
                {
                    projectile.GetComponentInChildren<Rigidbody>().velocity = root.TransformDirection(shootDirection) * power;
                }
                
                Destroy(projectile,15);
                yield return new WaitForSeconds(shootInterval);
            }

            yield return new WaitForSeconds(pauseInterval);
        }
    }
    private void Awake()
    {
        movement = FindObjectOfType<GetMovement>();
    }

}
