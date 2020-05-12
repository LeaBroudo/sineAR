using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{

	public bool audioEnabled;
	public float samplingFrequency = 44100f;

    private SineController sineScript;
	private GameObject camera;
    private float[] parentWaves;
    private int waveCount;

	public Wave [] waves;
    private float freqConversion = 220f;
    private float amplConversion = 0.05f;

    public float freqScale = 1f;
    public float ampScale = 1f;
	private float leftScale = 0.01f;
	private float rightScale = 0.01f;
	private float minScale = 0.1f;

	private bool inLearn2D = false;


	public void Awake()
	{
		sineScript = this.GetComponent<SineController>();
		samplingFrequency = AudioSettings.outputSampleRate;
		print("Audio Sample Rate: " + samplingFrequency);

		waves = new Wave[sineScript.maxParents];
		waves[0] = new Wave(sineScript.getFrequency(), 
							sineScript.getAmplitude(), 
							0f,
							samplingFrequency);
		waveCount = 1;

	    if (SceneManager.GetActiveScene().name == "Learn2D") {
            inLearn2D = true;
            leftScale = 1f;
            rightScale = 1f;
        } else {
            inLearn2D = false;
        }

		camera = GameObject.Find("ARCamera");
	}


	public void Update()
	{
		// If there are new waves to add from the mesh, add them!
		if ((sineScript.parentCount > 0) && (waveCount != sineScript.parentCount / 2)) {
			int numNewWaves = (sineScript.parentCount/2) - waveCount;
			print(numNewWaves);

			// get amp + freq for all new waves
			for (int i=0; i < numNewWaves; i++) {
				float newFreq = sineScript.parentWaves[waveCount * 2] * freqConversion;
				float newAmp =  sineScript.parentWaves[waveCount * 2 + 1] * amplConversion;
				Wave newWave = new Wave(newFreq, newAmp, waves[0].phase, 
										samplingFrequency);				
				waves[waveCount] = newWave;
				waveCount++;
			}
		}

		// Calculate L/R pan values for spatial audio based on ARCamera position
		if (!inLearn2D && camera != null){
			Vector3 fromWave = new Vector3(this.transform.position.x, 
										   camera.transform.position.y, 
										   this.transform.position.z);

			float angle = Vector3.SignedAngle(fromWave - camera.transform.position, 
											  camera.transform.forward, 
											  camera.transform.up);

			if (Mathf.Abs(angle) < 90f) { // In front of camera
				if (angle > 0f) { // In front to left
					leftScale = 1f;
					rightScale = Mathf.Clamp((90f - Mathf.Abs(angle))/90f, 0.1f, 1f);
				} else { 		 // In front to right
					leftScale = Mathf.Clamp((90f - Mathf.Abs(angle))/90f, 0.1f, 1f);
					rightScale = 1f;
				}
			} 
			else { // Behind camera
				if (angle > 0f) { // Behind to left
					leftScale = Mathf.Clamp((180f - Mathf.Abs(angle))/90f, 0.25f, 1f);
					rightScale = Mathf.Clamp(Mathf.Abs((90f - Mathf.Abs(angle))/90f),
											 0.1f, 0.25f); 
				} else {		  // Behind to right
					rightScale = Mathf.Clamp((180f - Mathf.Abs(angle))/90f, 0.25f, 1f);
					leftScale = Mathf.Clamp(Mathf.Abs((90f - Mathf.Abs(angle))/90f), 
											0.1f, 0.25f); 
				}
			}
		}
	}


	// Updates to scaling values from user manipulation
	void LateUpdate() {
		float amplitudeIncrement = 0.003f; // 0.005 more responsive but crackles

		// if single wave, use sineScript's conversions
		if (waveCount == 1) {
			waves[0].freq = sineScript.getFrequency();
			waves[0].amp  = Mathf.Clamp(sineScript.getAmplitude(),
										waves[0].amp - amplitudeIncrement,
										waves[0].amp + amplitudeIncrement);
			waves[0].incr = waves[0].freq * 2f * Mathf.PI / samplingFrequency;
		} 

		// else we have multiple waves, use parent wave as scale factor
		else if (freqScale != sineScript.getFrequency() / freqConversion ||
				 ampScale != sineScript.getAmplitude() / amplConversion) {
			
			amplitudeIncrement = 0.05f;
			freqScale = sineScript.getFrequency() / freqConversion;
			ampScale = Mathf.Clamp(sineScript.getAmplitude() / amplConversion, 
								   ampScale - amplitudeIncrement,
								   ampScale + amplitudeIncrement);

			for(int w = 0; w < waveCount; w++){
				waves[w].updateIncr(waves[w].freq, freqScale);
			}
		}

		// Update frequency of each wave, scaled by parent wave		
		for(int w = 0; w < waveCount; w++){
			waves[w].updateIncr(waves[w].freq, freqScale);
			print("Wave: "+w+" Freq: " + waves[w].freq*freqScale + 
				  " Amp: " + waves[w].amp*ampScale);
		}
	}


    // Send data from each of this object's waves to audio channels
	void OnAudioFilterRead(float[] data, int channels)
	{
		for (int i = 0; i < data.Length; i += channels) {
			data[i] = 0f;

			for (int j = 0; j < waveCount; j++) {
				waves[j].phase += waves[j].incr;
				data[i] += ampScale * waves[j].amp * Mathf.Sin(waves[j].phase); // Set L channel

				if (waves[j].phase > 2f * Mathf.PI) {
					waves[j].phase -= 2f * Mathf.PI;
				}
			}
			data[i] *= leftScale; // Scale L channel by pan value
			if (channels == 2) {
				data[i + 1] = data[i] / leftScale * rightScale; // Set R channel w/ right pan value
			}
		}
	}


    public struct Wave
	{
	    public float freq;
	    public float amp;
	    public float phase;
	    public float incr;
	    public float sampleFreq;

		public Wave (float frequency, float amplitude, float phaseArg = 0f, 
					 float samplingFrequency=44100f) {
			freq = frequency;
			amp = amplitude;
			phase = phaseArg;
			sampleFreq = samplingFrequency;
			incr = frequency * 2f * Mathf.PI / samplingFrequency;

		}

		public void updateIncr (float frequency, float scale) {
			incr = frequency * 2f * Mathf.PI * scale / sampleFreq;
		}
	}
}

