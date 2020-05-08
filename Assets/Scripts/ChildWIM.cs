using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildWIM : MonoBehaviour
{
    public GameObject player; 
    public GameObject camera;

    //public GameObject meshParent;  
    //public GameObject meshWIM;

    public float conversionFactor;

    //public bool editing = false; //TRUE WHEN HANDLE COLLIDING WITH IT

    private Dictionary<GameObject,GameObject> allWaves = new Dictionary<GameObject,GameObject>(); //child : parent
    private HashSet<string> editingWIMWaves = new HashSet<string>();
    
    // Start is called before the first frame update
    void Start()
    {
        conversionFactor = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        
        GameObject child, parent; 
        Vector3 childPosFromPlayerWIM, parentPosFromCam;
        Quaternion childRotFromPlayerWIM, parentRotFromCam;
        bool editing;
        
        //Go thru all waves to update child position based on parent
        foreach (KeyValuePair<GameObject,GameObject> waveSet in allWaves) {

            child = waveSet.Key;
            parent = waveSet.Value; 
            childPosFromPlayerWIM = getChildPosFromPlayerWIM(child, parent);
            parentPosFromCam = getParentPosFromCam(child, parent);
            childRotFromPlayerWIM = getChildRotFromPlayerWIM(child, parent);
            parentRotFromCam = getParentRotFromCam(child, parent);

            //But if child is in editing hash, then move parent
            if (editingWIMWaves.Contains(child.name)) {
                editing = true;
            }
            else {
                editing = false;
            }
            
            //Check position difference
            if (childPosFromPlayerWIM != parentPosFromCam / conversionFactor) {
                
                //when this position changed, position of parent changed. 
                if (editing) {
                    parent.transform.localPosition = (childPosFromPlayerWIM * conversionFactor);
                }
                //make sure this position is same as parent
                else {
                    child.transform.localPosition = parentPosFromCam / conversionFactor;
                }
            }

            //Check rotation difference
            if (childRotFromPlayerWIM != parentRotFromCam) {
                //when this rotation changed, position of parent changed. 
                if (editing) {
                    parent.transform.rotation = (childRotFromPlayerWIM * Quaternion.Inverse(parentRotFromCam)) * parent.transform.rotation;
                }
                //make sure this rotation is same as parent
                else {
                    child.transform.rotation = player.transform.rotation * parentRotFromCam;
                }
            }
        
        }
        
    }

    //Get parent pos from base, and child pos from WIM(this)

    //Find wave position relative to camera
    public Vector3 getParentPosFromCam(GameObject child, GameObject parent) { //this is off
        return camera.transform.InverseTransformPoint(parent.transform.position);
    }

    //Find meshWIM position relative to PlayerWIM
    public Vector3 getChildPosFromPlayerWIM(GameObject child, GameObject parent) {
        return player.transform.InverseTransformPoint(child.transform.position);
    }

     //Find wave rotation relative to camera
    public Quaternion getParentRotFromCam(GameObject child, GameObject parent) {
        //Quaternion.Inverse(Target.transform.rotation) * WorldRotation;
        return Quaternion.Inverse(camera.transform.rotation) * parent.transform.rotation;
        //camera.transform.InverseTransformPoint(meshParent.transform.position);
    }

    //Find meshWIM rotation relative to PlayerWIM
    public Quaternion getChildRotFromPlayerWIM(GameObject child, GameObject parent) {
        return Quaternion.Inverse(player.transform.rotation) * child.transform.rotation;
        //return player.transform.InverseTransformPoint(meshWIM.transform.position);
    }

    public void addWaveChild(GameObject child, GameObject parent) {
        
        //Add to dict
        allWaves.Add(child, parent);
        
        //Set position and scale
        child.transform.localPosition = getParentPosFromCam(child, parent) / conversionFactor;
        child.transform.localScale /= conversionFactor * 0.5f;
    }

    public void editChild(GameObject child, bool edit) {
        
        if (edit) {
            editingWIMWaves.Add(child.name);
        }
        else {
            editingWIMWaves.Remove(child.name);
        }
    }

}
