using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Drop : MonoBehaviour, IDropHandler
{
    public Camera overviewCamera;

    public void OnDrop(PointerEventData eventData)
    {
        RectTransform invPanel = transform as RectTransform;

        //if the image has been dragged out of the panel
        if (!RectTransformUtility.RectangleContainsScreenPoint(invPanel, Input.mousePosition))
        {
            GameObject wave = GameObject.FindWithTag("dragged");
            if (wave.name == "Sine")
            {
                overviewCamera.GetComponent<LearnController>().createNewWave();
                wave.tag = "Untagged";
            }
            if (wave.name == "Sawtooth")
            {
                overviewCamera.GetComponent<LearnController>().createSawWave();
                wave.tag = "Untagged";
            }
            if (wave.name == "Square")
            {
                overviewCamera.GetComponent<LearnController>().createSquareWave();
                wave.tag = "Untagged";
            }
            if (wave.name == "Triangle")
            {
                Debug.Log("TRIANGLE!!!");
                overviewCamera.GetComponent<LearnController>().createTriWave();
                wave.tag = "Untagged";
            }
        }
    }
}