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
        crossHairManager = FindAnyObjectByType<CrossHairManager>();
    currentHealth = maxHealth;
    WaveSpawner = FindAnyObjectByType<WaveSpawner>();
        die = GetComponentInChildren<ICanDie>();
  }

    // Update is called once per frame



    public bool isDead;
    CrossHairManager crossHairManager;
    public bool TakeDamage(float d)
    {
       int  amount = (int)d;
        currentHealth -= amount;
        if (!isDead)
        {
            crossHairManager.GetCrossHairByName("hit").startAnimation();
            crossHairManager.PlaySound("hit");
        }
    
        if (currentHealth <= 0 && !isDead)
        {
            //Instantiate(ammoBox, transform.position, Quaternion.identity, transform.parent);
            die.Die();
            if (Random.value <= spawnBoxChance && !hasSpawnedBox)
            {
                hasSpawnedBox = true;
                GameObject box = Instantiate(ammoBox, model.position, Quaternion.identity, transform.parent);
                int boxType = Random.Range(0, 4);
                box.GetComponent<AmmoBox>().InitBox(boxType);
            }

            if (!isDead)
            {
                isDead = true;
                --WaveSpawner.waves[WaveSpawner.CurrentWaveIndex].EnemiesLeft;

            }
            return true;
        }
        return false;
    }


}
