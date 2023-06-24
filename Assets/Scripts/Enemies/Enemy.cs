using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

  public int maxHealth = 100;
  public int currentHealth;
  public GameObject ammoBox;
    public Transform model;
    public float spawnBoxChance;
    private bool hasSpawnedBox = false;
    

  private WaveSpawner WaveSpawner;

  // Start is called before the first frame update
  void Start() {
    currentHealth = maxHealth;
    WaveSpawner = FindAnyObjectByType<WaveSpawner>();
  }

  // Update is called once per frame
  void Update() {
        
  }

  public void TakeDamage(int amount) {
    currentHealth -= amount;
    if (currentHealth <= 0) {
            //Instantiate(ammoBox, transform.position, Quaternion.identity, transform.parent);
      Destroy(gameObject);
            if (Random.value <= spawnBoxChance && !hasSpawnedBox)
            {
                hasSpawnedBox = true;
                GameObject box = Instantiate(ammoBox, model.position, Quaternion.identity, transform.parent);
                int boxType = Random.Range(0, 3);
                box.GetComponent<AmmoBox>().InitBox(boxType);
            }


            --WaveSpawner.waves[WaveSpawner.CurrentWaveIndex].EnemiesLeft;
    }
  }
}
