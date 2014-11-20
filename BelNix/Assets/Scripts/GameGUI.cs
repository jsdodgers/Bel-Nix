using UnityEngine;
using System.Collections;

public class GameGUI : MonoBehaviour {

	public MapGenerator mapGenerator;
	GUIStyle playerNormalStyle;
	GUIStyle playerBoldStyle;
	GUIStyle selectedButtonStyle = null;
	GUIStyle nonSelectedButtonStyle = null;
	GUIStyle selectedSubMenuTurnStyle = null;
	GUIStyle nonSelectedSubMenuTurnStyle = null;
	Vector2 position = new Vector2(0.0f, 0.0f);
	Rect scrollRect;
	bool scrollShowing;
	bool first = true;

	public bool showAttack = false;
	public bool showMovement = false;

	Vector2 notTurnMoveRangeSize = new Vector2(150.0f, 50.0f);
	Vector2 subMenuTurnActionSize = new Vector2(100.0f, 35.0f);

	public bool selectedMovement = false;
	public bool selectedStandard = false;
	public bool selectedMinor = false;
	public MovementType selectedMovementType = MovementType.None;
	public StandardType selectedStandardType = StandardType.None;


	// Use this for initialization
	void Start () {
		position = new Vector2(0.0f, 0.0f);
		//selectedButtonStyle = null;
		//nonSelectedButtonStyle = null;
		first = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	Texture2D makeTex( int width, int height, Color col )
	{
		Color[] pix = new Color[width * height];
		for( int i = 0; i < pix.Length; ++i )
		{
		//	Debug.Log("it is: " + (i/width));
			if (i/width == 0 || i/width == height-1) pix[i] = Color.black;
			else if (i%width == 0 || i % width == width-1) pix[i] = Color.black;
			else pix[ i ] = col;
		}
		Texture2D result = new Texture2D( width, height );
		result.SetPixels( pix );
		result.Apply();
		return result;
	}

	public Vector2 actionButtonsSize() {
//		return new Vector2(90.0f, 50.0f);
		return notTurnMoveRangeSize;
//		return new Vector2(90.0f, 40.0f);
	}

	public Rect rangeRect() {
		return new Rect(0.0f, Screen.height - notTurnMoveRangeSize.y*2 + 1, notTurnMoveRangeSize.x, notTurnMoveRangeSize.y*2-1);
	}

	public Rect actionRect() {
		float boxHeight = actionButtonsSize().y * 4 - 3;
		return new Rect(0.0f, Screen.height - boxHeight, actionButtonsSize().x, boxHeight);
	}

	public Rect moveButtonRect() {
		return new Rect(0.0f, actionRect().y + actionButtonsSize().y * 0, actionButtonsSize().x, actionButtonsSize().y);
	}

	public Rect attackButtonRect() {
		return new Rect(0.0f, actionRect().y + actionButtonsSize().y * 1 - 1, actionButtonsSize().x, actionButtonsSize().y);
	}

	public Rect minorButtonRect() {	
		return new Rect(0.0f, actionRect().y + actionButtonsSize().y * 2 - 2, actionButtonsSize().x, actionButtonsSize().y);
	}

	public Rect waitButtonRect() {
		return new Rect(0.0f, actionRect().y + actionButtonsSize().y * 3 - 3, actionButtonsSize().x, actionButtonsSize().y);
	}

	public Rect subMenuButtonsRect() {
//		System.Enum[] values = null;
		int values = 0;
		if (selectedMovement)
			values = mapGenerator.getCurrentUnit().numberMovements();
//			values = mapGenerator.getCurrentUnit().getMovementTypes();
		else if (selectedStandard)
			values = mapGenerator.getCurrentUnit().numberStandards();
//			values = mapGenerator.getCurrentUnit().getStandardTypes();
		if (values == 0) return new Rect(1000000.0f, 1000000.0f, 0.0f, 0.0f);
		float height = subMenuTurnActionSize.y * values - values + 1;
		float y = Mathf.Min(actionRect().y, Screen.height - height);
		float x = actionRect().width - 1;
		float width = subMenuTurnActionSize.x;
		return new Rect(x, y, width, height);
	}

	public Rect subMenuButtonRect(int i) {
		Rect r = subMenuButtonsRect();
		return new Rect(r.x, r.y + i * (subMenuTurnActionSize.y - 1), subMenuTurnActionSize.x, subMenuTurnActionSize.y);
	}

	public Rect confirmButtonRect() {
		Rect r = subMenuButtonsRect();
		return new Rect(r.x + r.width - 1, r.y, subMenuTurnActionSize.x, subMenuTurnActionSize.y);
	}

	public bool hasConfirmButton() {
		return (selectedMovement && (selectedMovementType == MovementType.BackStep || selectedMovementType == MovementType.Move)) ||
			(selectedStandard && (selectedStandardType == StandardType.Attack));
	}

	public bool mouseIsOnGUI() {
		Vector2 mousePos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
		if (mapGenerator) {
			if (mapGenerator.selectedUnit != null) {

				if (mapGenerator.selectedUnit == mapGenerator.getCurrentUnit() && mapGenerator.selectedUnits.Count == 0) {
					return actionRect().Contains(mousePos) || subMenuButtonsRect().Contains(mousePos) || (hasConfirmButton() && confirmButtonRect().Contains(mousePos));
				}
				else {
					return rangeRect().Contains(mousePos);
				}
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

	GUIStyle getSelectedButtonStyle() {
		if (selectedButtonStyle == null) {
			selectedButtonStyle = new GUIStyle(GUI.skin.button);
			Texture2D tex = makeTex((int)notTurnMoveRangeSize.x,(int)notTurnMoveRangeSize.y, new Color(22.5f/255.0f, 30.0f/255.0f, 152.5f/255.0f));
			selectedButtonStyle.normal.background = tex;//makeTex((int)notTurnMoveRangeSize.x,(int)notTurnMoveRangeSize.y,new Color(30.0f, 40.0f, 210.0f));
			selectedButtonStyle.hover.background = tex;//selectedButtonStyle.normal.background;
			selectedButtonStyle.active.background = tex;
			selectedButtonStyle.hover.textColor = Color.black;
			selectedButtonStyle.normal.textColor = Color.black;
			selectedButtonStyle.active.textColor = Color.black;
		}
		return selectedButtonStyle;
	}

	GUIStyle getNonSelectedButtonStyle() {
		if (nonSelectedButtonStyle == null) {
			nonSelectedButtonStyle = new GUIStyle(GUI.skin.button);
			Texture2D tex = makeTex((int)notTurnMoveRangeSize.x,(int)notTurnMoveRangeSize.y,new Color(30.0f/255.0f, 40.0f/255.0f, 210.0f/255.0f));
			nonSelectedButtonStyle.normal.background = tex;//makeTex((int)notTurnMoveRangeSize.x,(int)notTurnMoveRangeSize.y, new Color(15.0f, 20.0f, 105.0f));
			nonSelectedButtonStyle.hover.background = tex;//nonSelectedButtonStyle.normal.background;
			nonSelectedButtonStyle.active.background = tex;//getSelectedButtonStyle().normal.background;
			nonSelectedButtonStyle.active.textColor = nonSelectedButtonStyle.normal.textColor = nonSelectedButtonStyle.hover.textColor = Color.black;
		}
		return nonSelectedButtonStyle;
	}

	GUIStyle getSelectedSubMenuTurnStyle() {
		if (selectedSubMenuTurnStyle == null) {
			selectedSubMenuTurnStyle = new GUIStyle(GUI.skin.button);
			Texture2D tex = makeTex((int)subMenuTurnActionSize.x,(int)subMenuTurnActionSize.y, new Color(22.5f/255.0f, 30.0f/255.0f, 152.5f/255.0f));
			selectedSubMenuTurnStyle.normal.background = tex;//makeTex((int)notTurnMoveRangeSize.x,(int)notTurnMoveRangeSize.y,new Color(30.0f, 40.0f, 210.0f));
			selectedSubMenuTurnStyle.hover.background = tex;//selectedButtonStyle.normal.background;
			selectedSubMenuTurnStyle.active.background = tex;
			selectedSubMenuTurnStyle.hover.textColor = Color.black;
			selectedSubMenuTurnStyle.normal.textColor = Color.black;
			selectedSubMenuTurnStyle.active.textColor = Color.black;
		}
		return selectedSubMenuTurnStyle;
	}

	GUIStyle getNonSelectedSubMenuTurnStyle() {
		if (nonSelectedSubMenuTurnStyle == null) {
			nonSelectedSubMenuTurnStyle = new GUIStyle(GUI.skin.button);
			Texture2D tex = makeTex((int)subMenuTurnActionSize.x,(int)subMenuTurnActionSize.y,new Color(30.0f/255.0f, 40.0f/255.0f, 210.0f/255.0f));
			nonSelectedSubMenuTurnStyle.normal.background = tex;//makeTex((int)notTurnMoveRangeSize.x,(int)notTurnMoveRangeSize.y, new Color(15.0f, 20.0f, 105.0f));
			nonSelectedSubMenuTurnStyle.hover.background = tex;//nonSelectedButtonStyle.normal.background;
			nonSelectedSubMenuTurnStyle.active.background = tex;//getSelectedButtonStyle().normal.background;
			nonSelectedSubMenuTurnStyle.active.textColor = nonSelectedSubMenuTurnStyle.normal.textColor = nonSelectedSubMenuTurnStyle.hover.textColor = Color.black;
		}
		return nonSelectedSubMenuTurnStyle;
	}


	void OnGUI() {
	//	Debug.Log("OnGUI");
		if (first) {
			first = false;
			getSelectedButtonStyle();
			getSelectedSubMenuTurnStyle();
			getNonSelectedButtonStyle();
			getNonSelectedSubMenuTurnStyle();
		}
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
		if (mapGenerator.selectedUnit == null) {
			showAttack = false;
			showMovement = false;
		}
		else {

			if (mapGenerator.selectedUnit == mapGenerator.getCurrentUnit() && mapGenerator.selectedUnits.Count == 0) {
				//	Player p = mapGenerator.selectedPlayer.GetComponent<Player>();
				Unit p = mapGenerator.selectedUnit;
				//			if (mapGenerator.lastPlayerPath.Count >1 && !p.moving) {
				//		path = true;
				GUI.enabled = !p.usedMovement;
				if (selectedMovement && p.usedMovement) {
					selectedMovement = false;
					selectedMovementType = MovementType.None;
					mapGenerator.resetRanges();
				}
				if(GUI.Button(moveButtonRect(), "Movement", (selectedMovement || p.usedMovement ? getSelectedButtonStyle() : getNonSelectedButtonStyle()))) {
					//	Debug.Log("Move Player!");
					if (selectedStandard) {
						//		selectedStandardType = StandardType.None;
						deselectStandard();
					}
					if (selectedMovement == false) {// && selectedMovementType == MovementType.None) {
						selectedMovementType = MovementType.Move;
						selectMovementType(selectedMovementType);
					}
					selectedMovement = !selectedMovement;//true;
					selectedMinor = false;
					mapGenerator.resetRanges();

				}
				//		}
				GUI.enabled = !p.usedStandard;
				if (selectedStandard && p.usedStandard) {
					selectedStandard = false;
					selectedStandardType = StandardType.None;
				}
				//	if (p.attackEnemy!=null && !p.moving && !p.attacking) {
				if (GUI.Button(attackButtonRect(), "Standard", (selectedStandard || p.usedStandard ? getSelectedButtonStyle() : getNonSelectedButtonStyle()))) {
					if (selectedMovement) {
						selectedMovement = false;
					//	selectedMovementType = MovementType.None;
						mapGenerator.removePlayerPath();
					}
				//	if (selectedStandard == false) {// && selectedStandardType == StandardType.None) {
						selectedStandardType = StandardType.Attack;	
						selectStandardType(selectedStandardType);
				//	}
					selectedStandard = !selectedStandard;//true;
					selectedMinor = false;
					mapGenerator.resetRanges();

				}
				GUI.enabled = !p.usedMinor1 || !p.usedMinor2;
				if (selectedMinor && (p.usedMinor1 && p.usedMinor2)) selectedMinor = false;
				if (GUI.Button(minorButtonRect(), "Minor", (selectedMinor && !(p.usedMinor1 && p.usedMinor2) ? getSelectedButtonStyle() : getNonSelectedButtonStyle()))) {
					if (selectedMovement) {
				//		selectedMovementType = MovementType.None;
						selectedMovement = false;
						mapGenerator.resetRanges();
						mapGenerator.removePlayerPath();
					}
					if (selectedStandard) {
				//		selectedStandardType = StandardType.None;
						deselectStandard();
					}
					selectedMinor = !selectedMinor;//true;
				}
				GUI.enabled = true;
				if (GUI.Button(waitButtonRect(), "Wait", getNonSelectedButtonStyle())) {
					if (selectedMovement) {
				//		selectedMovementType = MovementType.None;
						selectedMovement = false;
						mapGenerator.resetRanges();
						mapGenerator.removePlayerPath();
					}
					if (selectedStandard) {
				//		selectedStandardType = StandardType.None;
						deselectStandard();
					}
					selectedMinor = false;
					if (!p.moving && !p.attacking)
						mapGenerator.nextPlayer();
				}

				if (selectedMovement) {
					MovementType[] types = mapGenerator.getCurrentUnit().getMovementTypes();
					for (int n=0;n<types.Length;n++) {
						GUI.enabled = types[n] != MovementType.BackStep || mapGenerator.getCurrentUnit().moveDistLeft == mapGenerator.getCurrentUnit().maxMoveDist;
						if (GUI.Button(subMenuButtonRect(n), types[n].ToString(), (selectedMovementType == types[n] ? getSelectedSubMenuTurnStyle() : getNonSelectedSubMenuTurnStyle()))) {
						//	if (types[n] != MovementType.Cancel) selectedMovementType = types[n];
							if (types[n] == selectedMovementType) selectedMovementType = MovementType.None;
							else selectedMovementType = types[n];
							selectMovementType(selectedMovementType);
						}
					}

					if (selectedMovementType == MovementType.BackStep || selectedMovementType == MovementType.Move) {
						if (GUI.Button(confirmButtonRect(), "Confirm", getNonSelectedSubMenuTurnStyle())) {
							if (mapGenerator.lastPlayerPath.Count > 1 && !p.moving) {
								p.startMoving(selectedMovementType == MovementType.BackStep);
								//		p.attacking = true;
							}
						}
					}
				}
				else if (selectedStandard) {
					StandardType[] types = mapGenerator.getCurrentUnit().getStandardTypes();
					for (int n=0;n<types.Length;n++) {
						GUI.enabled = true;//types[n] != MovementType.BackStep || mapGenerator.getCurrentUnit().moveDistLeft == mapGenerator.getCurrentUnit().maxMoveDist;
						if (GUI.Button(subMenuButtonRect(n), types[n].ToString(), (selectedStandardType == types[n] ? getSelectedSubMenuTurnStyle() : getNonSelectedSubMenuTurnStyle()))) {//(selectedMovementType == types[n] ? getSelectedSubMenuTurnStyle() : getNonSelectedSubMenuTurnStyle()))) {
							//	if (types[n] != MovementType.Cancel) selectedMovementType = types[n];
							if (types[n] == selectedStandardType) selectedStandardType = StandardType.None;
							else selectedStandardType = types[n];
							selectStandardType(selectedStandardType);
						}
					}

					if (selectedStandardType == StandardType.Attack) {
						if (GUI.Button(confirmButtonRect(), "Confirm", getNonSelectedSubMenuTurnStyle())) {
						
							
							if (p.attackEnemy!=null && !p.moving && !p.attacking) {
							/*
								if (mapGenerator.lastPlayerPath.Count > 1) {
									p.moving = true;
									p.removeTrail();
									p.setRotatingPath();
								}
								else {
									p.setRotationToAttackEnemy();
								}*/

								p.attacking = true;
							}
						}
					}
				}

			}
			else {
				
				selectedMovement = false;
				selectedStandard = false;
				selectedMinor = false;
				selectedMovementType = MovementType.None;
				selectedStandardType = StandardType.None;
//				GUI.SelectionGrid(new Rect(0, 0, 100, 100), -1, new string[]{"Movement"}, 1);
//				GUI.SelectionGrid(new Rect(0, 100, 100, 100), 0, new string[]{"Attack"}, 1);
				if (GUI.Button(new Rect(0.0f, Screen.height - notTurnMoveRangeSize.y, notTurnMoveRangeSize.x,notTurnMoveRangeSize.y), "Show Movement", (showMovement ? getSelectedButtonStyle() : getNonSelectedButtonStyle()))) {
					showMovement = !showMovement;
					mapGenerator.resetRanges();
				}
				if (GUI.Button(new Rect(0.0f, Screen.height - notTurnMoveRangeSize.y*2 + 1, notTurnMoveRangeSize.x,notTurnMoveRangeSize.y), "Show Attack Range", (showAttack ? getSelectedButtonStyle() : getNonSelectedButtonStyle()))) {
					showAttack = !showAttack;
					mapGenerator.resetRanges();
				}
			}
		}
	//	Debug.Log("OnGUIEnd");
	}

	void deselectStandard() {
		
		selectedStandard = false;
		if (mapGenerator.selectedUnit.attackEnemy) {
			mapGenerator.selectedUnit.attackEnemy.deselect();
		}
		mapGenerator.resetRanges();
	}

	public void selectStandardType(StandardType t) {
		Unit p = mapGenerator.selectedUnit;
		switch (t) {
		case StandardType.Cancel:
			selectedStandardType = StandardType.None;
			selectedStandard = false;
			mapGenerator.resetRanges();
			break;
		case StandardType.Attack:
			mapGenerator.resetRanges();
			break;
		default:
			mapGenerator.resetRanges();
			break;
		}
	}
	
	public void selectMovementType(MovementType t) {
		switch (t) {
		case MovementType.Cancel:
			selectedMovementType = MovementType.None;
			selectedMovement = false;
			mapGenerator.resetRanges();
			mapGenerator.removePlayerPath();
			break;
		case MovementType.BackStep:
		case MovementType.Move:

			mapGenerator.getCurrentUnit().selectMovementType(t);
			if (t == MovementType.BackStep)
				Debug.Log("BackStep: " + mapGenerator.lastPlayerPath.Count + "\n\n" + mapGenerator.selectedUnit.currentPath.Count);
			break;
		default:
			mapGenerator.resetRanges();
			mapGenerator.removePlayerPath();
			break;
		}
	}


}
