using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterController : MonoBehaviour
{
    public GameObject wavePrefab; 
    public GameObject waveParent;
    public Shader waveShader;
    public GameObject wand1;
    public GameObject wandPt1;
    public GameObject wand2;
    public GameObject wandPt2;

    public Button modifierWand1;
    private bool amplWand1; 
    public Button modifierWand2;
    private bool amplWand2; 

    public Button makeWave;

    public bool conduct; 
    public Button modeButton; 
    
    private List<GameObject> allWaves; 

    private Transform obj = null;
    private Vector3 offSet;
    private float dist; 
    private Vector3 offset = new Vector3(0,1f,0f);
    
    // Start is called before the first frame update
    void Start()
    {
        wavePrefab.SetActive(false);
        allWaves = new List<GameObject>();
        
        makeWave.onClick.AddListener(createSineWave);
        modeButton.onClick.AddListener(changeMode);

        modifierWand1.onClick.AddListener(changeSettingWand1);
        modifierWand2.onClick.AddListener(changeSettingWand2);
        amplWand1 = false;
        amplWand2 = false;
        changeSettingWand1();
        changeSettingWand2();
        
        conduct = true;
        changeMode();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp("w")) {
            createNewWave();
        }
        else if (Input.GetKeyUp("q")) {
            createSquareWave();
        }
        else if (Input.GetKeyUp("s")) {
            createSawWave();
        }
        else if (Input.GetKeyUp("t")) {
            createTriWave();
        }
        
        //GET MOUSE INPUT TO MOVE HANDLES AROUND
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);     // Gets the mouse position in the form of a ray.
  
        if (Input.GetButtonDown("Fire1") && !obj) {     // If we click the mouse...
        
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {

                obj = hit.transform;     
                offSet = obj.position-hit.point;       
                dist = (ray.origin - hit.point).magnitude;  
                
                if (obj.name.Split('_')[0] == "waveHandle") {
                    obj.GetComponent<SineController>().editingPos = true; 
                }
                print("Grabbed: "+obj.name);

            }
        }
    
        else if (Input.GetButtonUp("Fire1")) {
            
            if (obj != null) {
                
                print("Released: "+obj.name);

                if (obj.name.Split('_')[0] == "waveHandle") {
                    obj.GetComponent<SineController>().editingPos = false; 
                }
            }
            
            obj = null;      // Let go of the object.
        }
        
        //Drag selected object
        if (obj) {
            //print("Obj: "+obj.name);
            Vector3 pos = ray.GetPoint(dist) + offSet;    

            string objType = obj.name.Split('_')[0];
            
            if (objType == "amplitudeHandle") {
                obj.GetComponent<AmplitudeController>().SetPosition(pos);
            }
            else if (objType == "frequencyHandle") {
                obj.GetComponent<FrequencyController>().SetPosition(pos);
            }
            else if (objType.Split('_')[0] == "waveHandle") {
                obj.GetComponent<SineController>().SetPosition(pos);
            }
        }
    }

    public void createSineWave() {
        GameObject newWave = createNewWave();
    }

    public GameObject createNewWave() {
        ///* 
        if (!waveParent.active) {
            throw new System.ArgumentException("Wave's parent tracker must be tracking to create new wave.");
        }
        //*/
        
        //Instantiate prefab
        Vector3 spawnPos = wandPt1.transform.position + offset;
        GameObject newWave = Instantiate(wavePrefab, spawnPos, Quaternion.identity);
        newWave.SetActive(true);
        allWaves.Add(newWave);
        newWave.name = "waveHandle_" + allWaves.Count;

        //Get controller script
        SineController sineScript = newWave.GetComponent<SineController>();

        //Set Child names
        sineScript.setChildNames(allWaves.Count);

        //Add WIM Child
        sineScript.setWIM();

        //Get AudioController script
        AudioController audioScript = newWave.GetComponent<AudioController>();

        //Set parent
        newWave.transform.SetParent(waveParent.transform, true);

        return newWave;
        
    }

    public void createSquareWave() {
        
        //Get prefab
        GameObject newWave = createNewWave();
        SineController sineScript = newWave.GetComponent<SineController>();
        
        //Add parents
        //https://en.wikipedia.org/wiki/Square_wave
        //Using the equation under fourier analysis
        int noiseReduce = 6; //The greater this number, the more square the wave
        for (int i=1; i<noiseReduce; i+=2) {            
            sineScript.addCollidedParent(0.2f * (2*Mathf.PI * (float)i), 1f/(float)i); //the 0.2f is so the mesh looks right
        }
         
        
    }

    public void createSawWave() {
        
        //Get prefab
        GameObject newWave = createNewWave();
        SineController sineScript = newWave.GetComponent<SineController>();
        
        //Add parents
        //https://en.wikipedia.org/wiki/Sawtooth_wave
        //Using the x_sawtooth(t) equation 
        int noiseReduce = 4; //The greater this number, the more sawtooth the wave
        for (int i=1; i<noiseReduce; i++) {            
            sineScript.addCollidedParent(0.3f * (2*Mathf.PI * (float)i), -1f * Mathf.Pow(-1f,i) * (1f/(float)i)); 
            //the 0.3f is so the mesh looks right
            //the extra -1f is to invert the entire wave
        }

        
    }

    public void createTriWave() {
        
       //Get prefab
        GameObject newWave = createNewWave();
        SineController sineScript = newWave.GetComponent<SineController>();
        
        //Add parents
        //https://en.wikipedia.org/wiki/Triangle_wave
        //Using first function under harmonics
        int harmonics = 10; //The greater this number, the more triangle the wave
        for (int i=0; i<harmonics-1; i++) {            
            int mode = (2*i)+1;
            sineScript.addCollidedParent(0.3f * (2*Mathf.PI * mode), Mathf.Pow(-1f,i) * Mathf.Pow(mode, -2)); 
            //the 0.3f is so the mesh looks right
        }
        
    }

    //Sets wand to conduct mode
    public void SetWandConduct(GameObject wand) {
        //Turn off WandController, turn on WandConductor
        wand.GetComponent<WandController>().enabled = false; 
        wand.GetComponent<WandConductor>().enabled = true; 

        //Turn off box collider, turn on sphere collider
        wand.GetComponent<BoxCollider>().enabled = false; 
        wand.GetComponent<SphereCollider>().enabled = true; 
    }

    //Sets wand to regular mode
    public void SetWandRegular(GameObject wand) {
        //Turn off WandConductor, turn on WandController
        wand.GetComponent<WandConductor>().enabled = false; 
        wand.GetComponent<WandController>().enabled = true; 

        //Turn off sphere collider, turn on box collider
        wand.GetComponent<SphereCollider>().enabled = false; 
        wand.GetComponent<BoxCollider>().enabled = true; 
    }

    public void changeMode() {
        conduct = !conduct; 

        if (conduct) {
            print("Conductor Mode");
            modeButton.GetComponentInChildren<Text>().text = "Conductor Mode";

            SetWandConduct(wand1); 
            SetWandConduct(wand2); 

            //Update wand modifier buttons
            modifierWand1.gameObject.SetActive(true);
            modifierWand2.gameObject.SetActive(true);
            modifierWand1.GetComponentInChildren<Text>().text = amplWand1? "Affecting \nAmplitude" : "Affecting \nFrequency";
            modifierWand2.GetComponentInChildren<Text>().text = amplWand2? "Affecting \nAmplitude" : "Affecting \nFrequency";

        }
        else {
            print("Wand Mode");
            modeButton.GetComponentInChildren<Text>().text = "Wand Mode";

            SetWandRegular(wand1);
            SetWandRegular(wand2);

            //Update wand modifier buttons
            modifierWand1.GetComponentInChildren<Text>().text = "To use set \nConductor Mode";
            modifierWand2.GetComponentInChildren<Text>().text = "To use set \nConductor Mode";
            modifierWand1.gameObject.SetActive(false);
            modifierWand2.gameObject.SetActive(false);

        }
    }

    public void changeSettingWand1() {
        amplWand1 = !amplWand1;
        
        wand1.GetComponent<WandConductor>().setAmpl = amplWand1;
        if (conduct) {
            modifierWand1.GetComponentInChildren<Text>().text = amplWand1? "Affecting \nAmplitude" : "Affecting \nFrequency";
        }
    }

    public void changeSettingWand2() {
        amplWand2 = !amplWand2;
        
        wand2.GetComponent<WandConductor>().setAmpl = amplWand2;
        if (conduct) {
            modifierWand2.GetComponentInChildren<Text>().text = amplWand2? "Affecting \nAmplitude" : "Affecting \nFrequency";
        }
    }

}
