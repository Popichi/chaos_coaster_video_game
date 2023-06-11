using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetRandomPositionOnSurface : MonoBehaviour
{
    public Mesh mesh;
    MeshFilter meshFilter;
    // Start is called before the first frame update
    void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
        if (!meshFilter)
        {
            gameObject.AddComponent<MeshFilter>();
        }
        
            mesh = meshFilter.mesh;
        
    }

    // Update is called once per frame
    public Vector3 randomPosOnGrid(float offset)
    {

        Vector3[] vertices = mesh.vertices;
        int[] triangles = mesh.triangles;

        // Randomly pick a triangle
        int triangleIndex = Random.Range(0, triangles.Length / 3);

        // Get the vertices of the picked triangle
        Vector3 v1 = vertices[triangles[triangleIndex * 3]];
        Vector3 v2 = vertices[triangles[triangleIndex * 3 + 1]];
        Vector3 v3 = vertices[triangles[triangleIndex * 3 + 2]];

        // Transform vertices to world space
        Vector3 v1World = transform.TransformPoint(v1);
        Vector3 v2World = transform.TransformPoint(v2);
        Vector3 v3World = transform.TransformPoint(v3);

        // Generate random barycentric coordinates
        float u = Random.value;
        float v = Random.value;

        if (u + v > 1)
        {
            u = 1 - u;
            v = 1 - v;
        }

        // Compute the corresponding Cartesian coordinates
        Vector3 pointInTriangle = (1 - u - v) * v1World + u * v2World + v * v3World;
        return pointInTriangle + transform.forward * offset;
    }
}
