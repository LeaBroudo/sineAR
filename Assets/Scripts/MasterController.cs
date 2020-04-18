using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterController : MonoBehaviour
{
    public GameObject wavePrefab; 
    public Shader waveShader;
    
    private List<GameObject> allWaves; 
    
    // Start is called before the first frame update
    void Start()
    {
        allWaves = new List<GameObject>();
        createNewWave();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void createNewWave() {
        
        //Instantiate prefab
        Vector3 spawnPos = this.transform.position + new Vector3(0,0,0.5f);
        GameObject newWave = Instantiate(wavePrefab, spawnPos, Quaternion.identity);

        //Add material and Shader 
        newWave.GetComponent<MeshRenderer>().material = new Material(waveShader);
        
        //Add controller script
        SineController sineScript = newWave.AddComponent<SineController>();
        sineScript.mat = newWave.GetComponent<MeshRenderer>().material;
        sineScript.changeFrequency(0.5f);
        sineScript.changeAmplitude(-0.5f);

        //TODO: Make sound

        //Add wave to list
        allWaves.Add(newWave);


    }
}
