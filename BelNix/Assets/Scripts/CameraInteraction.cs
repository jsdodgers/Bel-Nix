using UnityEngine;
using System.Collections;

public class CameraInteraction : MonoBehaviour  {

	public Sprite spr;
	Texture2D tex;

	// Use this for initialization
	void Start ()  {
		tex = spr.texture;
	}
	
	// Update is called once per frame
	void Update ()  {
		Camera.main.orthographicSize = tex.height / 64.0f / 2.0f;
//		Camera.main.orthographicSize = Screen.height / 128.0f;
	}
}
