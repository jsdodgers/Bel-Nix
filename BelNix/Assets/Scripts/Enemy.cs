using UnityEngine;
using System.Collections;

public class Enemy : Unit {

//	public MapGenerator mapGenerator;

	/*
	public void setPosition(Vector3 pos) {
		position = pos;
		transform.localPosition = new Vector3(pos.x + .5f, pos.y - .5f, pos.z);
	//	currentMaxPath = 0;
	//	resetPath();
	}*/

	public override void setPosition(Vector3 pos) {
		setNewTilePosition(pos);
		position = pos;
		transform.localPosition = new Vector3(pos.x + .5f, pos.y - .5f, pos.z);
		currentMaxPath = 0;
		resetPath();
	}


	public override bool isDead() {
        return characterSheet.characterSheet.combatScores.isDead() || characterSheet.characterSheet.combatScores.isUnconscious() || characterSheet.characterSheet.combatScores.isDying();
	}

	// Use this for initialization
	void Start () {
		initializeVariables();
	}
	
	public override void initializeVariables() {
		team = 1;
		base.initializeVariables();
	}
	
	// Update is called once per frame
//	void Update () {
//	}
}
