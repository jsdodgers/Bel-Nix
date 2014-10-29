using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public MapGenerator mapGenerator;
	public int currentMoveDist = 5;
	public int attackRange = 1;
	public int viewDist = 11;
	public int maxMoveDist = 5;

	public Vector3 position;


	public void setPosition(Vector3 pos) {
		position = pos;
		transform.localPosition = new Vector3(pos.x + .5f, pos.y - .5f, pos.z);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
