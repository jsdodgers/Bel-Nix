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

//	public bool passable;
	public bool standable;

	public int passableRight;
	public int passableLeft;
	public int passableDown;
	public int passableUp;

	public int trigger;
	public int action; 

	public Tile() {

	}

	public Tile(GameObject go, int x1, int y1,float r, float g, float b, float a) {
		tileGameObject = go;
		standable = true;
//		passable = true;
		passableDown = 0;
		passableUp = 0;
		passableLeft = 0;
		passableRight = 0;
		trigger = 0;
		action = 0;
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
		string str = x + "," + y + "," + ((int)red) + "," + ((int)green) + "," + ((int)blue) + "," + ((int)(alpha*255)) +
			"," + (standable?1:0) + "," + passableUp + "," + passableRight + "," + passableDown + "," + passableLeft + "," + trigger + "," + action;
		return str;
	}


	public void parseTile(string tile) {
		string[] strs = tile.Split(new char[]{','});
		int curr = 0;
		passableUp = 0;
		passableDown = 0;
		passableLeft = 0;
		passableRight = 0;
		trigger = 0;
		action = 0;
		alpha = 0.4f;
		x = int.Parse(strs[curr]);curr++;if (strs.Length<=curr) return;
		y = int.Parse(strs[curr]);curr++;if (strs.Length<=curr) return;
		red = float.Parse(strs[curr]);curr++;if (strs.Length<=curr) return;
		green = float.Parse(strs[curr]);curr++;if (strs.Length<=curr) return;
		blue = float.Parse(strs[curr]);curr++;if (strs.Length<=curr) return;
		alpha = float.Parse(strs[curr])/255.0f;if (alpha==0) alpha = .4f;curr++;if (strs.Length<=curr) return;
		setSpriteColor();
		standable = int.Parse(strs[curr])==1;curr++;if (strs.Length<=curr) return;
		passableUp = int.Parse(strs[curr]);curr++;if (strs.Length<=curr) return;
		passableRight = int.Parse(strs[curr]);curr++;if (strs.Length<=curr) return;
		passableDown = int.Parse(strs[curr]);curr++;if (strs.Length<=curr) return;
		passableLeft = int.Parse(strs[curr]);curr++;if (strs.Length<=curr) return;
		trigger = int.Parse(strs[curr]);curr++;if (strs.Length<=curr) return;
		action = int.Parse(strs[curr]);curr++;if (strs.Length<=curr) return;
//		passable = int.Parse(strs[curr])==1;curr++;
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
