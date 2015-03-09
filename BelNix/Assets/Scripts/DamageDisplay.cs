using UnityEngine;
using System.Collections;

public class DamageDisplay : MonoBehaviour  {

	public int damageAmount;
	public bool hit;
	public float timeInitialized;
	public Color color;
	public Color backColor = Color.black;
	public bool going = false;
	public Vector3 position;
	Unit damagedUnit;
	float time = 0.3f;
	float fadeTime = 0.15f;
	float speed = 1.5f;
	GUIStyle borderStyle;
	GUIStyle labelStyle;


	public void begin(int damage, bool didHit, bool didCrit, Unit dUnit)  {
		begin(damage, didHit, didCrit, dUnit, Color.red);
	}

	public void begin(int damage, bool didHit, bool didCrit, Unit dUnit, Color c)  {
		damagedUnit = dUnit;
		Vector3 pos = dUnit.transform.position;
		float amountEach = .3f;
		int d = dUnit.damageNumber;
		dUnit.addDamageDisplay();
		int all = (d+1)/2;
		int upTo = d%2;
		pos.x += (upTo == 0 ? -1 : 1) * all * amountEach;
		position = pos;
		damageAmount = damage;
		hit = didHit;
		timeInitialized = Time.time;
		if (didHit && didCrit) color = Color.magenta;
		else if (didHit) color = c;
		else color = Color.gray;
		going = true;
	}

	public GUIStyle getLabelStyle()  {
		if (labelStyle == null)  {
			labelStyle = new GUIStyle("Label");
			labelStyle.fontSize = 40;
		}
		labelStyle.normal.textColor = labelStyle.active.textColor = labelStyle.hover.textColor = color;
		return labelStyle;
	}

	public GUIStyle getBorderStyle()  {
		if (borderStyle == null)  {
			borderStyle = new GUIStyle("Label");
			borderStyle.fontSize = 40;
		}
		borderStyle.normal.textColor = borderStyle.active.textColor = borderStyle.hover.textColor = backColor;
		return borderStyle;
	}

	Rect shiftRect(Rect r, int x, int y)  {
		return new Rect(r.x + x, r.y + y, r.width, r.height);
	}

	void OnGUI()  {
		if (!going) return;
		Vector3 screenPos = Camera.main.WorldToScreenPoint(position);
		screenPos.y = Screen.height - screenPos.y;
	//	screenPos.y -= yDif;
		GUIStyle style = getLabelStyle();
		string str = (hit ? damageAmount + "!" : "miss");
		GUIContent content = new GUIContent(str);
		Vector2 size = style.CalcSize(content);
		Rect r = new Rect(screenPos.x - size.x/2.0f, screenPos.y - size.y/2.0f, size.x, size.y);
		for (int n=-1;n<=1;n++)  {
			for (int m=-1;m<=1;m++)  {
				if (m==0 && n==0 || !(m==0 || n==0)) continue;
				GUI.Label(shiftRect(r, n, m), content, getBorderStyle());
			}
		}
		GUI.Label(r, content, style);
	}

	// Use this for initialization
	void Start ()  {
	
	}
	
	// Update is called once per frame
	void Update ()  {
		if (!going) return;
		if (Time.time - timeInitialized > time + fadeTime)  {
			going = false;
			damagedUnit.removeDamageDisplay();
			damagedUnit = null;
			Destroy(gameObject);
		}
		else  {
			backColor.a = color.a = 1.0f - Mathf.Max((Time.time - timeInitialized - time)/fadeTime, 0);
			float yDif = Time.deltaTime * speed;//(Time.time - timeInitialized) * speed;
			position.y += yDif;
		}
	}
}
