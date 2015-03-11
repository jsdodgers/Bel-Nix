using UnityEngine;
using System.Collections;

public class Player : Unit  {


	public override void setPosition(Vector3 pos)  {
		setNewTilePosition(pos);
		position = pos;
		transform.localPosition = new Vector3(pos.x + .5f, pos.y - .5f, pos.z);
		currentMaxPath = 0;
		resetPath();
	}
	

	// Use this for initialization
	void Start ()  {
		initializeVariables();

	}

	public override void initializeVariables()  {
		team = 0;
		base.initializeVariables();
//		currentPath = new ArrayList();
	//	currentMaxPath = 0;
	}
	
	// Update is called once per frame
//	void Update ()  {
	//	if (attacking)  {
	//		attackAnimation();
	//		attacking = false;
	//	}
	//	Debug.Log("Player Update");



	//	Debug.Log("Player Update End");
//	}

}
