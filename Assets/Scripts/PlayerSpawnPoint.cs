using PathCreation.Examples;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// TODO fix. player just spawns outside and falls
public class PlayerSpawnPoint : MonoBehaviour
{
    public GameObject[] spawnLocations;
    public GameObject player;
    private bool spawned = false;
    public GameObject follower;

    private Vector3 respawnLocation;
    private void Awake()
    {
        spawnLocations = GameObject.FindGameObjectsWithTag("spawnpoint");
        follower = GameObject.FindGameObjectWithTag("follower");
    }
    // Start is called before the first frame update
    void Start()
    {
        player = (GameObject)Resources.Load("PlayerAndCamera", typeof(GameObject));
        respawnLocation = player.transform.position;
        StartCoroutine(SpawnPlayer());
    }

    // Update is called once per frame
    void Update()
    {
        if (!spawned && follower.GetComponent<PathFollower>().isMoving == true)
        {
            Debug.Log("spawned" + spawned);
            Debug.Log("follower" + follower.GetComponent<PathFollower>().isMoving);
            
            
            spawned = true;
        }        
    }

    private IEnumerator SpawnPlayer()
    {
        yield return new WaitForSeconds(5);
        spawnLocations = GameObject.FindGameObjectsWithTag("spawnpoint");
        int spawn = Random.Range(0, spawnLocations.Length);
        Debug.Log("Spawning @ " + spawnLocations[spawn].transform.position);
        player = Instantiate(player, spawnLocations[spawn].transform.position, Quaternion.identity);
        player.transform.SetParent(follower.transform);

    }
}
