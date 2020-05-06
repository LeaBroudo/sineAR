using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Controller2D : MonoBehaviour
{
    public GameObject wavePrefab;
    public Shader waveShader;
    public GameObject spawnPoint;

    private List<GameObject> allWaves;

    private Transform obj = null;
    private Vector3 offSet;
    private float dist;

    public Sprite selectedButton;
    public Sprite otherButton;
    public GameObject panelA;
    public GameObject panelF;
    public GameObject paneladd;
    private GameObject aH;
    private GameObject fH;

    // Start is called before the first frame update
    void Start()
    {
        wavePrefab.SetActive(false);
        allWaves = new List<GameObject>();
        createNewWave();
        aH = GameObject.FindWithTag("amplitudeHandle");
        fH = GameObject.FindWithTag("frequencyHandle");
        aH.SetActive(true);
        fH.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //MOVE THE HANDLES USING TOUCH INPUT AND RAYCASTING
        if (Input.touchCount > 0)
        {
            Debug.Log("a touch has been registered");
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                Debug.Log("touch phase is stationary or moved");
                RaycastHit hit;
                Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.GetTouch(0).position);
                if (Physics.Raycast(ray, out /*distance*/hit))
                {
                    Debug.Log("a hit was made");
                    obj = hit.transform;
                    offSet = obj.position - hit.point;
                    dist = (ray.origin - hit.point).magnitude;    
                }

                if (obj)
                {
                    Vector3 pos = ray.GetPoint(dist) + offSet;
                    pos.z = obj.position.z;

                    if (obj.name == "amplitudeHandle")
                    {
                        Debug.Log("AMPLITUDE _______________________HANDLE");
                        obj.GetComponent<AmplitudeController>().SetPosition(pos);
                    }
                    else if (obj.name == "frequencyHandle")
                    {
                        Debug.Log("FREQUENCY _______________________HANDLE");
                        obj.GetComponent<FrequencyController>().SetPosition(pos);
                    }
                    else if (obj.name.Split('_')[0] == "waveHandle")
                    {
                        Debug.Log("WAVEHANDLE _______________________HANDLE");
                        pos.x = obj.position.x;
                        if (pos.y > 5)
                            pos.y = 5;
                        if (pos.y < -6)
                            pos.y = -6;
                        obj.GetComponent<SineController>().SetPosition(pos);
                    }
                }
            }
        }
    }
    public void createNewWave()
    {

        //Instantiate prefab
        Vector3 spawnPos = spawnPoint.transform.position;
        GameObject newWave = Instantiate(wavePrefab, spawnPos, Quaternion.identity);
        allWaves.Add(newWave);
        newWave.name = "waveHandle_" + allWaves.Count;

        //Get controller script
        SineController sineScript = newWave.GetComponent<SineController>();

        //Add material and Shader 
        sineScript.mesh.GetComponent<MeshRenderer>().material = new Material(waveShader);
        sineScript.setMaterial();

        //Set Object active
        newWave.SetActive(true);
    }
    public void SelectTab(Button button)
    {
        Button prevSelected = GameObject.FindWithTag("selected").GetComponent<Button>(); ;
        prevSelected.GetComponent<Image>().sprite = otherButton;
        prevSelected.tag = "buttonTab";
        button.GetComponent<Image>().sprite = selectedButton;
        button.tag = "selected";
        /*GameObject aH = GameObject.FindWithTag("amplitudeHandle");
        GameObject fH = GameObject.FindWithTag("frequencyHandle");*/

        if (button.name == "Amplutude_Button")
        {
            panelA.SetActive(true);
            panelF.SetActive(false);
            paneladd.SetActive(false);
            aH.SetActive(true);
            fH.SetActive(false);
            /*aH.GetComponent<CapsuleCollider>().enabled = true;
            aH.GetComponent<MeshRenderer>().enabled = true;
            fH.GetComponent<CapsuleCollider>().enabled = false;
            fH.GetComponent<MeshRenderer>().enabled = false;*/

        }
        else if(button.name == "Frequency_Button")
        {
            panelF.SetActive(true);
            panelA.SetActive(false);
            paneladd.SetActive(false);
            aH.SetActive(false);
            fH.SetActive(true);
            /*aH.GetComponent<CapsuleCollider>().enabled = false;
            aH.GetComponent<MeshRenderer>().enabled = false;
            fH.GetComponent<CapsuleCollider>().enabled = true;
            fH.GetComponent<MeshRenderer>().enabled = true;*/
        }
        else if(button.name == "ADD_Button")
        {
            paneladd.SetActive(true);
            panelA.SetActive(false);
            panelF.SetActive(false);
            GameObject wave = GameObject.FindWithTag("wave");
            wave.GetComponent<SphereCollider>().enabled = true;
            aH.SetActive(true);
            fH.SetActive(true);
            /*aH.GetComponent<CapsuleCollider>().enabled = true;
            aH.GetComponent<MeshRenderer>().enabled = true;
            fH.GetComponent<CapsuleCollider>().enabled = true;
            fH.GetComponent<MeshRenderer>().enabled = true;*/
        }
    }
}
