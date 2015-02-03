using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {

	public bool bothWays;
	public int visibility;

	Vector2 startPos;
	Vector2 endPos;
	public void parseWall(string wall, MapGenerator mapGenerator) {
		string[] walls = wall.Split(",".ToCharArray());
		int curr = 0;
		startPos = new Vector2(float.Parse(walls[curr++])/mapGenerator.gridSize, -float.Parse(walls[curr++])/mapGenerator.gridSize);
		endPos = new Vector2(float.Parse(walls[curr++])/mapGenerator.gridSize, -float.Parse(walls[curr++])/mapGenerator.gridSize);
		bothWays = int.Parse(walls[curr++])==1;
		visibility = int.Parse(walls[curr++]);
		setTransform();
	}

	public void setTransform() {
		transform.position = new Vector3(startPos.x + (endPos.x - startPos.x)/2.0f, startPos.y + (endPos.y - startPos.y)/2.0f, transform.position.z);
		float angle = MapGenerator.getAngle(startPos, endPos);// + 90.0f;
		while (angle >= 360) angle -= 360;
		transform.localEulerAngles = new Vector3(0.0f, 0.0f, angle);
		transform.localScale = new Vector3(Vector2.Distance(startPos, endPos), 0.06f, 1.0f);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
