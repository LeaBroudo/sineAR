using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play(){ SceneManager.LoadScene("SampleScene"); }
 
    public void Learn(){ SceneManager.LoadScene("LearnMenu"); }

    public void LearnAboutWaves(){ SceneManager.LoadScene("Learn2D"); }

    public void LearnAR(){ SceneManager.LoadScene("LearnAR"); }

    public void Home() { SceneManager.LoadScene("MainMenu"); }

    public void BackToLearnMenu() { SceneManager.LoadScene("LearnMenu"); }

    public void EndGame()
    {
        Debug.Log("QUIT APP");
        Application.Quit();
    }
}
