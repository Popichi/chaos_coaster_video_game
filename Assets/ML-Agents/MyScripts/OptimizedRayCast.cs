using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class OptimizedRayCast : MonoBehaviour
{
    public float rayDistance = 5f;
    public int numberOfRays = 1000;
    NativeArray<RaycastHit> results;
    NativeArray<RaycastCommand> commands;
    private void Start()
    {
        results = new NativeArray<RaycastHit>(numberOfRays, Allocator.Persistent);
        commands = new NativeArray<RaycastCommand>(numberOfRays, Allocator.Persistent);
    }
    private void FixedUpdate()
    {
        CastRays();
    }
    private void CastRays()
    {
        for (int i = 0; i < numberOfRays; i++)
        {
            float phi = Mathf.PI * 2.0f * i / numberOfRays;
            float theta = Mathf.Acos(1 - 2 * (i / (float)numberOfRays));
            float x = Mathf.Sin(theta) * Mathf.Cos(phi);
            float y = Mathf.Sin(theta) * Mathf.Sin(phi);
            float z = Mathf.Cos(theta);

            Vector3 direction = new Vector3(x, y, z);
            Ray ray = new Ray(transform.position, direction);
            Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                Debug.Log("Hit object: " + hit.collider.gameObject.name);
            }
        }
    }
    private void RaycastCircle()
    {


        for (int i = 0; i < numberOfRays; i++)
        {
            float phi = Mathf.PI * 2.0f * i / numberOfRays;
            float theta = Mathf.Acos(1 - 2 * (i / (float)numberOfRays));
            float x = Mathf.Sin(theta) * Mathf.Cos(phi);
            float y = Mathf.Sin(theta) * Mathf.Sin(phi);
            float z = Mathf.Cos(theta);

            Vector3 direction = new Vector3(x, y, z);
            commands[i] = new RaycastCommand(transform.position, direction, rayDistance);
        }

        JobHandle handle = RaycastCommand.ScheduleBatch(commands, results, 1, default(JobHandle));
        handle.Complete();

        for (int i = 0; i < numberOfRays; i++)
        {
            if (results[i].collider != null)
            {
                Debug.Log("Hit object: " + results[i].collider.gameObject.name);
                Debug.DrawRay(transform.position, commands[i].direction * rayDistance, Color.red);
            }
        }
    }
    private void OnDestroy()
    {
        results.Dispose();
        commands.Dispose();
    }
}