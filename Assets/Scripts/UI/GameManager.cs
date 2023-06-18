using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Reference to the WaveSpawner
    public WaveSpawner WaveSpawner;

    // Reference to the Victory UI
    public GameObject VictoryScreen;

    private void Start() {
        // Ensure the victory screen is not visible at the start
        VictoryScreen.SetActive(false);
    }

    private void Update() {
        // Check if the player has completed all waves and defeated all enemies
        if (WaveSpawner.CurrentWaveIndex >= WaveSpawner.waves.Length && AreAllEnemiesDefeated()) {
            WinGame();
        }
    }

    private bool AreAllEnemiesDefeated() {
        // Go through each wave
        foreach (Wave wave in WaveSpawner.waves) {
            // If there are enemies left in any wave, return false
            if (wave.EnemiesLeft > 0) {
                return false;
            }
        }

        // If no enemies are left in any wave, return true
        return true;
    }

    private void WinGame() {
        // Pause the game
        Time.timeScale = 0f;

        // Show the victory screen
        VictoryScreen.SetActive(true);
    }

    public void OnClickContinueButton() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}