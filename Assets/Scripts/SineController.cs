using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineController : MonoBehaviour
{
    //Base Wave form: ampl * sin(freq * x)
    //Derived Wave form: ampl * ((pAmpl1 * sin(freq * pFreq1*x)) + (pAmpl2 * sin(freq * pFreq2*x)) + ...)
    //  pAmpl1, pFreq1, etc. are from each of the wave's parents
    
    public float meshFreq = 1f; //frequency
    public float meshAmpl = 1f; //amplitude
    public int maxParents = 100;

    //TODO: GET meshFreq/ampl -> actual freq ampl 
    //110 - 3520 freq
    //.1 gain
    
    public Material mat; 
    private float[] parentWaves; //The frequency & wavelength of all waves this wave is made of: [pFreq1, pAmpl1, pFreq2, pAmpl2,...]
    private string parentString = "_ParentArray";
    private int parentCount = 0;
    
    void Awake()
    {
        parentWaves = new float[maxParents * 2];
        mat = this.GetComponent<MeshRenderer>().material; 
        print("Material: "+mat.name);

        mat.SetFloatArray(parentString, parentWaves);
        mat.SetFloat("_ParentCount", parentCount);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public float getWavelength() {
        float sonicSpeed = 343; //In m/s
        return sonicSpeed/meshFreq;
    }

    public float getFrequency() {
        return meshFreq;
    }

    public float getPeriod() {
        return 1.0f/meshFreq;
    }

    public float getAmpitude() {
        return meshAmpl;
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
