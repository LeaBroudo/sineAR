﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    public bool editingPos = false; 

    public Material baseMat; 
    public Material nonBaseMat; 
    private bool isBaseColor = true; 
    
    private Material mat; 
    public float[] parentWaves; //The frequency & wavelength of all waves this wave is made of: [pFreq1, pAmpl1, pFreq2, pAmpl2,...]
    public int parentCount = 0;
    private string parentString = "_ParentArray";
    
    private ChildWIM wimScript;
    public GameObject playerWIM;

    public float freqConversion; 
    public float amplConversion; 
    
    void Awake()
    {
        //Conversion
        freqConversion = 3520f / 5f; 
        amplConversion = 0.5f / 5f;

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void setMaterial() {
        mat = mesh.GetComponent<MeshRenderer>().material; 
        mat.color = Color.black;

        parentWaves = new float[maxParents * 2];
        mat.SetFloat("_ParentCount", parentCount);
        mat.SetFloatArray(parentString, parentWaves);
    }

    public void setWIM() {
        
        //Create WIM Mesh 

        //Instantiate(Object original, Transform parent);
        GameObject meshWIM = Instantiate(this.gameObject, playerWIM.transform);
        meshWIM.SetActive(true);
        wimScript = meshWIM.GetComponent<ChildWIM>();

        //Delete Handles
        SineController sineScriptWIM = meshWIM.GetComponent<SineController>();
        Destroy(sineScriptWIM.freqHandle);
        Destroy(sineScriptWIM.amplHandle);
        
        //Update Scripts
        meshWIM.GetComponent<ChildWIM>().enabled = true;
        meshWIM.GetComponent<SineController>().enabled = false;
        wimScript.meshParent = this.gameObject;
        wimScript.meshWIM = meshWIM;
        wimScript.parentScript = this.gameObject.GetComponent<SineController>();

        //Set position and scale
        meshWIM.transform.localPosition = wimScript.getParentPosFromCam() / wimScript.conversionFactor;
        meshWIM.transform.localScale /= wimScript.conversionFactor;

        //Set WIM to start
        wimScript.fullyInstantiated = true;


        //TODO: DELETE AUDIO CONTROLLER 



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
        checkColor();
    } 

    public void changeAmplitude(float d) {
        meshAmpl += d;
        mat.SetFloat("_Amplitude", meshAmpl);
        checkColor();
    } 

    //Returns true if this is a base wave (Has not had its own freq/ampl changed, and can have other waves added to it)
    public bool isBaseWave() {
         
        if (parentCount > 0) {
            return meshFreq == 1f && meshAmpl == 1f;
        }
        return true; 
    }

    public void checkColor() {

        if (isBaseColor && !isBaseWave()) {
            this.GetComponent<MeshRenderer>().material = nonBaseMat;
            wimScript.meshWIM.GetComponent<MeshRenderer>().material = nonBaseMat;
            isBaseColor = false; 
        }
        else if (!isBaseColor && isBaseWave()) {
            this.GetComponent<MeshRenderer>().material = baseMat;
            wimScript.meshWIM.GetComponent<MeshRenderer>().material = baseMat;
            isBaseColor = true; 
        }

    }
    
    //Add a parent wave to this one
    public void addCollidedParent(float pFreq, float pAmpl) {
        //print("adding parent");
        if (isBaseWave() && parentCount < 2*maxParents) {
            
            parentWaves[parentCount++] = pFreq;
            parentWaves[parentCount++] = pAmpl;
            
            //Update shader
            mat.SetFloatArray(parentString, parentWaves);
            mat.SetFloat("_ParentCount", parentCount);
        }
        
    }

    public float[] getFandA() {
        float[] fa = {meshFreq, meshAmpl}; 
        return fa;
    }

    public float[] getCollidedParents() {
        return parentWaves;
    }

    private void OnTriggerEnter(Collider other) {
        //print(this.name+" collided: "+other.name);
        
        //If currently editing position, do nothing
        //if (!editingPos && other.name == "pivot") {
        if (other.name == "pivot") {
            GameObject otherWave = other.gameObject.transform.parent.gameObject; 
            SineController otherScript = otherWave.GetComponent<SineController>();

            //Only combine if both are basewaves and other wave not editing position
            //if (!otherScript.editingPos && isBaseWave() && otherScript.isBaseWave()) {
            if (isBaseWave() && otherScript.isBaseWave()) {

                //Move own freq and amplitude to parent's array
                if (parentCount == 0) {
                    addCollidedParent(meshFreq, meshAmpl); 
                    meshFreq = 1f;
                    meshAmpl = 1f;
                }

                float[] otherParents = otherScript.getCollidedParents();
                //Add other wave's freq and ampl to this one's parents
                if (otherScript.parentCount == 0) {
                    addCollidedParent(otherScript.meshFreq, otherScript.meshAmpl); 
                }
                //Add other wave's parents to this one
                else {
                    float pFreq, pAmpl; 
                    for (int i=0; i < otherScript.parentCount;) {
                        pFreq = otherParents[i++];
                        pAmpl = otherParents[i++];
                        addCollidedParent(pFreq, pAmpl);
                    }
                }

                //Destroy the other wave and its WIM child
                string name = otherWave.name; 
                Destroy(otherScript.wimScript.meshWIM);
                Destroy(otherWave);
                print("deleted: "+ name);

                //Reset handle positions
                freqHandle.GetComponent<FrequencyController>().ResetPosition();
                amplHandle.GetComponent<AmplitudeController>().ResetPosition();

                /* 
                for (int i=0; i < parentCount;) {

                    print("pf: "+parentWaves[i++]+" pa: " +parentWaves[i++]);
                }
                print("f: "+meshFreq+" a: "+meshAmpl);
                */
            }
        }
    }

    public void SetPosition(Vector3 pos) {
        this.transform.position = pos; 
    }

    /* 
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
    */
}
