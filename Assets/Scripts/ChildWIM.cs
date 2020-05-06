﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildWIM : MonoBehaviour
{
    public GameObject player; 
    public GameObject camera;

    public GameObject meshParent;  
    public GameObject meshWIM;

    public float conversionFactor;

    public SineController parentScript; 

    public bool editing = false; //TRUE WHEN HANDLE COLLIDING WITH IT
    public bool fullyInstantiated = false; 
    
    // Start is called before the first frame update
    void Start()
    {
        conversionFactor = 10f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!fullyInstantiated) return;

        //Check position difference
        if (getMeshWIMPosFromPlayerWIM() != getParentPosFromCam() / conversionFactor) {
            
            //when this position changed, position of parent changed. 
            if (editing) {
                meshParent.transform.localPosition += (getMeshWIMPosFromPlayerWIM() * conversionFactor) - getParentPosFromCam();
            }
            //make sure this position is same as parent
            else {
                meshWIM.transform.localPosition = getParentPosFromCam() / conversionFactor;
            }
        }

        //Check rotation difference
        if (getMeshWIMRotFromPlayerWIM() != getParentRotFromCam()) {
            //when this rotation changed, position of parent changed. 
            if (editing) {
                meshParent.transform.rotation = (getMeshWIMRotFromPlayerWIM() * Quaternion.Inverse(getParentRotFromCam())) * meshParent.transform.rotation;
            }
            //make sure this rotation is same as parent
            else {
                meshWIM.transform.rotation = player.transform.rotation * getParentRotFromCam();
            }
        }
        
        
    }

    //Find wave position relative to camera
    public Vector3 getParentPosFromCam() {
        return camera.transform.InverseTransformPoint(meshParent.transform.position);
    }

    //Find meshWIM position relative to PlayerWIM
    public Vector3 getMeshWIMPosFromPlayerWIM() {
        return player.transform.InverseTransformPoint(meshWIM.transform.position);
    }

     //Find wave rotation relative to camera
    public Quaternion getParentRotFromCam() {
        //Quaternion.Inverse(Target.transform.rotation) * WorldRotation;
        return Quaternion.Inverse(camera.transform.rotation) * meshParent.transform.rotation;
        //camera.transform.InverseTransformPoint(meshParent.transform.position);
    }

    //Find meshWIM rotation relative to PlayerWIM
    public Quaternion getMeshWIMRotFromPlayerWIM() {
        return Quaternion.Inverse(player.transform.rotation) * meshWIM.transform.rotation;
        //return player.transform.InverseTransformPoint(meshWIM.transform.position);
    }

}
