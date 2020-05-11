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

    public float freqScale = 1;
    public float ampScale = 1;
	public Wave [] waves;

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
	}


    // Play all active parentWaves audio frequencies
	void OnAudioFilterRead(float[] data, int channels)
	{
		for (int i = 0; i < data.Length; i += channels) {
			data[i] = 0;

			for (int j = 0; j < waveCount; j++) {
				waves[j].phase += waves[j].incr;
				// data[i] += waves[j].amp * Mathf.Sin(timestamp + (i/2 + 1) * waves[j].freq * 2f * Mathf.PI / samplingFrequency );
				data[i] += waves[j].amp * Mathf.Sin(waves[j].phase);
				// audio += waves[j].amp * Mathf.Sin((float)AudioSettings.dspTime);
			}
			data[i] *= ampScale;

			if (channels == 2) {
				data[i + 1] = data[i]; // mirror audio in two channel headphones for now
			}

		}
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