using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private float countdown = 0;

    [SerializeField] private GameObject[] SpawnPoints;
    public Transform TrainingGround;
    public GetMovement getMovement;
    public Transform mapMoving;

    public bool spawn;
    public Transform up;
    public Transform forward;
    public Transform position;
    public PlayerUI visuals;


    public Wave[] waves;

    public int CurrentWaveIndex = 0;

    private bool ReadyToCountDown;

    private void Start() {
        
        ReadyToCountDown = true;
        
        if (spawn) {
        for (int i = 0; i < waves.Length; ++i) {
            
            waves[i].EnemiesLeft = waves[i].enemies.Length;
        }
        }
    }

    public void ReduceEnemy()
    {
        --waves[CurrentWaveIndex].EnemiesLeft;
        visuals.UpdateEnemiesLeft(waves[CurrentWaveIndex].EnemiesLeft);
    }

    private void Update() {
        if (spawn)
        {
            //Debug.Log("CurrentWaveIndex: " + CurrentWaveIndex);
            if (CurrentWaveIndex >= waves.Length)
            {
                
                //Debug.Log("You survived every wave!");
                return;
            }

            if (ReadyToCountDown)
            {
                countdown -= Time.deltaTime;
            }

            if (countdown <= 0)
            {
                ReadyToCountDown = false;
                countdown = waves[CurrentWaveIndex].TimeToNextWave;
                if (CurrentWaveIndex < waves.Length)
                {
                    StartCoroutine(SpawnWave());
                }
            }

            if (waves[CurrentWaveIndex].EnemiesLeft <= 0)
            {
               
                    ReadyToCountDown = true;
               
                    ++CurrentWaveIndex;
                
            }
        }
       
    }

    private IEnumerator SpawnWave() {
       
            for (int i = 0; i < waves[CurrentWaveIndex].enemies.Length; ++i) {
                // Choose a random spawn point
                GameObject SpawnPoint = SpawnPoints[Random.Range(0, SpawnPoints.Length-1)];
                
                // Instantiate the enemy at the chosen spawn point
                Enemy enemy = Instantiate(waves[CurrentWaveIndex].enemies[i], SpawnPoint.transform);

                enemy.transform.SetParent(TrainingGround);
                yield return new WaitForSeconds(waves[CurrentWaveIndex].TimeToNextEnemy);
            }
        visuals.UpdateWaveCounter(CurrentWaveIndex + 1, waves.Length);
        visuals.UpdateEnemiesLeft(waves[CurrentWaveIndex].EnemiesLeft);
    }
}

[System.Serializable]
public class Wave {
    public Enemy[] enemies;
    public float TimeToNextEnemy;
    public float TimeToNextWave;

    [HideInInspector] public int EnemiesLeft;
}