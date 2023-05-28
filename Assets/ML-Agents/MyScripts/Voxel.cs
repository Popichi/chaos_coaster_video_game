using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voxel : MonoBehaviour
{
    // Start is called before the first frame update
    public int id;
    public bool touch;
    public string tag = "ground";
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag(tag))
        {
            touch = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag(tag))
        {
            touch = false;
        }
    }
}
