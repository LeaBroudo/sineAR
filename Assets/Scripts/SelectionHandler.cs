using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectionHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    
    public GameObject wand;
    private WandController wandScript;

    // Start is called before the first frame update
    void Start()
    {
        wandScript = wand.GetComponent<WandController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData) {
        wandScript.ClickSelect();
    }

    public void OnPointerUp(PointerEventData eventData) {
        wandScript.ClickSelect();
    }

}
