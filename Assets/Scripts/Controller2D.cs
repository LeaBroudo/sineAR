using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
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
    private GameObject wave;

    // Start is called before the first frame update
    void Start()
    {
        wavePrefab.SetActive(false);
        allWaves = new List<GameObject>();
        createNewWave(spawnPoint.transform.position);
        aH = GameObject.FindWithTag("amplitudeHandle");
        fH = GameObject.FindWithTag("frequencyHandle");
        wave = GameObject.FindWithTag("wave");
        aH.SetActive(true);
        fH.SetActive(false);
        wave.GetComponent<SphereCollider>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //MOVE THE HANDLES USING TOUCH INPUT AND RAYCASTING
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                if (EventSystem.current.IsPointerOverGameObject())
                    return;
                RaycastHit hit;
                Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.GetTouch(0).position);
                if (Physics.Raycast(ray, out hit))
                {
                    obj = hit.transform;
                    offSet = obj.position - hit.point;
                    dist = (ray.origin - hit.point).magnitude;    
                }

                if (obj)
                {
                    Vector3 pos = ray.GetPoint(dist) + offSet;
                    /*pos.z = obj.position.z;*/
                    pos.z = Mathf.Clamp(obj.position.z, -50, -50);

                    if (obj.name == "amplitudeHandle")
                    {
                        obj.GetComponent<AmplitudeController>().SetPosition(pos);
                    }
                    else if (obj.name == "frequencyHandle")
                    {
                        obj.GetComponent<FrequencyController>().SetPosition(pos);
                    }
                    /*else if (obj.name.Split('_')[0] == "waveHandle")*/
                    else if (obj.name == "waveHandle_1")
                            {
                        /*pos.x = obj.position.x;*/
                        /*pos.x = -5;*/
                        if (pos.y > 3)
                            pos.y = 3;
                        if (pos.y < -2)
                            pos.y = -2;
                        obj.GetComponent<SineController>().SetPosition(pos);
                    }
                }
            }
        }
    }
    public void createNewWave(Vector3 spawnPos)
    {

        //Instantiate prefab
        GameObject newWave = Instantiate(wavePrefab, spawnPos, Quaternion.identity);
        allWaves.Add(newWave);
        newWave.name = "waveHandle_" + allWaves.Count;

        //Get controller script
        SineController sineScript = newWave.GetComponent<SineController>();

        //Add material and Shader 
        /*sineScript.mesh.GetComponent<MeshRenderer>().material = new Material(waveShader);*/
        /*sineScript.setMaterial();*/

        //Get AudioController script
        AudioController audioScript = newWave.GetComponent<AudioController>();

        //Set Object active
        newWave.SetActive(true);
    }
    public void SelectTab(Button button)
    {
        Debug.Log("BUTTON - ALL WAVES PRINTED:" + allWaves.Count);
        Button prevSelected = GameObject.FindWithTag("selected").GetComponent<Button>(); ;
        prevSelected.GetComponent<Image>().sprite = otherButton;
        prevSelected.tag = "buttonTab";
        button.GetComponent<Image>().sprite = selectedButton;
        button.tag = "selected";
        /*ResetWave();*/

        if (button.name == "Amplutude_Button")
        {
            Debug.Log("BUTTON - Amplitude");
            panelA.SetActive(true);
            panelF.SetActive(false);
            paneladd.SetActive(false);
            aH.SetActive(true);
            fH.SetActive(false);
            wave.GetComponent<SphereCollider>().enabled = false;

        }
        else if(button.name == "Frequency_Button")
        {
            Debug.Log("BUTTON - Frequency");
            panelF.SetActive(true);
            panelA.SetActive(false);
            paneladd.SetActive(false);
            aH.SetActive(false);
            fH.SetActive(true);
            wave.GetComponent<SphereCollider>().enabled = false;
        }
        else if(button.name == "ADD_Button")
        {
            Debug.Log("BUTTON - Add");
            paneladd.SetActive(true);
            panelA.SetActive(false);
            panelF.SetActive(false);
            wave.GetComponent<SphereCollider>().enabled = true;
            Vector3 pos = new Vector3(wave.transform.position.x, 3, wave.transform.position.z);
            wave.GetComponent<SineController>().SetPosition(pos);
            aH.SetActive(true);
            fH.SetActive(true);
            createNewWave(new Vector3(spawnPoint.transform.position.x, 0, spawnPoint.transform.position.z));
        }
    }

    public void ResetWave()
    {
        GameObject wave_reset = allWaves[0];
        Debug.Log("wave_reset name is: " + wave_reset.name);
        int i = allWaves.Count - 1;
        while (i >= 0)
        {
            if (wave_reset == null)
            {
                wave_reset = allWaves[i - 1];
                allWaves.RemoveAt(i);
            }

            else if (wave_reset != null && wave_reset != allWaves[i])
            {
                if(allWaves[i] != null)
                    Object.Destroy(allWaves[i]);
                allWaves.RemoveAt(i);
            }

            i--;
        }
        wave_reset.GetComponent<SineController>().SetPosition(spawnPoint.transform.position);
        /*wave_reset.GetComponent<SineController>().mesh.GetComponent<MeshRenderer>().material = new Material(waveShader);*/
        /*wave_reset.GetComponent<SineController>().setMaterial();*/
        float[] fa = wave_reset.GetComponent<SineController>().getFandA();
        wave_reset.GetComponent<SineController>().ChangeFrequency(1f - fa[0]);
        wave_reset.GetComponent<SineController>().ChangeAmplitude(1f - fa[1]);
        wave = wave_reset;
        Debug.Log("WAVE name is: " + wave.name);
    }
}
