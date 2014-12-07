using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrapUnit : Unit {

	public Trap trap;
	public List<TrapUnit> fullTrap;
	public bool selectedForPlacement;

	public void setSelectedForPlacement() {
		selectedForPlacement = true;
	}

	public void unsetSelectedForPlacement() {
		selectedForPlacement = false;
		transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
	}

	public void doPlacementExpand() {
		if (!selectedForPlacement) return;
		float factor = 1.0f/10.0f;
		float speed = 3.0f;
		float addedScale = Mathf.Sin(Time.time * speed) * factor;
		float scale = 1.0f + factor + addedScale;
		transform.localScale = new Vector3(scale, scale, 1.0f);
	}

	
	public override void setPosition(Vector3 pos) {
		//	setNewTilePosition(pos);
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
		doPlacementExpand();
	}
}
