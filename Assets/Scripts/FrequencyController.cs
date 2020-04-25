using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrequencyController : MonoBehaviour
{
    public GameObject waveHandle; 
    
    private SineController sineScript;
    private float lastX;

    private float maxFreq = 12.24f;
    private float minFreq = 9.75f;
    
    // Start is called before the first frame update
    void Start()
    {
        sineScript = waveHandle.GetComponent<SineController>();
        lastX = this.transform.localPosition.x; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPosition(Vector3 pos) {
        
        Vector3 localPos = waveHandle.transform.InverseTransformPoint(pos); 
        
        if (localPos.x < minFreq || localPos.x > maxFreq) return; 
        //print("local: "+localPos);

        float y = this.transform.localPosition.y; 
        float z = this.transform.localPosition.z; 
        
        this.transform.localPosition = new Vector3(localPos.x, y, z);
        sineScript.changeFrequency(2f*(lastX-localPos.x));
        lastX = localPos.x; 

        //print("Audio freq: "+sineScript.getFrequency());
    }
}
