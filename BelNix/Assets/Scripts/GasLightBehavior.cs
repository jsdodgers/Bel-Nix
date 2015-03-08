using UnityEngine;
using System.Collections;

public class GasLightBehavior : MonoBehaviour {

	private bool flickering;

	// Use this for initialization
	void Start () {
	
		//Debug.Log("Lamp start!");
		//InvokeRepeating("updateLamp", 0, 0.1f);
	}
	
	// Update is called once per frame
	void Update () {
	

	}

	void Awake()
	{
		//Debug.Log("Lamp start!");
		int starter = Random.Range(0, 2);
		if(starter == 0)
		{
			startFlickering();
		}
		else
		{
			stopFlickering();
		}
		InvokeRepeating("updateLamp", 0, 0.05f);
	}

	private void startFlickering()
	{
		flickering = true;
		Invoke("stopFlickering", Random.Range(0.5f, 2.0f));
	}

	private void stopFlickering()
	{
		flickering = false;
		SpriteRenderer gasLamp = GetComponent<SpriteRenderer>();
		gasLamp.color = new Color (gasLamp.color.r, gasLamp.color.g, gasLamp.color.b, Random.Range(0.4f, 0.8f));
		Invoke("startFlickering", Random.Range(2.0f, 5.0f));
	}

	private void updateLamp()
	{
		if(!flickering)
		{
			return;
		}
		SpriteRenderer gasLamp = GetComponent<SpriteRenderer>();
		float currentAlpha = gasLamp.color.a * 255;
		float maxAlpha = 255;
		float minAlpha = 50;
		float step = Random.Range(0.0f, 0.1f) * 255;
		int direction = Random.Range (-1, 2);
		step *= direction;
		float newAlpha = currentAlpha + step;
		if(newAlpha < minAlpha)
		{
			newAlpha = minAlpha;
		}
		if(newAlpha > maxAlpha)
		{
			newAlpha = maxAlpha;
		}
		gasLamp.color = new Color (gasLamp.color.r, gasLamp.color.g, gasLamp.color.b, newAlpha/255);
		//Debug.Log ("newAlpha: " + newAlpha);
	}
}
