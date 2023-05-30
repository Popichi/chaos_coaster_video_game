using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class OptimizedRayCast : MonoBehaviour
{
    public Vector3 focalPoint = Vector3.zero;
    public Transform relativeTo;
    public float rayDistance = 5f;
    //public int numberOfRays = 1000;
    NativeArray<RaycastHit> results;
    NativeArray<RaycastCommand> commands;
    public Mesh mesh;
    void CleanMesh()
    {

            mesh = GetComponent<MeshFilter>().mesh;
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;

            // Use a dictionary to remove duplicates.
            System.Collections.Generic.Dictionary<Vector3, int> vertexDictionary = new System.Collections.Generic.Dictionary<Vector3, int>();

            for (int i = 0; i < vertices.Length; i++)
            {
                if (!vertexDictionary.ContainsKey(vertices[i]))
                {
                    vertexDictionary.Add(vertices[i], vertexDictionary.Count);
                }
            }

            // Reassign the vertices and triangles.
            Vector3[] newVertices = new Vector3[vertexDictionary.Count];
            int[] newTriangles = new int[triangles.Length];

            vertexDictionary.Keys.CopyTo(newVertices, 0);

            for (int i = 0; i < triangles.Length; i++)
            {
                newTriangles[i] = vertexDictionary[vertices[triangles[i]]];
            }

            mesh.Clear();

            mesh.vertices = newVertices;
            mesh.triangles = newTriangles;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
        
    
}
    int numberOfRays = 0;
    private void Start()
    {
        if (!relativeTo)
        {
            relativeTo = transform;
        }
        numberOfRays = mesh.vertexCount;
        results = new NativeArray<RaycastHit>(numberOfRays, Allocator.Persistent);
        commands = new NativeArray<RaycastCommand>(numberOfRays, Allocator.Persistent);
    }
    private void FixedUpdate()
    {
        CastMultiThreadRays();
    }
    private void CastMainThreadRays()
    {
        for (int i = 0; i < numberOfRays; i++)
        {

            Vector3 direction = new Vector3(0, 0, 0);
            Ray ray = new Ray(transform.position, direction);
            Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, rayDistance))
            {
                Debug.Log("Hit object: " + hit.collider.gameObject.name);
            }
        }
    }
    private void CastMultiThreadRays()
    {

        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < numberOfRays; i++)
        {
            Vector3 direction;
            if (transform != relativeTo)
            {
                direction = relativeTo.InverseTransformPoint(transform.TransformPoint(vertices[i])) - focalPoint;
            }
            else
            {
                direction = (transform.TransformPoint(vertices[i])) - focalPoint;
            }
           
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