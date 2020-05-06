using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class VButtonCTRL : MonoBehaviour, IVirtualButtonEventHandler
{
    
    public GameObject vbObj; 
    public GameObject wand;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        vbObj = GameObject.Find("virtualButton_1");
        vbObj.GetComponent<VirtualButtonBehaviour>().RegisterEventHandler(this);
    }

    public void OnButtonPressed(VirtualButtonBehaviour vb) {
        print("HiiiII!!!");
    }

    public void OnButtonReleased(VirtualButtonBehaviour vb ) { 
        print("BYEE!!!");
    }
}
