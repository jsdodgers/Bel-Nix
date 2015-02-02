using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {

	public GridManager gridManager;
	public bool bothWays;
	public Color color;
	public int visibility;
	public LineRenderer lineRenderer;
	public GameObject startCircle;
	public GameObject endCircle;
	public GameObject blocked;
	public GameObject blockedBoth;
	CapsuleCollider capsule;
//	PolygonCollider2D polyCollider;
	BoxCollider2D boxCollider;
	public Vector3 startPos = new Vector3();
	public Vector3 endPos = new Vector3();
	bool changed = true;
	bool shownI = false;
	public string stringValue() {
		return getVecString(startPos) + "," + getVecString(endPos) + "," + (bothWays ? 1 : 0) + "," + visibility + "," + getColorString(color);
	}

	public void parseWall(string wall) {
		int curr = 0;
		string[] spl = wall.Split(",".ToCharArray());
		setStart(float.Parse(spl[curr++])/gridManager.tileSize - gridManager.gridX/2.0f, gridManager.gridY/2.0f - float.Parse(spl[curr++])/gridManager.tileSize);
		setEnd(float.Parse(spl[curr++])/gridManager.tileSize - gridManager.gridX/2.0f, gridManager.gridY/2.0f - float.Parse(spl[curr++])/gridManager.tileSize);
		setBothWays(int.Parse(spl[curr++])==1);
		visibility = int.Parse(spl[curr++]);
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
/*		capsule = gameObject.AddComponent<CapsuleCollider>();
		capsule.enabled = true;
		capsule.radius = 0.905f;
		capsule.center = Vector3.zero;
		capsule.direction = 2;*/
	}
	
	// Update is called once per frame
	void Update () {
		if (changed) {
			changed = false;
/*			capsule.transform.position = startPos + (endPos - startPos)/2.0f;
			capsule.transform.LookAt(startPos);
			capsule.height = (endPos - startPos).magnitude;
			capsule.gameObject.layer = 8;
			changed = false;*/
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
	}
}
