using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialManager : MonoBehaviour
{
    public GameObject Select;
    public GameObject ChangeMode;
    public GameObject delete;
    public GameObject Panel;

    public void ChangePanel(GameObject nextPanel)
    {
        if(nextPanel.name == "1_addwave")
        {
            Select.SetActive(false);
            ChangeMode.SetActive(false);
            delete.SetActive(false);
            Panel.SetActive(false);
        }

        else if(nextPanel.name == "2_dragtoscene")
        {
            Select.SetActive(false);
            Panel.SetActive(true);
        }

        else if (nextPanel.name == "3_manipulate") { Select.SetActive(true); }
        
        else if (nextPanel.name == "4_combinewaves") { ChangeMode.SetActive(false); }
        
        else if (nextPanel.name == "5_conductormode")
        {
            ChangeMode.SetActive(true);
            delete.SetActive(false);
        }

        else if (nextPanel.name == "6_deletewave") { delete.SetActive(true); }
        
        /*else if (nextPanel.name == "7_done"){}*/

        nextPanel.SetActive(true);
        gameObject.SetActive(false);
    }

    public void Play() { SceneManager.LoadScene("3DManipulation"); }

}
