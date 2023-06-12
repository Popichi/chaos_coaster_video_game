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
    public LayerMask mask;
    public bool clean;
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
        if(clean)
        CleanMesh();
        if (!root)
        {
            root = transform;
        }
        numberOfRays = mesh.vertices.Length;
        results = new NativeArray<RaycastHit>(numberOfRays, Allocator.Persistent);
        commands = new NativeArray<RaycastCommand>(numberOfRays, Allocator.Persistent);


    }
    void CleanMesh()
    {

        
        Vector3[] vertices = mesh.vertices;
        Debug.Log("Vertices before: " + vertices.Length);
        int[] triangles = mesh.triangles;
        Vector2 []uvs = mesh.uv;
        Debug.Log("UVS before: " + uvs.Length);
        // Use a dictionary to remove duplicates.
        System.Collections.Generic.Dictionary<Vector3, int> vertexDictionary = new System.Collections.Generic.Dictionary<Vector3, int>();
        System.Collections.Generic.Dictionary<Vector2, int> uvDictionary = new System.Collections.Generic.Dictionary<Vector2, int>();

        for (int i = 0; i < vertices.Length; i++)
        {
            if (!vertexDictionary.ContainsKey(vertices[i]))
            {
                vertexDictionary.Add(vertices[i], vertexDictionary.Count);
            }
        }
        for (int i = 0; i < uvs.Length; i++)
        {
            if (!uvDictionary.ContainsKey(uvs[i]))
            {
                uvDictionary.Add(uvs[i], uvDictionary.Count);
            }
        }

        // Reassign the vertices and triangles.
        Vector3[] newVertices = new Vector3[vertexDictionary.Count];
        int[] newTriangles = new int[triangles.Length];
        int [] newUV = new int [vertexDictionary.Count];
        vertexDictionary.Keys.CopyTo(newVertices, 0);
        vertexDictionary.Values.CopyTo(newUV, 0);
        for (int i = 0; i < triangles.Length; i++)
        {
            newTriangles[i] = vertexDictionary[vertices[triangles[i]]];
        }
        Vector2[] newUV2 = new Vector2[newUV.Length];
        for (int i = 0; i < newUV2.Length; i++)
        {
            newUV2[i] = new float2(uvs[newUV[i]]);
        }

            mesh.Clear();
        //mesh.uv = newUV;
        mesh.vertices = newVertices;
        mesh.uv = newUV2;
        mesh.triangles = newTriangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        Debug.Log("Vertices after: " + mesh.vertices.Length);
        Debug.Log("uv after: " + mesh.uv.Length);
    }
    int numberOfRays = 0;
    private void Start()
    {

    }
    private void FixedUpdate()
    {
        CastMainThreadRays();
    }
    public void SaveInTexture(float3 v, Vector2 uv)
    {
        if (!texture2D)
        {
            Debug.Log("textureNull");
            return;
        }
        uv = math.clamp(uv, 0, 1);
        v = (v + 1) / 2.0f;
        v = math.clamp(v,0,1);
        Color c = new Color(v.x,v.y,v.z);
        int x = (int)(uv.x * (texture2D.width-1));
        int y = (int)(uv.y * (texture2D.height-1));
        
        int index = x + y * texture2D.width;
        //Debug.Log(index);
        colors[index] = c;
    }
    public bool debug = true;
    Color[] colors;
    private void CastMainThreadRays()
    {
        //ResetPixels();
        Vector3[] vertices = mesh.vertices;
        Vector2[] uvs = mesh.uv;
        colors = texture2D.GetPixels();
        for (int i = 0; i < numberOfRays; i++)
        {
            
            Vector3 direction = (transform.TransformPoint(vertices[i]) - root.position).normalized;
            Ray ray = new Ray(transform.position, direction);
            Color debugC = Color.green;
            

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, rayMaxDistance, mask))
            {
                Vector3 v = (root.InverseTransformDirection(hit.normal).normalized * hit.distance)/ rayMaxDistance;
                //Debug.Log(i+" :i uvs[i]" + uvs[i]);
                SaveInTexture(v, uvs[i]);
                if (debug)
                {


                    debugC = Color.red;
                    Debug.DrawRay(ray.origin, ray.direction * hit.distance, debugC);
                }
                }
            else
            {
                if(debug)
                Debug.DrawRay(ray.origin, ray.direction * rayMaxDistance, debugC);
            }
            
        }
        texture2D.SetPixels(colors);
        texture2D.Apply();
    }
    public void ResetPixels()
    {
        Color[] colors = this.texture2D.GetPixels();
        int ii = 0;
        foreach (var x in colors)
        {
            colors[ii] = Color.black;
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