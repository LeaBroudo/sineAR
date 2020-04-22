using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{

	public float frequency = 440f;
	private float increment;
	private float phase;
	private float samplingFrequency = 48000f;
	const float pi = Mathf.PI;

	public float gain;
	public float volume = 0.1f;

	public float [] frequencies;
	public int freqIdx;


	void Start()
	{
		frequencies = new float[9];
		frequencies[0] = 440;
		frequencies[1] = 494;
		frequencies[2] = 554;
		frequencies[3] = 587;
		frequencies[4] = 659;
		frequencies[5] = 740;
		frequencies[6] = 831;
		frequencies[7] = 880;
		frequencies[8] = 432; // added in this tone for fun cause some people think that A should actually be
							  // tuned to 432 instead of 440
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			gain = volume;
			frequency = frequencies[freqIdx];
			freqIdx += 1;
			freqIdx = freqIdx % frequencies.Length;
		}

		if (Input.GetKeyUp(KeyCode.Space))
		{
			gain = 0f;
		}
	}

	void OnAudioFilterRead(float[] data, int channels)
	{
		increment = frequency * 2f * pi / samplingFrequency;
		Debug.Log("Number of channels:" + channels + ", data.Length: " + data.Length);

		for (int i = 0; i < data.Length; i += channels)
		{
			phase += increment;
			data[i] = gain * Mathf.Sin(phase);

			if (channels == 2)
			{
				data[i + 1] = data[i]; // mirror audio in two channel headphones for now
			}

			if (phase > 2 * pi) {
				phase -= 2 * pi;
			}
		}
	}
}