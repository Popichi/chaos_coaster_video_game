using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

  public int maxHealth = 100;
  public int currentHealth;

  private WaveSpawner WaveSpawner;

  // Start is called before the first frame update
  void Start() {
    currentHealth = maxHealth;
    WaveSpawner = GetComponentInParent<WaveSpawner>();
  }

  // Update is called once per frame
  void Update() {
        
  }

  public void TakeDamage(int amount) {
    currentHealth -= amount;
    if (currentHealth <= 0) {
      Destroy(gameObject);
      --WaveSpawner.waves[WaveSpawner.CurrentWaveIndex].EnemiesLeft;
    }
  }
}
