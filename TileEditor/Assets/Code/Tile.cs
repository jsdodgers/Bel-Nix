using UnityEngine;
using System.Collections;

public class Tile {

	public GameObject tileGameObject;
	public int x;
	public int y;


	public float red;
	public float green;
	public float blue;
	public float alpha;

	public bool passable;
	public bool standable;

	public Tile() {

	}

	public Tile(GameObject go, int x1, int y1,float r, float g, float b, float a) {
		tileGameObject = go;
		standable = true;
		passable = true;
		setColor(r, g, b, a);
		x = x1;
		y = y1;
	}

	public void setColor(float r, float g, float b, float a) {	
		red = r;
		green = g;
		blue = b;
		alpha = a;
		setSpriteColor();
	}

	public void setColor(Color c) {
		setColor(c.r,c.g,c.b,c.a);
	}

	public void setSpriteColor() {
		if (tileGameObject) {	
//			Debug.Log("set
			SpriteRenderer sR = tileGameObject.GetComponent<SpriteRenderer>();
			sR.color = new Color(red/255.0f,green/255.0f,blue/255.0f,alpha);
		}
	}

	public string stringValue() {
		string str = x + "," + y + "," + ((int)red) + "," + ((int)green) + "," + ((int)blue) + "," + ((int)alpha*255) +
			"," + (standable?1:0) + "," + (passable?1:0);
		return str;
	}


	public void parseTile(string tile) {
		string[] strs = tile.Split(new char[]{','});
		int curr = 0;
		x = int.Parse(strs[curr]);curr++;
		y = int.Parse(strs[curr]);curr++;
		red = float.Parse(strs[curr]);curr++;
		green = float.Parse(strs[curr]);curr++;
		blue = float.Parse(strs[curr]);curr++;
		standable = int.Parse(strs[curr])==1;curr++;
		passable = int.Parse(strs[curr])==1;curr++;
		setSpriteColor();
	}

	public static int xForTile(string tile) {
		string[] strs = tile.Split(new char[]{','});
		return int.Parse(strs[0]);
	}

	public static int yForTile(string tile) {
		string[] strs = tile.Split(new char[]{','});
		return int.Parse(strs[1]);
	}
}
