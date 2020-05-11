using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrequencyController : MonoBehaviour
{
    public GameObject waveHandle; 
    public GameObject Fnum;
    
    private SineController sineScript;
    private float lastX;

    private float maxFreq = 12.24f;
    private float minFreq = 9.75f;
    private Vector3 origin; 
    
    // Start is called before the first frame update
    void Start()
    {
        sineScript = waveHandle.GetComponent<SineController>();
        lastX = this.transform.localPosition.x; 

        origin = this.transform.localPosition;

        SetText();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPosition(Vector3 pos) {
        
        Vector3 localPos = waveHandle.transform.InverseTransformPoint(pos); 
        
        if (localPos.x < minFreq || localPos.x > maxFreq) return; 

        float y = this.transform.localPosition.y; 
        float z = this.transform.localPosition.z; 
        
        this.transform.localPosition = new Vector3(localPos.x, y, z);
        sineScript.changeFrequency(2f*(lastX-localPos.x));
        lastX = localPos.x; 

        SetText();
    }

    public void ResetPosition() {
        this.transform.localPosition = origin; 
        lastX = origin.x;
        SetText();
    }

    public void SetText() {
        float num = Mathf.Round(sineScript.getFrequency() * 100) / 100f;
        Fnum.GetComponent<TextMesh>().text = num.ToString();
    }

}
