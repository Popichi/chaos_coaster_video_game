using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class SensingTouch : MonoBehaviour
{
    
    public int sizeX = 20;
    public int sizeY = 20;
    public Texture2D feelTex;
    public Collider c;
    //public RenderTexture render;
    Transform colliderT;
    public float sensitivity = 100f;
    Vector3 bounds;
    Vector3 midBound;
    
    public int id = 0;
    // Start is called before the first frame update
    //Renderer renderer;
    public wraper mainWraper;

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
    RenderTextureSensor sensor;

    public int stack = 2;
    void Awake()
    {
        
        feelTex = new Texture2D(sizeX, sizeY, TextureFormat.RGB24, false);
        mainWraper = new wraper(feelTex, true);
        mainWraper.resetPixels();

        Debug.Log("size of Texture" + feelTex.height);

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


        //Debug

        //render = new RenderTexture(sizeX, sizeY, 0, RenderTextureFormat.ARGB32);
        if (debugCanvas != null)
        {
           // renderer = debugCanvas.GetComponent<Renderer>();
            //renderer.material.mainTexture = render;
        }
        
        string s = "Sensor" + id;
        id++;
        //sensor = GetComponent<RenderTextureSensor>();
        if (useCNN)
        {
            //sensor = new RenderTextureSensor(render, true, s, SensorCompressionType.None);
            SkinSensorComponent r = gameObject.AddComponent<SkinSensorComponent>();
            r.sensingTouch = this;
            r.Grayscale = true;
            //r.RenderTexture = render;
            r.texture2D = mainWraper.feelTex;
            r.CompressionType = SensorCompressionType.PNG;
            r.SensorName = s;
            r.ObservationStacks = stack;
        }
        
        

        

        
        
            
        //render = new RenderTexture(sizeX, sizeY, 17.RFloat, false);
    }

    // Update is called once per frame
    
    
    public bool useCNN = true;
    public class wraper 
    {
        public void reducePixels(float factor)
        {
            if (useCNN)
            {
                var a = feelTex.GetPixels();

                for (int i = 0; i < a.Length; ++i)
                {

                    a[i].r *= factor;
                }
                feelTex.SetPixels(a);
            }
            else
            {
                int pp = 0;
                foreach (var x in tex)
                {
                    int y = pp / width;
                    tex[pp - (y * width), y].r *= factor;
                    pp++;
                }
            }
            
        }
        public void AddObservation()
        {
            Apply();
            //Graphics.Blit(feelTex, t);
            resetPixels();
        }
        public void Apply(bool a=false)
        {
            if (useCNN)
            {
                feelTex.Apply(a);
            }

        }
        public void resetPixels(bool apply = false)
        {
            if (useCNN)
            {
                Color[] colors = this.feelTex.GetPixels();
                int ii = 0;
                foreach (var x in colors)
                {
                    colors[ii] = new Color(0, 0, 0);
                    ii++;
                }
                feelTex.SetPixels(colors);
                if (apply)
                {
                    feelTex.Apply();
                }
            }
            else
            {
                int pp = 0;
                foreach (var x in tex)
                {
                    int y = pp / width;
                    tex[pp-(y*width), y] = new Color(0, 0, 0);
                    pp++;
                }
            }
            
        }
        public int width
        {
            get { if (useCNN) { return feelTex.width; } else { return tex.GetLength(0); } }
        }
            
        public int height
        {
            get { if (useCNN) { return feelTex.height; } else { return tex.GetLength(1); } }
            
        }
        public bool useCNN = true;
        public wraper(Texture2D t, bool useCNN)
        {
            feelTex = t;
            this.useCNN = useCNN;
            tex = new Color[feelTex.width, feelTex.height];

        }
        public void SetPixel(int x, int y, Color c)
        {
            if (useCNN)
            {
                feelTex.SetPixel(x, y, c);
            }
            else
            {
                tex[x, y] = c;
            }
        }
        public Color GetPixel(int x, int y)
        {
            if (useCNN)
            {
               return feelTex.GetPixel(x, y);
            }
            else
            {
                return tex[x,y];
            }
        }
        public void SetObservation()
        {
            if (useCNN)
            {

            }
        }
        public Texture2D feelTex;
        public Color[,] tex; 

    }

    public void bilinear(Vector2 uv, wraper myWraper, float value)
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

        Color tmpC = myWraper.GetPixel(xI, yI);
        float result = (1 - a) * (1 - b) * value;
        tmpC.r += result;

        tmpC.r = Mathf.Clamp01(tmpC.r);
        myWraper.SetPixel(xI, yI, tmpC);

        if ((xI + 1 < myWraper.width))
        {
            tmpC = myWraper.GetPixel(xI + 1, yI);
            result = (a) * (1 - b) * value;
            tmpC.r += result;
            tmpC.r = Mathf.Clamp01(tmpC.r);
            myWraper.SetPixel(xI + 1, yI, tmpC);
        }

        if ((yI + 1 < myWraper.height))
        {
            tmpC = myWraper.GetPixel(xI, yI + 1);
            result = (1 - a) * (b) * value;
            tmpC.r += result;
            tmpC.r = Mathf.Clamp01(tmpC.r);
            myWraper.SetPixel(xI, yI + 1, tmpC);
        }

        if ((yI + 1 < myWraper.height) && (xI + 1 < myWraper.width))
        {
            tmpC = myWraper.GetPixel(xI + 1, yI + 1);
            result = (a) * (b) * value;
            tmpC.r += result;
            tmpC.r = Mathf.Clamp01(tmpC.r);
            myWraper.SetPixel(xI + 1, yI + 1, tmpC);
        }
    }
    public void nearest(Vector2 uv, wraper myWraper, float value)
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
        myWraper.SetPixel(xI, yI, new Color(value, 0, 0));
    }
    public void setPixel(Vector2 uv, float intensity, wraper myWraper)
    {

        intensity = Mathf.Clamp01(intensity / sensitivity);
        bilinear(uv, myWraper, intensity);
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
    public side closestBoxDistance(Vector3 point) // local world
    {
        float minD = float.MaxValue;
        side tmp = side.error;
        if (point.x > 0 && minD > bounds.x - point.x)
        {
            tmp = side.right;
            minD = bounds.x - point.x;
            if (minD < 0)
            {
                //Debug.LogError("distance negative: " + minD);
            }
        }
        if (point.x <= 0 && minD > bounds.x - Mathf.Abs(point.x))
        {
            tmp = side.left;
            minD = bounds.x - Mathf.Abs(point.x);
            if (minD < 0)
            {
                //Debug.LogError("distance negative: " + minD);
            }
        }
        if (point.y > 0 && minD > bounds.y - point.y)
        {
            tmp = side.top;
            minD = bounds.y - point.y;
            if (minD < 0)
            {
                //Debug.LogError("distance negative: " + minD);
            }
        }
        if (point.y <= 0 && minD > bounds.y - Mathf.Abs(point.y))
        {
            tmp = side.button;
            minD = bounds.y - Mathf.Abs(point.y);
            if (minD < 0)
            {
                //Debug.LogError("distance negative: " + minD);
            }
        }
        if (point.z > 0 && minD > bounds.z - point.z)
        {
            tmp = side.back;
            minD = bounds.z - point.z;
            if (minD < 0)
            {
                //Debug.LogError("distance negative: " + minD);
            }
        }
        if (point.z <= 0 && minD > bounds.z - Mathf.Abs(point.z))
        {
            tmp = side.front;
            minD = bounds.z - Mathf.Abs(point.z);
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
    public string[] reactOn;
    private void OnCollisionStay(Collision collision)
    {
        bool react = false;
        string ss = collision.gameObject.tag;
        foreach(var x in reactOn)
        {
            if (ss.Equals(x))
            {
                react = true;
                break;
            }
        }
        if (react)
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
                setPixel(uv, intensity, mainWraper);
                //Debug.Log("Setting PIxel, uv=" + uv + " intensity=" + intensity + " side: " + s);

                //get uv of local Point


                //Vector3 n = (nearest - transform.position).normalized;
                //nearest += n * 0.01f;
                //Debug.Log("Contact at " + nearest);
                ////hits = Physics.RaycastAll(x.point, -n, 0.05f);
                //if (Physics.Raycast(nearest, -n, out hit, 0.012f))
                //{
                //    Vector2 uv = new Vector2(hit.textureCoord.x, hit.textureCoord.y);
                //    Debug.Log("uv0=" + hit.textureCoord);
                //    Debug.Log("uv1=" + hit.textureCoord2);
                //    setPixel(uv, intensity);
                //    colision = true;
                //    Debug.Log("Hit, Intensity:" + intensity);
                //    Debug.DrawLine(nearest, nearest - n* intensity, Color.blue, 5); 
                //}


            }
            mainWraper.Apply();
        }
    }
    
}
