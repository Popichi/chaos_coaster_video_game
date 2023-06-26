using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, ITakeDamage
{

  public int maxHealth = 100;
  public int currentHealth;
  public GameObject ammoBox;
    public Transform model;
    public float spawnBoxChance;
    private bool hasSpawnedBox = false;
    

  private WaveSpawner WaveSpawner;

    ICanDie die;
  // Start is called before the first frame update
  void Start() {
    currentHealth = maxHealth;
    WaveSpawner = FindAnyObjectByType<WaveSpawner>();
        die = GetComponentInChildren<ICanDie>();
  }

  // Update is called once per frame




    public bool TakeDamage(float d)
    {
       int  amount = (int)d;
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            //Instantiate(ammoBox, transform.position, Quaternion.identity, transform.parent);
            die.Die();
            if (Random.value <= spawnBoxChance && !hasSpawnedBox)
            {
                hasSpawnedBox = true;
                GameObject box = Instantiate(ammoBox, model.position, Quaternion.identity, transform.parent);
                int boxType = Random.Range(0, 3);
                box.GetComponent<AmmoBox>().InitBox(boxType);
            }


            --WaveSpawner.waves[WaveSpawner.CurrentWaveIndex].EnemiesLeft;
            return true;
        }
        return false;
    }


}
