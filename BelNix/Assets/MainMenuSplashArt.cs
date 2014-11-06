using UnityEngine;
using System.Collections;

public class MainMenuSplashArt : MonoBehaviour {

	public Texture splashArt;
	// Use this for initialization
	void Start () {
	
	}

	void OnGUI ()
	{
		GUI.depth = 10;
		if (!splashArt) {
			Debug.LogError("Assign a Texture in the inspector.");
			return;
		}
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), splashArt, ScaleMode.StretchToFill, true, 0);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
