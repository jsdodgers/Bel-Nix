using UnityEngine;
using System.Collections;

public class Tile {

	public GameObject tileGameObject;
	public float red;
	public float green;
	public float blue;
	public float alpha;

	public bool passable;
	public bool standable;

	public Tile() {

	}

	public Tile(GameObject go,float r, float g, float b, float a) {
		tileGameObject = go;
		setColor(r, g, b, a);
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
}
