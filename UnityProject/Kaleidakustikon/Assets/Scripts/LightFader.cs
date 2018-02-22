using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightFader : MonoBehaviour {

	private Light light;

	private float fadeTime;
	private float elapsedTime;
	private float goalIntensity;
	private float currentIntensity;
	private bool fading;

	// Use this for initialization
	void Start () {
		light = GetComponent <Light> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (fading) {
			elapsedTime += Time.deltaTime;
			var t = elapsedTime / fadeTime;
			t = t * t * t * (t * (6f * t - 15f) + 10f);
			var newIntensity = currentIntensity + (goalIntensity - currentIntensity) * t;
			if (elapsedTime > fadeTime) {
				light.intensity = goalIntensity;
				fading = false;
			} else {
				light.intensity = newIntensity;
			}
		}
	}

	public void fadeIntensity(float toIntensity, float time){
		goalIntensity = toIntensity;
		currentIntensity = light.intensity;
		fadeTime = time;
		elapsedTime = 0;
		fading = true;
	}
}
