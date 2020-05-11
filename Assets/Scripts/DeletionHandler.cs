using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeletionHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
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
        t.text = "Not Deleting";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData) {
        wandScript1.ClickDelete();
        wandScript2.ClickDelete();
        t.text = "Deleting";

    }

    public void OnPointerUp(PointerEventData eventData) {
        wandScript1.ClickDelete();
        wandScript2.ClickDelete();
        t.text = "Not Deleting";
    }

}