using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void Learn()
    {
        SceneManager.LoadScene("Learn");
    }

    public void EndGame()
    {
        Debug.Log("QUIT APP");
        Application.Quit();
    }
}
