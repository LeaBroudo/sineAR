using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

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
	private float leftScale = 0f;
	private float rightScale = 0f;
	private float minScale = 0.1f;

    public struct Wave
	{
	    public float freq;
	    public float amp;
	    public float phase;
	    public float incr;

		public Wave (float frequency, float amplitude, float phaseArg = 0f) {
			freq = frequency;
			amp = amplitude;
			phase = phaseArg;
			incr = frequency * 2f * Mathf.PI / 44100f;
		}

		public void updateIncr (float frequency, float scale) {
			incr = frequency * 2f * Mathf.PI * scale / 44100f;
		}

	}

	public void Awake()
	{
		sineScript = this.GetComponent<SineController>();
		
		samplingFrequency = AudioSettings.outputSampleRate;
		print(samplingFrequency);

		waves = new Wave[sineScript.maxParents];
		waves[0] = new Wave(sineScript.getFrequency(), 
							sineScript.getAmplitude());
		waveCount = 1;

		camera = GameObject.Find("ARCamera");

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
				Wave newWave = new Wave(newFreq, newAmp, waves[0].phase);
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

		// 	leftScale = 1f;
		// 	rightScale = 1f;
		// 	print("forward with left, right" + leftScale + rightScale);
		// } else if (angle > 25f) { // Wave is to the left
		// 	leftScale = Mathf.Clamp(angle % 90f / 90f, 0.75f, 1f);
		// 	rightScale = Mathf.Clamp(angle % 90f / 180f, 0.1f, .5f);
		// 	print("left with left, right" + leftScale + rightScale);
		// }
		// else { // Wave is to the right
		// 	rightScale = Mathf.Clamp(Mathf.Abs(angle) % 90f / 90f, 0.75f, 1f);
		// 	leftScale = Mathf.Clamp(Mathf.Abs(angle) % 90f / 180f, 0.1f, .5f);
			print("left: " + leftScale + " right: " + rightScale);
		// }
		// print(angle);

		// leftScale = Mathf.Clamp(leftScale, minScale, 1f);
		// rightScale = Mathf.Clamp(rightScale, minScale, 1f);
	}


    // Play all active parentWaves audio frequencies
	void OnAudioFilterRead(float[] data, int channels)
	{
		// float start = (float) AudioSettings.dspTime;
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
				data[i + 1] = data[i] * rightScale; // mirror audio in two channel headphones
			}

		}
		// for (int j = 1; j  < waveCount; j++){
		// 	waves[j].phase = waves[0].phase;
		// }
	}

	// public void makeSawtooth () {
	// 	return;
	// }

	// public void makeSquare () {
	// 	waves[0].freq = freqConversion;
	// 	waves[0].amp = 0.05f;
	// 	// waves[1] = new Wave(freqConversion * 3f, .05f / 3f);
	// 	waves[1].freq = freqConversion*3f;
	// 	waves[1].amp = 0.05f/3f;
	// 	// waves[2] = new Wave(freqConversion * 5f, .05f / 5f);
	// 	waves[2].freq = freqConversion*5f;
	// 	waves[2].amp = 0.05f/5f;
	// 	waveCount = 3;
	// 	for(int w = 0; w < waveCount; w++){
	// 			// waves[w].updateIncr(waves[w].freq, freqScale);
	// 		print("Wave: "+w+" Freq: " + waves[w].freq + " Amp: " + waves[w].amp);
	// 	}
	// 	// defaultShape = false;
	// }

	// public void makeTriangle () {
	// 	return;		
	// }

	public void setEnabled(bool toSet=true)
	{
		audioEnabled = toSet;
	}
}