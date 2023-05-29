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
    // Start is called before the first frame update
    public void SubscribeDrone(ARDDrone a)
    {
        drones.Add(a); 
    }
    void Awake()
    {
        drones = new List<ARDDrone>();
        //drones = FindObjectsOfType<ARDGun>().ToList();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
