using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum Tab {R, C, V, B, T, Cancel, None}
public enum Mission {Primary, Secondary, Optional, None}
public class GameGUI : MonoBehaviour {

	public static Trap selectedTrap;
	public static Turret selectedTurret;
	public static bool selectedTrapTurret;

	static Unit hovering = null;
	public static MapGenerator mapGenerator;
	public static Log log;
	static GUIStyle playerNormalStyle;
	static GUIStyle playerBoldStyle;
	static GUIStyle selectedButtonStyle = null;
	static GUIStyle nonSelectedButtonStyle = null;
	static GUIStyle selectedSubMenuTurnStyle = null;
	static GUIStyle confirmButtonStyle = null;
	static Vector2 position = new Vector2(0.0f, 0.0f);
	static Rect scrollRect;
	static bool scrollShowing;
	static bool first = true;
	public static int temperedHandsMod = 0;
	public static bool escapeMenuOpen = false;

	public static bool showAttack = false;
	public static bool showMovement = false;

	static Vector2 notTurnMoveRangeSize = new Vector2(150.0f, 50.0f);
	static Vector2 subMenuTurnActionSize = new Vector2(100.0f, 35.0f);
	static Vector2 turretSelectSize = new Vector2(250.0f, 100.0f);

	public static Vector2 selectionUnitScrollPosition = new Vector2(0.0f, 0.0f);
	public static Vector2 turretsScrollPosition = new Vector2(0.0f, 0.0f);
	public static Vector2 trapsScrollPosition = new Vector2(0.0f, 0.0f);
	static Vector2 turnOrderScrollPos = new Vector2(0.0f, 0.0f);

	public static bool selectedMovement = false;
	public static bool selectedStandard = false;
	public static bool selectedMinor = false;
	public static bool turretDirection = false;
	public static MovementType selectedMovementType = MovementType.None;
	public static StandardType selectedStandardType = StandardType.None;
	public static MinorType selectedMinorType = MinorType.None;


	public static Tab clipboardTab = Tab.T;
	public static Mission openMission = Mission.Primary;

	static Texture2D actionTexture3;
	static Texture2D hotkeysBackTextureCenter;
	static Texture2D hotkeysBackTextureLeft;
	static Texture2D clipBoardBodyTexture;

	public static void resetVars() {
		clipboardTab = Tab.T;
		openMission = Mission.Primary;
		selectedMovement = false;
		selectedStandard = false;
		selectedMinor = false;
		selectedStandardType = StandardType.None;
		selectedMovementType = MovementType.None;
		selectedMinorType = MinorType.None;
		escapeMenuOpen = false;
		temperedHandsMod = 0;
		selectedTrap = null;
		selectedTurret = null;selectedTrapTurret = false;
		selectionUnitScrollPosition = new Vector2(0.0f, 0.0f);
		turretsScrollPosition = new Vector2(0.0f, 0.0f);
		trapsScrollPosition = new Vector2(0.0f, 0.0f);
		turnOrderScrollPos = new Vector2(0.0f, 0.0f);
	}

	// Use this for initialization
	public static void initialize() {
		clipboardTab = Tab.T;
		position = new Vector2(0.0f, 0.0f);
		first = true;
		actionTexture3 = Resources.Load<Texture>("UI/SocketBase_v2") as Texture2D;

		hotkeysBackTextureLeft = Resources.Load<Texture>("UI/action-bar-left") as Texture2D;
		hotkeysBackTextureCenter = Resources.Load<Texture>("UI/action-bar-center") as Texture2D;
		clipBoardBodyTexture = Resources.Load<Texture>("UI/clipboard-body") as Texture2D;
	}

	static Texture2D getHotKeysBackTexture(int n) {
		if (n == -1 || n == Mathf.Max(2, numberActions)) return hotkeysBackTextureLeft;
		return hotkeysBackTextureCenter;
	}
	
	// Update is called once per frame
	void Update () {
	}

	public static void setConfirmShown() {
		if (showingConfirm != hasConfirmButton()) {
			showingConfirm = !showingConfirm;
			ConfirmButton but = ConfirmButton.Standard;
			if (selectedMinor) but = ConfirmButton.Minor;
			else if (selectedMovement) but = ConfirmButton.Movement;
			BattleGUI.setConfirmButtonShown(but, showingConfirm);
		}
	}

	// Make the black-bordered solid color texture used throughout the programmer-art UI
	static Texture2D makeTex( int width, int height, Color col )
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

	// Return the size of an action icon  (couldn't this be stored as a field instead?)
	public static Vector2 actionIconSize() {
		return new Vector2(40.0f, 40.0f);
	}


	public static Rect actionIconRect(int n) {
		float y = actionBarTotalRect().y + (actionBarTotalSize().y - actionIconSize().y)/2.0f;
		float totesWidth = numberActions * actionBarTotalSize().y + (actionBarTotalSize().y - actionIconSize().y);
		float first = (Screen.width - totesWidth)/2.0f;
		float x = first + actionBarTotalSize().y*n + actionBarTotalSize().y - actionIconSize().x;
		return new Rect(x, y, actionIconSize().x, actionIconSize().y);
	}

	public static Vector2 actionButtonsSize() {
//		return new Vector2(90.0f, 50.0f);
		return new Vector2(150.0f, 40.0f);
		return notTurnMoveRangeSize;
//		return new Vector2(90.0f, 40.0f);
	}

	public static Vector2 actionButtonsTotalSize() {
		return new Vector2(236.0f, 200.0f);
	}

	public static Rect rangeRect() {
		return new Rect(0.0f, Screen.height - notTurnMoveRangeSize.y*2 + 1, notTurnMoveRangeSize.x, notTurnMoveRangeSize.y*2-1);
	}

	public static int numberActions;
	public static Vector2 actionBarSectionSize() {
		return new Vector2(60.0f, 60.0f);
	}
	public static Vector2 actionBarSideSize() {
		return new Vector2(30.0f, 60.0f);
	}
	public static Vector2 actionBarTotalSize() {
		return new Vector2(actionBarSideSize().x * 2 + actionBarSectionSize().x * Mathf.Max(2, numberActions), actionBarSectionSize().y);
	}
	public static Rect actionBarTotalRect() {
		return new Rect((Screen.width - actionBarTotalSize().x)/2.0f, Screen.height - Log.consoleHeight - actionBarTotalSize().y + 10.0f, actionBarTotalSize().x, actionBarTotalSize().y);
	}
	public static Rect actionBarRect(int n) {
		float width = actionBarSectionSize().x;
		float x = actionBarTotalRect().x;
		if (n ==-1) width = actionBarSideSize().x;
		else {
			x = actionBarTotalRect().x + actionBarSideSize().x + actionBarSectionSize().x * n;
			if (n >= Mathf.Max(2, numberActions))  {
				width = -actionBarSideSize().x;
				x += actionBarSideSize().x;
			}

		}
		return new Rect(x, actionBarTotalRect().y, width, actionBarSectionSize().y);
	}

	public static Rect actionRect() {
		float boxHeight = actionButtonsSize().y * 3 + (1) * 20.0f;
		return new Rect(-20.0f, Screen.height - actionButtonsTotalSize().y + 10.0f, actionButtonsTotalSize().x, actionButtonsTotalSize().y);
	}

	public static Rect turretTypeRect(int n) {
		return new Rect(Screen.width - turretSelectSize.x, turretTypesRect().y + turretSelectSize.y*n - n, turretSelectSize.x, turretSelectSize.y);
	}

	public static Rect trapTypeRect(int n) {
		return new Rect(trapTypesRect().x, trapTypesRect().y + turretSelectSize.y*n - n, turretSelectSize.x, turretSelectSize.y);
	}

	public static Rect turretTypesRect() {
		float height = turretSelectSize.y * 3 - 2;
		return new Rect(Screen.width - turretSelectSize.x, (Screen.height - height)/2.0f, turretSelectSize.x, height);
	}

	public static Vector2 trapOkButtons() {
		float x = turretSelectSize.x/2.0f-30.0f/2.0f;
		return new Vector2(x, x/2.0f);
	}

	public static Vector2 trapOkButtonsSize() {
		float height = trapOkButtons().y + 20.0f;
		float width = turretSelectSize.x;
		return new Vector2(width, height);
	}

	public static Rect trapOkButton(int n) {
		float x = trapTypesRect().x + 10.0f;
		float y = trapTypesRect().y + trapTypesScrollRect().height + 10.0f;
		return new Rect(x + n * (trapOkButtons().x + 10.0f), y, trapOkButtons().x, trapOkButtons().y);
	}

	public static Rect trapTypesScrollRect() {
		float height = turretSelectSize.y * 3 - 2;
		float width = turretSelectSize.x;
		float x = (Screen.width - width)/2.0f;
		float y = (Screen.height - height)/2.0f;
		return new Rect(x, y, width, height);
	}

	public static Rect trapTypesRect() {
		Rect r = trapTypesScrollRect();
		r.height += trapOkButtonsSize().y;
		return r;
		float height = turretSelectSize.y * 3 - 2 + trapOkButtonsSize().y;
		float width = turretSelectSize.x;
		float x = (Screen.width - width)/2.0f;
		float y = (Screen.height - height)/2.0f;
		return new Rect(x, y, width, height);
	}

	public static Rect moveButtonRect() {
		return new Rect(actionRect().x + actionButtonsTotalSize().x - actionButtonsSize().x - 6.0f, actionRect().y + actionButtonsSize().y * 0 + 20.0f, actionButtonsSize().x, actionButtonsSize().y);
	}

	public static Rect attackButtonRect() {
		return new Rect(actionRect().x + actionButtonsTotalSize().x - actionButtonsSize().x - 6.0f, actionRect().y + actionButtonsSize().y * 1 + 30.0f, actionButtonsSize().x, actionButtonsSize().y);
	}

	public static Rect minorButtonRect() {	
		return new Rect(actionRect().x + actionButtonsTotalSize().x - actionButtonsSize().x - 6.0f, actionRect().y + actionButtonsSize().y * 2 + 40.0f, actionButtonsSize().x, actionButtonsSize().y);
	}
	
	public static Rect waitButtonRect() {
		return new Rect(actionRect().x + actionButtonsTotalSize().x - actionButtonsSize().x, actionRect().y + actionButtonsSize().y * 3 + 5.0f, actionButtonsSize().x, actionButtonsSize().y);
	}

