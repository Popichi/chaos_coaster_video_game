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

    private void OnCollisionEnter(Collision collision)
    {
        IReactOnDeathPlane r = collision.gameObject.GetComponent<IReactOnDeathPlane>();
        r.ReactOnDeathPlane();
    }
}
