using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel : MonoBehaviour
{
    // Start is called before the first frame update
    public int id;
    public bool touch;
    public string tag = "ground";
    public CreateVoxelMap map;
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag(tag))
        {
            touch = true;
            map.occupancyMap[id] = true;
        }
    }
    

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(tag))
        {
            touch = false;
            map.occupancyMap[id] = false;
        }
    }
    void OnDrawGizmos()
    {
        if (touch)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawSphere(transform.position, transform.localScale.x / 2);
        }

           
    }
}
