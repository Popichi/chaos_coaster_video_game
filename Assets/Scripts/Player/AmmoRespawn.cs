using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoRespawn : MonoBehaviour
{
    public AmmoBox boxPrefab;
    public GameObject[] boxes;
    private Vector3[] boxLocations;
    private float respawnTimer = 1.5f;
    private bool[] isDead;

    // Start is called before the first frame update
    void Start()
    {
        boxLocations = new Vector3[boxes.Length];
        isDead = new bool[boxes.Length];
        for(int i = 0; i < boxes.Length; i++)
        {
            boxLocations[i] = boxes[i].transform.position;
            boxes[i].GetComponent<AmmoBox>().respawnerIndex = i;

        }
    }

    public void NotifyDestroyed(int index)
    {
        StartCoroutine(RespawnAmmo(index));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator RespawnAmmo(int index)
    {
        yield return new WaitForSecondsRealtime(respawnTimer);
        AmmoBox newBox = Instantiate(boxPrefab, boxLocations[index], Quaternion.identity);
        newBox.gameObject.transform.localRotation = Quaternion.Euler(-90, 0, 0);
        newBox.InitBox(index);
        newBox.respawnerIndex = index;
        boxes[index] = newBox.gameObject;
        isDead[index] = false;
    }
}
