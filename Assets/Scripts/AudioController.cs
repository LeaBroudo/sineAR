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
    private float[] parentWaves;
    private int waveCount;
    private float freqConversion = 220f;
    private float amplConversion = 0.05f;

    public float freqScale = 1f;
    public float ampScale = 1f;
	public Wave [] waves;

	private GameObject camera;
	private float leftScale = 0.01f;
	private float rightScale = 0.01f;
	private float minScale = 0.1f;

	private bool inLearn2D = false;

    public struct Wave
	{
	    public float freq;
	    public float amp;
	    public float phase;
	    public float incr;
	    public float sampleFreq;

		public Wave (float frequency, float amplitude, float phaseArg = 0f, float samplingFrequency=44100f) {
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

	public void Awake()
	{
		sineScript = this.GetComponent<SineController>();
		
		samplingFrequency = AudioSettings.outputSampleRate;
		print(samplingFrequency);

		waves = new Wave[sineScript.maxParents];
		waves[0] = new Wave(sineScript.getFrequency(), 
							sineScript.getAmplitude(), 
							0f,
							samplingFrequency);
		waveCount = 1;

		camera = GameObject.Find("ARCamera");

	    if (SceneManager.GetActiveScene().name == "Learn2D") {
            inLearn2D = true;
            leftScale = 1f;
            rightScale = 1f;
        } else {
            inLearn2D = false;
        }

		setEnabled(true);
	}

	public void Update()
	{
		if ((sineScript.parentCount > 0) && (waveCount != sineScript.parentCount / 2)) {
			int numNewWaves = (sineScript.parentCount/2) - waveCount;
			print(numNewWaves);

			// get amp + freq for all new waves
			for (int i=0; i < numNewWaves; i++) {
				float newFreq = sineScript.parentWaves[waveCount * 2] * freqConversion;
				float newAmp =  sineScript.parentWaves[waveCount * 2 + 1] * amplConversion;
				Wave newWave = new Wave(newFreq, newAmp, waves[0].phase, samplingFrequency);
				// print("new freq, amp:" + newFreq + '\n' + newAmp + '\n' );
				
				waves[waveCount] = newWave;
				waveCount++;
			}
		}

		// if single wave, use sineScript's conversions
		if (waveCount == 1) {
			waves[0].freq = sineScript.getFrequency();
			waves[0].amp  = sineScript.getAmplitude();
			waves[0].incr = waves[0].freq * 2f * Mathf.PI / samplingFrequency;
		} 

		// else we have multiple waves, use parent wave as scale factor
		// CAN ALSO JUST QUERY MESH FREQ...
		else if (freqScale != sineScript.getFrequency() / freqConversion 
				   || ampScale != sineScript.getAmplitude() / amplConversion) {
			freqScale = sineScript.getFrequency() / freqConversion ;
			ampScale = sineScript.getAmplitude() / ( amplConversion);
			for(int w = 0; w < waveCount; w++){
				waves[w].updateIncr(waves[w].freq, freqScale);
			}
		}

		for(int w = 0; w < waveCount; w++){
			waves[w].updateIncr(waves[w].freq, freqScale);
			print("Wave: "+w+" Freq: " + waves[w].freq*freqScale + " Amp: " + waves[w].amp*ampScale);
		}

		if (!inLearn2D && camera != null){
			Vector3 fromWave = new Vector3(this.transform.position.x, camera.transform.position.y, this.transform.position.z);
			float angle = Vector3.SignedAngle(fromWave - camera.transform.position, camera.transform.forward, camera.transform.up);
			print("angle: " + angle);
			if (Mathf.Abs(angle) < 90f) { // In front
				if (angle > 0f) { // In front to left
					leftScale = 1f;
					rightScale = Mathf.Clamp((90f - Mathf.Abs(angle)) / 90f, 0.1f, 1f);
				} else { 		 // In front to the right
					leftScale = Mathf.Clamp((90f - Mathf.Abs(angle)) / 90f, 0.1f, 1f);
					rightScale = 1f;
				}
			} else { // Behind
				if (angle > 0f) { // Behind to the left
					leftScale = Mathf.Clamp((180f - Mathf.Abs(angle)) / 90f, 0.25f, 1f);
					rightScale = Mathf.Clamp(Mathf.Abs((90f - Mathf.Abs(angle)) / 90f), 0.1f, 0.25f); 
				} else {		  // Behind to the right
					rightScale = Mathf.Clamp((180f - Mathf.Abs(angle)) / 90f, 0.25f, 1f);
					leftScale = Mathf.Clamp(Mathf.Abs((90f - Mathf.Abs(angle)) / 90f), 0.1f, 0.25f); 
				}
			}
		}
		print("left: " + leftScale + " right: " + rightScale);
	}


    // Play all active parentWaves audio frequencies
	void OnAudioFilterRead(float[] data, int channels)
	{
		// float start = (float) AudioSettings.dspTime;
		if (audioEnabled){
			for (int i = 0; i < data.Length; i += channels) {
				data[i] = 0;
				// float start = (float) AudioSettings.dspTime;
				for (int j = 0; j < waveCount; j++) {
					// float phase = waves[0].phase;
					waves[j].phase += waves[j].incr;
					// start += waves[j].freq * 2f * Mathf.PI * freqScale / samplingFrequency;
					// data[i] += waves[j].amp * Mathf.Sin(timestamp + (i/2 + 1) * waves[j].freq * 2f * Mathf.PI / samplingFrequency );
					data[i] += waves[j].amp * Mathf.Sin(waves[j].phase);

					if (waves[j].phase > 2f * Mathf.PI) {
						waves[j].phase -= 2f * Mathf.PI;
					}
					// audio += waves[j].amp * Mathf.Sin((float)AudioSettings.dspTime);
				}
				
				data[i] *= ampScale * leftScale;

				if (channels == 2) {
					data[i + 1] = data[i] / leftScale * rightScale; // mirror audio in two channel headphones
				}
			}
		}
	}


	public void setEnabled(bool toSet=true)
	{
		audioEnabled = toSet;
	}
}