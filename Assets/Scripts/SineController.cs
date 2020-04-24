using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineController : MonoBehaviour
{
    //Base Wave form: ampl * sin(freq * x)
    //Derived Wave form: ampl * ((pAmpl1 * sin(freq * pFreq1*x)) + (pAmpl2 * sin(freq * pFreq2*x)) + ...)
    //  pAmpl1, pFreq1, etc. are from each of the wave's parents
    
    public GameObject pivot; 
    public GameObject mesh; 
    public GameObject freqHandle; 
    public GameObject amplHandle; 
    
    public float meshFreq = 1f; //frequency
    public float meshAmpl = 1f; //amplitude
    public int maxParents = 100;
    
    public Material mat; 
    private float[] parentWaves; //The frequency & wavelength of all waves this wave is made of: [pFreq1, pAmpl1, pFreq2, pAmpl2,...]
    private string parentString = "_ParentArray";
    private int parentCount = 0;

    private float freqConversion; 
    private float amplConversion; 
    
    void Start()
    {
        //110 - 3520 freq
        //.1 gain (using .5 as max volume for now)
        freqConversion = 3520f / 5f; 
        amplConversion = 0.5f / 5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setMaterial() {
        mat = mesh.GetComponent<MeshRenderer>().material; 
        print("Material: "+mat.name);

        parentWaves = new float[maxParents * 2];
        mat.SetFloat("_ParentCount", parentCount);
        mat.SetFloatArray(parentString, parentWaves);
    }

    public float getWavelength() {
        float sonicSpeed = 343; //In m/s
        return sonicSpeed/getFrequency();
    }

    //Returns Audio Frequency
    public float getFrequency() {
        return meshFreq * freqConversion;
    }

    //Returns Audio Period
    public float getPeriod() {
        return 1.0f/getFrequency();
    }

    //Returns Audio Amplitude
    public float getAmplitude() {
        return meshAmpl * amplConversion;
    }

    public void changeFrequency(float d) {
        meshFreq += d;
        mat.SetFloat("_Frequency", meshFreq);
    } 

    public void changeAmplitude(float d) {
        meshAmpl += d;
        mat.SetFloat("_Amplitude", meshAmpl);
    } 

    //Returns true if this is a base wave (Has not had its own freq/ampl changed, and can have other waves added to it)
    public bool isBaseWave() {
         
        if (parentCount > 0) {
            return meshFreq == 1f && meshAmpl == 1f;
        }
        return true; 
    }
    
    //Add a parent wave to this one
    public void addCollidedParent(float pFreq, float pAmpl) {
        print("adding parent");
        if (isBaseWave() && parentCount < 2*maxParents) {
            
            parentWaves[parentCount++] = pFreq;
            parentWaves[parentCount++] = pAmpl;
            
            //Update shader
            mat.SetFloatArray(parentString, parentWaves);
            mat.SetFloat("_ParentCount", parentCount);
            print("updated shader");
        }
        
    }

    public float computeWaveAtPoint(float x) {

        //return: ampl * ((pAmpl1 * sin(freq * pFreq1*x)) + (pAmpl2 * sin(freq * pFreq2*x)) + ...)
        
        float final = 0; 
        float pFreq, pAmpl;
        
        for (int i=0; i < parentWaves.Length; ) {
            
            pFreq = parentWaves[i++]; 
            pAmpl = parentWaves[i++]; 
            
            final += pAmpl * Mathf.Sin(meshFreq * pFreq * x);
        }
        final *= meshAmpl;

        return final;

    }
}
