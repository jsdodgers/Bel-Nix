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
	static Vector2 turnOrderScrollPos = new Vector2(0.0f, 0.0f);

	public bool selectedMovement = false;
	public bool selectedStandard = false;
	public bool selectedMinor = false;
	public bool turretDirection = false;
	public MovementType selectedMovementType = MovementType.None;
	public StandardType selectedStandardType = StandardType.None;
	public MinorType selectedMinorType = MinorType.None;


	public Tab openTab = Tab.None;
	public Tab clipboardTab = Tab.T;
	public Mission openMission = Mission.Primary;

	static Texture2D actionTexture3;
	static Texture2D hotkeysBackTextureCenter;
	static Texture2D hotkeysBackTextureLeft;
	static Texture2D clipBoardBodyTexture;

	// Use this for initialization
	void Start () {
		clipboardTab = Tab.T;
		position = new Vector2(0.0f, 0.0f);
		//selectedButtonStyle = null;
		//nonSelectedButtonStyle = null;
		first = true;
		actionTexture3 = Resources.Load<Texture>("UI/SocketBase_v2") as Texture2D;

		hotkeysBackTextureLeft = Resources.Load<Texture>("UI/action-bar-left") as Texture2D;
		hotkeysBackTextureCenter = Resources.Load<Texture>("UI/action-bar-center") as Texture2D;
		clipBoardBodyTexture = Resources.Load<Texture>("UI/clipboard-body") as Texture2D;
	}

	Texture2D getHotKeysBackTexture(int n) {
		if (n == -1 || n == Mathf.Max(2, numberActions)) return hotkeysBackTextureLeft;
		return hotkeysBackTextureCenter;
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

	public Vector2 actionIconSize() {
		return new Vector2(40.0f, 40.0f);
	}

	public Rect actionIconRect(int n) {
		float y = actionBarTotalRect().y + (actionBarTotalSize().y - actionIconSize().y)/2.0f;
		float totesWidth = numberActions * actionBarTotalSize().y + (actionBarTotalSize().y - actionIconSize().y);
		float first = (Screen.width - totesWidth)/2.0f;
		float x = first + actionBarTotalSize().y*n + actionBarTotalSize().y - actionIconSize().x;
		return new Rect(x, y, actionIconSize().x, actionIconSize().y);
	}

	public Vector2 actionButtonsSize() {
//		return new Vector2(90.0f, 50.0f);
		return new Vector2(150.0f, 40.0f);
		return notTurnMoveRangeSize;
//		return new Vector2(90.0f, 40.0f);
	}

	public Vector2 actionButtonsTotalSize() {
		return new Vector2(236.0f, 200.0f);
	}

	public Rect rangeRect() {
		return new Rect(0.0f, Screen.height - notTurnMoveRangeSize.y*2 + 1, notTurnMoveRangeSize.x, notTurnMoveRangeSize.y*2-1);
	}

	public int numberActions;
	public Vector2 actionBarSectionSize() {
		return new Vector2(60.0f, 60.0f);
	}
	public Vector2 actionBarSideSize() {
		return new Vector2(30.0f, 60.0f);
	}
	public Vector2 actionBarTotalSize() {
		return new Vector2(actionBarSideSize().x * 2 + actionBarSectionSize().x * Mathf.Max(2, numberActions), actionBarSectionSize().y);
	}
	public Rect actionBarTotalRect() {
		return new Rect((Screen.width - actionBarTotalSize().x)/2.0f, Screen.height - Log.consoleHeight - actionBarTotalSize().y + 10.0f, actionBarTotalSize().x, actionBarTotalSize().y);
	}
	public Rect actionBarRect(int n) {
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

	public Rect actionRect() {
		float boxHeight = actionButtonsSize().y * 3 + (1) * 20.0f;
		return new Rect(-20.0f, Screen.height - actionButtonsTotalSize().y + 10.0f, actionButtonsTotalSize().x, actionButtonsTotalSize().y);
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
		return new Rect(actionRect().x + actionButtonsTotalSize().x - actionButtonsSize().x - 6.0f, actionRect().y + actionButtonsSize().y * 0 + 20.0f, actionButtonsSize().x, actionButtonsSize().y);
	}

	public Rect attackButtonRect() {
		return new Rect(actionRect().x + actionButtonsTotalSize().x - actionButtonsSize().x - 6.0f, actionRect().y + actionButtonsSize().y * 1 + 30.0f, actionButtonsSize().x, actionButtonsSize().y);
	}

	public Rect minorButtonRect() {	
		return new Rect(actionRect().x + actionButtonsTotalSize().x - actionButtonsSize().x - 6.0f, actionRect().y + actionButtonsSize().y * 2 + 40.0f, actionButtonsSize().x, actionButtonsSize().y);
	}
	
	public Rect waitButtonRect() {
		return new Rect(actionRect().x + actionButtonsTotalSize().x - actionButtonsSize().x, actionRect().y + actionButtonsSize().y * 3 + 5.0f, actionButtonsSize().x, actionButtonsSize().y);
	}

	public Rect waitButtonAlwaysRect() {
		return new Rect(Screen.width - actionButtonsSize().x, 0.0f, actionButtonsSize().x, actionButtonsSize().y);
	}

	Vector2 tabButtonSize = new Vector2(45.0f, 60.0f);
	public Rect getTabButtonRect(Tab t) {
		float x = 0.0f;
		float y = 0.0f;
		if (t == Tab.T || t == Tab.M) {
			x = clipBoardBodyRect().x - tabButtonSize.x;
			y = clipBoardBodyRect().y + 10.0f;
			if (t == Tab.M) {
				y += tabButtonSize.y + 5.0f;
			}
		}
		return new Rect(x, y, tabButtonSize.x, tabButtonSize.y);
	}
	public bool clipboardUp = true;
	public const float clipboardBodyWidth = 158.0f;
	public Vector2 clipboardBodySize() {
		return new Vector2(clipboardBodyWidth, (clipboardUp ? 250.0f : 160.0f));
	}
	public Vector2 clipboardClipSize() {
		return new Vector2(150.0f, 50.0f);
	}
	public Rect clipBoardBodyRect() {
		return new Rect(Screen.width - clipboardBodySize().x, Screen.height - clipboardBodySize().y, clipboardBodySize().x, clipboardBodySize().y);
	}
	public Rect clipBoardClipRect() {
		return new Rect(clipBoardBodyRect().x + (clipboardBodySize().x - clipboardClipSize().x)/2.0f, clipBoardBodyRect().y + 10.0f - clipboardClipSize().y, clipboardClipSize().x, clipboardClipSize().y);
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
			else {
				if (clipBoardBodyRect().Contains(mousePos)) return true;
				if (clipBoardClipRect().Contains(mousePos)) return true;
				if (getTabButtonRect(Tab.T).Contains(mousePos) || getTabButtonRect(Tab.M).Contains(mousePos)) return true;
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

	Dictionary<string, GUIStyle> selectedButtonStyles = null;
	Dictionary<string, GUIStyle> unselectedButtonStyles = null;
	Dictionary<string, GUIStyle> disabledButtonStyles = null;
	GUIStyle getSelectedButtonStyle(string name) {
		if (selectedButtonStyles == null) selectedButtonStyles = new Dictionary<string, GUIStyle>();
		if (!selectedButtonStyles.ContainsKey(name)) {
			GUIStyle st = new GUIStyle("Button");
			st.normal.background = st.hover.background = Resources.Load<Texture>("UI/" + name + "_hover") as Texture2D;
			st.active.background = Resources.Load<Texture>("UI/" + name + "_lit") as Texture2D;
			selectedButtonStyles[name] = st;
		}
		return selectedButtonStyles[name];
	}
	
	GUIStyle getNonSelectedButtonStyle(string name) {
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
	
	GUIStyle getDisabledButtonStyle(string name) {
		if (disabledButtonStyles == null) disabledButtonStyles = new Dictionary<string, GUIStyle>();
		if (!disabledButtonStyles.ContainsKey(name)) {
			GUIStyle st = new GUIStyle("Button");
			st.hover.background = st.normal.background = st.active.background = Resources.Load<Texture>("UI/" + name + "_unlit") as Texture2D;
			disabledButtonStyles[name] = st;
		}
		return disabledButtonStyles[name];
	}

	GUIStyle getSelectedButtonStyle() {
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


	
	static Texture2D turnOrderNameBackgroundTexture = null;
	Texture2D getTurnOrderNameBackgroundTexture() {
		if (turnOrderNameBackgroundTexture == null) {
			turnOrderNameBackgroundTexture = Unit.makeTexBorder((int)turnOrderNameWidth, (int)turnOrderSectionHeight, new Color(0.5f, 0.8f, 0.1f));
		}
		return turnOrderNameBackgroundTexture;
	}
	
	static Texture2D turnOrderSectionBackgroundTexture = null;
	Texture2D getTurnOrderSectionBackgroundTexture() {
		if (turnOrderSectionBackgroundTexture == null) {
			turnOrderSectionBackgroundTexture = Unit.makeTexBorder((int)turnOrderSectionHeight, (int)turnOrderSectionHeight, new Color(0.5f, 0.8f, 0.1f));
		}
		return turnOrderSectionBackgroundTexture;
	}
	
	static Texture2D turnOrderNameBackgroundTextureEnemy = null;
	Texture2D getTurnOrderNameBackgroundTextureEnemy() {
		if (turnOrderNameBackgroundTextureEnemy == null) {
			turnOrderNameBackgroundTextureEnemy = Unit.makeTexBorder((int)turnOrderNameWidth, (int)turnOrderSectionHeight, new Color(0.8f, 0.2f, 0.1f));
		}
		return turnOrderNameBackgroundTextureEnemy;
	}
	
	static Texture2D turnOrderSectionBackgroundTextureEnemy = null;
	Texture2D getTurnOrderSectionBackgroundTextureEnemy() {
		if (turnOrderSectionBackgroundTextureEnemy == null) {
			turnOrderSectionBackgroundTextureEnemy = Unit.makeTexBorder((int)turnOrderSectionHeight, (int)turnOrderSectionHeight, new Color(0.8f, 0.2f, 0.1f));
		}
		return turnOrderSectionBackgroundTextureEnemy;
	}

	
	static GUIStyle turnOrderSectionStyle;
	static GUIStyle turnOrderSectionStyleEnemy;
	GUIStyle getTurnOrderSectionStyle(Unit u) {
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
	GUIStyle getTurnOrderNameStyle(Unit u) {
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
	static GUIStyle getTabButtonStyle() {
		if (tabButtonStyle == null) {
			tabButtonStyle = new GUIStyle("Button");
			tabButtonStyle.normal.background = tabButtonStyle.hover.background = tabButtonStyle.active.background = Resources.Load<Texture>("UI/tab-button-left") as Texture2D;
			tabButtonStyle.normal.textColor = tabButtonStyle.hover.textColor = tabButtonStyle.active.textColor = Color.black;
		}
		return tabButtonStyle;
	}


	static GUIStyle playerInfoStyle;
	GUIStyle getPlayerInfoStyle() {
		if (playerInfoStyle == null) {
			playerInfoStyle = new GUIStyle("Label");
			playerInfoStyle.normal.textColor = Color.white;
			playerInfoStyle.fontSize = 11;
		}
		return playerInfoStyle;
	}

	static GUIStyle titleTextStyle = null;
	public GUIStyle getTitleTextStyle() {
		if (titleTextStyle == null) {
			titleTextStyle = new GUIStyle("Label");
			titleTextStyle.normal.textColor = Color.white;
			titleTextStyle.fontSize = 15;
		}
		return titleTextStyle;
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

	public void clickMovement() {
		if (mapGenerator.performingAction() || mapGenerator.currentUnitIsAI() || mapGenerator.isInCharacterPlacement()) return;
		Unit p = mapGenerator.selectedUnit;
		if (p==null || p.usedMovement) return;
		if (selectedMovement) return;
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
	

	public void selectTypeAt(int index) {
		if (selectedStandard) {
			StandardType[] standards = mapGenerator.getCurrentUnit().getStandardTypes();
			if (index >= standards.Length) return;
			selectStandard(standards[index]);
		}
		else if (selectedMovement) {
			MovementType[] movements = mapGenerator.getCurrentUnit().getMovementTypes();
			if (index >= movements.Length) return;
			selectMovement(movements[index]);
		}
		else if (selectedMinor) {
			MinorType[] minors = mapGenerator.getCurrentUnit().getMinorTypes();
			if (index >= minors.Length) return;
			selectMinor(minors[index]);
		}
	}

	public void selectNextOfType() {
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

	public void selectPreviousOfType() {
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

	
	void selectUnit(Unit player) {
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

		
	static float t = 0;
	static int dir = 1;
	public void OnGUI() {
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
			if (GUI.Button(beginButtonRect(), "Engage", getBeginButtonStyle())) {
				if (mapGenerator.playerTransform.childCount!=0) {
					mapGenerator.enterPriority();
					foreach (Unit u in mapGenerator.priorityOrder) {
						u.setRotationToMostInterestingTile();
					}
				}
			}
		}

		// Game GUI
		else {
			Rect clipBoardRect = clipBoardBodyRect();
			if (GUI.Button(getTabButtonRect(Tab.T), "T", getTabButtonStyle())) {
				clipboardTab = Tab.T;
			}
			if (GUI.Button(getTabButtonRect(Tab.M), "M", getTabButtonStyle())) {
				clipboardTab = Tab.M;
			}
			GUI.DrawTexture(clipBoardRect, clipBoardBodyTexture);
			if (GUI.Button(clipBoardClipRect(), "", getClipBoardClipStyle())) {
				clipboardUp = !clipboardUp;
			}
			if (clipboardTab == Tab.M) {
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
				if (GUI.Button(new Rect(x, y, widths, height), primaryContent, missionStyle)) {
					openMission = Mission.Primary;
				}
				x += widths;
				if (GUI.Button(new Rect(x, y, widths, height), secondaryContent, missionStyle)) {
					openMission = Mission.Secondary;
				}
				x += widths;
				if (GUI.Button(new Rect(x, y, widths, height), optionalContent, missionStyle)) {
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
					if (GUI.Button(r, new GUIContent("","" + playerNum), getTurnOrderSectionStyle(player))) {
						selectUnit(player);
					}

					num = new GUIContent("" + (playerNum + 1));
					numSize = st.CalcSize(num);
					GUI.Label(new Rect(x + (turnOrderSectionHeight - numSize.x)/2.0f, y + (turnOrderSectionHeight - numSize.y)/2.0f, numSize.x, numSize.y), num, getPlayerInfoStyle());
					x += turnOrderSectionHeight - 1.0f;
					r = new Rect(x, y, turnOrderNameWidth, turnOrderSectionHeight);
					//	GUI.DrawTexture(r, (player.team == 0 ? getTurnOrderNameBackgroundTexture() : getTurnOrderNameBackgroundTextureEnemy()));
					if (GUI.Button(r, new GUIContent("","" + playerNum), getTurnOrderNameStyle(player))) {
						selectUnit(player);
					}

					name = new GUIContent(player.characterSheet.personalInfo.getCharacterName().fullName());
					nameSize = st.CalcSize(name);
					GUI.Label(new Rect(x + 3.0f, y + (turnOrderSectionHeight - nameSize.y)/2.0f, Mathf.Min(nameSize.x, turnOrderNameWidth - 4.0f), nameSize.y), name, getPlayerInfoStyle());
					x += turnOrderNameWidth - 1.0f;
					r = new Rect(x, y, turnOrderSectionHeight, turnOrderSectionHeight);
					//	GUI.DrawTexture(r, (player.team == 0 ? getTurnOrderSectionBackgroundTexture() : getTurnOrderSectionBackgroundTextureEnemy()));
					if (GUI.Button(r, new GUIContent("","" + playerNum), getTurnOrderSectionStyle(player))) {
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
				GUI.DrawTexture(actionRect(), actionTexture3);
				bool enabled = !p.usedMovement;
				if (selectedMovement && p.usedMovement) {
					deselectMovement();
			//		selectedMovement = false;
			//		selectedMovementType = MovementType.None;
			//		mapGenerator.resetRanges();
				}
				if(GUI.Button(moveButtonRect(), "", (p.usedMovement ? getDisabledButtonStyle("movement") : (selectedMovement ? getSelectedButtonStyle("movement") : getNonSelectedButtonStyle("movement"))))) {
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
				if (GUI.Button(attackButtonRect(), "", (p.usedStandard || p.isProne() ? getDisabledButtonStyle("standard") :(selectedStandard ? getSelectedButtonStyle("standard") : getNonSelectedButtonStyle("standard"))))) {
					if (enabled)
						clickStandard();

				}
				enabled = p.minorsLeft > 0;//!p.usedMinor1 || !p.usedMinor2;
				if (selectedMinor && p.minorsLeft==0) {
					if (selectedMinorType == MinorType.Loot) previouslyOpenTab = Tab.I;
					deselectMinor();//selectedMinor = false;
				}
				if (GUI.Button(minorButtonRect(), "", (p.minorsLeft <= 0 ? getDisabledButtonStyle("minor") : (selectedMinor ? getSelectedButtonStyle("minor") : getNonSelectedButtonStyle("minor")))) && !mapGenerator.performingAction() && !mapGenerator.currentUnitIsAI()) {
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
						GUI.enabled = types[n] != MovementType.BackStep || mapGenerator.getCurrentUnit().moveDistLeft == mapGenerator.getCurrentUnit().maxMoveDist;
						if (GUI.Button(actionIconRect(n), (n < 9 ? (n+1) + "" : (n==9?"0":"")), (selectedMovementType == types[n] ? getSelectedActionStyle("Temp " + typeName) : getNonSelectedActionStyle("Temp " + typeName))) && !mapGenerator.performingAction() && !mapGenerator.currentUnitIsAI()) {
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
						string typeName = Unit.getNameOfStandardType(types[n]);
						GUI.enabled = true;//types[n] != MovementType.BackStep || mapGenerator.getCurrentUnit().moveDistLeft == mapGenerator.getCurrentUnit().maxMoveDist;
						if (GUI.Button(actionIconRect(n), (n < 9 ? (n+1) + "" : (n==9?"0":"")), (selectedStandardType == types[n] ? getSelectedActionStyle("Temp " + typeName) : getNonSelectedActionStyle("Temp " + typeName))) && !mapGenerator.performingAction() && !mapGenerator.currentUnitIsAI()) {//(selectedMovementType == types[n] ? getSelectedSubMenuTurnStyle() : getNonSelectedSubMenuTurnStyle()))) {
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
						string typeName = Unit.getNameOfMinorType(types[n]);
						GUI.enabled = true;//types[n] != MovementType.BackStep || mapGenerator.getCurrentUnit().moveDistLeft == mapGenerator.getCurrentUnit().maxMoveDist;
						if (GUI.Button(actionIconRect(n), (n < 9 ? (n+1) + "" : (n==9?"0":"")), (selectedMinorType == types[n] ? getSelectedActionStyle("Temp " + typeName) : getNonSelectedActionStyle("Temp " + typeName))) && !mapGenerator.performingAction() && !mapGenerator.currentUnitIsAI()) {//(selectedMovementType == types[n] ? getSelectedSubMenuTurnStyle() : getNonSelectedSubMenuTurnStyle()))) {
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
			if (GUI.Button(new Rect(Screen.width/2 - Screen.width/12, Screen.height*2/3 - Screen.height/16, Screen.width/12, Screen.height/12), "Back to Base")) {
					Application.LoadLevel(2);
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
