using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour {

	public GridManager gridManager;
	public string textureName;
	public int visibility;
	public bool canRange = false;
	public bool canMelee = false;
	public LineRenderer lineRenderer;
	public GameObject startCircle;
	public GameObject endCircle;
//	public GameObject blocked;
//	public GameObject blockedBoth;
//	CapsuleCollider capsule;
//	PolygonCollider2D polyCollider;
	BoxCollider2D boxCollider;
	SpriteRenderer spriteRenderer;
	public Vector3 startPos = new Vector3();
//	public Vector3 endPos = new Vector3();
//	public Vector3 openPos = new Vector3();
	public float closedAngle = 0.0f;
	public float openAngle = 0.0f;
	public bool clockWise = true;
	public float length;
	public float width = 1.0f;
	bool changed = true;
	bool shownI = false;
/*	public string stringValue() {
		return getVecString(startPos) + "," + getVecString(endPos) + "," + getVecString(openPos) + "," + (clockWise ? 1 : 0) + "," + width + "," + visibility + "," + (canRange?1:0) + "," + (canMelee?1:0) + "," + getColorString(color);
	}

	public void parseDoor(string door) {
		int curr = 0;
		string[] spl = door.Split(",".ToCharArray());
		setStart(float.Parse(spl[curr++])/gridManager.tileSize - gridManager.gridX/2.0f, gridManager.gridY/2.0f - float.Parse(spl[curr++])/gridManager.tileSize);
		setEnd(float.Parse(spl[curr++])/gridManager.tileSize - gridManager.gridX/2.0f, gridManager.gridY/2.0f - float.Parse(spl[curr++])/gridManager.tileSize);
		setBothWays(int.Parse(spl[curr++])==1);
		visibility = int.Parse(spl[curr++]);
		if (spl.Length > 11) {
			canRange = int.Parse(spl[curr++])==1;
			canMelee = int.Parse(spl[curr++])==1;
		}
		setColor(new Color(int.Parse(spl[curr++])/255.0f,int.Parse(spl[curr++])/255.0f,int.Parse(spl[curr++])/255.0f));
	}


	public string getColorString(Color c) {
		return ((int)(c.r * 255)) + "," + ((int)(c.g * 255)) + "," + ((int)(c.b * 255));
	}

	public string getVecString(Vector3 vec) {
		return (int)((vec.x + gridManager.gridX/2.0f)*gridManager.tileSize) + "," + (int)((gridManager.gridY - (vec.y + gridManager.gridY/2.0f))*gridManager.tileSize);
	}

	public void setStart(float x, float y) {
		startPos = new Vector3(x, y, 0.0f);
		lineRenderer.SetPosition(0, startPos);
		changed = true;
	}

	public void setEnd(float x, float y) {
		endPos = new Vector3(x, y, 0.0f);
		lineRenderer.SetPosition(1, endPos);
		changed = true;
	}

	public void setOpen(float x, float y) {
		openPos = new Vector3(x, y, 0.0f);
	}

	public void setColor(Color c) {
		c.a = .6f;
		color = c;
		lineRenderer.SetColors(c, c);
	}

	public void setBothWays(bool bW) {
		bothWays = bW;
		setBlockedShown(shownI);
	}

	public void setBlockedShown(bool shown) {
		shownI = shown;
		blocked.SetActive(shown);
		blockedBoth.SetActive(shown && bothWays);
	}

	public void setCirclesShown(bool shown) {
		startCircle.SetActive(shown);
		endCircle.SetActive(shown);
//		startCircle.GetComponent<SpriteRenderer>().enabled = shown;
//		endCircle.GetComponent<SpriteRenderer>().enabled = shown;
	}

	// Use this for initialization
	void Start () {
		boxCollider = gameObject.AddComponent<BoxCollider2D>();
		boxCollider.size = new Vector2(100.0f, 50.0f);
		boxCollider.center = Vector2.zero;
//		polyCollider = gameObject.AddComponent<PolygonCollider2D>();
//		polyCollider.gameObject.layer = 8;

	}
	
	// Update is called once per frame
	void Update () {
		if (changed) {
			changed = false;

			float dist = Vector2.Distance(new Vector2(startPos.x, startPos.y), new Vector2(endPos.x, endPos.y));//(startPos - endPos).magnitude;
			boxCollider.size = new Vector2(dist, 0.1f);
			transform.position = startPos + (endPos - startPos)/2.0f;
		//	transform.LookAt(startPos);
			//			transform.eulerAngles = new Vector3(0.0f, 0.0f, Mathf.Tan(Mathf.Abs(startPos.y - endPos.y) / Mathf.Abs(startPos.x - endPos.x)));
			float deg = (dist == 0 ? 0 : Mathf.Acos((endPos.x - startPos.x)/dist) * 180/Mathf.PI);
			transform.eulerAngles = new Vector3(0.0f, 0.0f, (endPos.y > startPos.y ? deg : 360 - deg));
			startCircle.transform.position = new Vector3(startPos.x, startPos.y, -1.0f);
			endCircle.transform.position = new Vector3(endPos.x, endPos.y, -1.0f);
		}
	}*/
}