	public static Rect waitButtonAlwaysRect() {
		return new Rect(Screen.width - actionButtonsSize().x, 0.0f, actionButtonsSize().x, actionButtonsSize().y);
	}


	//------------------------------------------------------------ Clipboard Stuff
	static Vector2 tabButtonSize = new Vector2(45.0f, 60.0f);
	public static Rect getTabButtonRect(Tab t) {
		float x = 0.0f;
		float y = 0.0f;
		if (t == Tab.T || t == Tab.R) {
			x = clipBoardBodyRect().x - tabButtonSize.x;
			y = clipBoardBodyRect().y + 10.0f;
			if (t == Tab.R) {
				y += tabButtonSize.y + 5.0f;
			}
		}
		else if (t == Tab.C || t == Tab.V) {
			x = UnitGUI.bannerX + UnitGUI.bannerWidth - 20.0f;
			y = 0.0f;
			if (t == Tab.V) {
				y += tabButtonSize.y + 5.0f;
			}
		}
		return new Rect(x, y, tabButtonSize.x, tabButtonSize.y);
	}
	public static bool clipboardUp = true;
	public const float clipboardBodyWidth = 158.0f;
	public static Vector2 clipboardBodySize() {
		return new Vector2(clipboardBodyWidth, (clipboardUp ? 250.0f : 160.0f));
	}
	public static Vector2 clipboardClipSize() {
		return new Vector2(150.0f, 50.0f);
	}
	public static Rect clipBoardBodyRect() {
		return new Rect(Screen.width - clipboardBodySize().x, Screen.height - clipboardBodySize().y, clipboardBodySize().x, clipboardBodySize().y);
	}
	public static Rect clipBoardClipRect() {
		return new Rect(clipBoardBodyRect().x + (clipboardBodySize().x - clipboardClipSize().x)/2.0f, clipBoardBodyRect().y + 10.0f - clipboardClipSize().y, clipboardClipSize().x, clipboardClipSize().y);
	}

