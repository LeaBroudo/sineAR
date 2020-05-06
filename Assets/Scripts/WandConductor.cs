using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WandConductor : MonoBehaviour
{
    public bool setAmpl = false; //If true, wand controls amplitude, otherwise frequency
    
    private string objType = "";
    
    //public Material freqMat;
    //public Material amplMat;

    //public GameObject handleCyl;
    //public GameObject handleTorus;
    //public GameObject wandPt;

    private Dictionary<GameObject,float> nearbyWaves = new Dictionary<GameObject,float>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {   
        
        Dictionary<GameObject,float> tempNearby = new Dictionary<GameObject,float>();
        
        //Iterate through nearby waves and change their characteristics
        foreach (KeyValuePair<GameObject,float> waveEntry in nearbyWaves) {
            
            GameObject wave = waveEntry.Key;
            float dist = waveEntry.Value;
            float newDist = (this.transform.position - wave.transform.position).magnitude;
            tempNearby.Add(wave, newDist);

            //If wand moves closer to wave, value goes up
            float diff = (newDist - dist) * 10;
            print("diff: "+diff);

            //Change Amplitude
            if (setAmpl) {
                wave.GetComponent<SineController>().ChangeAmplitude(diff);
            }
            //Change Frequency
            else {
                wave.GetComponent<SineController>().ChangeFrequency(diff);
            }
        }

        nearbyWaves = tempNearby;
    }

    private void OnTriggerEnter(Collider other) {
        
        string[] fullName = other.name.Split('_');
        objType = fullName[0]; 

        //if (objType == "waveHandle" || objType == "amplitudeHandle" || objType == "frequencyHandle" || objType == "waveMesh") {
        if (objType == "waveHandle") {

            GameObject wave = other.gameObject;
            
            float dist = (this.transform.position - wave.transform.position).magnitude;
            
            if (nearbyWaves.ContainsKey(wave)) {
                nearbyWaves[wave] = dist; 
            }
            else {
                nearbyWaves.Add(wave, dist);
            }
        
            print("In range: "+other.name);
        }
    }

    private void OnTriggerExit(Collider other) {

        string[] fullName = other.name.Split('_');
        objType = fullName[0]; 

        //if (objType == "waveHandle" || objType == "amplitudeHandle" || objType == "frequencyHandle" || objType == "waveMesh") {
        if (objType == "waveHandle") {    

            try {
                nearbyWaves.Remove(other.gameObject);
                print("Out of range: "+other.name);
            }
            catch {}
            
        }
        
    }
    
}
