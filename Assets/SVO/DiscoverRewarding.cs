using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class DiscoverRewarding : MonoBehaviour
{
    public Iid myID;
    public GameObject RewardVoxel;
    public GameObject initVoxel;
    public List<RewardVoxelScript> rewardVoxelScripts;
    public float initSize = 10;
    public Agent agent;
    public float minSize = 1f;

    public float rewardFactor = 0.05f;
    // Start is called before the first frame update

    public void reset()
    {
        for(int i = 0; i < rewardVoxelScripts.Count; ++i)
        {
            var a = rewardVoxelScripts[i];
            Destroy(a.gameObject);
            rewardVoxelScripts[i] = null;
        }
        rewardVoxelScripts.Clear();
    }
    private void Start()
    {
        rewardVoxelScripts = new List<RewardVoxelScript>();
        if(transform.parent)
        myID = transform.parent.GetComponentInChildren<Iid>();
        if(myID == null)
        {
            Debug.LogWarning("myiDnull");
            
        }
        if (!initVoxel)
        {
            SpawnVoxel(transform.position, transform.rotation, initSize);
        }
        else
        {

            AddVoxel(initVoxel);
        }
        

    }
    public RewardVoxelScript SpawnVoxel(Vector3 pos, Quaternion rot, float size)
    {
        GameObject g = Instantiate(RewardVoxel, pos, rot);
        g.transform.localScale = new Vector3(size, size, size);
        return AddVoxel(g);
    }
    public RewardVoxelScript init(RewardVoxelScript r)
    {
        r.discoverRewarding = this;
        r.transform.parent = transform;
        return r;
    }
    public RewardVoxelScript AddVoxel(RewardVoxelScript r)
    {   
       
        rewardVoxelScripts.Add(r);
        r = init(r);
        return r;
    }
    public RewardVoxelScript AddVoxel(GameObject r)
    {
        RewardVoxelScript rr = r.GetComponent<RewardVoxelScript>();
        rewardVoxelScripts.Add(rr);
        rr = init(rr);
        return rr;
    }
    public void DeleteVoxel(RewardVoxelScript r)
    {
        rewardVoxelScripts.Remove(r);
        Destroy(r.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        




    }


}
