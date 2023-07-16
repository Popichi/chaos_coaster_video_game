using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void OnClickPlayButton() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void OnClickTutorialButton()
    {
        SceneManager.LoadScene("TutorialAlpha");
    }

    public void OnClickCreditsButton()
    {
        SceneManager.LoadScene("Credits");
    }


    public void OnClickQuitButton()
    {
        Debug.Log("Quitting the game...");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
