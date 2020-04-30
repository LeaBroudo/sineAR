using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{

	public bool enabled;
	public float frequency; /* Range ~ [110,3520] */
	public float gain;
	// public float volume = 0.1f;

	private float increment; // Step along wave
	private float phase; 
	private float samplingFrequency = 48000f;

	public float [] frequencies;
	public int freqIdx;

    private SineController sineScript;
    private float[] parentWaves;
    private int parentCount;

    // private bool isParent;


	void Start()
	{
		sineScript = this.GetComponent<SineController>();
		parentCount = sineScript.parentCount;
		frequency = 440;
		gain = 0.1f;
		// isParent = false;
		setEnabled(true);
	}

	void Update()
	{
		parentCount = sineScript.parentCount;
		if (parentCount > 0) { // conditional for parent --> update parentWaves just once and then w msg
			parentWaves = sineScript.getCollidedParents();
		}
		print(frequency);
	}

    // Play all active parentWaves audio frequencies
	void OnAudioFilterRead(float[] data, int channels)
	{
		if (enabled){
			if (parentCount == 0) {
				frequency = sineScript.getFrequency() / 1.75f;
				gain = sineScript.getAmplitude() / 2.5f ;
			} 
			else {
			// 	float childFreqs = 0;
			// 	for (int wCount=0; wCount < parentCount/2; wCount++) {
			// 		childFreqs += (parentWaves[])
			// 	gain = sineScript.getAmplitude();
			// }

			increment = frequency * 2f * Mathf.PI / samplingFrequency;
			Debug.Log("Number of channels:" + channels + ", data.Length: " + data.Length);

			for (int i = 0; i < data.Length; i += channels) {
				phase += increment;
				data[i] = gain * Mathf.Sin(phase);

				if (channels == 2) {
					data[i + 1] = data[i]; // mirror audio in two channel headphones for now
				}

				if (phase > 2 * Mathf.PI) {
					phase -= 2 * Mathf.PI;
				}
			}
		}
	}


	public void setEnabled(bool toSet=true)
	{
		enabled = toSet;
	}
}