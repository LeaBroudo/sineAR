using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmplitudeController : MonoBehaviour
{
    public GameObject waveHandle; 
    public GameObject Anum;
    
    private SineController sineScript;
    private float lastY; 

    private float maxAmpl = 3.66f;
    private float minAmpl = 1.16f;
    private Vector3 origin; 

    // Start is called before the first frame update
    void Start()
    {
        sineScript = waveHandle.GetComponent<SineController>();
        lastY = this.transform.localPosition.y; 

        origin = this.transform.localPosition;

        SetText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPosition(Vector3 pos) {
        
        Vector3 localPos = waveHandle.transform.InverseTransformPoint(pos); 
        
        if (localPos.y < minAmpl || localPos.y > maxAmpl) return; 

        float x = this.transform.localPosition.x; 
        float z = this.transform.localPosition.z; 
        
        this.transform.localPosition = new Vector3(x, localPos.y, z);
        sineScript.changeAmplitude(2f*(localPos.y-lastY));
        lastY = localPos.y; 

        SetText();
    }

    public void ResetPosition() {
        this.transform.localPosition = origin; 
        lastY = origin.y;
        SetText();
    }

    public void SetText() {
        
        float num = Mathf.Round(sineScript.getAmplitude() * 100) / 100f;
        Anum.GetComponent<TextMesh>().text = num.ToString();
    }

    
}
