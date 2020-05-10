using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectionHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    
    public GameObject wand1;
    private WandController wandScript1;

    public GameObject wand2;
    private WandController wandScript2;

    private Text t; 

    // Start is called before the first frame update
    void Start()
    {
        wandScript1 = wand1.GetComponent<WandController>();
        wandScript2 = wand2.GetComponent<WandController>();

        t = this.GetComponentInChildren<Text>();
        t.text = "Not Selecting";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData) {
        wandScript1.ClickSelect();
        wandScript2.ClickSelect();
        t.text = "Selecting";

    }

    public void OnPointerUp(PointerEventData eventData) {
        wandScript1.ClickSelect();
        wandScript2.ClickSelect();
        t.text = "Not Selecting";
    }

}
