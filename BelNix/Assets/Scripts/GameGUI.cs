using UnityEngine;
using System.Collections;

public class GameGUI : MonoBehaviour {

	public MapGenerator mapGenerator;
	GUIStyle playerNormalStyle;
	GUIStyle playerBoldStyle;
	Vector2 position = new Vector2(0.0f, 0.0f);
	Rect scrollRect;
	bool scrollShowing;

	// Use this for initialization
	void Start () {
		position = new Vector2(0.0f, 0.0f);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Vector2 actionButtonsSize() {
		return new Vector2(90.0f, 40.0f);
	}

	public Rect actionRect() {
		float boxHeight = actionButtonsSize().y * 4;
		return new Rect(0.0f, Screen.height - boxHeight, actionButtonsSize().x, boxHeight);
	}

	public Rect moveButtonRect() {
	//	float moveWidth = 90.0f;
	//	float moveHeight = 40.0f;
	//	float moveX = 10.0f;
	//	float moveY = Screen.height - moveHeight - 10.0f;
	//	return new Rect(moveX, moveY, moveWidth, moveHeight);
		return new Rect(0.0f, actionRect().y + actionButtonsSize().y * 0, actionButtonsSize().x, actionButtonsSize().y);
	}

	public Rect attackButtonRect() {
		return new Rect(0.0f, actionRect().y + actionButtonsSize().y * 1, actionButtonsSize().x, actionButtonsSize().y);
/*		Rect r = moveButtonRect();
		if (mapGenerator != null) {
			if (mapGenerator.selectedUnit!=null && !mapGenerator.selectedUnit.moving) {
				if (mapGenerator.lastPlayerPath.Count >1) {
					r.y -= r.height + 10.0f;
				}
			}
		}
		return r;*/
	}

	public Rect minorButtonRect() {	
		return new Rect(0.0f, actionRect().y + actionButtonsSize().y * 2, actionButtonsSize().x, actionButtonsSize().y);
	}

	public Rect waitButtonRect() {
		return new Rect(0.0f, actionRect().y + actionButtonsSize().y * 3, actionButtonsSize().x, actionButtonsSize().y);
	}



	public bool mouseIsOnGUI() {
		Vector2 mousePos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
		if (mapGenerator) {
			if (mapGenerator.selectedUnit != null && mapGenerator.selectedUnit == mapGenerator.getCurrentUnit() && mapGenerator.selectedUnits.Count == 0) {
			//	Player p = mapGenerator.selectedPlayer.GetComponent<Player>();
				/*	if (mapGenerator.lastPlayerPath.Count >1 && !mapGenerator.selectedUnit.moving) {
					if (moveButtonRect().Contains(mousePos)) {
						return true;
					}
				}
				if (mapGenerator.selectedUnit.attackEnemy!=null && !mapGenerator.selectedUnit.moving && !mapGenerator.selectedUnit.attacking) {
					if (attackButtonRect().Contains(mousePos)) {
						return true;
					}
				}*/
				return actionRect().Contains(mousePos);
			}
		}
		return false;
	}

	public bool mouseIsOnScrollView() {
		Vector2 mousePos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
		if (mapGenerator) {
			if (scrollShowing) {
				return scrollRect.Contains(mousePos);
			}
		}
		return false;
	}

	GUIStyle getNormalStyle() {
		if (playerNormalStyle == null) {
			playerNormalStyle = new GUIStyle(GUI.skin.label);
		//	GUIContent cont = new GUIContent("ab
			playerNormalStyle.fontStyle = FontStyle.Normal;
			playerNormalStyle.fontSize = 15;
		}
		return playerNormalStyle;
	}

	GUIStyle getBoldStyle() {
		if (playerBoldStyle == null) {
			playerBoldStyle = new GUIStyle(GUI.skin.label);
			playerBoldStyle.fontStyle = FontStyle.Bold;
			playerBoldStyle.fontSize = 15;
			playerBoldStyle.normal.textColor = Color.green;
		}
		return playerBoldStyle;
	}

	void OnGUI() {
	//	Debug.Log("OnGUI");
		if (mapGenerator == null) return;

		float maxPlayerListWidth = 200.0f;
		float maxPlayerListHeight = 300.0f;
		float actualPlayerListWidth = 0.0f;
		float actualPlayerListHeight = 0.0f;
		float between = 5.0f;
		GUIContent[] contents = new GUIContent[mapGenerator.priorityOrder.Count];
		Vector2[] sizes = new Vector2[mapGenerator.priorityOrder.Count];
		for (int n=0;n<mapGenerator.priorityOrder.Count;n++) {
			//Debug.Log(n + " -- " + mapGenerator.priorityOrder[n].characterName);
			contents[n] = new GUIContent(mapGenerator.priorityOrder[n].characterName);
		}
		for (int n=0;n<mapGenerator.priorityOrder.Count;n++) {
			GUIStyle st = getNormalStyle();
			if (mapGenerator.priorityOrder[n] == mapGenerator.getCurrentUnit()) st = getBoldStyle();
			Vector2 size = st.CalcSize(contents[n]);
			sizes[n] = size;
			actualPlayerListWidth = Mathf.Max(actualPlayerListWidth, size.x + between*2);
			actualPlayerListHeight += size.y + between;
		}
		float boxWidth = Mathf.Min(maxPlayerListWidth, actualPlayerListWidth);
		float boxHeight = Mathf.Min(maxPlayerListHeight, actualPlayerListHeight);
		if (actualPlayerListWidth > maxPlayerListWidth) {
			boxHeight = Mathf.Min(maxPlayerListHeight, boxHeight + 16.0f);
		}
		if (actualPlayerListHeight > maxPlayerListHeight) {
			boxWidth = Mathf.Min(maxPlayerListWidth, boxWidth + 16.0f);
		}
		float scrollWidth = actualPlayerListWidth;
		float scrollHeight = actualPlayerListHeight;
		float boxX = Screen.width - boxWidth - 5.0f;
		GUI.Box (new Rect(boxX, 5.0f, boxWidth, boxHeight), "");
		position = GUI.BeginScrollView(new Rect(boxX, 5.0f, boxWidth, boxHeight), position, new Rect(boxX, 5.0f, scrollWidth, scrollHeight));
		float y = 5.0f;
		scrollRect = new Rect(boxX, 5.0f, boxWidth, boxHeight);
		scrollShowing = actualPlayerListHeight > maxPlayerListHeight;
		for (int n=0; n<mapGenerator.priorityOrder.Count;n++) {
			GUIStyle st = getNormalStyle();
			if (mapGenerator.priorityOrder[n] == mapGenerator.getCurrentUnit()) st = getBoldStyle();
			GUI.Label(new Rect(boxX + between, y, sizes[n].x, sizes[n].y), contents[n], st);
			y += sizes[n].y + between;
		}
		GUI.EndScrollView();
		bool path = false;
		if (mapGenerator.selectedUnit != null && mapGenerator.selectedUnit == mapGenerator.getCurrentUnit() && mapGenerator.selectedUnits.Count == 0) {
			//	Player p = mapGenerator.selectedPlayer.GetComponent<Player>();
			Unit p = mapGenerator.selectedUnit;
//			if (mapGenerator.lastPlayerPath.Count >1 && !p.moving) {
		//		path = true;
			GUI.enabled = !p.usedMovement;
			if(GUI.Button(moveButtonRect(), "Move")) {
				//	Debug.Log("Move Player!");
				if (mapGenerator.lastPlayerPath.Count > 1 && !p.moving) {
					p.moving = true;
//					p.rotating = true;
					p.setRotatingPath();
			//		p.attacking = true;
				}
			}
	//		}
			GUI.enabled = !p.usedStandard;
		//	if (p.attackEnemy!=null && !p.moving && !p.attacking) {
			if (GUI.Button(attackButtonRect(), "Attack")) {
				if (p.attackEnemy!=null && !p.moving && !p.attacking) {
					if (mapGenerator.lastPlayerPath.Count > 1) {
						p.moving = true;
						p.setRotatingPath();
					}
					else {
						p.setRotationToAttackEnemy();
					}
					p.attacking = true;
				}
			}
			GUI.enabled = !p.usedMinor1 || !p.usedMinor2;
			if (GUI.Button(minorButtonRect(), "Minor")) {

			}
			GUI.enabled = true;
			if (GUI.Button(waitButtonRect(), "Wait")) {
				if (!p.moving && !p.attacking)
					mapGenerator.nextPlayer();
			}
		}
	//	Debug.Log("OnGUIEnd");
	}

}
