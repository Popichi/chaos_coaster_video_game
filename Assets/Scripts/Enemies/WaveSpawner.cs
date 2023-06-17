using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private float countdown;

    [SerializeField] private GameObject SpawnPoint;

    public Wave[] waves;

    public int CurrentWaveIndex = 0;

    private bool ReadyToCountDown;

    private void Start() {

        ReadyToCountDown = true;

        for (int i = 0; i < waves.Length; ++i) {
            waves[i].EnemiesLeft = waves[i].enemies.Length;
        }
    }

    private void Update() {

        if (CurrentWaveIndex >= waves.Length) {
            Debug.Log("You survived every wave!");
            return;
        }

        if (ReadyToCountDown) {
            countdown -= Time.deltaTime;
        }

        if (countdown <= 0) {
            ReadyToCountDown = false;
            countdown = waves[CurrentWaveIndex].TimeToNextWave;
            StartCoroutine(SpawnWave());
        }

        if (waves[CurrentWaveIndex].EnemiesLeft == 0) {
            ReadyToCountDown = true;
            ++CurrentWaveIndex;
        }
    }

    private IEnumerator SpawnWave() {
            if (CurrentWaveIndex < waves.Length) {
            
                for (int i = 0; i < waves[CurrentWaveIndex].enemies.Length; ++i) {
                Enemy enemy = Instantiate(waves[CurrentWaveIndex].enemies[i], SpawnPoint.transform);
                enemy.transform.SetParent(SpawnPoint.transform);
                yield return new WaitForSeconds(waves[CurrentWaveIndex].TimeToNextEnemy);
            }
        }
    }
}

[System.Serializable]
public class Wave {
    public Enemy[] enemies;
    public float TimeToNextEnemy;
    public float TimeToNextWave;

    [HideInInspector] public int EnemiesLeft;
}