using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class ARDGunManager : MonoBehaviour
{
    public GameObject target;
    public GameObject root;
    public float turbineStrength = 30;
    public List<ARDDrone> drones;
    public float expellingVelocity = 10;
    public Vector2 maxAngles = new Vector2(30f,360f);
    public float startAngularVel = 2;
    public float randomVel =1.5f;
    public bool isTraining;
    public enum RewardModeARD
    {
        Velocity
    }
    
    // Start is called before the first frame update
    public void SubscribeDrone(ARDDrone a)
    {
        drones.Add(a); 
    }
    public float GetStartVelocity()
    {
        return expellingVelocity + (Random.value - 0.5f) * randomVel;
    }
    public Vector3 RandomVectorWithLimitedAngle()
    {
        //chatGPT code for easy Lapalien
        // Generate a random angle around the y-axis
        float angleAroundY = Random.Range(0f, 360f);

        // Generate a random angle between 0 and maxAngle towards the xz-plane
        float angleAwayFromY = Random.Range(0f, maxAngles.x);

        // Create a Quaternion that rotates 'angleAwayFromY' degrees around the x-axis
        Quaternion rotationAwayFromY = Quaternion.Euler(angleAwayFromY, 0, 0);

        // Create a Quaternion that rotates 'angleAroundY' degrees around the y-axis
        Quaternion rotationAroundY = Quaternion.Euler(0, angleAroundY, 0);

        // Combine the rotations and apply them to Vector3.up
        Vector3 randomVector = rotationAroundY * rotationAwayFromY * Vector3.up;

        return randomVector;
    }
    void Awake()
    {
        drones = new List<ARDDrone>();
        //drones = FindObjectsOfType<ARDGun>().ToList();

    }
    public RewardModeARD modeARD;
    private void Start()
    {
      
    }
    public Vector3 GetSpawnPoint()
    {
        return Vector3.zero;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
