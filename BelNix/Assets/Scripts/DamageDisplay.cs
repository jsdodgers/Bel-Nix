using UnityEngine;
using System.Collections;

public class DamageDisplay : MonoBehaviour {

	public int damageAmount;
	public bool hit;
	public float timeInitialized;
	public Color color;
	public bool going = false;
	public Vector3 position;
	float time = 0.25f;
	float speed = 5.0f;
	GUIStyle labelStyle;

	public void begin(int damage, bool didHit, Vector3 pos) {
		position = pos;
		damageAmount = damage;
		hit = didHit;
		timeInitialized = Time.time;
		if (didHit) color = Color.red;
		else color = Color.gray;
		going = true;
	}

	public GUIStyle getLabelStyle() {
		if (labelStyle == null) {
			labelStyle = new GUIStyle("Label");
			labelStyle.fontSize = 50;
		}
		labelStyle.normal.textColor = labelStyle.active.textColor = labelStyle.hover.textColor = color;
		return labelStyle;
	}

	void OnGUI() {
		if (!going) return;
		Vector3 screenPos = Camera.main.WorldToScreenPoint(position);
		screenPos.y = Screen.height - screenPos.y;
	//	screenPos.y -= yDif;
		GUIStyle style = getLabelStyle();
		string str = (hit ? "" + damageAmount : "miss");
		GUIContent content = new GUIContent(str);
		Vector2 size = style.CalcSize(content);
		Rect r = new Rect(screenPos.x - size.x/2.0f, screenPos.y - size.y/2.0f, size.x, size.y);
		GUI.Label(r, content, style);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (!going) return;
		if (Time.time - timeInitialized > time) {
			going = false;
			Destroy(gameObject);
		}
		else {
			color.a = 1.0f - (Time.time - timeInitialized)/time;
			float yDif = Time.deltaTime * speed;//(Time.time - timeInitialized) * speed;
			position.y += yDif;
		}
	}
}