	//----------------------------------------------------------- Menus/Confirmation Stuff?
	public static Rect subMenuButtonsRect() {
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

	public static Rect subMenuButtonRect(int i) {
		Rect r = subMenuButtonsRect();
		return new Rect(r.x, r.y + i * (subMenuTurnActionSize.y - 1), subMenuTurnActionSize.x, subMenuTurnActionSize.y);
	}

	public static Rect confirmButtonRect() {
		Rect r = subMenuButtonsRect();
		return new Rect((Screen.width - subMenuTurnActionSize.x)/2.0f, actionBarTotalRect().y - subMenuTurnActionSize.y + 2.0f, subMenuTurnActionSize.x, subMenuTurnActionSize.y);
	}

	static float beginButtonWidth = 150.0f;
	static float beginButtonHeight = 50.0f;
	public static Rect beginButtonRect() {
		return new Rect((Screen.width - mapGenerator.selectionWidth - beginButtonWidth)/2.0f, Screen.height - beginButtonHeight, beginButtonWidth, beginButtonHeight);
	}

	static float temperedHandsWidth = 400.0f;
	static float temperedHandsHeight = 150.0f;
	public static Rect temperedHandsRect() {
		return new Rect((Screen.width - temperedHandsWidth)/2.0f, (Screen.height - temperedHandsHeight)/2.0f, temperedHandsWidth, temperedHandsHeight);
	}

	public static bool showingConfirm = false;
	public static bool hasConfirmButton() {
		return ((selectedMovement && (selectedMovementType == MovementType.BackStep || selectedMovementType == MovementType.Move)) && mapGenerator.getCurrentUnit().currentPath.Count > 1) ||
			((selectedStandard && (selectedStandardType == StandardType.Attack || selectedStandardType == StandardType.InstillParanoia || selectedStandardType == StandardType.OverClock || selectedStandardType == StandardType.Throw || selectedStandardType == StandardType.Intimidate)) && mapGenerator.getCurrentUnit().attackEnemy != null) ||
				((selectedStandard && (selectedStandardType == StandardType.Place_Turret)) && mapGenerator.turretBeingPlaced != null) ||
				((selectedStandard && (selectedStandardType == StandardType.Lay_Trap)) && mapGenerator.currentTrap.Count>0) ||
				((selectedMinor && (selectedMinorType == MinorType.Mark || selectedMinorType == MinorType.Escape)) && mapGenerator.getCurrentUnit().attackEnemy != null) ||
				((selectedMinor && (selectedMinorType == MinorType.Stealth || (selectedMinorType == MinorType.OneOfMany && oneOfManyConfirm))));
	}

	public static bool mouseIsOnGUI() {
		Vector2 mousePos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
	//	if (log.mouseIsOnGUI()) return true;
		if (mapGenerator) {
			if (escapeMenuOpen || mapGenerator.gameState != GameState.Playing) {
				for (int n=0;n<(escapeMenuOpen ? 3 : 2); n++) {
					if (getMenuRect(n, escapeMenuOpen).Contains(mousePos)) return true;
				}
			}
			if (mapGenerator.isInCharacterPlacement()) {
				if (beginButtonRect().Contains(mousePos)) return true;
			}
			else {
				if (clipBoardBodyRect().Contains(mousePos)) return true;
				if (clipBoardClipRect().Contains(mousePos)) return true;
				if (getTabButtonRect(Tab.T).Contains(mousePos) || getTabButtonRect(Tab.R).Contains(mousePos)) return true;
			}
			if (mapGenerator.selectedUnit != null) {
				bool onPlayer = mapGenerator.selectedUnits.Count == 0 && mapGenerator.selectedUnit.guiContainsMouse(mousePos);
				bool onWait = waitButtonAlwaysRect().Contains(mousePos);
				bool others = onPlayer || onWait;
				if (mapGenerator.selectedUnit == mapGenerator.getCurrentUnit() && mapGenerator.selectedUnits.Count == 0) {
					if (actionRect().Contains(mousePos) || actionBarTotalRect().Contains(mousePos) || (hasConfirmButton() && confirmButtonRect().Contains(mousePos)) || others) return true;
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
			else {
				if (mapGenerator.getCurrentUnit().doingTemperedHands && temperedHandsRect().Contains(mousePos)) return true;
			}
		}
		return false;
	}

	public static bool mouseIsOnScrollView() {
		Vector2 mousePos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
		if (mapGenerator) {
			if (scrollShowing) {
				return scrollRect.Contains(mousePos);
			}
		}
		return false;
	}


	//---------------------------------------------------------- Style Stuff
	static GUIStyle getNormalStyle() {
		if (playerNormalStyle == null) {
			playerNormalStyle = new GUIStyle(GUI.skin.label);
		//	GUIContent cont = new GUIContent("ab
			playerNormalStyle.fontStyle = FontStyle.Normal;
			playerNormalStyle.fontSize = 15;
		}
		return playerNormalStyle;
	}

	static GUIStyle getBoldStyle() {
		if (playerBoldStyle == null) {
			playerBoldStyle = new GUIStyle(GUI.skin.label);
			playerBoldStyle.fontStyle = FontStyle.Bold;
			playerBoldStyle.fontSize = 15;
			playerBoldStyle.normal.textColor = Color.green;
		}

		return playerBoldStyle;
	}

	static Dictionary<string, GUIStyle> selectedButtonStyles = null;
	static Dictionary<string, GUIStyle> unselectedButtonStyles = null;
	static Dictionary<string, GUIStyle> disabledButtonStyles = null;
	static GUIStyle getSelectedButtonStyle(string name) {
		if (selectedButtonStyles == null) selectedButtonStyles = new Dictionary<string, GUIStyle>();
		if (!selectedButtonStyles.ContainsKey(name)) {
			GUIStyle st = new GUIStyle("Button");
			st.normal.background = st.hover.background = Resources.Load<Texture>("UI/" + name + "_hover") as Texture2D;
			st.active.background = Resources.Load<Texture>("UI/" + name + "_lit") as Texture2D;
			selectedButtonStyles[name] = st;
		}
		return selectedButtonStyles[name];
	}
	
	static GUIStyle getNonSelectedButtonStyle(string name) {
		if (unselectedButtonStyles == null) unselectedButtonStyles = new Dictionary<string, GUIStyle>();
		if (!unselectedButtonStyles.ContainsKey(name)) {
			GUIStyle st = new GUIStyle("Button");
			st.hover.background =  Resources.Load<Texture>("UI/" + name + "_hover") as Texture2D;
			st.normal.background = Resources.Load<Texture>("UI/" + name + "_lit") as Texture2D;
			st.active.background = Resources.Load<Texture>("UI/" + name + "_pressed") as Texture2D;
			unselectedButtonStyles[name] = st;
		}
		return unselectedButtonStyles[name];
	}
	
	static GUIStyle getDisabledButtonStyle(string name) {
		if (disabledButtonStyles == null) disabledButtonStyles = new Dictionary<string, GUIStyle>();
		if (!disabledButtonStyles.ContainsKey(name)) {
			GUIStyle st = new GUIStyle("Button");
			st.hover.background = st.normal.background = st.active.background = Resources.Load<Texture>("UI/" + name + "_unlit") as Texture2D;
			disabledButtonStyles[name] = st;
		}
		return disabledButtonStyles[name];
	}

	static GUIStyle getSelectedButtonStyle() {
		if (selectedButtonStyle == null) {
			selectedButtonStyle = new GUIStyle(GUI.skin.button);
		//	Texture2D tex = makeTex((int)notTurnMoveRangeSize.x,(int)notTurnMoveRangeSize.y, new Color(22.5f/255.0f, 30.0f/255.0f, 152.5f/255.0f));
			//selectedButtonStyle.normal.background = tex;//makeTex((int)notTurnMoveRangeSize.x,(int)notTurnMoveRangeSize.y,new Color(30.0f, 40.0f, 210.0f));
		//	selectedButtonStyle.hover.background = tex;//selectedButtonStyle.normal.background;
		//	selectedButtonStyle.active.background = tex;
			selectedButtonStyle.hover.background = selectedButtonStyle.normal.background = selectedButtonStyle.active.background = Resources.Load<Texture>("") as Texture2D;
			selectedButtonStyle.hover.textColor = Color.white;
			selectedButtonStyle.normal.textColor = Color.white;
			selectedButtonStyle.active.textColor = Color.white;
		}
		return selectedButtonStyle;
	}
	
	static GUIStyle getNonSelectedButtonStyle() {
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
	static GUIStyle getSelectedButtonTurretStyle() {
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
	static GUIStyle getNonSelectedButtonTurretStyle() {
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
	static Texture2D getTurretBackgroundTexture() {
		if (turretBackgroundTexture == null) {
			Rect r = turretTypesRect();
			turretBackgroundTexture = makeTex((int)r.width,(int)r.height,new Color(30.0f/255.0f, 40.0f/255.0f, 210.0f/255.0f));
		}
		return turretBackgroundTexture;
	}

	static Texture2D trapBackgroundTexture;
	static Texture2D getTrapBackgroundTexture() {
		if (trapBackgroundTexture==null) {
			Rect r = trapTypesRect();
			trapBackgroundTexture = makeTex((int)r.width,(int)r.height,new Color(30.0f/255.0f, 40.0f/255.0f, 210.0f/255.0f));
		}
		return trapBackgroundTexture;
	}

	static GUIStyle trapSelectButtonsStyle;
	static GUIStyle getTrapSelectButtonsStyle() {
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

	static GUIStyle beginButtonStyle;
	static GUIStyle getBeginButtonStyle() {
		if (beginButtonStyle == null) {
			beginButtonStyle = new GUIStyle("button");
			Texture2D tex = makeTex((int)beginButtonWidth, (int)beginButtonHeight, new Color(30.0f/255.0f, 40.0f/255.0f, 210.0f/255.0f));
			beginButtonStyle.normal.background = beginButtonStyle.hover.background = beginButtonStyle.active.background = tex;
			beginButtonStyle.normal.textColor = beginButtonStyle.hover.textColor = beginButtonStyle.active.textColor = Color.white;
		}
		return beginButtonStyle;
	}
	static GUIStyle clipBoardClipStyle = null;
	static GUIStyle getClipBoardClipStyle() {
		if (clipBoardClipStyle==null) {
			clipBoardClipStyle = new GUIStyle("Button");
			clipBoardClipStyle.normal.background = clipBoardClipStyle.hover.background = Resources.Load<Texture>("UI/clipboard-clip") as Texture2D;
			clipBoardClipStyle.active.background = Resources.Load<Texture>("UI/clipboard-clip-pressed") as Texture2D;
		}
		return clipBoardClipStyle;
	}

	static GUIStyle missionTypeSelectStyle;
	static GUIStyle getMissionTypeSelectStyle() {
		if (missionTypeSelectStyle == null) {
			missionTypeSelectStyle = new GUIStyle("Button");
			missionTypeSelectStyle.normal.background = missionTypeSelectStyle.active.background = missionTypeSelectStyle.hover.background = null;
			missionTypeSelectStyle.padding = new RectOffset(0, 0, 0, 0);
			missionTypeSelectStyle.margin = new RectOffset(0, 0, 0, 0);
			missionTypeSelectStyle.fontSize = 10;
		}
		return missionTypeSelectStyle;
	}

	static Dictionary<string, GUIStyle> selectedActionStyles = new Dictionary<string, GUIStyle>();
	static Dictionary<string, GUIStyle> nonSelectedActionStyles = new Dictionary<string, GUIStyle>();

	static GUIStyle getSelectedActionStyle(string name) {
		if (!selectedActionStyles.ContainsKey(name)) {
			GUIStyle st = new GUIStyle("Button");
			st.normal.background = st.hover.background = st.active.background = Resources.Load<Texture>("UI/Hotkey Icons/" + name + " Selected") as Texture2D;
			selectedActionStyles[name] = st;
			st.alignment = TextAnchor.LowerRight;
			st.padding = new RectOffset(1, 0, 0, -1);
			st.fontSize = 9;
			st.normal.textColor = st.active.textColor = st.hover.textColor = Color.white;
		}
		return selectedActionStyles[name];
	}

	static GUIStyle getNonSelectedActionStyle(string name) {
		if (!nonSelectedActionStyles.ContainsKey(name)) {
			GUIStyle st = new GUIStyle("Button");
			st.normal.background = st.hover.background = st.active.background = Resources.Load<Texture>("UI/Hotkey Icons/" + name) as Texture2D;
			nonSelectedActionStyles[name] = st;
			st.alignment = TextAnchor.LowerRight;
			st.padding = new RectOffset(1, 0, 0, -1);
			st.fontSize = 9;
			st.normal.textColor = st.active.textColor = st.hover.textColor = Color.white;
		}
		return nonSelectedActionStyles[name];
	}

	static GUIStyle getSelectedSubMenuTurnStyle() {
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

	static GUIStyle getConfirmButtonStyle() {
		if (confirmButtonStyle == null) {
			confirmButtonStyle = new GUIStyle(GUI.skin.button);
			Texture2D tex = makeTex((int)subMenuTurnActionSize.x,(int)subMenuTurnActionSize.y,new Color(30.0f/255.0f, 40.0f/255.0f, 210.0f/255.0f));
			confirmButtonStyle.normal.background = confirmButtonStyle.hover.background = confirmButtonStyle.active.background = Resources.Load<Texture>("UI/tab-button") as Texture2D;
			confirmButtonStyle.active.textColor = confirmButtonStyle.normal.textColor = confirmButtonStyle.hover.textColor = Color.white;
		}
		return confirmButtonStyle;
	}


	
	static Texture2D turnOrderNameBackgroundTexture = null;
	static Texture2D getTurnOrderNameBackgroundTexture() {
		if (turnOrderNameBackgroundTexture == null) {
			turnOrderNameBackgroundTexture = Unit.makeTexBorder((int)turnOrderNameWidth, (int)turnOrderSectionHeight, new Color(0.5f, 0.8f, 0.1f));
		}
		return turnOrderNameBackgroundTexture;
	}
	
	static Texture2D turnOrderSectionBackgroundTexture = null;
	static Texture2D getTurnOrderSectionBackgroundTexture() {
		if (turnOrderSectionBackgroundTexture == null) {
			turnOrderSectionBackgroundTexture = Unit.makeTexBorder((int)turnOrderSectionHeight, (int)turnOrderSectionHeight, new Color(0.5f, 0.8f, 0.1f));
		}
		return turnOrderSectionBackgroundTexture;
	}
	
	static Texture2D turnOrderNameBackgroundTextureEnemy = null;
	static Texture2D getTurnOrderNameBackgroundTextureEnemy() {
		if (turnOrderNameBackgroundTextureEnemy == null) {
			turnOrderNameBackgroundTextureEnemy = Unit.makeTexBorder((int)turnOrderNameWidth, (int)turnOrderSectionHeight, new Color(0.8f, 0.2f, 0.1f));
		}
		return turnOrderNameBackgroundTextureEnemy;
	}
	
	static Texture2D turnOrderSectionBackgroundTextureEnemy = null;
	static Texture2D getTurnOrderSectionBackgroundTextureEnemy() {
		if (turnOrderSectionBackgroundTextureEnemy == null) {
			turnOrderSectionBackgroundTextureEnemy = Unit.makeTexBorder((int)turnOrderSectionHeight, (int)turnOrderSectionHeight, new Color(0.8f, 0.2f, 0.1f));
		}
		return turnOrderSectionBackgroundTextureEnemy;
	}

	
	static GUIStyle turnOrderSectionStyle;
	static GUIStyle turnOrderSectionStyleEnemy;
	static GUIStyle getTurnOrderSectionStyle(Unit u) {
		if (u.team == 0) {
			if (turnOrderSectionStyle == null) {
				turnOrderSectionStyle = new GUIStyle("button");
				turnOrderSectionStyle.normal.background = turnOrderSectionStyle.hover.background = turnOrderSectionStyle.active.background = getTurnOrderSectionBackgroundTexture();
			}
			return turnOrderSectionStyle;
		}
		else {
			if (turnOrderSectionStyleEnemy == null) {
				turnOrderSectionStyleEnemy = new GUIStyle("button");
				turnOrderSectionStyleEnemy.normal.background = turnOrderSectionStyleEnemy.hover.background = turnOrderSectionStyleEnemy.active.background = getTurnOrderSectionBackgroundTextureEnemy();
			}
			return turnOrderSectionStyleEnemy;
		}
	}


	
	static GUIStyle turnOrderNameStyle;
	static GUIStyle turnOrderNameStyleEnemy;
	static GUIStyle getTurnOrderNameStyle(Unit u) {
		if (u.team == 0) {
			if (turnOrderNameStyle == null) {
				turnOrderNameStyle = new GUIStyle("button");
				turnOrderNameStyle.normal.background = turnOrderNameStyle.hover.background = turnOrderNameStyle.active.background = getTurnOrderNameBackgroundTexture();
			}
			return turnOrderNameStyle;
		}
		else {
			if (turnOrderNameStyleEnemy == null) {
				turnOrderNameStyleEnemy = new GUIStyle("button");
				turnOrderNameStyleEnemy.normal.background = turnOrderNameStyleEnemy.hover.background = turnOrderNameStyleEnemy.active.background = getTurnOrderNameBackgroundTextureEnemy();
			}
			return turnOrderNameStyleEnemy;
		}
	}
	
	static GUIStyle tabButtonStyle;
	public static GUIStyle getTabButtonStyle() {
		if (tabButtonStyle == null) {
			tabButtonStyle = new GUIStyle("Button");
			tabButtonStyle.normal.background = tabButtonStyle.hover.background = tabButtonStyle.active.background = Resources.Load<Texture>("UI/tab-button-left") as Texture2D;
			tabButtonStyle.normal.textColor = tabButtonStyle.hover.textColor = tabButtonStyle.active.textColor = Color.black;
		}
		return tabButtonStyle;
	}
	static GUIStyle tabButtonRightStyle;
	public static GUIStyle getTabButtonRightStyle() {
		if (tabButtonRightStyle == null) {
			tabButtonRightStyle = new GUIStyle("Button");
			tabButtonRightStyle.normal.background = tabButtonRightStyle.hover.background = tabButtonRightStyle.active.background = Resources.Load<Texture>("UI/tab-button-right") as Texture2D;
			tabButtonRightStyle.normal.textColor = tabButtonRightStyle.hover.textColor = tabButtonRightStyle.active.textColor = Color.black;
		}
		return tabButtonRightStyle;
	}


	static GUIStyle playerInfoStyle;
	static GUIStyle getPlayerInfoStyle() {
		if (playerInfoStyle == null) {
			playerInfoStyle = new GUIStyle("Label");
			playerInfoStyle.normal.textColor = Color.white;
			playerInfoStyle.fontSize = 11;
		}
		return playerInfoStyle;
	}

	static GUIStyle titleTextStyle = null;
	public static GUIStyle getTitleTextStyle() {
		if (titleTextStyle == null) {
			titleTextStyle = new GUIStyle("Label");
			titleTextStyle.normal.textColor = Color.white;
			titleTextStyle.fontSize = 15;
		}
		return titleTextStyle;
	}
	static GUIStyle namesStyle = null;
	static GUIStyle getNamesStyle() {
		if (namesStyle==null) {
			namesStyle = new GUIStyle("Label");
			namesStyle.fontSize = 12;
			namesStyle.normal.textColor = Color.white;
			namesStyle.alignment = TextAnchor.MiddleCenter;
		}
		return namesStyle;
	}

	static GUIStyle wonStyle = null;
	static GUIStyle lostStyle = null;
	static GUIStyle backStyle = null;
	static GUIStyle getWonStyle() {
		if (wonStyle == null) {
			wonStyle = new GUIStyle("Label");
			wonStyle.fontSize = 200;
			wonStyle.normal.textColor = Color.green;
			wonStyle.alignment = TextAnchor.MiddleCenter;
		}
		return wonStyle;
	}
	static GUIStyle getLostStyle() {
		if (lostStyle == null) {
			lostStyle = new GUIStyle("Label");
			lostStyle.fontSize = 200;
			lostStyle.normal.textColor = Color.red;
			lostStyle.alignment = TextAnchor.MiddleCenter;
		}
		return lostStyle;
	}
	static GUIStyle getBackStyle() {
		if (backStyle == null) {
			backStyle = new GUIStyle("Label");
			backStyle.fontSize = 200;
			backStyle.normal.textColor = Color.black;
			backStyle.alignment = TextAnchor.MiddleCenter;
		}
		return backStyle;
	}


	//------------------------------------------------------------ Button Behavior stuff
	public static void clickWait() {
		if (mapGenerator.performingAction() || mapGenerator.currentUnitIsAI() || mapGenerator.isInCharacterPlacement()) return;
		Unit p = mapGenerator.selectedUnit;
		if (selectedMovement) {
			deselectMovementType(selectedMovementType);
		}
		if (selectedStandard) {
			deselectStandardType(selectedStandardType);
		}
		if (selectedMinor) {
			deselectMinorType(selectedMinorType);
		}
		mapGenerator.nextPlayer();
	}

	public static void clickStandard() {
		if (mapGenerator.performingAction() || mapGenerator.currentUnitIsAI() || mapGenerator.isInCharacterPlacement()) return;
		Unit p = mapGenerator.selectedUnit;
		if (p==null || p.usedStandard || p.isProne()) return;
		if (p.usedStandard) return;
		if (selectedStandard) return;
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

	public static void clickMovement() {
		if (mapGenerator.performingAction() || mapGenerator.currentUnitIsAI() || mapGenerator.isInCharacterPlacement()) return;
		Unit p = mapGenerator.selectedUnit;
		if (p==null || p.usedMovement) return;
		if (selectedMovement) return;
		if (selectedStandard) {
			//		selectedStandardType = StandardType.None;
			deselectStandard();
		}

		selectedMovement = !selectedMovement;
		if (selectedMovement && !p.getMovementTypes().Contains(selectedMovementType)) selectedMovementType = MovementType.None;
		selectMovementType(selectedMovementType);
		if (selectedMinor) {
			deselectMinor();
		}
		mapGenerator.resetRanges();
	}



	public static void clickMinor() {
		if (mapGenerator.performingAction() || mapGenerator.currentUnitIsAI() || mapGenerator.isInCharacterPlacement()) return;
		Unit p = mapGenerator.selectedUnit;
		if (p==null || p.minorsLeft==0) return;
		if (selectedMinor) return;
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

	public static bool standardEnabled(StandardType type) {
		return ((type != StandardType.Attack && type != StandardType.OverClock) || mapGenerator.getCurrentUnit().hasWeapon());
	}

	public static bool movementEnabled(MovementType type) {
		return type != MovementType.BackStep || mapGenerator.getCurrentUnit().moveDistLeft == mapGenerator.getCurrentUnit().maxMoveDist;
	}

	public static bool minorEnabled(MinorType type) {
		return (type != MinorType.TemperedHands || mapGenerator.getCurrentUnit().temperedHandsUsesLeft > 0) && (type != MinorType.Escape || !mapGenerator.getCurrentUnit().escapeUsed) && (type != MinorType.Invoke || mapGenerator.getCurrentUnit().invokeUsesLeft > 0);
	}

	public static void selectTypeAt(int index) {
		if (selectedStandard) {
			StandardType[] standards = mapGenerator.getCurrentUnit().getStandardTypes();
			if (index >= standards.Length || !standardEnabled(standards[index])) return;
			selectStandard(standards[index]);
		}
		else if (selectedMovement) {
			MovementType[] movements = mapGenerator.getCurrentUnit().getMovementTypes();
			if (index >= movements.Length || !movementEnabled(movements[index])) return;
			selectMovement(movements[index]);
		}
		else if (selectedMinor) {
			MinorType[] minors = mapGenerator.getCurrentUnit().getMinorTypes();
			if (index >= minors.Length || !minorEnabled(minors[index])) return;
			selectMinor(minors[index]);
		}
	}

	public static void selectPreviousAction() {
		selectActionBy(-1);
	}

	public static void selectNextAction() {
		selectActionBy(1);
	}
	
	public static void selectActionBy(int by) {
		Unit u = mapGenerator.getCurrentUnit();
		MovementType[] movementTypes = u.getMovementTypes();
		StandardType[] standardTypes = u.getStandardTypes();
		MinorType[] minorTypes = u.getMinorTypes();
		bool someArmShown = (!u.usedStandard && BattleGUI.armShown(ActionArm.Standard)) || (!u.usedMovement && BattleGUI.armShown(ActionArm.Movement)) || (u.minorsLeft > 0 && BattleGUI.armShown(ActionArm.Minor));
		bool movement = !u.usedMovement && (!someArmShown || BattleGUI.armShown(ActionArm.Movement));
		bool standard = !u.usedStandard && (!someArmShown || BattleGUI.armShown(ActionArm.Standard));
		bool minor = u.minorsLeft > 0 && (!someArmShown || BattleGUI.armShown(ActionArm.Minor));
		int totalTypes = (movement ? movementTypes.Length : 0) + (standard ? standardTypes.Length : 0) + (minor ? minorTypes.Length : 0);
		int currentType = 0;
		bool found = false;
		if (movement && !found) {
			foreach (MovementType type in movementTypes) {
				if (selectedMovement && type == selectedMovementType) {
					found = true;
					break;
				}
				currentType++;
			}
		}
		if (standard && !found) {
			foreach (StandardType type in standardTypes) {
				if (selectedStandard && type == selectedStandardType) {
					found = true;
					break;
				}
				currentType++;
			}
		}
		if (minor && !found) {
			foreach (MinorType type in minorTypes) {
				if (selectedMinor && type == selectedMinorType) {
					found = true;
					break;
				}
				currentType++;
			}
		}
		if (currentType >= totalTypes) currentType = totalTypes-1;
		currentType += by;
		while (currentType < 0) currentType += totalTypes;
		currentType %= totalTypes;
		if (movement) {
			if (currentType < movementTypes.Length) {
				selectMovementType(movementTypes[currentType]);
				return;
			}
			else currentType -= movementTypes.Length;
		}
		if (standard) {
			if (currentType < standardTypes.Length) {
				selectStandardType(standardTypes[currentType]);
				return;
			}
			else currentType -= standardTypes.Length;
		}
		if (minor) {
			if (currentType < minorTypes.Length) {
				selectMinorType(minorTypes[currentType]);
				return;
			}
			else currentType -= minorTypes.Length;
		}
	}

	
	public static void selectActionAt(int actionInd) {
		Unit u = mapGenerator.getCurrentUnit();
		MovementType[] movementTypes = u.getMovementTypes();
		StandardType[] standardTypes = u.getStandardTypes();
		MinorType[] minorTypes = u.getMinorTypes();
		bool someArmShown = (!u.usedStandard && BattleGUI.armShown(ActionArm.Standard)) || (!u.usedMovement && BattleGUI.armShown(ActionArm.Movement)) || (u.minorsLeft > 0 && BattleGUI.armShown(ActionArm.Minor));
		bool movement = !u.usedMovement && (!someArmShown || BattleGUI.armShown(ActionArm.Movement));
		bool standard = !u.usedStandard && (!someArmShown || BattleGUI.armShown(ActionArm.Standard));
		bool minor = u.minorsLeft > 0 && (!someArmShown || BattleGUI.armShown(ActionArm.Minor));
		int totalTypes = (movement ? movementTypes.Length : 0) + (standard ? standardTypes.Length : 0) + (minor ? minorTypes.Length : 0);
		int currentType = actionInd;

		if (currentType >= totalTypes) return;//currentType = totalTypes-1;
	//	currentType += by;
	//	while (currentType < 0) currentType += totalTypes;
	//	currentType %= totalTypes;
		if (movement) {
			if (currentType < movementTypes.Length) {
				selectMovementType(movementTypes[currentType]);
				return;
			}
			else currentType -= movementTypes.Length;
		}
		if (standard) {
			if (currentType < standardTypes.Length) {
				selectStandardType(standardTypes[currentType]);
				return;
			}
			else currentType -= standardTypes.Length;
		}
		if (minor) {
			if (currentType < minorTypes.Length) {
				selectMinorType(minorTypes[currentType]);
				return;
			}
			else currentType -= minorTypes.Length;
		}
	}

	public static void selectNextOfType() {
		if (mapGenerator.getCurrentUnit() != mapGenerator.selectedUnit || mapGenerator.getCurrentUnit()==null) return;
		if (selectedMovement && !mapGenerator.getCurrentUnit().usedStandard) clickStandard();
		else if ((selectedStandard || selectedMovement) && mapGenerator.getCurrentUnit().minorsLeft>0) clickMinor();
		else clickMovement();
		return;
		if (selectedStandard) {
			StandardType[] standards = mapGenerator.getCurrentUnit().getStandardTypes();
			int index = System.Array.IndexOf(standards,selectedStandardType);
			index++;
			if (index >= standards.Length) index = 0;
			selectStandard(standards[index]);
		}
		else if (selectedMovement) {
			MovementType[] movements = mapGenerator.getCurrentUnit().getMovementTypes();
			int index = System.Array.IndexOf(movements,selectedMovementType);
			index++;
			if (index >= movements.Length) index = 0;
			selectMovement(movements[index]);
		}
		else if (selectedMinor) {
			MinorType[] minors = mapGenerator.getCurrentUnit().getMinorTypes();
			int index = System.Array.IndexOf(minors,selectedMinorType);
			index++;
			if (index >= minors.Length) index = 0;
			selectMinor(minors[index]);
		}
	}

	public static void selectPreviousOfType() {
		if (mapGenerator.getCurrentUnit() != mapGenerator.selectedUnit || mapGenerator.getCurrentUnit()==null) return;
		if (selectedMinor && !mapGenerator.getCurrentUnit().usedStandard) clickStandard();
		else if ((selectedStandard || selectedMinor) && !mapGenerator.getCurrentUnit().usedMovement) clickMovement();
		else clickMinor();
		return;
		if (selectedStandard) {
			StandardType[] standards = mapGenerator.getCurrentUnit().getStandardTypes();
			int index = System.Array.IndexOf(standards,selectedStandardType);
			index--;
			if (index >= standards.Length-1) index = 0;
			if (index < 0) index = standards.Length-1;
			selectStandard(standards[index]);
		}
		else if (selectedMovement) {
			MovementType[] movements = mapGenerator.getCurrentUnit().getMovementTypes();
			int index = System.Array.IndexOf(movements,selectedMovementType);
			index--;
			if (index >= movements.Length-1) index = 0;
			if (index < 0) index = movements.Length-1;
			selectMovement(movements[index]);
		}
		else if (selectedMinor) {
			MinorType[] minors = mapGenerator.getCurrentUnit().getMinorTypes();
			int index = System.Array.IndexOf(minors,selectedMinorType);
			index--;
			if (index >= minors.Length-1) index = 0;
			if (index < 0) index = minors.Length-1;
			selectMinor(minors[index]);
		}
	}

	public static void useTemperedHands() {
		mapGenerator.getCurrentUnit().useTemperedHands(temperedHandsMod);
		temperedHandsMod = 0;
		BattleGUI.resetTemperedHands();
	}

	public static void selectMinor(MinorType minorType) {
		if (!selectedMinor) {
			clickMinor();
		//	selectedMinorType = minorType;
		}
		else if (minorType == selectedMinorType) selectedMinorType = MinorType.None;
		else selectedMinorType = minorType;
		selectMinorType(selectedMinorType);
	}

	public static void selectMovement(MovementType movementType) {
		if (!selectedMovement) {
			clickMovement();
			selectedMovementType = movementType;
		}
		else if (movementType == selectedMovementType) selectedMovementType = MovementType.None;
		else selectedMovementType = movementType;
		selectMovementType(selectedMovementType);
	}

	public static void selectMove() {
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

	public static void selectStandard(StandardType standardType) {
		if (!selectedStandard) {
			clickStandard();
			selectedStandardType = standardType;
		}
		else if (standardType == selectedStandardType) selectedStandardType = StandardType.None;
		else selectedStandardType = standardType;
		selectStandardType(selectedStandardType);
	}
	//	
	public static void selectAttack() {
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

	
	static void selectUnit(Unit player) {
		if (player != mapGenerator.selectedUnit) {
			mapGenerator.deselectAllUnits();
			mapGenerator.selectUnit(player, false);
			if (player.transform.parent == mapGenerator.playerTransform || player.transform.parent == mapGenerator.enemyTransform)
				mapGenerator.moveCameraToSelected(false);
		}
	}

	const float turnOrderSectionHeight = 30.0f;
	const float turnOrderTableX = 15.0f;
	const float turnOrderNameWidth = clipboardBodyWidth - turnOrderTableX * 2 - turnOrderSectionHeight * 2;


	//------------------------------------------------------------ onGUI stuff
	static float t = 0;
	static int dir = 1;
	public static void doGUI() {
		bool interact = true;// !escapeMenuOpen && !(mapGenerator != null && mapGenerator.getCurrentUnit()!= null && mapGenerator.getCurrentUnit().doingTemperedHands);
		float speed = 1.0f/3.0f;
		t += Time.deltaTime * speed * dir;
		Color start = Color.cyan;
		Color end = Color.black;
		float max = 0.9f;
		float min = 0.35f;
		if (t > max) {
			dir = -1;
			t = max;
		}
		if (t < min) {
			dir = 1;
			t = min;
		}			//	Debug.Log("OnGUI");
			
		if (first) {
			first = false;
			getSelectedButtonStyle();
			getSelectedSubMenuTurnStyle();
			getNonSelectedButtonStyle();
			getConfirmButtonStyle();
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
                GUIContent content = new GUIContent(u.characterSheet.characterSheet.personalInformation.getCharacterName().fullName());
				Vector2 size = st.CalcSize(content);
				float height = st.CalcHeight(content, width);
				GUI.Label(new Rect(Screen.width - mapGenerator.selectionWidth, y, width, height + 0 * size.y), content, st);
				y += mapGenerator.spriteSeparator + mapGenerator.spriteSize;
			}

			GUI.EndScrollView();
			if (mapGenerator.selectedSelectionObject) {
				Vector3 pos = Camera.main.WorldToScreenPoint(mapGenerator.selectedSelectionObject.transform.position);
				Unit u = mapGenerator.selectedSelectionObject.GetComponent<Unit>();
                GUIContent content = new GUIContent(u.characterSheet.characterSheet.personalInformation.getCharacterName().fullName());
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
			if (mapGenerator.playerTransform.childCount > 3) {
				if (GUI.Button(beginButtonRect(), "Engage", getBeginButtonStyle())) {
					mapGenerator.enterPriority();
					foreach (Unit u in mapGenerator.priorityOrder) {
						u.setRotationToMostInterestingTile();
					}
                    BattleGUI.toggleUI();
				}
			}
		}

		// Game GUI
		else {
			// BattleGUI;
			return;
			float consoleRight = 160.0f;
			if (!clipboardUp) consoleRight += 45.0f;
			float consoleLeft = 200.0f;
			if (!mapGenerator.selectedUnit) consoleLeft = 0.0f;
			else if (mapGenerator.selectedUnit != mapGenerator.getCurrentUnit() || mapGenerator.selectedUnits.Count != 0) consoleLeft = 150.0f;
		//	log.doGUI(consoleLeft, consoleRight);
			Rect clipBoardRect = clipBoardBodyRect();
			if (GUI.Button(getTabButtonRect(Tab.T), "T", getTabButtonStyle()) && interact) {
				clipboardTab = Tab.T;
			}
			if (GUI.Button(getTabButtonRect(Tab.R), "R", getTabButtonStyle()) && interact) {
				clipboardTab = Tab.R;
			}
			GUI.DrawTexture(clipBoardRect, clipBoardBodyTexture);
			if (GUI.Button(clipBoardClipRect(), "", getClipBoardClipStyle()) && interact) {
				clipboardUp = !clipboardUp;
			}
			if (clipboardTab == Tab.R) {
				float y = clipBoardRect.y + 10.0f;
				GUIStyle titleStyle = getTitleTextStyle();
				GUIContent turnOrder = new GUIContent("Missions");
				Vector2 turnOrderSize = titleStyle.CalcSize(turnOrder);
				GUI.Label(new Rect(clipBoardRect.x + (clipBoardRect.width - turnOrderSize.x)/2.0f, y , turnOrderSize.x, turnOrderSize.y), turnOrder, titleStyle);
				float x = clipBoardRect.x;
				y += turnOrderSize.y;
				float widths = clipBoardRect.width / 3.0f;
				GUIStyle missionStyle = getMissionTypeSelectStyle();
				GUIContent primaryContent = new GUIContent("Primary");
				GUIContent secondaryContent = new GUIContent("Secondary");
				GUIContent optionalContent = new GUIContent("Optional");
				float height = missionStyle.CalcSize(primaryContent).y;
				if (GUI.Button(new Rect(x, y, widths, height), primaryContent, missionStyle) && interact) {
					openMission = Mission.Primary;
				}
				x += widths;
				if (GUI.Button(new Rect(x, y, widths, height), secondaryContent, missionStyle) && interact) {
					openMission = Mission.Secondary;
				}
				x += widths;
				if (GUI.Button(new Rect(x, y, widths, height), optionalContent, missionStyle) && interact) {
					openMission = Mission.Optional;
				}
				x = clipBoardRect.x + 10.0f;
				y += height;
				GUIContent objectives = new GUIContent(openMission.ToString());
				Vector2 objectivesSize = titleStyle.CalcSize(objectives);
				GUI.Label(new Rect(x + (clipBoardRect.width - objectivesSize.x)/2.0f, y, objectivesSize.x, objectivesSize.y), objectives, titleStyle);
				
				y += objectivesSize.y;
//				float y = missionTopHeight + objectivesSize.y + 20.0f;
//				float x = paperDollFullWidth + missionTabWidth + 10.0f;
				float toggleHeight = 20.0f;
				float toggleWidth = 200.0f;
				//			GUI.enabled = false;
				GUI.Toggle(new Rect(x, y, toggleWidth, toggleHeight), (openMission == Mission.Optional ? true : false), "Main Objective");
				x += 20.0f;
				y += toggleHeight;
				GUI.Toggle(new Rect(x, y, toggleWidth, toggleHeight), true, (openMission == Mission.Primary ? "How you do it" : (openMission == Mission.Secondary ? "Destroy Enemy" : "Enjoy the view")));
				y += toggleHeight;
				GUI.Toggle(new Rect(x, y, toggleWidth, toggleHeight), (openMission != Mission.Secondary ? true : false), (openMission == Mission.Primary ? "This too" : (openMission == Mission.Secondary ? "Reinforcements" : "Daydream")));
				y += toggleHeight;
				GUI.Toggle(new Rect(x, y, toggleWidth, toggleHeight), (openMission != Mission.Primary ? true : false), (openMission == Mission.Primary ? "And this as well" : (openMission == Mission.Secondary ? "Conquer" : "Eat Snacks")));
				y += toggleHeight;
				if (openMission == Mission.Optional) {
					GUI.Toggle (new Rect(x, y, toggleWidth, toggleHeight), true, "Nap Time!");
					y += toggleHeight;
				}

			}
			else if (clipboardTab == Tab.T) {
				float y = clipBoardRect.y + 10.0f;
				GUIStyle titleStyle = getTitleTextStyle();
				GUIContent turnOrder = new GUIContent("Turn Order");
				Vector2 turnOrderSize = titleStyle.CalcSize(turnOrder);
				GUI.Label(new Rect(clipBoardRect.x + (clipBoardRect.width - turnOrderSize.x)/2.0f, y , turnOrderSize.x, turnOrderSize.y), turnOrder, titleStyle);
			
				y += turnOrderSize.y;
				int numPlayers = mapGenerator.priorityOrder.Count;
				int currentPlayer = mapGenerator.currentUnit;
				if (currentPlayer < 0) currentPlayer = 0;

				float height =  (numPlayers) * (turnOrderSectionHeight - 1.0f) + 1.0f + 5.0f;
				float add = -5.0f;
				if (height < clipBoardRect.height - (y - clipBoardRect.y)) {
					add = 0.0f;
				}
				GUIStyle st = getPlayerInfoStyle();
				st.wordWrap = false;
				float x = clipBoardRect.x + turnOrderTableX + add;
				GUIContent num = new GUIContent("Pos");
				Vector2 numSize = st.CalcSize(num);
				GUI.Label(new Rect(x + (turnOrderSectionHeight - numSize.x)/2.0f, y + (turnOrderSectionHeight - numSize.y)/2.0f, numSize.x, numSize.y), num, st);
				x += turnOrderSectionHeight - 1.0f;
				GUIContent name = new GUIContent("Name");
				Vector2 nameSize = st.CalcSize(name);
				GUI.Label(new Rect(x + (turnOrderNameWidth - nameSize.x)/2.0f, y + (turnOrderSectionHeight - nameSize.y)/2.0f, nameSize.x, nameSize.y), name, st);
				x += turnOrderNameWidth - 1.0f;
				GUIContent initiative = new GUIContent("Roll");
				Vector2 initiativeSize = st.CalcSize(initiative);
				GUI.Label (new Rect(x + (turnOrderSectionHeight - initiativeSize.x)/2.0f, y + (turnOrderSectionHeight - initiativeSize.y)/2.0f, initiativeSize.x, initiativeSize.y), initiative, st);
				y+=turnOrderSectionHeight;
				turnOrderScrollPos = GUI.BeginScrollView(new Rect(clipBoardRect.x, y, clipBoardRect.width - 2.0f, clipBoardRect.height - (y - clipBoardRect.y)), turnOrderScrollPos, new Rect(clipBoardRect.x, y, clipBoardRect.width - 16.0f - 2.0f, height));

				for (int n=0;n<numPlayers;n++) {
					int playerNum = (n + currentPlayer) % numPlayers;
					Unit player = mapGenerator.priorityOrder[playerNum];
					if (player == mapGenerator.selectedUnit) {
						st.normal.textColor = Color.Lerp (start, end, t);
						st.fontStyle = FontStyle.Bold;
					}
					x = clipBoardRect.x + turnOrderTableX + add;
					Rect r = new Rect(x, y, turnOrderSectionHeight, turnOrderSectionHeight);
					//	Rect r2 = new Rect(x + (turnOrderSectionHeight
					//	GUI.DrawTexture(r, (player.team == 0 ? getTurnOrderSectionBackgroundTexture() : getTurnOrderSectionBackgroundTextureEnemy()));
					if (GUI.Button(r, new GUIContent("","" + playerNum), getTurnOrderSectionStyle(player)) && interact) {
						selectUnit(player);
					}

					num = new GUIContent("" + (playerNum + 1));
					numSize = st.CalcSize(num);
					GUI.Label(new Rect(x + (turnOrderSectionHeight - numSize.x)/2.0f, y + (turnOrderSectionHeight - numSize.y)/2.0f, numSize.x, numSize.y), num, getPlayerInfoStyle());
					x += turnOrderSectionHeight - 1.0f;
					r = new Rect(x, y, turnOrderNameWidth, turnOrderSectionHeight);
					//	GUI.DrawTexture(r, (player.team == 0 ? getTurnOrderNameBackgroundTexture() : getTurnOrderNameBackgroundTextureEnemy()));
					if (GUI.Button(r, new GUIContent("","" + playerNum), getTurnOrderNameStyle(player)) && interact) {
						selectUnit(player);
					}

					name = new GUIContent(player.characterSheet.personalInfo.getCharacterName().fullName());
					nameSize = st.CalcSize(name);
					GUI.Label(new Rect(x + 3.0f, y + (turnOrderSectionHeight - nameSize.y)/2.0f, Mathf.Min(nameSize.x, turnOrderNameWidth - 4.0f), nameSize.y), name, getPlayerInfoStyle());
					x += turnOrderNameWidth - 1.0f;
					r = new Rect(x, y, turnOrderSectionHeight, turnOrderSectionHeight);
					//	GUI.DrawTexture(r, (player.team == 0 ? getTurnOrderSectionBackgroundTexture() : getTurnOrderSectionBackgroundTextureEnemy()));
					if (GUI.Button(r, new GUIContent("","" + playerNum), getTurnOrderSectionStyle(player)) && interact) {
						selectUnit(player);
					}
					initiative = new GUIContent(player.getInitiative() + "");
					initiativeSize = st.CalcSize(initiative);
					GUI.Label (new Rect(x + (turnOrderSectionHeight - initiativeSize.x)/2.0f, y + (turnOrderSectionHeight - initiativeSize.y)/2.0f, initiativeSize.x, initiativeSize.y), initiative, getPlayerInfoStyle());

					y += turnOrderSectionHeight - 1.0f;
					if (player == mapGenerator.selectedUnit) {
						st.normal.textColor = Color.white;
						st.fontStyle = FontStyle.Normal;
					}
				}

				GUI.EndScrollView();
			}
			if (mapGenerator.getCurrentUnit().doingTemperedHands) {
				Rect r = temperedHandsRect();
				Vector2 buttSize = new Vector2(150.0f, 50.0f);
				Vector2 arrowSize = new Vector2(20.0f, 20.0f);
				float ins = 20.0f;
				float arrowsY = r.y + 30.0f;
				GUI.enabled = temperedHandsMod > -mapGenerator.getCurrentUnit().characterSheet.combatScores.getTechniqueMod();
				if (GUI.Button(new Rect(r.x + ins, arrowsY, arrowSize.x, arrowSize.y), "<")) temperedHandsMod--;
				GUI.enabled = temperedHandsMod < mapGenerator.getCurrentUnit().characterSheet.combatScores.getTechniqueMod();
				if (GUI.Button(new Rect(r.x + r.width - arrowSize.x - ins, arrowsY, arrowSize.x, arrowSize.y), ">")) temperedHandsMod++;
				GUI.enabled = true;
				float eachWidth = r.width/2.0f - arrowSize.x - ins;
				GUIStyle st = getCenteredTextStyle();
				GUIContent hitContent = new GUIContent("Hit:\n" + (-temperedHandsMod));
				GUIContent damageContent = new GUIContent("Damage:\n" + temperedHandsMod);
				float hitHeight = st.CalcHeight(hitContent, eachWidth);
				float damageHeight = st.CalcHeight(damageContent, eachWidth);
				GUI.Label(new Rect(r.x + ins + arrowSize.x, arrowsY + (arrowSize.y - hitHeight)/2.0f, eachWidth, hitHeight), hitContent, st);
				GUI.Label(new Rect(r.x + ins + arrowSize.x + eachWidth, arrowsY + (arrowSize.y - damageHeight)/2.0f, eachWidth, damageHeight), damageContent, st);

				GUI.Box(r,"");
				if (GUI.Button(new Rect(r.x + ins, r.y + r.height - buttSize.y - 5.0f, buttSize.x, buttSize.y), "Cancel")) {
			//		mapGenerator.getCurrentUnit().doingTemperedHands = false;
					temperedHandsMod = 0;
				}
				if (GUI.Button(new Rect(r.x + r.width - buttSize.x - ins, r.y + r.height - buttSize.y - 5.0f, buttSize.x, buttSize.y), "Confirm")) {
					mapGenerator.getCurrentUnit().useTemperedHands(temperedHandsMod);
					temperedHandsMod = 0;
				}
			}
		}
		if (mapGenerator.currentUnit >= 0) {
			if (GUI.Button(waitButtonAlwaysRect(), "End Turn (Q)", getNonSelectedButtonStyle()) && !mapGenerator.performingAction() && !mapGenerator.currentUnitIsAI() && interact) {
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
				GUI.DrawTexture(actionRect(), actionTexture3);
				bool enabled = !p.usedMovement;
				if (selectedMovement && p.usedMovement) {
					deselectMovement();
			//		selectedMovement = false;
			//		selectedMovementType = MovementType.None;
			//		mapGenerator.resetRanges();
				}
				if(GUI.Button(moveButtonRect(), "", (p.usedMovement ? getDisabledButtonStyle("movement") : (selectedMovement ? getSelectedButtonStyle("movement") : getNonSelectedButtonStyle("movement")))) && interact) {
					//	Debug.Log("Move Player!");
					if (enabled)
						clickMovement();

				}
				//		}
				enabled = !p.usedStandard && !p.isProne();
				if (selectedStandard && p.usedStandard) {
					deselectStandard();
//					selectedStandard = false;
//					selectedStandardType = StandardType.None;
				}
				//	if (p.attackEnemy!=null && !p.moving && !p.attacking) {
				if (GUI.Button(attackButtonRect(), "", (p.usedStandard || p.isProne() ? getDisabledButtonStyle("standard") :(selectedStandard ? getSelectedButtonStyle("standard") : getNonSelectedButtonStyle("standard")))) && interact) {
					if (enabled)
						clickStandard();

				}
				enabled = p.minorsLeft > 0;//!p.usedMinor1 || !p.usedMinor2;
				if (selectedMinor && p.minorsLeft==0) {
		//			if (selectedMinorType == MinorType.Loot) previouslyOpenTab = Tab.B;
					deselectMinor();//selectedMinor = false;
				}
				if (GUI.Button(minorButtonRect(), "", (p.minorsLeft <= 0 ? getDisabledButtonStyle("minor") : (selectedMinor ? getSelectedButtonStyle("minor") : getNonSelectedButtonStyle("minor")))) && !mapGenerator.performingAction() && !mapGenerator.currentUnitIsAI() && interact) {
					if (enabled)
						clickMinor();
				}
				GUI.enabled = true;
			//	if (GUI.Button(waitButtonRect(), "End Turn", getNonSelectedButtonStyle()) && !mapGenerator.performingAction() && !mapGenerator.currentUnitIsAI()) {
			//		clickWait();
			//	}
//				GUI.DrawTexture(actionBarRect(), hotkeysBackTexture);
				if (selectedMovement) {
					numberActions = mapGenerator.getCurrentUnit().getMovementTypes().Count();
				}
				else if (selectedStandard) {
					numberActions = mapGenerator.getCurrentUnit().getStandardTypes().Count();
				}
				else if (selectedMinor) {
					numberActions = mapGenerator.getCurrentUnit().getMinorTypes().Count();
				}
				else numberActions = 2;
				for (int n=-1; n <= Mathf.Max(2, numberActions); n++) {
					GUI.DrawTexture(actionBarRect(n), getHotKeysBackTexture(n));
				}
				if (selectedMovement) {
					MovementType[] types = mapGenerator.getCurrentUnit().getMovementTypes();
					for (int n=0;n<types.Length;n++) {
						string typeName = Unit.getNameOfMovementType(types[n]);
						GUI.enabled = movementEnabled(types[n]);
						if (GUI.Button(actionIconRect(n), (n < 9 ? (n+1) + "" : (n==9?"0":"")), (selectedMovementType == types[n] ? getSelectedActionStyle("Temp " + typeName) : getNonSelectedActionStyle("Temp " + typeName))) && !mapGenerator.performingAction() && !mapGenerator.currentUnitIsAI() && interact) {
						//	if (types[n] != MovementType.Cancel) selectedMovementType = types[n];
							selectMovement(types[n]);
						}
						GUI.enabled = true;
					}

					if ((selectedMovementType == MovementType.BackStep || selectedMovementType == MovementType.Move) && mapGenerator.getCurrentUnit().currentPath.Count > 1) {
						if (GUI.Button(confirmButtonRect(), "Confirm", getConfirmButtonStyle()) && !mapGenerator.performingAction() && !mapGenerator.currentUnitIsAI() && interact) {
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
						StandardType type = types[n];
						string typeName = Unit.getNameOfStandardType(type);
						GUI.enabled = standardEnabled(type);//types[n] != MovementType.BackStep || mapGenerator.getCurrentUnit().moveDistLeft == mapGenerator.getCurrentUnit().maxMoveDist;
						if (GUI.Button(actionIconRect(n), (n < 9 ? (n+1) + "" : (n==9?"0":"")), (selectedStandardType == types[n] ? getSelectedActionStyle("Temp " + typeName) : getNonSelectedActionStyle("Temp " + typeName))) && !mapGenerator.performingAction() && !mapGenerator.currentUnitIsAI() && interact) {//(selectedMovementType == types[n] ? getSelectedSubMenuTurnStyle() : getNonSelectedSubMenuTurnStyle()))) {
							//	if (types[n] != MovementType.Cancel) selectedMovementType = types[n];
							selectStandard(types[n]);
						}
						GUI.enabled = true;
					}

					if (((selectedStandardType == StandardType.Attack || selectedStandardType == StandardType.OverClock || selectedStandardType == StandardType.Throw || selectedStandardType == StandardType.Intimidate) && mapGenerator.getCurrentUnit().attackEnemy != null) || (selectedStandardType==StandardType.Place_Turret && mapGenerator.turretBeingPlaced != null) || (selectedStandardType==StandardType.Lay_Trap && mapGenerator.currentTrap.Count>0) && interact) {
						if (GUI.Button(confirmButtonRect(), "Confirm", getConfirmButtonStyle()) && !mapGenerator.performingAction() && !mapGenerator.currentUnitIsAI()) {
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
						string typeName = Unit.getNameOfMinorType(types[n]);
						GUI.enabled = minorEnabled(types[n]);//types[n] != MovementType.BackStep || mapGenerator.getCurrentUnit().moveDistLeft == mapGenerator.getCurrentUnit().maxMoveDist;
						if (GUI.Button(actionIconRect(n), (n < 9 ? (n+1) + "" : (n==9?"0":"")), (selectedMinorType == types[n] ? getSelectedActionStyle("Temp " + typeName) : getNonSelectedActionStyle("Temp " + typeName))) && !mapGenerator.performingAction() && !mapGenerator.currentUnitIsAI() && interact) {//(selectedMovementType == types[n] ? getSelectedSubMenuTurnStyle() : getNonSelectedSubMenuTurnStyle()))) {
							//	if (types[n] != MovementType.Cancel) selectedMovementType = types[n];
							selectMinor(types[n]);
						}
						GUI.enabled = true;
					}
					
					if ((((selectedMinorType == MinorType.Mark) && mapGenerator.getCurrentUnit().attackEnemy != null) || (selectedMinorType == MinorType.Escape && mapGenerator.getCurrentUnit().currentPath.Count > 1) || selectedMinorType == MinorType.Stealth)  && interact) {
						if (GUI.Button(confirmButtonRect(), "Confirm", getConfirmButtonStyle()) && !mapGenerator.performingAction() && !mapGenerator.currentUnitIsAI()) {
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
				if (GUI.Button(new Rect(0.0f, Screen.height - notTurnMoveRangeSize.y, notTurnMoveRangeSize.x,notTurnMoveRangeSize.y), "Show Movement", (showMovement ? getSelectedButtonStyle() : getNonSelectedButtonStyle())) && interact) {
					showMovement = !showMovement;
					mapGenerator.resetRanges();
				}
				if (GUI.Button(new Rect(0.0f, Screen.height - notTurnMoveRangeSize.y*2 + 1, notTurnMoveRangeSize.x,notTurnMoveRangeSize.y), "Show Attack Range", (showAttack ? getSelectedButtonStyle() : getNonSelectedButtonStyle())) && interact) {
					showAttack = !showAttack;
					mapGenerator.resetRanges();
				}
			}
			if (selectedStandard && selectedStandardType==StandardType.Place_Turret) {
				List<Turret> turrets = mapGenerator.getCurrentUnit().getTurrets();
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
					if (GUI.Button(r, "", (selectedTurretIndex==n ? getSelectedButtonTurretStyle() : getNonSelectedButtonTurretStyle())) && interact) {
						selectedTurretIndex = n;
					}
					float x = 5.0f + r.x;
					Vector2 size = turrets[n].getSize();
					size.x *= UnitGUI.inventoryCellSize;
					size.y *= UnitGUI.inventoryCellSize;
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
				List<Turret> turrets = mapGenerator.getCurrentUnit().getTurrets();
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
					if (GUI.Button(r, "", (selectedTurretIndex==n ? getSelectedButtonTurretStyle() : getNonSelectedButtonTurretStyle())) && interact) {
						selectedTurretIndex = n;
					}
					float x = 5.0f + r.x;
					Vector2 size = turrets[n].getSize();
					size.x *= UnitGUI.inventoryCellSize;
					size.y *= UnitGUI.inventoryCellSize;
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
				List<Trap> traps = mapGenerator.getCurrentUnit().getTraps();
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
					if (GUI.Button(r, "", (selectedTrapIndex==n ? getSelectedButtonTurretStyle() : getNonSelectedButtonTurretStyle())) && interact) {
						selectedTrapIndex = n;
					}
					float x = 5.0f + r.x;
					Vector2 size = trap.getSize();
					size.x *= UnitGUI.inventoryCellSize;
					size.y *= UnitGUI.inventoryCellSize;
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

				if (GUI.Button(trapOkButton(0), "Cancel", getTrapSelectButtonsStyle()) && interact) {
					selectStandard(StandardType.Lay_Trap);
				}
				if (GUI.Button(trapOkButton(1), "Select", getTrapSelectButtonsStyle()) && interact) {
					selectCurrentTrap();
					mapGenerator.resetRanges();
				}
			}


		}
		// Show Win/Lose screen if the game is over
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
		// Show Escape/Pause Menu options
		if (false && (escapeMenuOpen || mapGenerator.gameState != GameState.Playing)) {
			if (GUI.Button(getMenuRect(0, escapeMenuOpen), "Back to Base")) {
				Application.LoadLevel(2);
			}
		//	y += height + 10.0f;
			if (GUI.Button(getMenuRect(1, escapeMenuOpen), "Quit")) {
				Application.Quit();
			}
		//	y += height + 10.0f;
			if (escapeMenuOpen && GUI.Button(getMenuRect(2, escapeMenuOpen), "Cancel")) {
				escapeMenuOpen = false;
			}
		}
	//	Debug.Log("OnGUIEnd");
		
		string tt = GUI.tooltip;
		if (tt != null && tt!="") {
			int num = int.Parse(tt);
			if (hovering != null) hovering.removeHovering();
			hovering = mapGenerator.priorityOrder[num];
			hovering.setHovering();
		}
		else if (hovering != null) {
			hovering.removeHovering();
		}
	}

	public static Rect getMenuRect(int num, bool escape=false) {
		float width = 150.0f;
		float height = 50.0f;
		float x = Screen.width/2.0f - width/2.0f;
		float y = Screen.height/2.0f - ((height + 10.0f) * (escape ? 3 : 2))/2.0f;
		return new Rect(x, y + num * (height + 10.0f), width, height);
	}

	static GUIStyle centeredTextStyle = null;
	public static GUIStyle getCenteredTextStyle() {
		if (centeredTextStyle == null) {
			centeredTextStyle = new GUIStyle("Label");
			centeredTextStyle.alignment = TextAnchor.MiddleCenter;
		}
		return centeredTextStyle;
	}

	static GUIStyle turretPartStyle = null;
	public static GUIStyle getTurretPartStyle() {
		if (turretPartStyle == null) {
			turretPartStyle = new GUIStyle("Label");
			turretPartStyle.active.textColor = turretPartStyle.hover.textColor = turretPartStyle.normal.textColor = Color.white;
			turretPartStyle.padding = new RectOffset(0, 0, 0, 0);
			turretPartStyle.fontSize = 13;
		}
		return turretPartStyle;
	}

//	public static Trap selectedTrap = null;

	public static void selectCurrentTrap() {
//		selectedTrap = getCurrentTrap();
	}

	public static void showCurrentTrap() {
		List<Trap> traps = mapGenerator.getCurrentUnit().getTraps();
		float height = traps.Count * turretSelectSize.y - traps.Count + 1;
		Rect r = trapTypeRect(selectedTrapIndex);
		Rect tR = trapTypesScrollRect();
		float y = tR.y;
		trapsScrollPosition.y = Mathf.Max(r.y - y + r.height - tR.height, Mathf.Min(trapsScrollPosition.y, r.y - y));
	}

	public static Trap getCurrentTrap() {
		return getTrap(selectedTrapIndex);
	}

	public static Trap getTrap(int n) {
		List<Trap> traps = mapGenerator.getCurrentUnit().getTraps();
		if (n >= traps.Count || n < 0) return null;
		return traps[n];
	}

	public static Turret getCurrentTurret() {
		return getTurret(selectedTurretIndex);
	}

	public static Turret getTurret(int n) {
		List<Turret> turrets = mapGenerator.getCurrentUnit().getTurrets();
		if (n >= turrets.Count || n<0) return null;
		return turrets[n];
	}

	public static void showCurrentTurret() {
		List<Turret> turrets = mapGenerator.getCurrentUnit().getTurrets();
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

	static void deselectMinor() {
		if (looting) {
			looting = false;
			UnitGUI.inventoryOpen = inventoryWasOpenLoot;
//			openTab = previouslyOpenTab;
		}
		selectedMinor = false;

	}

	static void deselectMovement() {
		//		selectedMovementType = MovementType.None;
		selectedMovement = false;
	}

	static void deselectStandard() {
		selectedStandard = false;
//		selectedStandardType = StandardType.None;

	}

	public static void deselectCurrentAction() {
		if (selectedStandard) {
			deselectStandardType(selectedStandardType);
			if (showingConfirm) {
				BattleGUI.setConfirmButtonShown(ConfirmButton.Standard, false);
				showingConfirm = false;
			}
		}
		else if (selectedMovement) {
			deselectMovementType(selectedMovementType);
			if (showingConfirm) {
				BattleGUI.setConfirmButtonShown(ConfirmButton.Movement, false);
				showingConfirm = false;
			}
		}
		else if (selectedMinor) {
			deselectMinorType(selectedMinorType);
			if (showingConfirm) {
				BattleGUI.setConfirmButtonShown(ConfirmButton.Minor, false);
				showingConfirm = false;
			}
		}
	}
	public static void deselectMinorType(MinorType t) {
		selectedMinor = false;
		selectedMinorType = MinorType.None;
		BattleGUI.selectMinorType(t, false);
		switch (t) {
		case MinorType.TemperedHands:
			temperedHandsMod = 0;
			BattleGUI.hideClassFeatureCanvas(ClassFeatureCanvas.TemperedHands);
			break;
		case MinorType.OneOfMany:
			BattleGUI.hideClassFeatureCanvas(ClassFeatureCanvas.OneOfMany);
			break;
		case MinorType.Mark:
		case MinorType.Escape:
			mapGenerator.resetRanges();
			break;
		case MinorType.Invoke:
			if (mapGenerator.getCurrentUnit().primalControlUnit != null)
				mapGenerator.getCurrentUnit().setPrimalControl(0);
			mapGenerator.resetRanges();
			break;
		default:
			break;
		}
		if (mapGenerator.selectedUnit.attackEnemy) {
			mapGenerator.selectedUnit.attackEnemy.deselect();
			mapGenerator.resetAttack();
		}
	}
	public static void deselectStandardType(StandardType t) {
		BattleGUI.selectStandardType(t, false);
		selectedStandard = false;
		selectedStandardType = StandardType.None;
		switch (t) {
		case StandardType.Attack:
		case StandardType.OverClock:
		case StandardType.Throw:
		case StandardType.InstillParanoia:
			if (mapGenerator.selectedUnit.attackEnemy) {
				mapGenerator.selectedUnit.attackEnemy.deselect();
				mapGenerator.resetAttack();
			}
			break;
		case StandardType.Intimidate:
			if (mapGenerator.getCurrentUnit().primalControlUnit != null)
				mapGenerator.getCurrentUnit().setPrimalControl(0);
			if (mapGenerator.selectedUnit.attackEnemy) {
				mapGenerator.selectedUnit.attackEnemy.deselect();
				mapGenerator.resetAttack();
			}
			break;
		case StandardType.Lay_Trap:
			selectedTrap = null;
			BattleGUI.hideTurretSelect();
			break;
		case StandardType.Place_Turret:
			selectedTurretIndex = 0;
			BattleGUI.hideTurretSelect();
			break;
		default:
			break;
		}
		mapGenerator.resetRanges();
	}
	public static void deselectMovementType(MovementType t) {
		selectedMovement = false;
		selectedMovementType = MovementType.None;
		BattleGUI.selectMovementType(t, false);
		switch (t) {
		default:
			mapGenerator.resetRanges();
			mapGenerator.removePlayerPath();
			break;
		}

	}

	public static bool looting = false;
	public static bool inventoryWasOpenLoot = false;
	public static bool oneOfManyConfirm = false;

	public static void selectMinorType(MinorType t) {
		if (t != selectedMinorType || (!selectedMinor && t != MinorType.None)) deselectCurrentAction();
		selectedMinor = t != MinorType.None;
		if (t == selectedMinorType) return;
		BattleGUI.selectMinorType(t);
		selectedMinorType = t;
		mapGenerator.resetCurrentKeysTile();
		Unit p = mapGenerator.selectedUnit;
		switch (t) {
		case MinorType.TemperedHands:
			temperedHandsMod = 0;
			BattleGUI.showClassFeatureCanvas(ClassFeatureCanvas.TemperedHands);
			break;
		case MinorType.OneOfMany:
			if (!mapGenerator.selectedUnit.hasOneOfManyHider()) {
				oneOfManyConfirm = false;
				BattleGUI.showClassFeatureCanvas(ClassFeatureCanvas.OneOfMany);
			}
			else {
				oneOfManyConfirm = true;
			}
			break;
		case MinorType.Loot:
			looting = true;
			inventoryWasOpenLoot = UnitGUI.inventoryOpen;
			UnitGUI.inventoryOpen = true;
			break;
		case MinorType.Mark:
		case MinorType.Invoke:
			mapGenerator.resetRanges();
			break;
		case MinorType.Stealth:
			break;
		case MinorType.Escape:
			p.selectMinorType(t);
			break;
		default:
			break;
		}
	}
	public static void selectMinorType(string t) {
		switch(t) {
		case "Loot":
			selectMinorType(MinorType.Loot);
			break;
		case "Stealth":
			selectMinorType(MinorType.Stealth);
			break;
		case "Escape":
			selectMinorType(MinorType.Escape);
			break;
		case "Invoke":
			selectMinorType(MinorType.Invoke);
			break;
		case "Mark":
			selectMinorType(MinorType.Mark);
			break;
		case "One Of Many":
			selectMinorType(MinorType.OneOfMany);
			break;
		case "Tempered Hands":
			selectMinorType(MinorType.TemperedHands);
			break;
		default:
			selectMinorType(MinorType.None);
			break;
		}
	}

	public static int selectedTrapIndex = 0;
	public static int selectedTurretIndex = 0;
	public static void selectStandardType(StandardType t) {
		if (t != selectedStandardType || (!selectedStandard && t != StandardType.None)) deselectCurrentAction();
		selectedStandard = t != StandardType.None;
		if (t == selectedStandardType) return;
		BattleGUI.selectStandardType(t);
		selectedStandardType = t;
		mapGenerator.resetCurrentKeysTile();
		Unit p = mapGenerator.selectedUnit;
		switch (t) {
		case StandardType.Attack:
		case StandardType.OverClock:
		case StandardType.Throw:
		case StandardType.Intimidate:
		case StandardType.InstillParanoia:
			mapGenerator.resetRanges();
			break;
		case StandardType.Place_Turret:
			BattleGUI.turnOnTurretSelect(mapGenerator.getCurrentUnit());
//			mapGenerator.resetRanges();
			selectedTrapTurret = false;
			break;
		case StandardType.Lay_Trap:
			BattleGUI.turnOnTrapSelect(mapGenerator.getCurrentUnit());
//			selectedTrapIndex = 0;
			trapsScrollPosition = new Vector2(0.0f, 0.0f);
//			selectedTrap = null;
			selectedTrapTurret = false;
//			mapGenerator.resetRanges();
			break;
		default:
			break;
		}
	}

	public static void selectStandardType(string t)
	{
		switch(t)
		{
		case "Attack":
			selectStandardType(StandardType.Attack);
			break;
		case "Over Clock":
			selectStandardType(StandardType.OverClock);
			break;
		case "Throw":
			selectStandardType(StandardType.Throw);
			break;
		case "Intimidate":
			selectStandardType(StandardType.Intimidate);
			break;
		case "Instill Paranoia":
			selectStandardType(StandardType.InstillParanoia);
			break;
		case "Place Turret":
			selectStandardType(StandardType.Place_Turret);
			break;
		case "Lay Trap":
			selectStandardType(StandardType.Lay_Trap);
			break;
		case "Inventory":
			selectStandardType(StandardType.Inventory);
			break;
		default:
			selectStandardType(StandardType.None);
			break;
		}
	}

	static void OnStart() {
		selectedStandardType = StandardType.None;
		selectedMovementType = MovementType.None;
		selectedMinorType = MinorType.None;
		selectedStandard = false;
		selectedMinor = false;
		selectedMovement = false;
	}
	
	public static void selectMovementType(MovementType t) {
		if (t != selectedMovementType || (!selectedMovement && t != MovementType.None)) deselectCurrentAction();
		selectedMovement = t != MovementType.None;
		if (t == selectedMovementType) return;
		BattleGUI.selectMovementType(t);
//		MovementType oldT = selectedMovementType;
		selectedMovementType = t;
		mapGenerator.resetCurrentKeysTile();
		switch (t) {
		case MovementType.BackStep:
		case MovementType.Move:

			mapGenerator.getCurrentUnit().selectMovementType(t);
			break;
		case MovementType.Recover:
			mapGenerator.getCurrentUnit().recover();
		//	mapGenerator.resetRanges();
		//	mapGenerator.removePlayerPath();
			break;
		default:
		//	mapGenerator.resetRanges();
		//	mapGenerator.removePlayerPath();
			break;
		}
	}

	public static void selectMovementType(string t)
	{
		switch(t)
		{
		case "Backstep":
	//		if(!selectedMovement) selectMove();
			selectMovementType(MovementType.BackStep);
			break;
		case "Recover":
	//		if(!selectedMovement) selectMove();
			selectMovementType(MovementType.Recover);
			break;
		case "Move":
	//		if(!selectedMovement) selectMove();
			selectMovementType(MovementType.Move);
			break;		
		default:
	//		if(!selectedMovement) selectMove();
			selectMovementType(MovementType.None);
			break;
		}
	}
	
}
