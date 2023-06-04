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
    void Start()
    {
        threadGroupsX = material.mainTexture.width / 8;
        threadGroupsY = material.mainTexture.height / 8;
        renderTexture.enableRandomWrite = true;



       


        particles = new MyParticle[numberOfParticles];
        for(int i = 0; i < particles.Length;++i)
        {
            particles[i].pos = new Vector3(((UnityEngine.Random.value - 0.5f) * 2), ((UnityEngine.Random.value - 0.5f) * 2), ((UnityEngine.Random.value - 0.5f) * 2));
            particles[i].vel = UnityEngine.Random.insideUnitSphere*0.1f;
            particles[i].mass = UnityEngine.Random.Range(minMaxMass.x, minMaxMass.y);
        }
        particleBuffer = new ComputeBuffer(numberOfParticles, sizeof(float) * 7);
        particleBuffer.SetData(particles);

        particlesNew = new MyNewParticle[numberOfParticles];
        particleNewBuffer = new ComputeBuffer(numberOfParticles, sizeof(float) * 6);
        particleNewBuffer.SetData(particlesNew);

        kernel1 = computeShader.FindKernel("CSMain");
        kernel2 = computeShader.FindKernel("CSMap");
        kernel2 = computeShader.FindKernel("CSRed");

        computeShader.SetBuffer(kernel2, "particleBuffer", particleBuffer);
        computeShader.SetBuffer(kernel2, "particleNewBuffer", particleNewBuffer);

        computeShader.SetBuffer(kernel1, "particleBuffer", particleBuffer);
        computeShader.SetBuffer(kernel1, "particleNewBuffer", particleNewBuffer);

        computeShader.SetTexture(kernel3, "Result", renderTexture);
        computeShader.SetTexture(kernel2, "Result", renderTexture);



    }
    private void OnDestroy()
    {
        particleBuffer.Release();
        particleNewBuffer.Release();
    }
    // Update is called once per frame


    int threadGroupsX;
    int threadGroupsY;
    void Update()
    {
        computeShader.SetFloat("time", Time.deltaTime);
        computeShader.Dispatch(kernel1, numberOfParticles / 1024 + 1, 1, 1);

        computeShader.Dispatch(kernel2, numberOfParticles / 1024 + 1, 1, 1);

        computeShader.Dispatch(kernel3, threadGroupsX, threadGroupsY, 1);
    }
}
