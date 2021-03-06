﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WandController : MonoBehaviour
{
    
    public GameObject otherWand; 
    private WandController otherCtrl; 
    //public Button selectButton;
    //public Button deleteButton;

    public GameObject grabbedObj = null;

    private string objType = "";
    
    public Material freqMat;
    public Material amplMat;

    public GameObject handleCyl;
    public GameObject handleTorus;
    public GameObject wandPt;

    public bool selecting = false; 
    public bool deleting = false; 

    public GameObject WIM; 
    private ChildWIM wimScript;

    private Vector3 offset = new Vector3(0f,0f,3f);
    private IEnumerator followAnim;

    // Start is called before the first frame update
    void Start()
    {
        //Get other wand's script
        otherCtrl = otherWand.GetComponent<WandController>();
        wimScript = WIM.GetComponent<ChildWIM>();

    }

    // Update is called once per frame
    void Update()
    {   
    
    }

    private void OnTriggerEnter(Collider other) {
        
        string[] fullName = other.name.Split('_');
        objType = fullName[0]; 

        print("del: "+deleting+" type: "+objType);
        
        //Delete wave
        if (deleting && objType == "waveHandle") {
            deleteWave(other.gameObject);
            return;
        }
        //Do nothing if selection button not pressed
        else if (!selecting) {
            print("Can't grab, not currently selecting.");
            return;
        }

        if (objType == "waveHandle" || objType == "amplitudeHandle" || objType == "frequencyHandle" || objType == "WIMwave") {
            grabbedObj = other.gameObject;
            
            if (objType == "WIMwave") {
                wimScript.editChild(grabbedObj, true);
            }
            
            if (followAnim != null)
                StopCoroutine(followAnim);
            
            followAnim = FollowWand(objType);
            StartCoroutine(followAnim);

            print("Grabbed: "+grabbedObj.name);
            
        }
        else {
            print("Tried to grab ungrabbable object: "+other.name);
        }
        

    }

    private void OnTriggerExit(Collider other) {

        try {
            string go = grabbedObj.name;
        }
        catch{
            //Nothing is grabbed
            return;
        }

        Release();
        
    }

    public void deleteWave(GameObject wave) {
        SineController sineScript = wave.GetComponent<SineController>();
            
        //Make sure in correct scene, otherwise there's no WIM wave to even delete
        if (sineScript.in3DManipulation) {
            wimScript.CleanUp(sineScript.waveWIM);
        }
        
        print("deleted: "+wave.name);
        Destroy(wave);
    }

    private IEnumerator FollowWand(string objType) {
        
        print("Following wand: "+grabbedObj.name);
        
        Vector3 initPos = grabbedObj.transform.position;
        Vector3 finalPos = wandPt.transform.position;
        float distToTarget = Vector3.Distance(initPos, finalPos);

        //while (distToTarget > 0.1f)
        while (selecting && grabbedObj)
        {
            Vector3 pos = Vector3.Lerp(initPos, finalPos, 1f);

            if (objType == "waveHandle"){
                grabbedObj.GetComponent<SineController>().SetPosition(pos);
            } 
            else if (objType == "WIMwave") {
                grabbedObj.transform.position = pos;
            }
            else if (objType == "amplitudeHandle") {
                grabbedObj.GetComponent<AmplitudeController>().SetPosition(pos);
            }
            else if (objType == "frequencyHandle") {
                grabbedObj.GetComponent<FrequencyController>().SetPosition(pos);
            }


            distToTarget = Vector3.Distance(initPos, finalPos);
            finalPos = wandPt.transform.position;
            yield return new WaitForFixedUpdate();
        }
        

        followAnim = null;
        yield return null;

    }

    public void ClickSelect() {
        selecting = !selecting;

        Release();
    }

    public void ClickDelete() {
        print("delete: "+deleting);
        deleting = !deleting;
    }

    public void Release() {
        
        if (!selecting && grabbedObj) {
            if (followAnim != null)
                StopCoroutine(followAnim);

            print("Released: "+grabbedObj.name + " first: "+grabbedObj.name.Split('_')[0]);
            if (grabbedObj.name.Split('_')[0] == "WIMwave") {
                //print("hii");
                wimScript.editChild(grabbedObj, false);
            }
            grabbedObj = null;
        }
    }
}
