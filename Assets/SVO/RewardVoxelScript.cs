using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardVoxelScript : MonoBehaviour
{
    public DiscoverRewarding discoverRewarding;
    Vector3 position;
    Vector3 pos;
    public bool minSize;
    float halfSize;
    // Start is called before the first frame update

    void Extend(GameObject other)
    {
        Iid id = other.GetComponentInParent<Iid>();
        if (minSize || (id != null && discoverRewarding.myID.GetID() != id.GetID()))
        {
            return;
        }
        if(minSize && (id != null && discoverRewarding.myID.GetID() == id.GetID()))
        {
            discoverRewarding.agent.AddReward(-discoverRewarding.rewardFactor);
            return;
        }
        halfSize = transform.localScale.x / 2;
        if (halfSize - 0.01f <= discoverRewarding.minSize)
        {
            minSize = true;
            return;
        }

        discoverRewarding.agent.AddReward(halfSize * discoverRewarding.rewardFactor);
        position = transform.position;


        spawnVoxel(1, 1, 1);
        spawnVoxel(1, 1, -1);
        spawnVoxel(1, -1, 1);
        spawnVoxel(1, -1, -1);

        spawnVoxel(-1, 1, 1);
        spawnVoxel(-1, 1, -1);
        spawnVoxel(-1, -1, 1);
        spawnVoxel(-1, -1, -1);

        discoverRewarding.DeleteVoxel(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        Extend(other.gameObject);
    }
    RewardVoxelScript spawnVoxel(float x, float y, float z)
    {
        pos = position + (x * transform.right + z * transform.forward + y * transform.up) * halfSize/2;
        return discoverRewarding.SpawnVoxel(pos, transform.rotation,halfSize);
    }
}
