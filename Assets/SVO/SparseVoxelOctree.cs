using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class SparseVoxelOctree : MonoBehaviour
{
    public bool debug;
    public GameObject voxel;
    public bool[] occupancyMap;

    public Transform root;
    public GameObject cubePrefab;

    // Start is called befor the first frame update
    public List<GameObject> voxels;
    public List<Voxel> voxelsV;

    void placeVoxels()
    {

        voxels = new List<GameObject>();
        voxelsV = new List<Voxel>();
        Vector3 offset = root.transform.position;
        offset.x -= voxelSizeMeters / 2;
        offset.y -= voxelSizeMeters / 2;
        offset.z -= voxelSizeMeters / 2;

        cubePrefab.transform.localScale = new Vector3(voxelSizeMeters / 2, voxelSizeMeters / 2, voxelSizeMeters / 2);
        for (int x = 0; x < Mathf.RoundToInt(rootVoxelSizeMeters / voxelSizeMeters); ++x)
        {
            for (int y = 0; y < Mathf.RoundToInt(rootVoxelSizeMeters / voxelSizeMeters); ++y)
            {
                for (int z = 0; z < Mathf.RoundToInt(rootVoxelSizeMeters / voxelSizeMeters); ++z)
                {
                    Vector3 pos = new Vector3(x, y, z) * voxelSizeMeters;
                    GameObject g = Instantiate(cubePrefab, pos, Quaternion.identity);
                    g.transform.position += offset;
                    voxels.Add(g);
                    var v = g.GetComponent<Voxel>();
                    v.id = (int)(x + (y * voxelSizeMeters / 2) + (z * voxelSizeMeters / 2 * voxelSizeMeters / 2));
                    voxelsV.Add(v);
                    g.transform.parent = root;
                }
            }
        }
    }

    public void initialize(float leafHalfSize, float halfSize)
    {
        int oneRow = Mathf.RoundToInt(halfSize / leafHalfSize);
        int numberOfLeafVoxels = oneRow * oneRow * oneRow;
        int total_parents = 0; // Initialize total parents to zero

        int n = numberOfLeafVoxels;
        while (n > 1)
        {
            n = n / 8;
            int parents_at_this_level = n;
            total_parents += parents_at_this_level;
        }

        Debug.Log(total_parents);
        int arraySize = numberOfLeafVoxels + total_parents;
        occupancyMap = new bool[arraySize];

    }
    // Start is called before the first frame update
    public float voxelSizeMeters;
    public float rootVoxelSizeMeters;
    void Start()
    {
        initialize(voxelSizeMeters, rootVoxelSizeMeters);
        placeVoxels();
    }
    public string[] layers;
    int layerMask;
    public void Overlap(Vector3 pos, float3 halfsize, int arrayID)
    {
        //parent
        Collider[] colliders = Physics.OverlapBox(root.position, halfsize, root.rotation, layerMask);
        if (colliders.Length != 0)
        {

        }

    }
    public void UpdateTree()
    {

        Vector3 boxSize = new Vector3(rootVoxelSizeMeters, rootVoxelSizeMeters, rootVoxelSizeMeters);
        layerMask = LayerMask.GetMask(layers);
        Overlap(root.position, rootVoxelSizeMeters / 2, 0);

    }
    // Update is called once per frame
    void Update()
    {

    }
}
