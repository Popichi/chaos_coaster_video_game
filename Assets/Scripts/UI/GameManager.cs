using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Reference to the WaveSpawner
    public WaveSpawner WaveSpawner;

    // Reference to the Victory UI
    public GameObject VictoryScreen;

    // Refrenece to Player Script
    public GameObject player;

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
        DisablePlayerUI();
        // Pause the game
        Time.timeScale = 0f;
        // Freeze Player
        player.GetComponentInChildren<PlayerController>().Freeze(true);
        // Show the victory screen
        VictoryScreen.SetActive(true);
    }

    private void DisablePlayerUI()
    {
        //disable other canvases
        Canvas[] canvases;

        canvases = player.GetComponentsInChildren<Canvas>();

        foreach (Canvas canvas in canvases)
        {
            canvas.enabled = false;
        }       
    }

    public void OnClickContinueButton() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}