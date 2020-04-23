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
        
        ///*// For testing
        //sineScript.changeFrequency(0.5f);
        //sineScript.changeAmplitude(-0.5f);

        sineScript.addCollidedParent(1f, 1f);
        sineScript.addCollidedParent(2f, 1f);
        sineScript.addCollidedParent(3f, 1f);
        sineScript.addCollidedParent(4f, 1f);
        sineScript.addCollidedParent(5f, 1f);
        //*/

        //TODO: Make sound

        //Add wave to list
        allWaves.Add(newWave);


    }
}
