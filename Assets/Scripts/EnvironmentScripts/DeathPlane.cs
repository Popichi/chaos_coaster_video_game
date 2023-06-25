using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IReactOnDeathPlane
{
    public void ReactOnDeathPlane();
}

public class DeathPlane : MonoBehaviour
{
    // Start is called before the first frame update
    public void Start()
    {
        Renderer r = GetComponent<MeshRenderer>();
        if (r)
        {
            Destroy(r);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        IReactOnDeathPlane r = collision.gameObject.GetComponent<IReactOnDeathPlane>();
        if(r!=null)
        r.ReactOnDeathPlane();
    }
}
