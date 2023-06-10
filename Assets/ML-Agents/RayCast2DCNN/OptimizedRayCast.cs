using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Unity.MLAgents.Sensors;
using Unity.Mathematics;

public class OptimizedRayCast : MonoBehaviour
{
    public Vector3 focalPoint = Vector3.zero;
    public Transform root;
    public float rayMaxDistance = 5f;
    //public int numberOfRays = 1000;
    NativeArray<RaycastHit> results;
    NativeArray<RaycastCommand> commands;
    public Mesh mesh;
    public Texture2D texture2D;
    public int width;
    public int height;

    [HideInInspector]
    public static int id = 0;
    public int stack = 1;

    private void Awake()
    {
        texture2D = new Texture2D(width, height, TextureFormat.RGB24, false);
        string s = "RayCNNSensor" + id;
        id++;
        //init sensor
        RayCNNComponent r = gameObject.AddComponent<RayCNNComponent>();
        r.optimizedRayCast = this;
        r.Grayscale = false;
        //r.RenderTexture = render;
        r.texture2D = texture2D;
        r.CompressionType = SensorCompressionType.PNG;
        r.SensorName = s;
        r.ObservationStacks = stack;



    }
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
        if (!root)
        {
            root = transform;
        }
        numberOfRays = mesh.vertexCount;
        results = new NativeArray<RaycastHit>(numberOfRays, Allocator.Persistent);
        commands = new NativeArray<RaycastCommand>(numberOfRays, Allocator.Persistent);
    }
    private void FixedUpdate()
    {
        CastMainThreadRays();
    }
    public void SaveInTexture(float3 v, Vector2 uv)
    {
        v = (v + 1) / 2.0f;
        v = math.clamp(v,0,1);
        Color c = new Color(v.x,v.y,v.z);
        
        texture2D.SetPixel((int)(uv.x * texture2D.width),(int)(uv.y*texture2D.height),c); 
    }
    Color[] colors;
    private void CastMainThreadRays()
    {
        ResetPixels();
        Vector3[] vertices = mesh.vertices;
        Vector2[] uvs = mesh.uv;
        //colors = texture2D.GetPixels();
        for (int i = 0; i < numberOfRays; i++)
        {
            
            Vector3 direction = (transform.TransformPoint(vertices[i]) - root.position).normalized;
            Ray ray = new Ray(transform.position, direction);
            Color debugC = Color.green;
            

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, rayMaxDistance))
            {
                Vector3 v = (hit.distance * ray.direction) / rayMaxDistance;
                v = root.InverseTransformDirection(v);
                Debug.Log(v);
                SaveInTexture(v, uvs[i]);
                debugC = Color.red;
                Debug.DrawRay(ray.origin, ray.direction * hit.distance, debugC);
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * rayMaxDistance, debugC);
            }
            
        }
        texture2D.Apply();
    }
    public void ResetPixels()
    {
        Color[] colors = this.texture2D.GetPixels();
        int ii = 0;
        foreach (var x in colors)
        {
            colors[ii] = new Color(0, 0, 0);
            ii++;
        }
        texture2D.SetPixels(colors);
        texture2D.Apply();
        
    }
    private void CastMultiThreadRays()
    {

        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < numberOfRays; i++)
        {
            Vector3 direction = vertices[i].normalized;
            

            commands[i] = new RaycastCommand(transform.position, direction, rayMaxDistance);
        }

        JobHandle handle = RaycastCommand.ScheduleBatch(commands, results, 1, default(JobHandle));
        handle.Complete();

        for (int i = 0; i < results.Length; i++)
        {
            if (results[i].collider != null)
            {
               
                Debug.DrawRay(transform.position, commands[i].direction * results[i].distance, Color.red);
            }
        }
    }
    private void OnDestroy()
    {
        results.Dispose();
        commands.Dispose();
    }
}