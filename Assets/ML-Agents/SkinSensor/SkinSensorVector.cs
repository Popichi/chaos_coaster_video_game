using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;

public class SkinSensorVector : MonoBehaviour
{
    public int sizeX = 12;
    public int sizeY = 12;
    public int dimension;
    
    public Collider c;
    
    Transform colliderT;
    public float sensitivity = 0.1f;
    Vector3 bounds;
    Vector3 midBound;

    public int id = 0;
    // Start is called before the first frame update
    //Renderer renderer;
    //wraper mainWraper;

    //public RenderTextureSensor sensor;
    public Bounds TransformBoundsToLocal(Transform _transform, Bounds _globalBound)
    {
        var center = _transform.InverseTransformPoint(_globalBound.center);

        // transform the local extents' axes
        var extents = _globalBound.extents;
        var axisX = _transform.InverseTransformVector(extents.x, 0, 0);
        var axisY = _transform.InverseTransformVector(0, extents.y, 0);
        var axisZ = _transform.InverseTransformVector(0, 0, extents.z);

        // sum their absolute value to get the world extents
        extents.x = Mathf.Abs(axisX.x) + Mathf.Abs(axisY.x) + Mathf.Abs(axisZ.x);
        extents.y = Mathf.Abs(axisX.y) + Mathf.Abs(axisY.y) + Mathf.Abs(axisZ.y);
        extents.z = Mathf.Abs(axisX.z) + Mathf.Abs(axisY.z) + Mathf.Abs(axisZ.z);

        return new Bounds { center = center, extents = extents };
    }
    Bounds localBounds;
    public GameObject debugCanvas;
    //RenderTextureSensor sensor;


    NativeArray<float> feelTex1;
    NativeArray<float> lastFrameTex;
    NativeArray<float> feelTex2derivate;
    NativeArray<float> tmpArray;

    struct SetToValueJob : IJobParallelFor
    {
        public NativeArray<float> values;
        public float value;

        public void Execute(int index)
        {
            
            values[index] = value;
        }
    }
    struct CalculateDerivateJob : IJobParallelFor
    {
        public NativeArray<float> valuesThisFrame;
        public NativeArray<float> valuesLastFrame;
        public NativeArray<float> res;
        

        public void Execute(int index)
        {

            res[index] = math.clamp(valuesThisFrame[index] -  valuesLastFrame[index],-1,1);
        }
    }
    void Awake()
    {


        //feelTex = new Texture2D(sizeX, sizeY, TextureFormat.RFloat, false);
        c = GetComponent<Collider>();

        colliderT = c.transform;
        localBounds = TransformBoundsToLocal(colliderT, c.bounds);
        bounds = localBounds.extents;

        if (bounds.x == 0)
        {
            Debug.LogWarning("bounds x zero");
            bounds.x = 0.1f;
        }
        if (bounds.y == 0)
        {
            Debug.LogWarning("bounds y zero");
            bounds.y = 0.1f;
        }
        if (bounds.z == 0)
        {
            Debug.LogWarning("bounds z zero");
            bounds.z = 0.1f;
        }
        midBound = localBounds.center;



        string s = "Sensor" + id;
        id++;
        //sensor = GetComponent<RenderTextureSensor>();







        //render = new RenderTexture(sizeX, sizeY, 17.RFloat, false);
    }
    public void Start()
    {
        feelTex1 = new NativeArray<float>(sizeX * sizeY, Allocator.Persistent);
        lastFrameTex = new NativeArray<float>(sizeX * sizeY, Allocator.Persistent);
        feelTex2derivate = new NativeArray<float>(sizeX * sizeY, Allocator.Persistent);
        tmpArray = new NativeArray<float>(sizeX * sizeY, Allocator.Persistent);
    }

    // Update is called once per frame
    JobHandle h1, h2, h3, h4;
    public void reset()
    {
        h1.Complete();
        h2.Complete();
        h3.Complete();
        //Debug.LogError("length" + feelTex1.Length);
        SetToValueJob j = new SetToValueJob();
        j.value = 0;
        j.values = feelTex1;
        
        
        
        SetToValueJob j2 = new SetToValueJob();
        j2.value = 0;
        j2.values = feelTex2derivate;
        
        
        SetToValueJob j3 = new SetToValueJob();
        j3.value = 0;
        j3.values = lastFrameTex;
        

        h2 = j2.Schedule(feelTex2derivate.Length, 1);
        h3 = j3.Schedule(lastFrameTex.Length, 1);
        h1 = j.Schedule(feelTex1.Length, 1);

        

    }
    public void AddObservation(VectorSensor sensor)
    {
        h1.Complete();
        h2.Complete();
        h3.Complete();
        h4.Complete();
        for (int i = 0; i < feelTex1.Length; ++i)
        {
            sensor.AddObservation(feelTex1[i]);
        }
        CalculateDerivateJob c = new CalculateDerivateJob();
        c.res = feelTex2derivate;
        c.valuesLastFrame = lastFrameTex;
        c.valuesThisFrame = feelTex1;
        JobHandle handle = c.Schedule(feelTex1.Length, 1);
        handle.Complete();
        for (int i = 0; i < feelTex1.Length; ++i)
        {
            sensor.AddObservation(feelTex2derivate[i]);
        }


        tmpArray = lastFrameTex;
        lastFrameTex = feelTex1;
        feelTex1 = tmpArray;
        SetToValueJob j = new SetToValueJob();
        j.value = 0;
        j.values = feelTex1;
        JobHandle handle2 = j.Schedule(feelTex1.Length, 1);
        handle2.Complete();

    }

