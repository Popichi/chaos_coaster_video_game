using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCover : MonoBehaviour
{
    List<GameObject> covers;
    public int numberCovers = 12;
    public GameObject coverPrefab;
    public Transform trainingGround;
    TargetController targetController;
    // Start is called before the first frame update
    void Start()
    {
        targetController = FindAnyObjectByType<TargetController>();
            covers = new List<GameObject>();
            for (int i = 0; i < numberCovers; ++i)
            {
                covers.Add(Instantiate(coverPrefab));
                covers[i].transform.parent = trainingGround;
            }
        Respawn();
        
    }
    public void Respawn()
    {
        numberA = numberCovers;
        for (int i = 0; i < numberCovers; ++i)
        {
            covers[i].active = true;
            Rigidbody r = covers[i].GetComponent<Rigidbody>();
            r.velocity = Vector3.zero;
            r.angularVelocity = Vector3.zero;
            r.transform.rotation = Quaternion.Euler(0, Random.value * 360, 0);
            covers[i].transform.position = targetController.getRandom.randomPosOnGrid(2);
        }
    }
    // Update is called once per frame
    int numberA = 0;
    public int maxFrames = 5000;
    int frame;
    public float percent = 0.7f;
    void FixedUpdate()
    {
        frame++;
        if (frame > maxFrames)
        {
            frame = 0;
            Respawn();
        }
        for (int i = 0; i < numberCovers; ++i)
        {
            if (covers[i].transform.position.y < -100)
            {
                covers[i].active = false;
                numberA--;
            }

        }
        float f = (1.0f * numberA) / numberCovers;
        if (f < percent)
        {
            Respawn();
        }
    }
}
