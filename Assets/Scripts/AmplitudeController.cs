using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmplitudeController : MonoBehaviour
{
    public GameObject waveHandle; 
    
    private SineController sineScript;
    private float lastY; 

    private float maxAmpl = 6f;
    private float minAmpl = 1.15f;
    
    // Start is called before the first frame update
    void Start()
    {
        sineScript = waveHandle.GetComponent<SineController>();
        lastY = this.transform.localPosition.y; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPosition(Vector3 pos) {
        
        Vector3 localPos = waveHandle.transform.InverseTransformPoint(pos); 
        
        if (localPos.y < minAmpl || localPos.y > maxAmpl) return; 
        //print("local: "+localPos);

        float x = this.transform.localPosition.x; 
        float z = this.transform.localPosition.z; 
        
        this.transform.localPosition = new Vector3(x, localPos.y, z);
        sineScript.changeAmplitude(2f*(localPos.y-lastY));
        lastY = localPos.y; 
    }
    
}
