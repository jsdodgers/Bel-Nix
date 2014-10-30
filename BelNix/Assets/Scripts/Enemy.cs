using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	
	public Vector3 position;
	
	
	public void setPosition(Vector3 pos) {
		position = pos;
		transform.localPosition = new Vector3(pos.x + .5f, pos.y - .5f, pos.z);
	//	currentMaxPath = 0;
	//	resetPath();
	}



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
