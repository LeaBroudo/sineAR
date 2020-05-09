using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildWIM : MonoBehaviour
{
    public GameObject player; 
    public GameObject camera;
    public GameObject waveGround; 
    public GameObject waveGroundWIM; 
    public GameObject worldItems; 

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
        waveGroundWIM.transform.localScale = waveGround.transform.localScale / conversionFactor;
    }

    // Update is called once per frame
    void Update()
    {
        //Update waveGroundWIM
        waveGroundWIM.transform.localPosition = getPosFromCam(waveGround) / conversionFactor;
        waveGroundWIM.transform.rotation = player.transform.rotation * getRotFromCam(waveGround);
        
        GameObject child, parent; 
        Vector3 childPosFromPlayerWIM, parentPosFromCam;
        Quaternion childRotFromPlayerWIM, parentRotFromCam;
        bool editing;
        
        //Go thru all waves to update child position based on parent
        foreach (KeyValuePair<GameObject,GameObject> waveSet in allWaves) {

            child = waveSet.Key;
            parent = waveSet.Value; 
            childPosFromPlayerWIM = getPosFromPlayerWIM(child);
            parentPosFromCam = getPosFromCam(parent);
            childRotFromPlayerWIM = getRotFromPlayerWIM(child);
            parentRotFromCam = getRotFromCam(parent);

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
    public Vector3 getPosFromCam(GameObject parent) { 
        return camera.transform.InverseTransformPoint(parent.transform.position);
    }

    //Find meshWIM position relative to PlayerWIM
    public Vector3 getPosFromPlayerWIM(GameObject child) {
        return player.transform.InverseTransformPoint(child.transform.position);
    }

     //Find wave rotation relative to camera
    public Quaternion getRotFromCam(GameObject parent) {
        return Quaternion.Inverse(camera.transform.rotation) * parent.transform.rotation;
    }

    //Find meshWIM rotation relative to PlayerWIM
    public Quaternion getRotFromPlayerWIM(GameObject child) {
        return Quaternion.Inverse(player.transform.rotation) * child.transform.rotation;
    }

    public void addWaveChild(GameObject child, GameObject parent) {
        
        //Add to dict
        allWaves.Add(child, parent);
        
        //Set position and scale
        child.transform.SetParent(worldItems.transform);
        child.transform.localPosition = getPosFromCam(parent) / conversionFactor;
        child.transform.localScale /= conversionFactor;
    }

    public void editChild(GameObject child, bool edit) {
        
        if (edit) {
            editingWIMWaves.Add(child.name);
        }
        else {
            editingWIMWaves.Remove(child.name);
        }
    }

    public void CleanUp(GameObject child) {
    
        if (allWaves.ContainsKey(child)) {
            allWaves.Remove(child);
        }
        if (editingWIMWaves.Contains(child.name)) {
            editingWIMWaves.Remove(child.name);
        }
        Destroy(child);
    }

}
