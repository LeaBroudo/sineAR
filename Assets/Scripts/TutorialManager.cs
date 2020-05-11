using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    /*GameObject activePanel;*/
    public void ChangePanel(GameObject nextPanel)
    {
        nextPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    public void Play() { SceneManager.LoadScene("3DManipulation"); }

}
