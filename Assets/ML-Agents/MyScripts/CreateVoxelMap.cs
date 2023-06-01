using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.MLAgents.Sensors;
public class CreateVoxelMap : MonoBehaviour
{
    public Transform root;
    public GameObject cube;
    public float voxelSize;
    public int3 size;
   
    // Start is called before the first frame update
    public List<GameObject> voxels ;
    public List<Voxel> voxelsV;
    public bool[] occupancyMap;

    public void GetObservation(VectorSensor s)
    {
        foreach(var a in occupancyMap)
        {
            s.AddObservation(a);
        }

    }
    void Start()
    {

        voxels = new List<GameObject>();
        voxelsV = new List<Voxel>();
        Vector3 offset = root.transform.position;
        offset.x -= size.x / 2.0f;
        offset.y -= size.y / 2.0f;
        offset.z -= size.z / 2.0f;
        int n = 0;
        cube.transform.localScale = new Vector3(voxelSize, voxelSize, voxelSize);
        for(int x = 0; x < Mathf.RoundToInt(size.x / voxelSize); ++x)
        {
            for (int y = 0; y < Mathf.RoundToInt(size.y / voxelSize); ++y)
            {
                for (int z = 0; z < Mathf.RoundToInt(size.z / voxelSize); ++z)
                {
                    n++;
                    Vector3 pos = new Vector3(x,y,z)*voxelSize;
                    GameObject g = Instantiate(cube, pos, Quaternion.identity);
                    g.transform.position += offset;
                    voxels.Add(g);
                    var v = g.GetComponent<Voxel>();
                    v.id = x + (y * size.x) + (z * size.x * size.y);
                    v.gameObject.name = "Voxel" + v.id;
                    voxelsV.Add(v);
                    g.transform.parent = root;
                    v.map = this;
                }
            }
        }
        occupancyMap = new bool[n];

    }

    // Update is called once per frame

}
