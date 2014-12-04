﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum Tab {M, C, K, I, T, None}
public enum Mission {Primary, Secondary, Optional, None}
public class GameGUI : MonoBehaviour {

	public MapGenerator mapGenerator;
	public Log log;
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

	public Vector2 selectionUnitScrollPosition = new Vector2(0.0f, 0.0f);

	public bool selectedMovement = false;
	public bool selectedStandard = false;
	public bool selectedMinor = false;
	public MovementType selectedMovementType = MovementType.None;
	public StandardType selectedStandardType = StandardType.None;


	public Tab openTab = Tab.None;
	public Mission openMission = Mission.Primary;

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

	public Rect waitButtonAlwaysRect() {
		return new Rect(Screen.width - actionButtonsSize().x, 0.0f, actionButtonsSize().x, actionButtonsSize().y);
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

	float beginButtonWidth = 150.0f;
	float beginButtonHeight = 50.0f;
	public Rect beginButtonRect() {
		return new Rect((Screen.width - mapGenerator.selectionWidth - beginButtonWidth)/2.0f, Screen.height - beginButtonHeight, beginButtonWidth, beginButtonHeight);
	}

	public bool hasConfirmButton() {
		return ((selectedMovement && (selectedMovementType == MovementType.BackStep || selectedMovementType == MovementType.Move)) && mapGenerator.getCurrentUnit().currentPath.Count > 1) ||
			((selectedStandard && (selectedStandardType == StandardType.Attack || selectedStandardType == StandardType.Throw || selectedStandardType == StandardType.Intimidate)) && mapGenerator.getCurrentUnit().attackEnemy != null);
	}

	public bool mouseIsOnGUI() {
		Vector2 mousePos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
		if (log.mouseIsOnGUI()) return true;
		if (mapGenerator) {
			if (mapGenerator.isInCharacterPlacement()) {
				if (beginButtonRect().Contains(mousePos)) return true;
			}
			if (mapGenerator.selectedUnit != null) {
				bool onPlayer = mapGenerator.selectedUnits.Count == 0 && mapGenerator.selectedUnit.guiContainsMouse(mousePos);
				bool onWait = waitButtonAlwaysRect().Contains(mousePos);
				bool others = onPlayer || onWait;
				if (mapGenerator.selectedUnit == mapGenerator.getCurrentUnit() && mapGenerator.selectedUnits.Count == 0) {
					if (actionRect().Contains(mousePos) || subMenuButtonsRect().Contains(mousePos) || (hasConfirmButton() && confirmButtonRect().Contains(mousePos)) || others) return true;
				}
				else {
					if (rangeRect().Contains(mousePos) || others) return true;
				}
			}
			if (mapGenerator.getCurrentUnit()==null) {
				if (mousePos.x >= Screen.width - 100.0f) return true;
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
			selectedButtonStyle.hover.textColor = Color.white;
			selectedButtonStyle.normal.textColor = Color.white;
			selectedButtonStyle.active.textColor = Color.white;
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
			nonSelectedButtonStyle.active.textColor = nonSelectedButtonStyle.normal.textColor = nonSelectedButtonStyle.hover.textColor = Color.white;
		}
		return nonSelectedButtonStyle;
	}

	GUIStyle beginButtonStyle;
	GUIStyle getBeginButtonStyle() {
		if (beginButtonStyle == null) {
			beginButtonStyle = new GUIStyle("button");
			Texture2D tex = makeTex((int)beginButtonWidth, (int)beginButtonHeight, new Color(30.0f/255.0f, 40.0f/255.0f, 210.0f/255.0f));
			beginButtonStyle.normal.background = beginButtonStyle.hover.background = beginButtonStyle.active.background = tex;
			beginButtonStyle.normal.textColor = beginButtonStyle.hover.textColor = beginButtonStyle.active.textColor = Color.white;
		}
		return beginButtonStyle;
	}

	GUIStyle getSelectedSubMenuTurnStyle() {
		if (selectedSubMenuTurnStyle == null) {
			selectedSubMenuTurnStyle = new GUIStyle(GUI.skin.button);
			Texture2D tex = makeTex((int)subMenuTurnActionSize.x,(int)subMenuTurnActionSize.y, new Color(22.5f/255.0f, 30.0f/255.0f, 152.5f/255.0f));
			selectedSubMenuTurnStyle.normal.background = tex;
			selectedSubMenuTurnStyle.hover.background = tex;
			selectedSubMenuTurnStyle.active.background = tex;
			selectedSubMenuTurnStyle.hover.textColor = Color.white;
			selectedSubMenuTurnStyle.normal.textColor = Color.white;
			selectedSubMenuTurnStyle.active.textColor = Color.white;
		}
		return selectedSubMenuTurnStyle;
	}

	GUIStyle getNonSelectedSubMenuTurnStyle() {
		if (nonSelectedSubMenuTurnStyle == null) {
			nonSelectedSubMenuTurnStyle = new GUIStyle(GUI.skin.button);
			Texture2D tex = makeTex((int)subMenuTurnActionSize.x,(int)subMenuTurnActionSize.y,new Color(30.0f/255.0f, 40.0f/255.0f, 210.0f/255.0f));
			nonSelectedSubMenuTurnStyle.normal.background = tex;
			nonSelectedSubMenuTurnStyle.hover.background = tex;
			nonSelectedSubMenuTurnStyle.active.background = tex;
			nonSelectedSubMenuTurnStyle.active.textColor = nonSelectedSubMenuTurnStyle.normal.textColor = nonSelectedSubMenuTurnStyle.hover.textColor = Color.white;
		}
		return nonSelectedSubMenuTurnStyle;
	}

	GUIStyle namesStyle = null;
	GUIStyle getNamesStyle() {
		if (namesStyle==null) {
			namesStyle = new GUIStyle("Label");
			namesStyle.fontSize = 12;
			namesStyle.normal.textColor = Color.white;
			namesStyle.alignment = TextAnchor.MiddleCenter;
		}
		return namesStyle;
	}

	GUIStyle wonStyle = null;
	GUIStyle lostStyle = null;
	GUIStyle backStyle = null;
	GUIStyle getWonStyle() {
		if (wonStyle == null) {
			wonStyle = new GUIStyle("Label");
			wonStyle.fontSize = 200;
			wonStyle.normal.textColor = Color.green;
			wonStyle.alignment = TextAnchor.MiddleCenter;
		}
		return wonStyle;
	}
	GUIStyle getLostStyle() {
		if (lostStyle == null) {
			lostStyle = new GUIStyle("Label");
			lostStyle.fontSize = 200;
			lostStyle.normal.textColor = Color.red;
			lostStyle.alignment = TextAnchor.MiddleCenter;
		}
		return lostStyle;
	}
	GUIStyle getBackStyle() {
		if (backStyle == null) {
			backStyle = new GUIStyle("Label");
			backStyle.fontSize = 200;
			backStyle.normal.textColor = Color.black;
			backStyle.alignment = TextAnchor.MiddleCenter;
		}
		return backStyle;
	}

	public void clickWait() {
		if (mapGenerator.performingAction() || mapGenerator.currentUnitIsAI() || mapGenerator.isInCharacterPlacement()) return;
		Unit p = mapGenerator.selectedUnit;
		if (selectedMovement) {
			deselectMovement();
		}
		if (selectedStandard) {
			deselectStandard();
		}
		selectedMinor = false;
		mapGenerator.nextPlayer();
	}

	public void clickStandard() {
		if (mapGenerator.performingAction() || mapGenerator.currentUnitIsAI() || mapGenerator.isInCharacterPlacement()) return;
		Unit p = mapGenerator.selectedUnit;
		if (p==null || p.usedStandard || p.isProne()) return;
		if (p.usedStandard) return;
		if (selectedMovement) {
			deselectMovement();
		}
		//	if (selectedStandard == false) {// && selectedStandardType == StandardType.None) {
//		selectedStandardType = StandardType.Attack;	
		selectedStandard = !selectedStandard;//true;
		if (selectedStandard && !p.getStandardTypes().Contains(selectedStandardType)) selectedStandardType = StandardType.None;
		selectStandardType(selectedStandardType);
		//	}
		selectedMinor = false;
		mapGenerator.resetRanges();
	}

	public void clickMovement() {
		if (mapGenerator.performingAction() || mapGenerator.currentUnitIsAI() || mapGenerator.isInCharacterPlacement()) return;
		Unit p = mapGenerator.selectedUnit;
		if (p==null || p.usedMovement) return;
		if (selectedStandard) {
			//		selectedStandardType = StandardType.None;
			deselectStandard();
		}
		/*
		if (selectedMovement == false) {// && selectedMovementType == MovementType.None) {
			selectedMovement = true;
			if (p.getMovementTypes()[0] == MovementType.Move) {
				selectedMovementType = MovementType.Move;
				selectMovementType(selectedMovementType);
			}
			else {
				selectedMovementType = MovementType.None;
			}
		}
		else {
			selectedMovement = false;
			selectedMovementType = MovementType.None;
			selectMovementType(selectedMovementType);
		}*/
		selectedMovement = !selectedMovement;
		if (selectedMovement && !p.getMovementTypes().Contains(selectedMovementType)) selectedMovementType = MovementType.None;
		selectMovementType(selectedMovementType);
		selectedMinor = false;
		mapGenerator.resetRanges();
	}

	public void clickMinor() {
		if (mapGenerator.performingAction() || mapGenerator.currentUnitIsAI() || mapGenerator.isInCharacterPlacement()) return;
		Unit p = mapGenerator.selectedUnit;
		if (p==null || p.minorsLeft==0) return;
		if (selectedMovement) {
			//		selectedMovementType = MovementType.None;
//			selectedMovement = false;
//			mapGenerator.resetRanges();
//			mapGenerator.removePlayerPath();
			deselectMovement();
		}
		if (selectedStandard) {
			//		selectedStandardType = StandardType.None;
			deselectStandard();
		}
		selectedMinor = !selectedMinor;//true;
	}

	public void selectNextOfType() {
		if (selectedStandard) {
			StandardType[] standards = mapGenerator.getCurrentUnit().getStandardTypes();
			int index = System.Array.IndexOf(standards,selectedStandardType);
			index++;
			if (index >= standards.Length-1) index = 0;
			selectStandard(standards[index]);
		}
		else if (selectedMovement) {
			MovementType[] movements = mapGenerator.getCurrentUnit().getMovementTypes();
			int index = System.Array.IndexOf(movements,selectedMovementType);
			index++;
			if (index >= movements.Length-1) index = 0;
			selectMovement(movements[index]);
		}
	}

	public void selectPreviousOfType() {
		if (selectedStandard) {
			StandardType[] standards = mapGenerator.getCurrentUnit().getStandardTypes();
			int index = System.Array.IndexOf(standards,selectedStandardType);
			index--;
			if (index >= standards.Length-2) index = 0;
			if (index < 0) index = standards.Length-2;
			selectStandard(standards[index]);
		}
		else if (selectedMovement) {
			MovementType[] movements = mapGenerator.getCurrentUnit().getMovementTypes();
			int index = System.Array.IndexOf(movements,selectedMovementType);
			index--;
			if (index >= movements.Length-2) index = 0;
			if (index < 0) index = movements.Length-2;
			selectMovement(movements[index]);
		}
	}

	public void selectMovement(MovementType movementType) {
		if (!selectedMovement) {
			clickMovement();
			selectedMovementType = movementType;
		}
		else if (movementType == selectedMovementType) selectedMovementType = MovementType.None;
		else selectedMovementType = movementType;
		selectMovementType(selectedMovementType);
	}

	public void selectMove() {
		selectMovement(MovementType.Move);
		/*
		if (selectedStandard) {
			deselectStandard();
		}
		if (selectedMovement == false) {// && selectedMovementType == MovementType.None) {
			selectedMovement = true;
			if (mapGenerator.getCurrentUnit().getMovementTypes()[0] == MovementType.Move) {
				selectedMovementType = MovementType.Move;
				selectMovementType(selectedMovementType);
			}
			else {
				selectedMovementType = MovementType.None;
			}
		}
		selectedMinor = false;
		mapGenerator.resetRanges();*/
	}

	public void selectStandard(StandardType standardType) {
		if (!selectedStandard) {
			clickStandard();
			selectedStandardType = standardType;
		}
		else if (standardType == selectedStandardType) selectedStandardType = StandardType.None;
		else selectedStandardType = standardType;
		selectStandardType(selectedStandardType);
	}
	//	
	public void selectAttack() {
		selectStandard(StandardType.Attack);
		/*
		if (selectedMovement) {
			selectedMovement = false;
			//	selectedMovementType = MovementType.None;
			mapGenerator.removePlayerPath();
		}
		//	if (selectedStandard == false) {// && selectedStandardType == StandardType.None) {
		selectedStandard = true;
		selectedStandardType = StandardType.Attack;	
		selectStandardType(selectedStandardType);
		//	}
		selectedMinor = false;
		mapGenerator.resetRanges();
		 */
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

		if (mapGenerator.isInCharacterPlacement()) {
			float width = mapGenerator.selectionWidth;
		//	if (Screen.height < mapGenerator.selectionUnits.Count * (mapGenerator.spriteSize + mapGenerator.spriteSeparator) + mapGenerator.spriteSeparator)
		//		width -= 16.0f;
			float scrollHeight = mapGenerator.spriteSeparator + (mapGenerator.spriteSeparator + mapGenerator.spriteSize) * (mapGenerator.selectionUnits == null ? 0 : mapGenerator.selectionUnits.Count + (mapGenerator.selectionCurrentIndex>=0?1:0));
			if (Screen.height < scrollHeight)
				width -= 16.0f;
			selectionUnitScrollPosition = GUI.BeginScrollView(new Rect(Screen.width - mapGenerator.selectionWidth, 0.0f, mapGenerator.selectionWidth, Screen.height), selectionUnitScrollPosition, new Rect(Screen.width - mapGenerator.selectionWidth, 0.0f, mapGenerator.selectionWidth - 16.0f, scrollHeight));
			float y = mapGenerator.spriteSeparator + mapGenerator.spriteSize - 10.0f;
			GUIStyle st = getNamesStyle();
			for (int n=0;n<mapGenerator.selectionUnits.Count;n++) {
				if (n==mapGenerator.selectionCurrentIndex) {
//					Unit u2 = mapGenerator.selectedSelectionObject.GetComponent<Unit>();
					y += mapGenerator.spriteSeparator + mapGenerator.spriteSize;
				}
				Unit u = mapGenerator.selectionUnits[n];
				GUIContent content = new GUIContent(u.characterSheet.personalInfo.getCharacterName().fullName());
				Vector2 size = st.CalcSize(content);
				float height = st.CalcHeight(content, width);
				GUI.Label(new Rect(Screen.width - mapGenerator.selectionWidth, y, width, height + 0 * size.y), content, st);
				y += mapGenerator.spriteSeparator + mapGenerator.spriteSize;
			}

			GUI.EndScrollView();
			if (mapGenerator.selectedSelectionObject) {
				Vector3 pos = Camera.main.WorldToScreenPoint(mapGenerator.selectedSelectionObject.transform.position);
				Unit u = mapGenerator.selectedSelectionObject.GetComponent<Unit>();
				GUIContent content = new GUIContent(u.characterSheet.personalInfo.getCharacterName().fullName());
				float height = st.CalcHeight(content, width);
				GUI.Label(new Rect(pos.x - width/2.0f, Screen.height - (pos.y - mapGenerator.spriteSize/2.0f + 10.0f), width, height), content, st);
				
			}
			if (scrollHeight > Screen.height && mapGenerator.selectedSelectionObject != null) {
				float mY = Screen.height - Input.mousePosition.y;
				float dist = 20.0f;
				float amount = 3.0f;
				if (mY <= dist) {
					amount = (dist - mY)/3.0f;
					selectionUnitScrollPosition.y = Mathf.Max(0.0f, selectionUnitScrollPosition.y - amount);
				}
				if (mY >= Screen.height - dist) {
					amount = (mY - (Screen.height - dist))/3.0f;
					selectionUnitScrollPosition.y = Mathf.Min(scrollHeight - Screen.height, selectionUnitScrollPosition.y + amount);
				}
			}
			if (GUI.Button(beginButtonRect(), "Engage", getBeginButtonStyle())) {
				mapGenerator.enterPriority();
				foreach (Unit u in mapGenerator.priorityOrder) {
					u.setRotationToMostInterestingTile();
				}
			}
		}

		if (mapGenerator.currentUnit >= 0) {
			if (GUI.Button(waitButtonAlwaysRect(), "Wait", getNonSelectedButtonStyle()) && !mapGenerator.performingAction() && !mapGenerator.currentUnitIsAI()) {
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
				if (!mapGenerator.getCurrentUnit().moving && !mapGenerator.getCurrentUnit().attacking)
					mapGenerator.nextPlayer();
			}
		}

		if (mapGenerator.selectedUnit != null && mapGenerator.selectedUnits.Count==0) {
			Unit u = mapGenerator.selectedUnit;
			u.drawGUI();
		}
		bool path = false;
		if (mapGenerator.selectedUnit == null) {
		//	showAttack = false;
		//	showMovement = false;
		}
		else {

			if (mapGenerator.selectedUnit == mapGenerator.getCurrentUnit() && mapGenerator.selectedUnits.Count == 0) {
				//	Player p = mapGenerator.selectedPlayer.GetComponent<Player>();
				Unit p = mapGenerator.selectedUnit;
				//			if (mapGenerator.lastPlayerPath.Count >1 && !p.moving) {
				//		path = true;
				GUI.enabled = !p.usedMovement;
				if (selectedMovement && p.usedMovement) {
					deselectMovement();
			//		selectedMovement = false;
			//		selectedMovementType = MovementType.None;
			//		mapGenerator.resetRanges();
				}
				if(GUI.Button(moveButtonRect(), "Movement", (selectedMovement || p.usedMovement ? getSelectedButtonStyle() : getNonSelectedButtonStyle()))) {
					//	Debug.Log("Move Player!");
					clickMovement();

				}
				//		}
				GUI.enabled = !p.usedStandard && !p.isProne();
				if (selectedStandard && p.usedStandard) {
					deselectStandard();
//					selectedStandard = false;
//					selectedStandardType = StandardType.None;
				}
				//	if (p.attackEnemy!=null && !p.moving && !p.attacking) {
				if (GUI.Button(attackButtonRect(), "Standard", (selectedStandard || p.usedStandard ? getSelectedButtonStyle() : getNonSelectedButtonStyle()))) {
					clickStandard();

				}
				GUI.enabled = p.minorsLeft > 0;//!p.usedMinor1 || !p.usedMinor2;
				if (selectedMinor && p.minorsLeft==0) selectedMinor = false;
				if (GUI.Button(minorButtonRect(), "Minor", (selectedMinor && p.minorsLeft>0 ? getSelectedButtonStyle() : getNonSelectedButtonStyle())) && !mapGenerator.performingAction() && !mapGenerator.currentUnitIsAI()) {
					clickMinor();
				}
				GUI.enabled = true;
				if (GUI.Button(waitButtonRect(), "Wait", getNonSelectedButtonStyle()) && !mapGenerator.performingAction() && !mapGenerator.currentUnitIsAI()) {
					clickWait();
				}

				if (selectedMovement) {
					MovementType[] types = mapGenerator.getCurrentUnit().getMovementTypes();
					for (int n=0;n<types.Length;n++) {
						GUI.enabled = types[n] != MovementType.BackStep || mapGenerator.getCurrentUnit().moveDistLeft == mapGenerator.getCurrentUnit().maxMoveDist;
						if (GUI.Button(subMenuButtonRect(n), types[n].ToString(), (selectedMovementType == types[n] ? getSelectedSubMenuTurnStyle() : getNonSelectedSubMenuTurnStyle())) && !mapGenerator.performingAction() && !mapGenerator.currentUnitIsAI()) {
						//	if (types[n] != MovementType.Cancel) selectedMovementType = types[n];
							selectMovement(types[n]);
						}
					}

					if ((selectedMovementType == MovementType.BackStep || selectedMovementType == MovementType.Move) && mapGenerator.getCurrentUnit().currentPath.Count > 1) {
						if (GUI.Button(confirmButtonRect(), "Confirm", getNonSelectedSubMenuTurnStyle()) && !mapGenerator.performingAction() && !mapGenerator.currentUnitIsAI()) {
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
						if (GUI.Button(subMenuButtonRect(n), types[n].ToString(), (selectedStandardType == types[n] ? getSelectedSubMenuTurnStyle() : getNonSelectedSubMenuTurnStyle())) && !mapGenerator.performingAction() && !mapGenerator.currentUnitIsAI()) {//(selectedMovementType == types[n] ? getSelectedSubMenuTurnStyle() : getNonSelectedSubMenuTurnStyle()))) {
							//	if (types[n] != MovementType.Cancel) selectedMovementType = types[n];
							selectStandard(types[n]);
						}
					}

					if ((selectedStandardType == StandardType.Attack || selectedStandardType == StandardType.Throw || selectedStandardType == StandardType.Intimidate) && mapGenerator.getCurrentUnit().attackEnemy != null) {
						if (GUI.Button(confirmButtonRect(), "Confirm", getNonSelectedSubMenuTurnStyle()) && !mapGenerator.performingAction() && !mapGenerator.currentUnitIsAI()) {
							Debug.Log("Confirm: " + StandardType.Throw);
							if (selectedStandardType == StandardType.Attack) {
								p.startAttacking();
							}
							else if (selectedStandardType == StandardType.Throw) {
								p.startThrowing();
							}
							else if (selectedStandardType == StandardType.Intimidate) {
								p.startIntimidating();
							}
						}
					}
				}

			}
			else {
				/*
				selectedMovement = false;
				selectedStandard = false;
				selectedMinor = false;
				selectedMovementType = MovementType.None;
				selectedStandardType = StandardType.None;*/
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
		if (mapGenerator.gameState != GameState.Playing) {
			GUIContent content = new GUIContent((mapGenerator.gameState==GameState.Won ? "You Won!" : "You Lost!"));
			GUIStyle st = (mapGenerator.gameState==GameState.Won?getWonStyle():getLostStyle());
			int off = 1;
		/*	for (int n=-1;n<=1;n++) {
				for (int m=-1;m<=1;m++) {
					GUI.Label(new Rect(off*n,off*m,Screen.width, Screen.height), content, (n==0 && m==0 ? st : getBackStyle()));
				}
			}*/

			GUI.Label(new Rect(off,0,Screen.width, Screen.height), content, getBackStyle());
			GUI.Label(new Rect(-off,0,Screen.width, Screen.height), content, getBackStyle());
			GUI.Label(new Rect(0,off,Screen.width, Screen.height), content, getBackStyle());
			GUI.Label(new Rect(0,-off,Screen.width, Screen.height), content, getBackStyle());
			GUI.Label(new Rect(0,0,Screen.width, Screen.height), content, st);

		}
	//	Debug.Log("OnGUIEnd");
	}

	void deselectMovement() {
		//		selectedMovementType = MovementType.None;
		selectedMovement = false;
		mapGenerator.resetRanges();
		mapGenerator.removePlayerPath();
	}

	void deselectStandard() {
		selectedStandard = false;
//		selectedStandardType = StandardType.None;
		if (mapGenerator.selectedUnit.attackEnemy) {
			mapGenerator.selectedUnit.attackEnemy.deselect();
			mapGenerator.resetAttack();
		}
		mapGenerator.resetRanges();
	}

	public void selectStandardType(StandardType t) {
		mapGenerator.resetCurrentKeysTile();
		Unit p = mapGenerator.selectedUnit;
		switch (t) {
		case StandardType.Cancel:
			if (mapGenerator.selectedUnit.attackEnemy)
				mapGenerator.selectedUnit.attackEnemy.deselect();
			selectedStandardType = StandardType.None;
			selectedStandard = false;
			mapGenerator.resetRanges();
			break;
		case StandardType.Attack:
			mapGenerator.resetRanges();
			break;
		case StandardType.Throw:
			mapGenerator.resetRanges();
			break;
		case StandardType.Intimidate:
			mapGenerator.resetRanges();
			break;
		default:
			if (mapGenerator.selectedUnit.attackEnemy)
				mapGenerator.selectedUnit.attackEnemy.deselect();
			mapGenerator.resetRanges();
			break;
		}
	}

	void OnStart() {
		selectedStandardType = StandardType.None;
		selectedMovementType = MovementType.None;
	}
	
	public void selectMovementType(MovementType t) {
		mapGenerator.resetCurrentKeysTile();
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
		case MovementType.Recover:
			mapGenerator.getCurrentUnit().recover();
//			mapGenerator.getCurrentUnit().affliction = Affliction.None;
//			mapGenerator.getCurrentUnit().usedMovement = true;
			mapGenerator.resetRanges();
			mapGenerator.removePlayerPath();
			break;
		default:
			mapGenerator.resetRanges();
			mapGenerator.removePlayerPath();
			break;
		}
	}


}
