using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum Tab {M, C, K, I, T, Cancel, None}
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
	Vector2 turretSelectSize = new Vector2(250.0f, 100.0f);

	public Vector2 selectionUnitScrollPosition = new Vector2(0.0f, 0.0f);
	public Vector2 turretsScrollPosition = new Vector2(0.0f, 0.0f);
	public Vector2 trapsScrollPosition = new Vector2(0.0f, 0.0f);

	public bool selectedMovement = false;
	public bool selectedStandard = false;
	public bool selectedMinor = false;
	public bool turretDirection = false;
	public MovementType selectedMovementType = MovementType.None;
	public StandardType selectedStandardType = StandardType.None;
	public MinorType selectedMinorType = MinorType.None;


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

	public Rect turretTypeRect(int n) {
		return new Rect(Screen.width - turretSelectSize.x, turretTypesRect().y + turretSelectSize.y*n - n, turretSelectSize.x, turretSelectSize.y);
	}

	public Rect trapTypeRect(int n) {
		return new Rect(trapTypesRect().x, trapTypesRect().y + turretSelectSize.y*n - n, turretSelectSize.x, turretSelectSize.y);
	}

	public Rect turretTypesRect() {
		float height = turretSelectSize.y * 3 - 2;
		return new Rect(Screen.width - turretSelectSize.x, (Screen.height - height)/2.0f, turretSelectSize.x, height);
	}

	public Vector2 trapOkButtons() {
		float x = turretSelectSize.x/2.0f-30.0f/2.0f;
		return new Vector2(x, x/2.0f);
	}

	public Vector2 trapOkButtonsSize() {
		float height = trapOkButtons().y + 20.0f;
		float width = turretSelectSize.x;
		return new Vector2(width, height);
	}

	public Rect trapOkButton(int n) {
		float x = trapTypesRect().x + 10.0f;
		float y = trapTypesRect().y + trapTypesScrollRect().height + 10.0f;
		return new Rect(x + n * (trapOkButtons().x + 10.0f), y, trapOkButtons().x, trapOkButtons().y);
	}

	public Rect trapTypesScrollRect() {
		float height = turretSelectSize.y * 3 - 2;
		float width = turretSelectSize.x;
		float x = (Screen.width - width)/2.0f;
		float y = (Screen.height - height)/2.0f;
		return new Rect(x, y, width, height);
	}

	public Rect trapTypesRect() {
		Rect r = trapTypesScrollRect();
		r.height += trapOkButtonsSize().y;
		return r;
		float height = turretSelectSize.y * 3 - 2 + trapOkButtonsSize().y;
		float width = turretSelectSize.x;
		float x = (Screen.width - width)/2.0f;
		float y = (Screen.height - height)/2.0f;
		return new Rect(x, y, width, height);
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
		else if (selectedMinor)
			values = mapGenerator.getCurrentUnit().numberMinors();
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
			((selectedStandard && (selectedStandardType == StandardType.Attack || selectedStandardType == StandardType.Throw || selectedStandardType == StandardType.Intimidate)) && mapGenerator.getCurrentUnit().attackEnemy != null) ||
				((selectedStandard && (selectedStandardType == StandardType.Place_Turret)) && mapGenerator.turretBeingPlaced != null) ||
				((selectedStandard && (selectedStandardType == StandardType.Lay_Trap)) && mapGenerator.currentTrap.Count>0);
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
					if (selectedStandard && selectedStandardType==StandardType.Place_Turret)
						if (turretTypesRect().Contains(mousePos)) return true;
					if (selectedStandard && selectedStandardType==StandardType.Lay_Trap && selectedTrap == null)
						if (trapTypesRect().Contains(mousePos)) return true;
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

	static GUIStyle selectedButtonTurretStyle;
	GUIStyle getSelectedButtonTurretStyle() {
		if (selectedButtonTurretStyle == null) {
			selectedButtonTurretStyle = new GUIStyle(GUI.skin.button);
			Texture2D tex = makeTex((int)notTurnMoveRangeSize.x,(int)notTurnMoveRangeSize.y, new Color(22.5f/255.0f, 30.0f/255.0f, 152.5f/255.0f));
			selectedButtonTurretStyle.normal.background = tex;//makeTex((int)notTurnMoveRangeSize.x,(int)notTurnMoveRangeSize.y,new Color(30.0f, 40.0f, 210.0f));
			selectedButtonTurretStyle.hover.background = tex;//selectedButtonStyle.normal.background;
			selectedButtonTurretStyle.active.background = tex;
			selectedButtonTurretStyle.hover.textColor = Color.white;
			selectedButtonTurretStyle.normal.textColor = Color.white;
			selectedButtonTurretStyle.active.textColor = Color.white;
		}
		return selectedButtonTurretStyle;
	}

	static GUIStyle nonSelectedButtonTurretStyle;
	GUIStyle getNonSelectedButtonTurretStyle() {
		if (nonSelectedButtonTurretStyle == null) {
			nonSelectedButtonTurretStyle = new GUIStyle(GUI.skin.button);
			Texture2D tex = makeTex((int)notTurnMoveRangeSize.x,(int)notTurnMoveRangeSize.y,new Color(30.0f/255.0f, 40.0f/255.0f, 210.0f/255.0f));
			nonSelectedButtonTurretStyle.normal.background = tex;//makeTex((int)notTurnMoveRangeSize.x,(int)notTurnMoveRangeSize.y, new Color(15.0f, 20.0f, 105.0f));
			nonSelectedButtonTurretStyle.hover.background = tex;//nonSelectedButtonStyle.normal.background;
			nonSelectedButtonTurretStyle.active.background = tex;//getSelectedButtonStyle().normal.background;
			nonSelectedButtonTurretStyle.active.textColor = nonSelectedButtonTurretStyle.normal.textColor = nonSelectedButtonTurretStyle.hover.textColor = Color.white;
		}
		return nonSelectedButtonTurretStyle;
	}

	static Texture2D turretBackgroundTexture;
	Texture2D getTurretBackgroundTexture() {
		if (turretBackgroundTexture == null) {
			Rect r = turretTypesRect();
			turretBackgroundTexture = makeTex((int)r.width,(int)r.height,new Color(30.0f/255.0f, 40.0f/255.0f, 210.0f/255.0f));
		}
		return turretBackgroundTexture;
	}

	static Texture2D trapBackgroundTexture;
	Texture2D getTrapBackgroundTexture() {
		if (trapBackgroundTexture==null) {
			Rect r = trapTypesRect();
			trapBackgroundTexture = makeTex((int)r.width,(int)r.height,new Color(30.0f/255.0f, 40.0f/255.0f, 210.0f/255.0f));
		}
		return trapBackgroundTexture;
	}

	static GUIStyle trapSelectButtonsStyle;
	GUIStyle getTrapSelectButtonsStyle() {
		if (trapSelectButtonsStyle == null) {
			trapSelectButtonsStyle = new GUIStyle(GUI.skin.button);
			Texture2D tex = makeTex((int)trapOkButtons().x,(int)trapOkButtons().y, new Color(22.5f/255.0f, 30.0f/255.0f, 152.5f/255.0f));
			trapSelectButtonsStyle.normal.background = tex;//makeTex((int)notTurnMoveRangeSize.x,(int)notTurnMoveRangeSize.y,new Color(30.0f, 40.0f, 210.0f));
			trapSelectButtonsStyle.hover.background = tex;//selectedButtonStyle.normal.background;
			trapSelectButtonsStyle.active.background = tex;
			trapSelectButtonsStyle.hover.textColor = Color.white;
			trapSelectButtonsStyle.normal.textColor = Color.white;
			trapSelectButtonsStyle.active.textColor = Color.white;
		}
		return trapSelectButtonsStyle;
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
		if (selectedMinor) {
			deselectMinor();
		}
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
		if (selectedMinor) {
			deselectMinor();
		}
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
		if (selectedMinor) {
			deselectMinor();
		}
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
		if (selectedMinor && !p.getMinorTypes().Contains(selectedMinorType)) selectedMinorType = MinorType.None;
		selectMinorType((selectedMinor ? selectedMinorType : MinorType.None));

	}
	

	public void selectTypeAt(int index) {
		if (selectedStandard) {
			StandardType[] standards = mapGenerator.getCurrentUnit().getStandardTypes();
			if (index >= standards.Length-1) return;
			selectStandard(standards[index]);
		}
		else if (selectedMovement) {
			MovementType[] movements = mapGenerator.getCurrentUnit().getMovementTypes();
			if (index >= movements.Length-1) return;
			selectMovement(movements[index]);
		}
		else if (selectedMinor) {
			MinorType[] minors = mapGenerator.getCurrentUnit().getMinorTypes();
			if (index >= minors.Length-1) return;
			selectMinor(minors[index]);
		}
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
		else if (selectedMinor) {
			MinorType[] minors = mapGenerator.getCurrentUnit().getMinorTypes();
			int index = System.Array.IndexOf(minors,selectedMinorType);
			index++;
			if (index >= minors.Length-1) index = 0;
			selectMinor(minors[index]);
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
		else if (selectedMinor) {
			MinorType[] minors = mapGenerator.getCurrentUnit().getMinorTypes();
			int index = System.Array.IndexOf(minors,selectedMinorType);
			index--;
			if (index >= minors.Length-2) index = 0;
			if (index < 0) index = minors.Length-2;
			selectMinor(minors[index]);
		}
	}

	public void selectMinor(MinorType minorType) {
		if (!selectedMinor) {
			clickMinor();
			selectedMinorType = minorType;
		}
		else if (minorType == selectedMinorType) selectedMinorType = MinorType.None;
		else selectedMinorType = minorType;
		selectMinorType(selectedMinorType);
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
				if (mapGenerator.playerTransform.childCount!=0) {
					mapGenerator.enterPriority();
					foreach (Unit u in mapGenerator.priorityOrder) {
						u.setRotationToMostInterestingTile();
					}
				}
			}
		}

		if (mapGenerator.currentUnit >= 0) {
			if (GUI.Button(waitButtonAlwaysRect(), "End Turn", getNonSelectedButtonStyle()) && !mapGenerator.performingAction() && !mapGenerator.currentUnitIsAI()) {
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
				if (selectedMinor) {
					deselectMinor();
				}
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
				if (selectedMinor && p.minorsLeft==0) {
					if (selectedMinorType == MinorType.Loot) previouslyOpenTab = Tab.I;
					deselectMinor();//selectedMinor = false;
				}
				if (GUI.Button(minorButtonRect(), "Minor", (selectedMinor && p.minorsLeft>0 ? getSelectedButtonStyle() : getNonSelectedButtonStyle())) && !mapGenerator.performingAction() && !mapGenerator.currentUnitIsAI()) {
					clickMinor();
				}
				GUI.enabled = true;
				if (GUI.Button(waitButtonRect(), "End Turn", getNonSelectedButtonStyle()) && !mapGenerator.performingAction() && !mapGenerator.currentUnitIsAI()) {
					clickWait();
				}

				if (selectedMovement) {
					MovementType[] types = mapGenerator.getCurrentUnit().getMovementTypes();
					for (int n=0;n<types.Length;n++) {
						GUI.enabled = types[n] != MovementType.BackStep || mapGenerator.getCurrentUnit().moveDistLeft == mapGenerator.getCurrentUnit().maxMoveDist;
						if (GUI.Button(subMenuButtonRect(n), Unit.getNameOfMovementType(types[n]), (selectedMovementType == types[n] ? getSelectedSubMenuTurnStyle() : getNonSelectedSubMenuTurnStyle())) && !mapGenerator.performingAction() && !mapGenerator.currentUnitIsAI()) {
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
						if (GUI.Button(subMenuButtonRect(n), Unit.getNameOfStandardType(types[n]), (selectedStandardType == types[n] ? getSelectedSubMenuTurnStyle() : getNonSelectedSubMenuTurnStyle())) && !mapGenerator.performingAction() && !mapGenerator.currentUnitIsAI()) {//(selectedMovementType == types[n] ? getSelectedSubMenuTurnStyle() : getNonSelectedSubMenuTurnStyle()))) {
							//	if (types[n] != MovementType.Cancel) selectedMovementType = types[n];
							selectStandard(types[n]);
						}
					}

					if (((selectedStandardType == StandardType.Attack || selectedStandardType == StandardType.Throw || selectedStandardType == StandardType.Intimidate) && mapGenerator.getCurrentUnit().attackEnemy != null) || (selectedStandardType==StandardType.Place_Turret && mapGenerator.turretBeingPlaced != null) || (selectedStandardType==StandardType.Lay_Trap && mapGenerator.currentTrap.Count>0)) {
						if (GUI.Button(confirmButtonRect(), "Confirm", getNonSelectedSubMenuTurnStyle()) && !mapGenerator.performingAction() && !mapGenerator.currentUnitIsAI()) {
							Debug.Log("Confirm: " + StandardType.Throw);
							mapGenerator.performAction();
/*							if (selectedStandardType == StandardType.Attack) {
								p.startAttacking();
							}
							else if (selectedStandardType == StandardType.Throw) {
								p.startThrowing();
							}
							else if (selectedStandardType == StandardType.Intimidate) {
								p.startIntimidating();
							}*/
						}
					}
				}
				else if (selectedMinor) {
					MinorType[] types = mapGenerator.getCurrentUnit().getMinorTypes();
					for (int n=0;n<types.Length;n++) {
						GUI.enabled = true;//types[n] != MovementType.BackStep || mapGenerator.getCurrentUnit().moveDistLeft == mapGenerator.getCurrentUnit().maxMoveDist;
						if (GUI.Button(subMenuButtonRect(n), Unit.getNameOfMinorType(types[n]), (selectedMinorType == types[n] ? getSelectedSubMenuTurnStyle() : getNonSelectedSubMenuTurnStyle())) && !mapGenerator.performingAction() && !mapGenerator.currentUnitIsAI()) {//(selectedMovementType == types[n] ? getSelectedSubMenuTurnStyle() : getNonSelectedSubMenuTurnStyle()))) {
							//	if (types[n] != MovementType.Cancel) selectedMovementType = types[n];
							selectMinor(types[n]);
						}
					}
					/*
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
					}*/
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
			if (selectedStandard && selectedStandardType==StandardType.Place_Turret) {
				List<Turret> turrets = mapGenerator.getCurrentUnit().characterSheet.characterSheet.inventory.getTurrets();
				float height = turrets.Count * turretSelectSize.y - turrets.Count + 1;
				//	height *= 4;
				GUI.DrawTexture(turretTypesRect(), getTurretBackgroundTexture());
				Rect rr = turretTypesRect();
				rr.y += 1;
				rr.height -= 2;
				turretsScrollPosition = GUI.BeginScrollView(turretTypesRect(), turretsScrollPosition, new Rect(turretTypesRect().x, turretTypesRect().y, turretSelectSize.x - (turrets.Count > 3 ? 16 : 0), height));
				for (int n=0; n<turrets.Count;n++) {
					Turret turret = turrets[n];
					Rect r = turretTypeRect(n);
					if (GUI.Button(r, "", (selectedTurretIndex==n ? getSelectedButtonTurretStyle() : getNonSelectedButtonTurretStyle()))) {
						selectedTurretIndex = n;
					}
					float x = 5.0f + r.x;
					Vector2 size = turrets[n].getSize();
					size.x *= Unit.inventoryCellSize;
					size.y *= Unit.inventoryCellSize;
					GUI.DrawTexture(new Rect(x, r.y + (r.height - size.y)/2.0f, size.x, size.y), turret.inventoryTexture);
					x += size.x + 5.0f;
					GUIContent frameContent = new GUIContent("Frame: " + turret.frame.itemName);
					GUIContent energySourceContent = new GUIContent("Energy Source: " + turret.energySource.itemName);
					GUIContent gearContent = new GUIContent("Gear: " + turret.gear.itemName);
					GUIContent applicatorContent = new GUIContent("Applicator: " + turret.applicator.itemName);
					GUIStyle st = getTurretPartStyle();
					Vector2 frameSize = st.CalcSize(frameContent);
					Vector2 energySourceSize = st.CalcSize(energySourceContent);
					Vector2 gearSize = st.CalcSize(gearContent);
					Vector2 applicatorSize = st.CalcSize(applicatorContent);
					float y = r.y + (r.height - frameSize.y - energySourceSize.y - gearSize.y - applicatorSize.y)/2.0f;
					GUI.Label(new Rect(x, y, frameSize.x, frameSize.y), frameContent, st);
					y+=frameSize.y;
					GUI.Label(new Rect(x, y, energySourceSize.x, energySourceSize.y), energySourceContent, st);
					y+=energySourceSize.y;
					GUI.Label(new Rect(x, y, gearSize.x, gearSize.y), gearContent, st);
					y+=gearSize.y;
					GUI.Label(new Rect(x, y, applicatorSize.x, applicatorSize.y), applicatorContent, st);
					y+=applicatorSize.y;
					//					size.x *= 
				}
				GUI.EndScrollView();
			}
			
			if (selectedStandard && selectedStandardType==StandardType.Place_Turret) {
				List<Turret> turrets = mapGenerator.getCurrentUnit().characterSheet.characterSheet.inventory.getTurrets();
				float height = turrets.Count * turretSelectSize.y - turrets.Count + 1;
			//	height *= 4;
				GUI.DrawTexture(turretTypesRect(), getTurretBackgroundTexture());
				Rect rr = turretTypesRect();
				rr.y += 1;
				rr.height -= 2;
				turretsScrollPosition = GUI.BeginScrollView(turretTypesRect(), turretsScrollPosition, new Rect(turretTypesRect().x, turretTypesRect().y, turretSelectSize.x - (turrets.Count > 3 ? 16 : 0), height));
				for (int n=0; n<turrets.Count;n++) {
					Turret turret = turrets[n];
					Rect r = turretTypeRect(n);
					if (GUI.Button(r, "", (selectedTurretIndex==n ? getSelectedButtonTurretStyle() : getNonSelectedButtonTurretStyle()))) {
						selectedTurretIndex = n;
					}
					float x = 5.0f + r.x;
					Vector2 size = turrets[n].getSize();
					size.x *= Unit.inventoryCellSize;
					size.y *= Unit.inventoryCellSize;
					GUI.DrawTexture(new Rect(x, r.y + (r.height - size.y)/2.0f, size.x, size.y), turret.inventoryTexture);
					x += size.x + 5.0f;
					GUIContent frameContent = new GUIContent("Frame: " + turret.frame.itemName);
					GUIContent energySourceContent = new GUIContent("Energy Source: " + turret.energySource.itemName);
					GUIContent gearContent = new GUIContent("Gear: " + turret.gear.itemName);
					GUIContent applicatorContent = new GUIContent("Applicator: " + turret.applicator.itemName);
					GUIStyle st = getTurretPartStyle();
					Vector2 frameSize = st.CalcSize(frameContent);
					Vector2 energySourceSize = st.CalcSize(energySourceContent);
					Vector2 gearSize = st.CalcSize(gearContent);
					Vector2 applicatorSize = st.CalcSize(applicatorContent);
					float y = r.y + (r.height - frameSize.y - energySourceSize.y - gearSize.y - applicatorSize.y)/2.0f;
					GUI.Label(new Rect(x, y, frameSize.x, frameSize.y), frameContent, st);
					y+=frameSize.y;
					GUI.Label(new Rect(x, y, energySourceSize.x, energySourceSize.y), energySourceContent, st);
					y+=energySourceSize.y;
					GUI.Label(new Rect(x, y, gearSize.x, gearSize.y), gearContent, st);
					y+=gearSize.y;
					GUI.Label(new Rect(x, y, applicatorSize.x, applicatorSize.y), applicatorContent, st);
					y+=applicatorSize.y;
					//					size.x *= 
				}
				GUI.EndScrollView();
			}
			
			if (selectedStandard && selectedStandardType==StandardType.Lay_Trap && selectedTrap==null) {
				List<Trap> traps = mapGenerator.getCurrentUnit().characterSheet.characterSheet.inventory.getTraps();
				float height = traps.Count * turretSelectSize.y - traps.Count + 2;
				//	height *= 4;
				GUI.DrawTexture(trapTypesRect(), getTurretBackgroundTexture());
				Rect rr = trapTypesScrollRect();
				rr.y += 1;
				rr.height -= 2;
				trapsScrollPosition = GUI.BeginScrollView(trapTypesScrollRect(), trapsScrollPosition, new Rect(trapTypesScrollRect().x, trapTypesScrollRect().y, turretSelectSize.x - (traps.Count > 3 ? 16 : 0), height));
				for (int n=0; n<traps.Count;n++) {
					Trap trap = traps[n];
					Rect r = trapTypeRect(n);
					if (GUI.Button(r, "", (selectedTrapIndex==n ? getSelectedButtonTurretStyle() : getNonSelectedButtonTurretStyle()))) {
						selectedTrapIndex = n;
					}
					float x = 5.0f + r.x;
					Vector2 size = trap.getSize();
					size.x *= Unit.inventoryCellSize;
					size.y *= Unit.inventoryCellSize;
					GUI.DrawTexture(new Rect(x, r.y + (r.height - size.y)/2.0f, size.x, size.y), trap.inventoryTexture);
					x += size.x + 5.0f;
					GUIContent frameContent = new GUIContent("Frame: " + trap.frame.itemName);
				//	GUIContent energySourceContent = new GUIContent("Energy Source: " + trap.energySource.itemName);
					GUIContent triggerContent = new GUIContent("Trigger: " + trap.trigger.itemName);
					GUIContent gearContent = new GUIContent("Gear: " + trap.gear.itemName);
					GUIContent applicatorContent = new GUIContent("Applicator: " + trap.applicator.itemName);
					GUIStyle st = getTurretPartStyle();
					Vector2 frameSize = st.CalcSize(frameContent);
//					Vector2 energySourceSize = st.CalcSize(energySourceContent);
					Vector2 triggerSize = st.CalcSize(triggerContent);
					Vector2 gearSize = st.CalcSize(gearContent);
					Vector2 applicatorSize = st.CalcSize(applicatorContent);
					float y = r.y + (r.height - frameSize.y - triggerSize.y - gearSize.y - applicatorSize.y)/2.0f;
					GUI.Label(new Rect(x, y, frameSize.x, frameSize.y), frameContent, st);
					y+=frameSize.y;
					GUI.Label(new Rect(x, y, triggerSize.x, triggerSize.y), triggerContent, st);
					y+=triggerSize.y;
					GUI.Label(new Rect(x, y, gearSize.x, gearSize.y), gearContent, st);
					y+=gearSize.y;
					GUI.Label(new Rect(x, y, applicatorSize.x, applicatorSize.y), applicatorContent, st);
					y+=applicatorSize.y;
					//					size.x *= 
				}
				GUI.EndScrollView();

				if (GUI.Button(trapOkButton(0), "Cancel", getTrapSelectButtonsStyle())) {
					selectStandard(StandardType.Lay_Trap);
				}
				if (GUI.Button(trapOkButton(1), "Select", getTrapSelectButtonsStyle())) {
					selectCurrentTrap();
					mapGenerator.resetRanges();
				}
			}


		}
		if (mapGenerator.gameState != GameState.Playing) {
			GUIContent content = new GUIContent((mapGenerator.gameState==GameState.Won ? "You Won!" : "You Lost!"));
			GUIStyle st = (mapGenerator.gameState==GameState.Won?getWonStyle():getLostStyle());
			if (GUI.Button(new Rect(Screen.width/2 - Screen.width/12, Screen.height*2/3 - Screen.height/16, Screen.width/12, Screen.height/12), "Main Menu")) {
					Application.LoadLevel(0);
			}
			if (GUI.Button(new Rect(Screen.width/2 - Screen.width/12, Screen.height*2/3 + Screen.height/12, Screen.width/12, Screen.height/12), "Quit")) {
				Application.Quit();
			}
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

	GUIStyle turretPartStyle = null;
	public GUIStyle getTurretPartStyle() {
		if (turretPartStyle == null) {
			turretPartStyle = new GUIStyle("Label");
			turretPartStyle.active.textColor = turretPartStyle.hover.textColor = turretPartStyle.normal.textColor = Color.white;
			turretPartStyle.padding = new RectOffset(0, 0, 0, 0);
			turretPartStyle.fontSize = 13;
		}
		return turretPartStyle;
	}

	public Trap selectedTrap = null;

	public void selectCurrentTrap() {
		selectedTrap = getCurrentTrap();
	}

	public void showCurrentTrap() {
		List<Trap> traps = mapGenerator.getCurrentUnit().characterSheet.characterSheet.inventory.getTraps();
		float height = traps.Count * turretSelectSize.y - traps.Count + 1;
		Rect r = trapTypeRect(selectedTrapIndex);
		Rect tR = trapTypesScrollRect();
		float y = tR.y;
		trapsScrollPosition.y = Mathf.Max(r.y - y + r.height - tR.height, Mathf.Min(trapsScrollPosition.y, r.y - y));
	}

	public Trap getCurrentTrap() {
		return getTrap(selectedTrapIndex);
	}

	public Trap getTrap(int n) {
		List<Trap> traps = mapGenerator.getCurrentUnit().characterSheet.characterSheet.inventory.getTraps();
		if (n >= traps.Count || n < 0) return null;
		return traps[n];
	}

	public Turret getCurrentTurret() {
		return getTurret(selectedTurretIndex);
	}

	public Turret getTurret(int n) {
		List<Turret> turrets = mapGenerator.getCurrentUnit().characterSheet.characterSheet.inventory.getTurrets();
		if (n >= turrets.Count || n<0) return null;
		return turrets[n];
	}

	public void showCurrentTurret() {
		List<Turret> turrets = mapGenerator.getCurrentUnit().characterSheet.characterSheet.inventory.getTurrets();
		float height = turrets.Count * turretSelectSize.y - turrets.Count + 1;
		Rect r = turretTypeRect(selectedTurretIndex);
		Rect tR = turretTypesRect();
		float y = tR.y;
		Debug.Log("height: " + height + " r: " + r +" y: " + y);
		Debug.Log("Max(" + (r.y-y) + ", Min(" + turretsScrollPosition.y +", " + (height - r.height) + "))");
	//	turretsScrollPosition.y = Mathf.Max(r.y - y, Mathf.Min(turretsScrollPosition.y + turretTypesRect().height, height - r.height));
//		if (turretsScrollPosition.y > r.y - y) turretsScrollPosition.y = r.y - y;
//		if (turretsScrollPosition.y < r.y - y + r.height - tR.height) turretsScrollPosition.y = r.y - y + r.height - tR.height;
		turretsScrollPosition.y = Mathf.Max(r.y - y + r.height - tR.height, Mathf.Min(turretsScrollPosition.y, r.y - y));
//		if (turretsScrollPosition.y >
	}

	void deselectMinor() {
		if (looting) {
			looting = false;
			openTab = previouslyOpenTab;
		}
		selectedMinor = false;
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

	public void clickTab(Tab tab) {
		if (looting) {
			selectedMinorType = MinorType.None;
			selectMinorType(MinorType.None);
//			looting = false;
			previouslyOpenTab = Tab.Cancel;

		}
		if (openTab==tab) openTab = Tab.None;
		else openTab = tab;
	}

	public bool looting = false;
	public Tab previouslyOpenTab = Tab.None;
	public void selectMinorType(MinorType t) {
		mapGenerator.resetCurrentKeysTile();
		Unit p = mapGenerator.selectedUnit;
		switch (t) {
		case MinorType.Loot:
			looting = true;
			previouslyOpenTab = openTab;
			openTab = Tab.I;
			break;
		case MinorType.Cancel:
		default:
			if (previouslyOpenTab != Tab.Cancel)
				openTab = previouslyOpenTab;
			previouslyOpenTab = Tab.Cancel;
			looting = false;
			break;
		}
	}

	public int selectedTrapIndex = 0;
	public int selectedTurretIndex = 0;
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
		case StandardType.Place_Turret:
			selectedTurretIndex = 0;
			mapGenerator.resetRanges();
			break;
		case StandardType.Lay_Trap:
			selectedTrapIndex = 0;
			trapsScrollPosition = new Vector2(0.0f, 0.0f);
			selectedTrap = null;
			mapGenerator.resetRanges();
			break;
		default:
			selectedTrap = null;
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
