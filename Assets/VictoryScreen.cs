using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryScreen : MonoBehaviour
{

    public void LoadMenu()
    {
        Debug.Log("loading menu");
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

}
