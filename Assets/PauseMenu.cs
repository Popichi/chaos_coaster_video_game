using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public static bool GameIsPaused = false;
    public GameObject PauseMenuUI;
    public GameObject player;

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (GameIsPaused) {
                Resume();
            } else {
                Pause();
            }
        }
    }

    public void Resume () {
        EnablePlayerUI();
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        player.GetComponentInChildren<PlayerController>().Freeze(false);
    }

    void Pause () {
        DisablePlayerUI();
        player.GetComponentInChildren<PlayerController>().Freeze(true);
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMenu () {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame () {
        Debug.Log("Quitting game...");
        Application.Quit();
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


    private void EnablePlayerUI()
    {
        //disable other canvases
        Canvas[] canvases;

        canvases = player.GetComponentsInChildren<Canvas>();

        foreach (Canvas canvas in canvases)
        {
            canvas.enabled = true;
        }
    }
}
