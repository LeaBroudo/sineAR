using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterController : MonoBehaviour
{
    public GameObject wavePrefab; 
    public Shader waveShader;
    
    private List<GameObject> allWaves; 

    private Transform obj = null;
    private Vector3 offSet;
    private float dist; 
    
    // Start is called before the first frame update
    void Start()
    {
        wavePrefab.SetActive(false);
        allWaves = new List<GameObject>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp("w")) {
            createNewWave();
        }

        //GET MOUSE INPUT TO MOVE HANDLES AROUND
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);     // Gets the mouse position in the form of a ray.

        if (Input.GetButtonDown("Fire1") && !obj)
        {     // If we click the mouse...

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {

                obj = hit.transform;
                offSet = obj.position - hit.point;
                dist = (ray.origin - hit.point).magnitude;

                if (obj.name.Split('_')[0] == "waveHandle")
                {
                    obj.GetComponent<SineController>().editingPos = true;
                }

            }
        }

        else if (Input.GetButtonUp("Fire1"))
        {

            if (obj != null && obj.name.Split('_')[0] == "waveHandle")
            {
                obj.GetComponent<SineController>().editingPos = false;
            }

            obj = null;      // Let go of the object.
        }

        //Drag selected object
        if (obj) {
            
            Vector3 pos = ray.GetPoint(dist) + offSet;    
            
            if (obj.name == "amplitudeHandle") {
                obj.GetComponent<AmplitudeController>().SetPosition(pos);
            }
            else if (obj.name == "frequencyHandle") {
                obj.GetComponent<FrequencyController>().SetPosition(pos);
            }
            else if (obj.name.Split('_')[0] == "waveHandle") {
                obj.GetComponent<SineController>().SetPosition(pos);
            }
        }
    }

    public void createNewWave() {
        
        //Instantiate prefab
        Vector3 spawnPos = this.transform.position + new Vector3(-10f,0,20f);
        GameObject newWave = Instantiate(wavePrefab, spawnPos, Quaternion.identity);
        allWaves.Add(newWave);
        newWave.name = "waveHandle_" + allWaves.Count;

        //Get controller script
        SineController sineScript = newWave.GetComponent<SineController>();

        //Add material and Shader 
        sineScript.mesh.GetComponent<MeshRenderer>().material = new Material(waveShader);
        sineScript.setMaterial();

        //Get AudioController script
        AudioController audioScript = newWave.GetComponent<AudioController>();


        //Set Object active
        newWave.SetActive(true);
        
         /* For testing
        //sineScript.changeFrequency(0.5f);
        //sineScript.changeAmplitude(-0.5f);

        sineScript.addCollidedParent(1f, 1f);
        sineScript.addCollidedParent(2f, 1f);
        sineScript.addCollidedParent(3f, 1f);
        sineScript.addCollidedParent(4f, 1f);
        sineScript.addCollidedParent(5f, 1f);
         */

        //TODO: Make sound




    }
}
