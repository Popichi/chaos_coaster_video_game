using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class GravitySkyboxGenerator : MonoBehaviour
{
    public struct MyParticle
    {
        public float3 pos;
        public float3 vel;
        public float mass;
    }
    public struct MyNewParticle
    {
        public float3 pNew;
        public  float3 vNew;
    }
    // Start is called before the first frame update
    public int numberOfParticles = 100000;
    public Vector2 minMaxMass = new Vector2(0,1);
    MyParticle[] particles;
    ComputeBuffer particleBuffer;

    MyNewParticle[] particlesNew;
    ComputeBuffer particleNewBuffer;

    public ComputeShader computeShader;
    int kernel1;
    int kernel2;
    int kernel3;
    public Material material;
    public RenderTexture renderTexture;
    RenderTexture colorSumTexture2;
    public int sizeOfTexture = 4096;
    RenderTexture colorSumTexture;
    RenderTexture countTexture;







    [Range(4, 256)]
    public int resolution = 10;

    private void OnValidate()
    {
        //if (resolution < 4) resolution = 4;
        //GetComponent<MeshFilter>().mesh = GenerateSphere();
    }
    void OnRenderObject()
    {
        // Draw the particles
        material.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Points, numberOfParticles);
    }
    private Mesh GenerateSphere()
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[(resolution + 1) * (resolution + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[resolution * resolution * 6];

        float step = Mathf.PI / resolution;
        int vIndex = 0;
        int tIndex = 0;

        for (int lat = 0; lat <= resolution; lat++)
        {
            float theta = lat * step - Mathf.PI / 2;
            float y = Mathf.Sin(theta);
            float radius = Mathf.Cos(theta);

            for (int lon = 0; lon <= resolution; lon++)
            {
                float phi = lon * 2 * step - Mathf.PI;
                float x = Mathf.Cos(phi) * radius;
                float z = Mathf.Sin(phi) * radius;

                vertices[vIndex] = new Vector3(x, y, z);
                uv[vIndex] = new Vector2((float)lon / resolution, (float)lat / resolution);

                if (lat != resolution && lon != resolution)
                {
                    triangles[tIndex] = vIndex;
                    
                    triangles[tIndex + 1] = vIndex + resolution + 2;
                    triangles[tIndex + 2] = vIndex + resolution + 1;

                    
                    triangles[tIndex + 3] = vIndex + resolution + 2;
                    triangles[tIndex + 4] = vIndex;
                    triangles[tIndex + 5] = vIndex + 1;

                    tIndex += 6;
                }

                vIndex++;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        Vector3[] normals = mesh.normals;
        for (int i = 0; i < normals.Length; i++)
        {
            //normals[i] = -normals[i];
        }

        mesh.normals = normals;

        return mesh;
    }



    public Transform root;
    void Start()
    {




        //MeshFilter filter = GetComponent<MeshFilter>();
        //filter.mesh = GenerateSphere();
        if(root == null)
        {
            root = transform;
        }
        if (debug)
        {
            for (int i = 0; i < numberOfParticles; ++i)
            {
                GameObject g = Instantiate(pref);
                
                g.transform.parent = transform;
                prefs.Add(g);
            }
        }
        
       
        renderTexture = new RenderTexture(sizeOfTexture, sizeOfTexture, 24);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();
        
        //material.mainTexture = renderTexture;


        threadGroupsX = material.mainTexture.width / 8;
        threadGroupsY = material.mainTexture.height / 8;

        particles = new MyParticle[numberOfParticles];
        for(int i = 0; i < particles.Length;++i)
        {
            if(i == 0)
            {
                particles[i].pos = new Vector3(((UnityEngine.Random.value - 0.5f) * 2), ((UnityEngine.Random.value - 0.5f) * 2), ((UnityEngine.Random.value - 0.5f) * 2));
                particles[i].vel = UnityEngine.Random.insideUnitSphere * 0f;
                particles[i].mass = 1000;
                continue;
            }

            particles[i].pos = new Vector3(((UnityEngine.Random.value - 0.5f) * 2), ((UnityEngine.Random.value - 0.5f) * 2), ((UnityEngine.Random.value - 0.5f) * 2));
            particles[i].vel = UnityEngine.Random.insideUnitSphere*1f;
            particles[i].mass = UnityEngine.Random.Range(minMaxMass.x, minMaxMass.y);
        }


        //---------------------------
        colorSumTexture = new RenderTexture(sizeOfTexture, sizeOfTexture, 0, RenderTextureFormat.RInt);
        colorSumTexture.enableRandomWrite = true;
        colorSumTexture.Create();
        colorSumTexture2 = new RenderTexture(sizeOfTexture, sizeOfTexture, 0, RenderTextureFormat.RInt);
        colorSumTexture2.enableRandomWrite = true;
        colorSumTexture2.Create();

        countTexture = new RenderTexture(sizeOfTexture, sizeOfTexture, 0, RenderTextureFormat.RInt);
        countTexture.enableRandomWrite = true;
        countTexture.Create();

     



        // Clear the ColorSumTexture and CountTexture to initialize them



        // Assign the textures to the material
      


        //---------


        particleBuffer = new ComputeBuffer(numberOfParticles, sizeof(float) * 7);
        particleBuffer.SetData(particles);

        particlesNew = new MyNewParticle[numberOfParticles];
        particleNewBuffer = new ComputeBuffer(numberOfParticles, sizeof(float) * 6);
        particleNewBuffer.SetData(particlesNew);

        kernel1 = computeShader.FindKernel("CSMain");
        kernel2 = computeShader.FindKernel("CSMap");
        kernel3 = computeShader.FindKernel("CSRed");

        computeShader.SetBuffer(kernel2, "particleBuffer", particleBuffer);
        computeShader.SetBuffer(kernel2, "particleNewBuffer", particleNewBuffer);

        computeShader.SetBuffer(kernel1, "particleBuffer", particleBuffer);
        computeShader.SetBuffer(kernel1, "particleNewBuffer", particleNewBuffer);
        material.SetBuffer("particleBuffer", particleBuffer);

        computeShader.SetTexture(kernel3, "Result", renderTexture);
        computeShader.SetTexture(kernel2, "Result", renderTexture);


        // Set the textures in the compute shader
        computeShader.SetTexture(kernel2, "ColorSumTexture", colorSumTexture);
        computeShader.SetTexture(kernel2, "CountTexture", countTexture);
        computeShader.SetTexture(kernel3, "ColorSumTexture", colorSumTexture);
        computeShader.SetTexture(kernel3, "CountTexture", countTexture);

        computeShader.SetTexture(kernel3, "ColorSumTexture2", colorSumTexture2);
        computeShader.SetTexture(kernel2, "ColorSumTexture2", colorSumTexture2);

    }
    private void OnDestroy()
    {
        particleBuffer.Release();
        particleNewBuffer.Release();
    }
    // Update is called once per frame


    int threadGroupsX;
    int threadGroupsY;
    public GameObject pref;
    List<GameObject> prefs = new List<GameObject>();
    public bool debug;
    public float size = 10;
    public float minimalDistance = 100;
    public float maxDistanceDelta = 100;
    public float frequence = 100;
    public float strength;
    void Update()
    {

        computeShader.SetFloat("time", Time.deltaTime);
        computeShader.SetFloat("radiusD", minimalDistance);
        computeShader.SetFloat("radiusDMax", minimalDistance+maxDistanceDelta);
        computeShader.SetFloat("strength", strength);
        material.SetVector("parent", new float4(root.position,0));
        computeShader.Dispatch(kernel1, numberOfParticles / 1024 + 1, 1, 1);
        material.SetFloat("size",size);
        material.SetFloat("time", Time.time);
        material.SetFloat("frequence", frequence);
        computeShader.Dispatch(kernel2, numberOfParticles / 1024 + 1, 1, 1);
        if (debug)
        {
            particleBuffer.GetData(particles);
            for (int i = 0; i < numberOfParticles; ++i)
            {
                prefs[i].transform.position = particles[i].pos;

            }
        }
  
        

        //computeShader.Dispatch(kernel3, threadGroupsX, threadGroupsY, 1);
    }
}