    public float getF(int x, int y, NativeArray<float> t)
    {
        return t[x + y * sizeX];
    }
    public void setF(int x, int y, NativeArray<float> t, float v)
    {
        t[x + y * sizeX] = v;
    }
    public void bilinear(Vector2 uv, float value, NativeArray<float> feelTex1)
    {
        uv.x = Mathf.Clamp01(uv.x);
        uv.y = Mathf.Clamp01(uv.y);
        // y axis down, x right
        Vector2 uvTex = uv;

        uvTex.x *= (float)(sizeX - 1);
        uvTex.y *= (float)(sizeY - 1);

        float x = Mathf.Floor(uvTex.x);
        float y = Mathf.Floor(uvTex.y);
        int xI = Mathf.FloorToInt(x);
        int yI = Mathf.FloorToInt(y);

        float b = uvTex.y - y;
        float a = uvTex.x - x;
        //Debug.Log("uvx " + uvTex.x + "uv y " + uvTex.y + " XI " + xI + " yI " + yI);

        float tmpC = getF(xI,yI,feelTex1);
        float result = (1 - a) * (1 - b) * value;
        tmpC += result;

        tmpC = Mathf.Clamp01(tmpC);
        setF(xI, yI, feelTex1, tmpC);

        if ((xI + 1 < sizeX))
        {
            tmpC = getF(xI+1, yI, feelTex1);
            result = (a) * (1 - b) * value;
            tmpC += result;
            tmpC = Mathf.Clamp01(tmpC);
            setF(xI + 1, yI,feelTex1, tmpC);
        }

        if ((yI + 1 < sizeY))
        {
            tmpC = getF(xI, yI + 1, feelTex1);
            result = (1 - a) * (b) * value;
            tmpC += result;
            tmpC = Mathf.Clamp01(tmpC);
            setF(xI, yI + 1,feelTex1, tmpC);
        }

        if ((yI + 1 < sizeY) && (xI + 1 < sizeX))
        {
            tmpC = getF(xI + 1, yI + 1,feelTex1);
            result = (a) * (b) * value;
            tmpC += result;
            tmpC = Mathf.Clamp01(tmpC);
            setF(xI + 1, yI + 1,feelTex1, tmpC);
        }
    }
    public void nearest(Vector2 uv, float value)
    {
        uv.x = Mathf.Clamp01(uv.x);
        uv.y = Mathf.Clamp01(uv.y);
        // y axis down, x right
        Vector2 uvTex = uv;

        uvTex.x *= sizeX - 1;
        uvTex.y *= sizeY - 1;

        float x = Mathf.Floor(uvTex.x);
        float y = Mathf.Floor(uvTex.y);
        int xI = Mathf.FloorToInt(x);
        int yI = Mathf.FloorToInt(y);
        setF(xI, yI,feelTex1, value);
    }
    public void setPixel(Vector2 uv, float intensity)
    {

        intensity = Mathf.Clamp01(intensity / sensitivity);
        bilinear(uv, intensity, feelTex1);
        //bilinear(uv, feelTex, intensity);



    }
    bool colision = false;
    const int size = 10;
    ContactPoint[] contacts = new ContactPoint[size];
    public enum side
    {
        front,
        back,
        left,
        right,
        top,
        button, error
    }
    public side closestBoxDistance(Vector3 point1) // local world
    {
        float minD = float.MaxValue;
        side tmp = side.error;
        if (point1.x > 0 && minD > bounds.x - point1.x)
        {
            tmp = side.right;
            minD = bounds.x - point1.x;
            if (minD < 0)
            {
                //Debug.LogError("distance negative: " + minD);
            }
        }
        if (point1.x <= 0 && minD > bounds.x - Mathf.Abs(point1.x))
        {
            tmp = side.left;
            minD = bounds.x - Mathf.Abs(point1.x);
            if (minD < 0)
            {
                //Debug.LogError("distance negative: " + minD);
            }
        }
        if (point1.y > 0 && minD > bounds.y - point1.y)
        {
            tmp = side.top;
            minD = bounds.y - point1.y;
            if (minD < 0)
            {
                //Debug.LogError("distance negative: " + minD);
            }
        }
        if (point1.y <= 0 && minD > bounds.y - Mathf.Abs(point1.y))
        {
            tmp = side.button;
            minD = bounds.y - Mathf.Abs(point1.y);
            if (minD < 0)
            {
                //Debug.LogError("distance negative: " + minD);
            }
        }
        if (point1.z > 0 && minD > bounds.z - point1.z)
        {
            tmp = side.back;
            minD = bounds.z - point1.z;
            if (minD < 0)
            {
                //Debug.LogError("distance negative: " + minD);
            }
        }
        if (point1.z <= 0 && minD > bounds.z - Mathf.Abs(point1.z))
        {
            tmp = side.front;
            minD = bounds.z - Mathf.Abs(point1.z);
            if (minD < 0)
            {
                //Debug.LogError("distance negative: " + minD);
            }
        }



        return tmp;
    }
    /// <summary>
    /// returns the deformed uv coords for the texture
    /// </summary>
    /// <param name="s"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    private Vector2 getUVfromLocalPos(side s, Vector3 point)
    {
        Vector2 res = Vector2.zero;
        switch (s)
        {
            case side.right:
                {
                    //yundz
                    //scale to get values [0,1]
                    res.x = ((point.z / bounds.z) + 1) / 2;
                    res.y = ((point.y / bounds.y) + 1) / 2;
                    res.x /= 3;
                    res.y /= 4;
                    res.x += 2.0f / 3;
                    //res.y = 1 - res.y;
                    res.y += 1 / 4.0f;
                    res.y = 1 - res.y;

                    return res;

                }
            case side.left:
                {
                    //yundz
                    //scale to get values [0,1]
                    res.x = ((point.z / bounds.z) + 1) / 2;
                    res.y = ((point.y / bounds.y) + 1) / 2;
                    res.x /= 3;
                    res.y /= 4;
                    //res.x += 2.0f / 3;
                    //res.y = 1 - res.y;
                    res.y += 1 / 4.0f;
                    res.y = 1 - res.y;

                    return res;

                }
            case side.top:
                {
                    //xundz
                    //scale to get values [0,1]
                    res.x = ((point.x / bounds.x) + 1) / 2;
                    res.y = ((point.z / bounds.z) + 1) / 2;
                    res.x /= 3;
                    res.y /= 4;
                    res.x += 1.0f / 3;
                    //res.y += 1 / 4.0f;
                    //res.y = 1 - res.y;
                    res.y = 1 - res.y;
                    return res;

                }

            case side.button:
                {
                    //yundz
                    //scale to get values [0,1]
                    res.x = ((point.x / bounds.x) + 1) / 2;
                    res.y = ((point.z / bounds.z) + 1) / 2;
                    res.x /= 3;
                    res.y /= 4;
                    res.x += 1.0f / 3;
                    //res.y = 1 - res.y;
                    res.y += 2 / 4.0f;
                    res.y = 1 - res.y;
                    return res;

                }
            case side.front:
                {
                    //yundz
                    //scale to get values [0,1]
                    res.x = ((point.x / bounds.x) + 1) / 2;
                    res.y = ((point.y / bounds.y) + 1) / 2;
                    res.x /= 3;
                    res.y /= 4;
                    res.x += 1.0f / 3;
                    //res.y = 1 - res.y;
                    res.y += 1 / 4.0f;
                    res.y = 1 - res.y;

                    return res;

                }
            case side.back:
                {
                    //yundz
                    //scale to get values [0,1]
                    res.x = ((point.x / bounds.x) + 1) / 2;
                    res.y = ((point.y / bounds.y) + 1) / 2;
                    res.x /= 3;
                    res.y /= 4;
                    res.x += 1.0f / 3;

                    res.y += 3 / 4.0f;
                    res.y = 1 - res.y;

                    return res;

                }
            default: break;
        }
        return res;
    }
    private void OnCollisionStay(Collision collision)
    {
        //Debug.Log("Collision");
        colision = false;

        int number = collision.GetContacts(contacts);
        float intensity = collision.impulse.magnitude / (float)number;
        for (int i = 0; i < number; ++i)
        {

            //RaycastHit hit;
            Vector3 nearest = c.ClosestPoint(contacts[i].point);
            Vector3 local = colliderT.InverseTransformPoint(nearest);
            local -= midBound;
            side s = closestBoxDistance(local);
            Vector2 uv = getUVfromLocalPos(s, local);
            h1.Complete();
            setPixel(uv, intensity);

        }
        

    }
}
