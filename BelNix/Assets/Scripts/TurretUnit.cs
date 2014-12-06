using UnityEngine;
using System.Collections;

public class TurretUnit : Unit {

	public Turret turret;
	public Direction direction;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override bool deadOrDyingOrUnconscious() {
		return false;
	}

	public void setDirection(Direction dir) {
		direction = dir;
		switch (direction) {
		case Direction.Down:
			transform.localEulerAngles = new Vector3(0.0f, 0.0f, 180.0f);
			break;
		case Direction.Up:
			transform.localEulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
			break;
		case Direction.Left:
			transform.localEulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
			break;
		case Direction.Right:
			transform.localEulerAngles = new Vector3(0.0f, 0.0f, 270.0f);
			break;
		default:
			break;
		}
	}

	
	public override void setPosition(Vector3 pos) {
	//	setNewTilePosition(pos);
		position = pos;
		transform.localPosition = new Vector3(pos.x + .5f, pos.y - .5f, pos.z);
	//	currentMaxPath = 0;
	//	resetPath();
	}

}
