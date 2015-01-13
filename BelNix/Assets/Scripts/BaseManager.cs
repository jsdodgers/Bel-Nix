using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.IO;
using CharacterInfo;

public class BaseManager : MonoBehaviour {

    //Purse partyPurse = new Purse();
        // This should be initialized at the beginning of a new game
        // This has to be updated or added to each time a new character is created
        // This has to be added to whenever a mission ends
        // This has to be subtracted from whenever a purchase is made
            // (or upkeep is charged)
	public enum BaseState { Save, Mission, Barracks, None };
	private BaseState baseState = BaseState.None;
	Character displayedCharacter = null;
	Character hoveredCharacter = null;
	Character levelingUpCharacter = null;
	List<Character> units;
	string saveName = "";
	string[] saves;
//	bool saving = false;
	bool levelup = false;
	bool middleDraggin = false;
	bool rightDraggin = false;


	bool mouseLeftDown;
	bool mouseRightDown;
	bool mouseMiddleDown;

	string[] missions = new string[]{"Test Map 1", "Test Map 2", "Level Up Test Map"};
	int[] missionLevels = new int[]{4, 3, 5};
	Vector2 savesScrollPos = new Vector2();
	Vector2 barracksScrollPos = new Vector2();

	GameObject hoveredObject;

	string tooltip = "";
	Dictionary<string, string> tooltips = null;

	int oldTouchCount = 0;
	Vector3 lastPos = new Vector3();
	static Texture2D barracksTexture;
	static Texture2D bottomSheetTexture;
	// Use this for initialization
	void Start () {
	//	Item item = new Turret(new TestFrame(), new TestApplicator(), new TestGear(), new TestEnergySource());
		Item item = Item.deserializeItem((ItemCode)4,"5,,124,0,Units/Turrets/TurretPlaceholder,0,11,2:Test Frame:0:0::0:65,14,0:Test Applicator:30:0:Units/Turrets/Applicator:0:0:1:1:6:0:1:5:70:0:0,15,6:Test Gear:0:0:Units/Turrets/Gear:0,12,6:Test Energy Source:0:0:Units/Turrets/EnergySource:0:2");
		Debug.Log(item.getItemCode() + "   " + (int)item.getItemCode() + "   \n" + item.getItemData());
	//	BinaryFormatter bf = new BinaryFormatter();
	/*	XmlSerializer bf = new XmlSerializer(item.GetType());
		using (StringWriter textWriter = new StringWriter()) {
			bf.Serialize(textWriter, item);
			Debug.Log(textWriter.ToString());
		}*/
	
		units = new List<Character>();
		string[] chars = Saves.getCharacterList();
		for (int n=0;n<chars.Length-1;n++) {
			Character ch = new Character();
			ch.loadCharacterFromTextFile(chars[n]);
			ch.characterId = chars[n];
			units.Add(ch);
		}
		barracksTexture = Resources.Load<Texture>("UI/barracks-back") as Texture2D;
		bottomSheetTexture = Resources.Load<Texture>("UI/bottom-sheet-long") as Texture2D;
		tooltips = new Dictionary<string, string>();
		tooltips.Add("barracks", "Barracks");
		tooltips.Add("engineering", "Create Traps and Turrets");
		tooltips.Add("exit", "Exit to Main Menu");
		tooltips.Add("map", "Open Map");
		tooltips.Add("infirmary", "Infirmary");
		tooltips.Add("newcharacter", "Create a new Character");
		int nn=0;
		do {
			nn++;
			saveName = "Save " + nn;
		} while (Saves.hasSaveFileNamed(saveName));
	}


	// Update is called once per frame
	void Update () {
		handleInput();
		UnitGUI.doTabs();
	}

	void handleInput() {
		handleKeys();
		handleDrag();
		handleKeyPan();
		handleMouseMovement();
		handleMouseClick();
	}

	void handleMouseClick() {
		Vector2 mouse = Input.mousePosition;
		mouse.y = Screen.height - mouse.y;
		if (Input.GetMouseButtonDown(0)) {
			if (hoveredObject != null) {
				if (hoveredObject.tag == "exit")
					Application.LoadLevel(0);
				else if (hoveredObject.tag=="map") {
					loadMapScrollPos = new Vector2();
					baseState = BaseState.Mission;
				}
				else if (hoveredObject.tag=="barracks") {
					barracksScrollPos = new Vector2();
					baseState = BaseState.Barracks;
				}
				else if (hoveredObject.tag=="newcharacter") {
					PlayerPrefs.SetInt("playercreatefrom", Application.loadedLevel);
					Application.LoadLevel(1);
				}
			}
		
			if (!levelup && !(UnitGUI.containsMouse(mouse) && displayedCharacter!=null) && levelingUpCharacter == null) {
				if (hoveredCharacter == displayedCharacter) displayedCharacter = null;
				else if (hoveredCharacter != null) displayedCharacter = hoveredCharacter;
			}
		}
		if (UnitGUI.containsMouse(mouse) && Input.GetMouseButtonDown(0) && !rightDraggin && !middleDraggin) {
			if (UnitGUI.inventoryOpen && displayedCharacter != null) {
				UnitGUI.selectItem(displayedCharacter);
				//		selectedUnit.selectItem();
			}
		}
		if (Input.GetMouseButtonUp(0) && !rightDraggin && !middleDraggin) {
			if (UnitGUI.inventoryOpen && displayedCharacter != null) {
				//				selectedUnit.deselectItem();
				UnitGUI.deselectItem(displayedCharacter);
			}
		}
	}

	
	void handleKeyPan() {
		return;
		float xDiff = 0;
		float yDiff = 0;
		float eachFrame = 4.0f;
		//	if (shiftDown) eachFrame = 1.5f;
		eachFrame *= Time.deltaTime;
		if (Input.GetKey(KeyCode.W)) yDiff -= eachFrame;
		if (Input.GetKey(KeyCode.S)) yDiff += eachFrame;
		if (Input.GetKey(KeyCode.A)) xDiff += eachFrame;
		if (Input.GetKey(KeyCode.D)) xDiff -= eachFrame;
		if (xDiff==0 && yDiff==0) return;
		//	Vector3 pos = mapTransform.position;
		Vector3 pos = Camera.main.transform.position;
		pos.x -= xDiff;
		pos.y -= yDiff;
		Camera.main.transform.position = pos;
		//	mapTransform.position = pos;
		lastPos.x -= xDiff;
		lastPos.y -= yDiff;
	}

	bool shiftDown = false;
	bool altDown = false;
	bool controlDown = false;
	bool commandDown = false;
	Character expChanged = null;
	void handleKeys() {
		shiftDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		altDown = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
		controlDown = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
		commandDown = Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand);
		mouseLeftDown = Input.GetMouseButton(0);
		mouseRightDown = Input.GetMouseButton(1);
		mouseMiddleDown = Input.GetMouseButton(2);
		if (!rightDraggin) middleDraggin = ((mouseMiddleDown || (mouseLeftDown && Input.touchCount==2)) && middleDraggin) || Input.GetMouseButtonDown(2);
		if (!middleDraggin) rightDraggin = (rightDraggin && mouseRightDown) || Input.GetMouseButtonDown(1);
		if (baseState==BaseState.Barracks && displayedCharacter!=null) {
			if (Input.GetKeyDown(KeyCode.C)) {
				UnitGUI.clickTab(Tab.C);
			}
			if (Input.GetKeyDown(KeyCode.V)) {
				UnitGUI.clickTab(Tab.V);
			}
			if (Input.GetKeyDown(KeyCode.B)) {
				UnitGUI.clickTab(Tab.B);
			}
		}
		if (shiftDown && controlDown && (altDown || commandDown)) {
			if (displayedCharacter != null) {
				if ((commandDown && Input.GetKey(KeyCode.Minus)) || Input.GetKeyDown(KeyCode.Minus)) {
					if (expChanged != null && expChanged != displayedCharacter) expChanged.saveCharacter();
					expChanged = displayedCharacter;
					displayedCharacter.characterProgress.setExperience(Mathf.Max(0,displayedCharacter.characterProgress.getCharacterExperience()-100));
					displayedCharacter.saveCharacter();
				}
				if ((commandDown && Input.GetKey(KeyCode.Equals)) || Input.GetKeyDown(KeyCode.Equals)) {
					if (expChanged != null && expChanged != displayedCharacter) expChanged.saveCharacter();
					expChanged = displayedCharacter;
					displayedCharacter.characterProgress.addExperience(100);
					displayedCharacter.saveCharacter();
				}
			}
		}
		if (expChanged!=null) {
			if (Input.GetKeyUp(KeyCode.Equals) || Input.GetKeyUp(KeyCode.Plus)) {
				expChanged.saveCharacter();
				expChanged = null;
			}
		}
	}

	void handleDrag() {
		return;
		var mPos = Input.mousePosition;
		mPos.z = 10.0f;
		Vector3 pos1 = Camera.main.ScreenToWorldPoint(mPos);
		if (((middleDraggin && Input.touchCount == oldTouchCount) || rightDraggin)) {//  && Input.mousePosition.x < Screen.width*(1-boxWidthPerc)) {
			//= mainCamera.WorldToScreenPoint(cameraTransform.position);
			if (!Input.GetMouseButtonDown(0)) {
				float xDiff = pos1.x - lastPos.x;
				float yDiff = pos1.y - lastPos.y;
				//	Vector3 pos = mapTransform.position;
				//	pos.x += xDiff;
				//	pos.y += yDiff;
				//	mapTransform.position = pos;
				Vector3 pos = Camera.main.transform.position;
				pos.x -= xDiff;
				pos.y -= yDiff;
				Camera.main.transform.position = pos;
			}
		}
		lastPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		oldTouchCount = Input.touchCount;
	}

	float scale = 1.1f;

	void handleMouseMovement() {
		bool old = hoveredObject==null;
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 100.0f, 1<<13);
		//		Physics2D.Ray
		GameObject go = null;
		if (hit.collider != null) go = hit.collider.gameObject;
		if (baseState != BaseState.None) go = null;
		if (go != hoveredObject) {
			if (hoveredObject != null) {
				hoveredObject.transform.localScale = new Vector3(1, 1, 1);
			}
			hoveredObject = go;
			if (hoveredObject != null) {
				hoveredObject.transform.localScale = new Vector3(scale, scale, scale);
			}
		}
		if (go != null)
			tooltip = tooltips[go.tag];
		else tooltip = "";
		if (go==null && !old) handleMouseMovement();
	}

	static GUIStyle saveButtonsStyle = null;
	public static GUIStyle getSaveButtonsStyle() {
		if (saveButtonsStyle==null) {
			saveButtonsStyle = new GUIStyle("Button");
			saveButtonsStyle.active.background = saveButtonsStyle.hover.background = saveButtonsStyle.normal.background = null;
			saveButtonsStyle.alignment = TextAnchor.MiddleLeft;
		}
		return saveButtonsStyle;
	}

	string oldSaveName;
//	bool choosingMap = false;
	Vector2 loadMapScrollPos = new Vector2();
	void OnGUI() {
		hoveredCharacter = null;
		if (baseState == BaseState.None) {
			if (GUI.Button(new Rect(0, 0, 100, 50), "Save Game")) {
				saves = Saves.getSaveFiles();
				baseState = BaseState.Save;
				oldSaveName = saveName;
				savesScrollPos = new Vector2();
				string savesSt = "";
				foreach (string save in saves) {
					savesSt += save + "\n";
				}
			}
	//		Vector3 mousePos = Input.mousePosition;
	//		mousePos.y = Screen.height - mousePos.y;
			GUIContent toolContent = new GUIContent(tooltip);
			GUIStyle st = GUI.skin.label;
			Vector2 size = st.CalcSize(toolContent);
			float x = Screen.width - size.x - 5.0f;
	//		if (x + size.x + 5.0f > Screen.width) x = mousePos.x - size.x;//x = Screen.width - size.x - 5.0f;
	//		float y = mousePos.y - size.y;
			float y = 0.0f;	
			GUI.Label(new Rect(x, y, size.x, size.y), toolContent, st);
		}
		else if (baseState == BaseState.Mission) {
			float boxHeight = 250.0f;
			float boxWidth = 200.0f;
			float boxX = (Screen.width - boxWidth)/2.0f;
			float boxY = (Screen.height - boxHeight)/2.0f;
			float buttX = boxX + 20.0f;
			float buttWidth = boxWidth - 20.0f*2.0f;
			GUI.Box(new Rect(boxX, boxY, boxWidth, boxHeight), "Select Mission");
			GUI.BeginScrollView(new Rect(boxX, boxY + 25.0f, boxWidth, 40.0f * 4), loadMapScrollPos, new Rect(boxX, boxY + 25.0f, boxWidth - 16.0f, 40.0f * missions.Length));
			for (int n=0;n<Mathf.Min(missions.Length, missionLevels.Length); n++) {
				if (GUI.Button(new Rect(buttX, boxY + 25.0f + 40.0f * n, buttWidth, 40.0f), missions[n])) {
					Application.LoadLevel(missionLevels[n]);
				}
			}
			GUI.EndScrollView();
			if (GUI.Button(new Rect(buttX, boxY + 25.0f + 40.0f * 4 + 10.0f, buttWidth, 40.0f), "Cancel")) {
				baseState = BaseState.None;
			}
		}
		else if (baseState == BaseState.Save) {
			float width = 250.0f;
			float height = Screen.height * .8f;
			float x = (Screen.width - width)/2.0f;
			float y = (Screen.height - height)/2.0f;
			float boxY = y;
			GUI.Box(new Rect(x, y, width, height), "");
			float buttonWidth = 100.0f;
			float buttonHeight = 50.0f;
			float buttonY = y + height - buttonHeight - 5.0f;
			float buttonX1 = x + 15.0f;
			float buttonX2 = buttonX1 + buttonWidth + 20.0f;
			if (GUI.Button(new Rect(buttonX1, buttonY, buttonWidth, buttonHeight), "Cancel")) {
				baseState = BaseState.None;
				saveName = oldSaveName;
			}
			if (GUI.Button(new Rect(buttonX2, buttonY, buttonWidth, buttonHeight), "Save")) {
				Saves.saveAs(saveName);
				baseState = BaseState.None;
			}
			float textFieldHeight = 25.0f;
			saveName = GUI.TextField(new Rect(x + 5.0f, y + 5.0f, width - 10.0f, textFieldHeight), saveName);
			float savesHeight = 0.0f;
			GUIStyle st = getSaveButtonsStyle();
			foreach (string save in saves) {
				savesHeight += st.CalcSize(new GUIContent(save)).y;
			}
			y += 5.0f + textFieldHeight + 5.0f;
			float scrollHeight = buttonY - y - 5.0f;
			float scrollX = x + 5.0f;
			float scrollWidth = width - (scrollX - x) * 2.0f;
			savesScrollPos = GUI.BeginScrollView(new Rect(scrollX, y, scrollWidth, scrollHeight), savesScrollPos, new Rect(scrollX, y, scrollWidth - 16.0f, savesHeight));
			foreach (string save in saves) {
				GUIContent gc = new GUIContent(save);
				float h = st.CalcSize(gc).y;
				if (GUI.Button(new Rect(scrollX, y, scrollWidth, h), gc, st)) {
					saveName = save;
				}
				y += h;
			}
			GUI.EndScrollView();
		}
		else if (baseState == BaseState.Barracks) {
			int numHeight = 8;
			float topHeight = 20.0f;
			float buttonHeight = 40.0f;
			float buttonWidth = 200.0f;
			float bottomHeight = buttonHeight + 5.0f*2;
			Vector2 eachSize = new Vector2(476, 79);
			Vector2 totalSize = new Vector2(eachSize.x + 20.0f*2, eachSize.y * numHeight + topHeight + bottomHeight);
			while (totalSize.y > Screen.height - 50.0f && numHeight > 1) {
				numHeight--;
				totalSize = new Vector2(eachSize.x + 20.0f*2, eachSize.y * numHeight + topHeight + bottomHeight);
			}
			Vector2 boxOrigin = new Vector2((Screen.width - totalSize.x)/2.0f, (Screen.height - totalSize.y)/2.0f);
			GUI.Box(new Rect(boxOrigin.x, boxOrigin.y, totalSize.x, totalSize.y), "Barracks");
			Rect scrollRect = new Rect(boxOrigin.x + 20.0f, boxOrigin.y + topHeight, eachSize.x + 16.0f, eachSize.y * numHeight);
			bool inScroll = scrollRect.Contains(Event.current.mousePosition);
			if (UnitGUI.containsMouse(Event.current.mousePosition) && displayedCharacter!=null) {
				GUI.enabled = false;
			}
			barracksScrollPos = GUI.BeginScrollView(scrollRect, barracksScrollPos, new Rect(boxOrigin.x + 20.0f, boxOrigin.y + topHeight, eachSize.x, eachSize.y * units.Count));
			GUI.enabled = true;
			for (int n=0;n<units.Count;n++) {
				Character u = units[n];
				Rect totalRect = new Rect(boxOrigin.x + 20.0f, boxOrigin.y + topHeight + n*eachSize.y, eachSize.x, eachSize.y);
				GUI.DrawTexture(totalRect, barracksTexture);
				int largeSize = 16;
				int smallSize = 12;
				string startSize = "<size=" + smallSize + ">";
				string endSize = "</size>";
				GUIStyle st = getUnitInfoStyle(largeSize);
				string info = UnitGUI.getSmallCapsString(u.characterSheet.personalInformation.getCharacterName().fullName(), smallSize) + "\n" +
					UnitGUI.getSmallCapsString(u.characterSheet.characterProgress.getCharacterClass().getClassName().ToString(), smallSize) + " \n" +
						UnitGUI.getSmallCapsString(u.characterSheet.personalInformation.getCharacterRace().getRaceString(), smallSize) + "\n" +
						UnitGUI.getSmallCapsString(u.characterSheet.personalInformation.getCharacterBackground().ToString(), smallSize);
				GUIContent infoCont = new GUIContent(info);
				Vector2 infoSize = st.CalcSize(infoCont);
				GUI.Label(new Rect(boxOrigin.x + 25.0f, boxOrigin.y + topHeight + n*eachSize.y, eachSize.x - 5.0f, eachSize.y), infoCont, st);
				string exp = UnitGUI.getSmallCapsString("Level", smallSize) + ":" + startSize + u.characterSheet.characterProgress.getCharacterLevel() + endSize + "\n" +
					UnitGUI.getSmallCapsString("Experience", smallSize) + ":" + startSize + u.characterSheet.characterProgress.getCharacterExperience() + "/" + (u.characterSheet.characterProgress.getCharacterLevel()*100) + endSize + "\n" +
						UnitGUI.getSmallCapsString("Health", smallSize) + ":" + startSize + u.characterSheet.combatScores.getCurrentHealth() + "/" + u.characterSheet.combatScores.getMaxHealth() + endSize + "\n" +
						UnitGUI.getSmallCapsString("Composure", smallSize) + ":" + startSize + u.characterSheet.combatScores.getCurrentComposure() + "/" + u.characterSheet.combatScores.getMaxComposure() + endSize;
				GUIContent expCont = new GUIContent(exp);
				Vector2 expSize = st.CalcSize(expCont);
				float expWidth = 150.0f;
				float expX = boxOrigin.x + 20.0f + eachSize.x - expWidth;
				float y = boxOrigin.y + topHeight + n*eachSize.y;
				GUI.Label(new Rect(expX, y, expWidth, eachSize.y), expCont, st);
				Vector2 levelUpSize = new Vector2(70.0f, expSize.y/2.0f - 15.0f);
				Rect levelUpRect = new Rect(expX - levelUpSize.x - 5.0f, y + 10.0f, levelUpSize.x, levelUpSize.y);
				bool haslevelup = false;
				if (u.characterSheet.characterProgress.canLevelUp()) {
					if (GUI.Button(levelUpRect, "Level Up!") && levelingUpCharacter == null) {
						levelingUpCharacter = u;
						abilityScorePointsAvailable = 1;
						skillPointsAvailable = 1;
						sturdyScore = u.abilityScores.getSturdy();
						perceptionScore = u.abilityScores.getPerception();
						techniqueScore = u.abilityScores.getTechnique();
						wellVersedScore = u.abilityScores.getWellVersed();
						athleticsSkill = u.skillScores.scores[0];
						meleeSkill = u.skillScores.scores[1];
						rangedSkill = u.skillScores.scores[2];
						stealthSkill = u.skillScores.scores[3];
						mechanicalSkill = u.skillScores.scores[4];
						medicinalSkill = u.skillScores.scores[5];
						historicalSkill = u.skillScores.scores[6];
						politicalSkill = u.skillScores.scores[7];
						possibleFeatures = u.characterProgress.getCharacterClass().getPossibleFeatures(u.characterProgress.getCharacterLevel()+1);
						page = 0;
						selectedFeature = -1;
						Debug.Log("Level Up!!");
					}
					if (levelUpRect.Contains(Event.current.mousePosition)) haslevelup = true;
				}
				if (!haslevelup && inScroll) {
					if (totalRect.Contains(Event.current.mousePosition)) hoveredCharacter = u;
				}
			}
			if (UnitGUI.containsMouse(Event.current.mousePosition) && displayedCharacter!=null) {
				GUI.enabled = false;
			}
			GUI.EndScrollView();
			GUI.enabled = true;
			if (GUI.Button(new Rect((Screen.width - buttonWidth)/2.0f, boxOrigin.y + totalSize.y - 5.0f - buttonHeight, buttonWidth, buttonHeight), "Cancel")) {
				baseState = BaseState.None;
				displayedCharacter = null;
			}
			if (displayedCharacter != null) {
				UnitGUI.drawGUI(displayedCharacter, null, null);
			}
			if (levelingUpCharacter != null) {
				drawLevelUpGUI();
			}
		}
	}
	
	int calculateBoxHeight(int n)
	{
		int height = 0;
		
		height = 20 * n;
		
		return height;
	}
	
	int calculateMod(int abilityScore)
	{
		return abilityScore/2;
	}

	int abilityScorePointsAvailable = 1;
	int skillPointsAvailable = 1;
	int sturdyScore;
	int perceptionScore;
	int techniqueScore;
	int wellVersedScore;
	int athleticsSkill;
	int meleeSkill;
	int rangedSkill;
	int stealthSkill;
	int mechanicalSkill;
	int medicinalSkill;
	int historicalSkill;
	int politicalSkill;
	float cellWidth = 25.0f;
	float cellHeight = 20.0f;
	int page = 0;
	int selectedFeature = -1;
	int selectedWeaponFocus = -1;
	ClassFeature[] possibleFeatures;
	public bool canGoNextPage() {
		switch (page) {
		case 0:
			return abilityScorePointsAvailable == 0;
		case 1:
			return skillPointsAvailable == 0;
		case 2:
			return (possibleFeatures.Length<=1 || selectedFeature >=0) && hasFinishedAllSelections();
		default:
			return false;
		}
	}

	public ClassFeature getSelectedFeature() {
		return (possibleFeatures.Length==1 ? possibleFeatures[0] : possibleFeatures[selectedFeature]);
	}

	public bool hasFinishedAllSelections() {
		if (possibleFeatures.Length == 0) return true;
		if (possibleFeatures.Length > 1 && selectedFeature < 0) return false;
		ClassFeature feature = getSelectedFeature();
		switch (feature) {
		case ClassFeature.Weapon_Focus:
			return selectedWeaponFocus >= 0;
		default:
			return true;
		}
	}
	
	int setSkillDecreaseButton(int skill, Rect r, int skillLowerBound)
	{
		if(skill == skillLowerBound)
		{
			GUI.enabled = false;
		}
		if(GUI.Button(r, "<"))
		{
			skillPointsAvailable++;
			skill--;
		}
		GUI.enabled = true;

		return skill;
	}
	
	int setSkillIncreaseButton(int skill, Rect r)
	{
		if(skillPointsAvailable == 0)
		{
			GUI.enabled = false;
		}
		if(GUI.Button(r, ">"))
		{
			skillPointsAvailable--;
			skill++;
		}
		GUI.enabled = true;
		
		return skill;
	}


	public void drawLevelUpGUI() {
		Vector2 backgroundSize = new Vector2(500.0f, 500.0f);
		float xOrig = (Screen.width - backgroundSize.x)/2.0f;
		float yOrig = (Screen.height - backgroundSize.y)/2.0f;
		float boxX = xOrig;
		float boxY = yOrig;
		GUI.DrawTexture(new Rect(xOrig, yOrig, backgroundSize.x, backgroundSize.y), bottomSheetTexture);
		yOrig +=50.0f;
		switch (page) {
		case 0:
			xOrig = xOrig + (backgroundSize.x - cellWidth*16)/2.0f;
			drawAbilityScoresGUI(xOrig, yOrig);
			break;
		case 1:
			xOrig = xOrig + (backgroundSize.x - cellWidth*18)/2.0f;
			drawSkillScoresGUI(xOrig, yOrig);
			break;
		case 2:
			drawFeatureGUI(xOrig, yOrig, backgroundSize);
			break;
		default:
			break;
		}
		Vector2 cancelButtonSize = new Vector2(100.0f, 40.0f);
		Vector2 otherButtonSize = new Vector2(80.0f, cancelButtonSize.y);
		float buttonY = boxY + backgroundSize.y - cancelButtonSize.y - 40.0f;
		if (GUI.Button(new Rect(boxX + 30.0f, buttonY, cancelButtonSize.x, cancelButtonSize.y), "Cancel")) {
			levelingUpCharacter = null;
		}
		if (page==0) GUI.enabled = false;
		if (GUI.Button(new Rect(boxX + backgroundSize.x - otherButtonSize.x*2 - 30.0f, buttonY, otherButtonSize.x, otherButtonSize.y), "Back")) {
			page--;
		}
		GUI.enabled = canGoNextPage();
		if (GUI.Button(new Rect(boxX + backgroundSize.x - otherButtonSize.x - 30.0f, buttonY, otherButtonSize.x, otherButtonSize.y), (page==2?"Finish":"Next"))) {
			if (page == 2) {
				levelingUpCharacter.skillScores.scores = new int[]{athleticsSkill, meleeSkill, rangedSkill, stealthSkill, mechanicalSkill, medicinalSkill, historicalSkill, politicalSkill}; 
				levelingUpCharacter.abilityScores.setScores(sturdyScore, perceptionScore, techniqueScore, wellVersedScore);
				if (selectedFeature != -1) {
					int[] oldFeatures = levelingUpCharacter.characterProgress.getCharacterClass().chosenFeatures;
					int[] newFeatures = new int[oldFeatures.Length+1];
					for (int n=0;n<oldFeatures.Length;n++) newFeatures[n] = oldFeatures[n];
					newFeatures[newFeatures.Length-1] = selectedFeature;
					levelingUpCharacter.characterProgress.getCharacterClass().chosenFeatures = newFeatures;
				}
				if (possibleFeatures.Length>=1) {
					ClassFeature feature = getSelectedFeature();
					switch (feature) {
					case ClassFeature.Weapon_Focus:
						Debug.Log("Weapon Focus");
						levelingUpCharacter.characterProgress.setWeaponFocus(selectedWeaponFocus + 1);
						break;
					default:
						break;
					}
				}
				levelingUpCharacter.characterProgress.incrementLevel();
				levelingUpCharacter.saveCharacter();
				levelingUpCharacter = null;
			}
			else {
				page++;
			}
		}
	}
	public void drawFeatureGUI(float xOrig, float yOrig, Vector2 backgroundSize) {
		GUIStyle st = UnitGUI.getCourierStyle(24);
		string title = UnitGUI.getSmallCapsString((possibleFeatures.Length==0 ? "No Class Feature Learned" : "Class Feature Learned:"), 17);
		GUIContent titleContent = new GUIContent(title);
		Vector2 titleSize = st.CalcSize(titleContent);
		float y = yOrig;
		float x = xOrig + 30.0f;
		GUI.Label(new Rect(xOrig + (backgroundSize.x - titleSize.x)/2.0f, y, titleSize.x, titleSize.y), titleContent, st);
		y += titleSize.y + 30.0f;
		Vector2 buttonSize = new Vector2(150.0f, 30.0f);
		if (possibleFeatures.Length>1) {
			st.fontSize = 20;
			string featureSelectTitle = UnitGUI.getSmallCapsString("Select a Feature:", 14);
			GUIContent featureSelectTitleContent = new GUIContent(featureSelectTitle);
			Vector2 featureSelectTitleSize = st.CalcSize(featureSelectTitleContent);
			GUI.Label(new Rect(x, y, featureSelectTitleSize.x, featureSelectTitleSize.y), featureSelectTitleContent, st);
			y += featureSelectTitleSize.y;
			for (int n=0;n<possibleFeatures.Length;n++) {
				if (GUI.Button(new Rect(x, y, buttonSize.x, buttonSize.y), ClassFeatures.getName(possibleFeatures[n]))) {
					if (selectedFeature == n) selectedFeature = -1;
					else selectedFeature = n;
				}
				y += buttonSize.y;
			}
			y += 10.0f;
		}
		if (possibleFeatures.Length==1 || selectedFeature >=0) {
			ClassFeature feature = getSelectedFeature();
			st.fontSize = 20;
			string featureName = ClassFeatures.getName(feature);
			GUIContent featureNameContent = new GUIContent(UnitGUI.getSmallCapsString(featureName, 14));
			Vector2 featureNameSize = st.CalcSize(featureNameContent);
			GUI.Label(new Rect(x, y, featureNameSize.x, featureNameSize.y), featureNameContent, st);
			y += featureNameSize.y + 10.0f;
			st.fontSize = 16;
			string featureDisc = ClassFeatures.getDescription(feature);
			GUIContent featureDiscContent = new GUIContent(UnitGUI.getSmallCapsString(featureDisc, 10));
			float featureDiscHeight = st.CalcHeight(featureDiscContent, backgroundSize.x - 60.0f);
			GUI.Label(new Rect(x, y, backgroundSize.x - 60.0f, featureDiscHeight), featureDiscContent, st);
			y += featureDiscHeight + 25.0f;
			if (feature == ClassFeature.Weapon_Focus) {
				st.fontSize = 20;
				GUIContent selectTitleString = new GUIContent(UnitGUI.getSmallCapsString("Select a Weapon Focus:", 14));
				Vector2 selectTitleSize = st.CalcSize(selectTitleString);
				GUI.Label(new Rect(x, y, selectTitleSize.x, selectTitleSize.y), selectTitleString, st);
				y += selectTitleSize.y + 5.0f;
				GUIContent selectedString = new GUIContent(UnitGUI.getSmallCapsString("Selected", 14));
				Vector2 selectedSize = st.CalcSize(selectedString);
				string[] focuses = new string[]{"Piercing","Slashing","Crushing"};
				for (int n=0;n<focuses.Length;n++) {
					if (GUI.Button(new Rect(x, y, buttonSize.x, buttonSize.y), focuses[n])) {
						if (selectedWeaponFocus == n) selectedWeaponFocus = -1;
						else selectedWeaponFocus = n;
					}
					if (selectedWeaponFocus == n) GUI.Label(new Rect(x + buttonSize.x + 5.0f, y + (buttonSize.y - selectedSize.y)/2.0f, selectedSize.x, selectedSize.y), selectedString, st);
					y += buttonSize.y;
				}
			}
		}
	}

	public void drawSkillScoresGUI(float xOrig, float yOrig) {
		float x = xOrig, y = yOrig;
		GUI.Box(new Rect(x, y, cellWidth*18, cellHeight*2), "Character Creation: Skills");
		y += cellHeight*2;
		x = xOrig;
		GUI.Box(new Rect(x, y, cellWidth*8, cellHeight), "Points Available:");
		x += cellWidth*8;
		GUI.Box(new Rect(x, y, cellWidth*10, cellHeight), skillPointsAvailable.ToString());
		y += cellHeight;
		x = xOrig;
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight), "Category:");
		x += cellWidth*4;
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight), "Skill:");
		x += cellWidth*5;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), "Base:");
		x += cellWidth*4;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), "Mod:");
		x += cellWidth*3;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), "Total:");
		y += cellHeight;
		x = xOrig;
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight*2), "Physique:");
		x += cellWidth*4;
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight), "Athletics:");
		x += cellWidth*4;
		athleticsSkill = setSkillDecreaseButton(athleticsSkill, new Rect(x, y, cellWidth, cellHeight), levelingUpCharacter.skillScores.scores[0]);
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (athleticsSkill + levelingUpCharacter.characterProgress.getCharacterClass().getClassModifiers().getAthleticsModifier()).ToString());
		x += cellWidth*2;
		athleticsSkill = setSkillIncreaseButton(athleticsSkill, new Rect(x, y, cellWidth, cellHeight));
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth, cellHeight*2), "+");
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight*2), calculateMod(sturdyScore).ToString());
		x += cellWidth*2;
		GUI.Box(new Rect(x, y, cellWidth, cellHeight*2), "=");
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (athleticsSkill + calculateMod(sturdyScore) + levelingUpCharacter.characterProgress.getCharacterClass().getClassModifiers().getAthleticsModifier()).ToString());
		y += cellHeight;
		x = xOrig + cellWidth*4;
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight), "Melee:");
		x += cellWidth*4;
		meleeSkill = setSkillDecreaseButton(meleeSkill, new Rect(x, y, cellWidth, cellHeight), levelingUpCharacter.skillScores.scores[1]);
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (meleeSkill + levelingUpCharacter.characterProgress.getCharacterClass().getClassModifiers().getMeleeModifier()).ToString());
		x += cellWidth*2;
		meleeSkill = setSkillIncreaseButton(meleeSkill, new Rect(x, y, cellWidth, cellHeight));
		x += cellWidth*5;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (meleeSkill + calculateMod(sturdyScore) + levelingUpCharacter.characterProgress.getCharacterClass().getClassModifiers().getMeleeModifier()).ToString());
		y += cellHeight;
		x = xOrig;
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight*2), "Prowess:");
		x += cellWidth*4;
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight), "Ranged:");
		x += cellWidth*4;
		rangedSkill = setSkillDecreaseButton(rangedSkill, new Rect(x, y, cellWidth, cellHeight), levelingUpCharacter.skillScores.scores[2]);
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (rangedSkill + levelingUpCharacter.characterProgress.getCharacterClass().getClassModifiers().getRangedModifier()).ToString());
		x += cellWidth*2;
		rangedSkill = setSkillIncreaseButton(rangedSkill, new Rect(x, y, cellWidth, cellHeight));
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth, cellHeight*2), "+");
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight*2), calculateMod(perceptionScore).ToString());
		x += cellWidth*2;
		GUI.Box(new Rect(x, y, cellWidth, cellHeight*2), "=");
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (rangedSkill + calculateMod(perceptionScore) + levelingUpCharacter.characterProgress.getCharacterClass().getClassModifiers().getRangedModifier()).ToString());
		x = xOrig + cellWidth*4;
		y += cellHeight;
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight), "Stealth:");
		x += cellWidth*4;
		stealthSkill = setSkillDecreaseButton(stealthSkill, new Rect(x, y, cellWidth, cellHeight), levelingUpCharacter.skillScores.scores[3]);
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (stealthSkill + levelingUpCharacter.characterProgress.getCharacterClass().getClassModifiers().getStealthModifier()).ToString());
		x += cellWidth*2;
		stealthSkill = setSkillIncreaseButton(stealthSkill, new Rect(x, y, cellWidth, cellHeight));
		x += cellWidth*5;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (stealthSkill + calculateMod(perceptionScore) + levelingUpCharacter.characterProgress.getCharacterClass().getClassModifiers().getStealthModifier()).ToString());
		y += cellHeight;
		x = xOrig;
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight*2), "Mastery:");
		x += cellWidth*4;
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight), "Mechanical:");
		x += cellWidth*4;
		mechanicalSkill = setSkillDecreaseButton(mechanicalSkill, new Rect(x, y, cellWidth, cellHeight), levelingUpCharacter.skillScores.scores[4]);
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (mechanicalSkill + levelingUpCharacter.characterProgress.getCharacterClass().getClassModifiers().getMechanicalModifier()).ToString());
		x += cellWidth*2;
		mechanicalSkill = setSkillIncreaseButton(mechanicalSkill, new Rect(x, y, cellWidth, cellHeight));
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth, cellHeight*2), "+");
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight*2), calculateMod(techniqueScore).ToString());
		x += cellWidth*2;
		GUI.Box(new Rect(x, y, cellWidth, cellHeight*2), "=");
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (mechanicalSkill + calculateMod(techniqueScore) + levelingUpCharacter.characterProgress.getCharacterClass().getClassModifiers().getMechanicalModifier()).ToString());
		y += cellHeight;
		x = xOrig + cellWidth*4;
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight), "Medicinal:");
		x += cellWidth*4;
		medicinalSkill = setSkillDecreaseButton(medicinalSkill, new Rect(x, y, cellWidth, cellHeight), levelingUpCharacter.skillScores.scores[5]);
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (medicinalSkill + levelingUpCharacter.characterProgress.getCharacterClass().getClassModifiers().getMedicinalModifier()).ToString());
		x += cellWidth*2;
		medicinalSkill = setSkillIncreaseButton(medicinalSkill, new Rect(x, y, cellWidth, cellHeight));
		x += cellWidth*5;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (medicinalSkill + calculateMod(techniqueScore) + levelingUpCharacter.characterProgress.getCharacterClass().getClassModifiers().getMedicinalModifier()).ToString());
		y += cellHeight;
		x = xOrig;
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight*2), "Knowledge:");
		x += cellWidth*4;
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight), "Historical:");
		x += cellWidth*4;
		historicalSkill = setSkillDecreaseButton(historicalSkill, new Rect(x, y, cellWidth, cellHeight), levelingUpCharacter.skillScores.scores[6]);
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (historicalSkill + levelingUpCharacter.characterProgress.getCharacterClass().getClassModifiers().getHistoricalModifier()).ToString());
		x += cellWidth*2;
		historicalSkill = setSkillIncreaseButton(historicalSkill, new Rect(x, y, cellWidth, cellHeight));
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth, cellHeight*2), "+");
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight*2), calculateMod(wellVersedScore).ToString());
		x += cellWidth*2;
		GUI.Box(new Rect(x, y, cellWidth, cellHeight*2), "=");
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (historicalSkill + calculateMod(wellVersedScore) + levelingUpCharacter.characterProgress.getCharacterClass().getClassModifiers().getHistoricalModifier()).ToString());
		y += cellHeight;
		x = xOrig + cellWidth*4;
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight), "Political:");
		x += cellWidth*4;
		politicalSkill = setSkillDecreaseButton(politicalSkill, new Rect(x, y, cellWidth, cellHeight), levelingUpCharacter.skillScores.scores[7]);
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (politicalSkill + levelingUpCharacter.characterProgress.getCharacterClass().getClassModifiers().getPoliticalModifier()).ToString());
		x += cellWidth*2;
		politicalSkill = setSkillIncreaseButton(politicalSkill, new Rect(x, y, cellWidth, cellHeight));
		x += cellWidth*5;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (politicalSkill + calculateMod(wellVersedScore) + levelingUpCharacter.characterProgress.getCharacterClass().getClassModifiers().getPoliticalModifier()).ToString());
	}

	public void drawAbilityScoresGUI(float xOrig, float yOrig) {
		float x = xOrig;
		float y = yOrig;
		GUI.Box(new Rect(x, y, cellWidth*11, cellHeight*2), "Character Creation: Ability Scores");
		y += cellHeight*2;
		x = xOrig;
		GUI.Box(new Rect(x, y, cellWidth*5, cellHeight), "Points Available:");
		x += cellWidth*5;
		GUI.Box(new Rect(x, y, cellWidth*6, cellHeight), abilityScorePointsAvailable.ToString());
		y += cellHeight;
		x = xOrig;

		GUI.Box(new Rect(x, y, cellWidth*5, cellHeight), "Sturdy:");
		x += cellWidth*5;
		if(sturdyScore == levelingUpCharacter.abilityScores.getSturdy())
		{
			GUI.enabled = false;
		}
		if(GUI.Button(new Rect(x, y, cellWidth, cellHeight), "<"))
		{
			abilityScorePointsAvailable++;
			sturdyScore--;
		}
		x += cellWidth;
		GUI.enabled = true;
		
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight), sturdyScore.ToString());
		x += cellWidth*4;
		if(abilityScorePointsAvailable == 0)
		{
			GUI.enabled = false;
		}
		if(GUI.Button(new Rect(x, y, cellWidth, cellHeight), ">"))
		{
			abilityScorePointsAvailable--;
			sturdyScore++;
		}
		x += cellWidth;
		GUI.enabled = true;
		GUI.Box(new Rect(x, y, cellWidth*5, cellHeight), "Health:");
		y += cellHeight;
		x = xOrig;
		
		GUI.Box(new Rect(x, y, cellWidth*5, cellHeight), "Perception:");
		x += cellWidth*5;
		if(perceptionScore == levelingUpCharacter.abilityScores.getPerception())
		{
			GUI.enabled = false;
		}
		if(GUI.Button(new Rect(x, y, cellWidth, cellHeight), "<"))
		{
			abilityScorePointsAvailable++;
			perceptionScore--;
		}
		x += cellWidth;
		GUI.enabled = true;
		
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight), perceptionScore.ToString());
		x += cellWidth*4;
		if(abilityScorePointsAvailable == 0)
		{
			GUI.enabled = false;
		}
		if(GUI.Button(new Rect(x, y, cellWidth, cellHeight), ">"))
		{
			abilityScorePointsAvailable--;
			perceptionScore++;
		}
		x += cellWidth;
		GUI.enabled = true;
		GUI.Box(new Rect(x, y, cellWidth*5, cellHeight), (sturdyScore + perceptionScore + levelingUpCharacter.characterProgress.getCharacterClass().getClassModifiers().getHealthModifier() + levelingUpCharacter.personalInfo.getCharacterRace().getHealthModifier()).ToString());
		y += cellHeight;
		x = xOrig;
		
		GUI.Box(new Rect(x, y, cellWidth*5, cellHeight), "Technique:");
		x += cellWidth*5;
		if(techniqueScore == levelingUpCharacter.abilityScores.getTechnique())
		{
			GUI.enabled = false;
		}
		if(GUI.Button(new Rect(x, y, cellWidth, cellHeight), "<"))
		{
			abilityScorePointsAvailable++;
			techniqueScore--;
		}
		x += cellWidth;
		GUI.enabled = true;
		
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight), techniqueScore.ToString());
		x += cellWidth*4;
		if(abilityScorePointsAvailable == 0)
		{
			GUI.enabled = false;
		}
		if(GUI.Button(new Rect(x, y, cellWidth, cellHeight), ">"))
		{
			abilityScorePointsAvailable--;
			techniqueScore++;
		}
		x += cellWidth;
		GUI.enabled = true;
		GUI.Box(new Rect(x, y, cellWidth*5, cellHeight), "Composure:");
		y += cellHeight;
		x = xOrig;
		
		GUI.Box(new Rect(x, y, cellWidth*5, cellHeight), "Well-Versed:");
		x += cellWidth*5;
		if(wellVersedScore == levelingUpCharacter.abilityScores.getWellVersed())
		{
			GUI.enabled = false;
		}
		if(GUI.Button(new Rect(x, y, cellWidth, cellHeight), "<"))
		{
			abilityScorePointsAvailable++;
			wellVersedScore--;
		}
		x += cellWidth;
		GUI.enabled = true;
		
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight), wellVersedScore.ToString());
		x += cellWidth*4;
		if(abilityScorePointsAvailable == 0)
		{
			GUI.enabled = false;
		}
		if(GUI.Button(new Rect(x, y, cellWidth, cellHeight), ">"))
		{
			abilityScorePointsAvailable--;
			wellVersedScore++;
		}
		x += cellWidth;
		GUI.enabled = true;

		GUI.Box(new Rect(x, y, cellWidth*5, cellHeight), (techniqueScore + wellVersedScore + levelingUpCharacter.characterProgress.getCharacterClass().getClassModifiers().getComposureModifier() + levelingUpCharacter.personalInfo.getCharacterRace().getComposureModifier()).ToString());
		y += cellHeight;
		x = xOrig;
		
		GUI.Box(new Rect(x, y, cellWidth*11, cellHeight), "Combat Scores:");
		y += cellHeight;
		x = xOrig;
		GUI.Box(new Rect(x, y, cellWidth*5, cellHeight), "Initiative:");
		x += cellWidth*5;
		GUI.Box(new Rect(x, y, cellWidth*6, cellHeight), calculateMod(sturdyScore).ToString());
		y += cellHeight;
		x = xOrig;
		GUI.Box(new Rect(x, y, cellWidth*5, cellHeight), "Critical:");
		x += cellWidth*5;
		GUI.Box(new Rect(x, y, cellWidth*6, cellHeight), calculateMod(perceptionScore).ToString());
		y += cellHeight;
		x = xOrig;
		GUI.Box(new Rect(x, y, cellWidth*5, cellHeight), "Handling:");
		x += cellWidth*5;
		GUI.Box(new Rect(x, y, cellWidth*6, cellHeight), calculateMod(techniqueScore).ToString());
		y += cellHeight;
		x = xOrig;
		GUI.Box(new Rect(x, y, cellWidth*5, cellHeight), "Dominion:");
		x += cellWidth*5;
		GUI.Box(new Rect(x, y, cellWidth*6, cellHeight), calculateMod(wellVersedScore).ToString());
		
	
	}

	static GUIStyle unitInfoStyle;
	static GUIStyle getUnitInfoStyle(int fontSize) {
		if (unitInfoStyle==null) {
			unitInfoStyle = new GUIStyle("Label");
			unitInfoStyle.font = Resources.Load<Font>("Fonts/Courier New");
			unitInfoStyle.active.textColor = unitInfoStyle.normal.textColor = unitInfoStyle.hover.textColor = Color.black;
		}
		unitInfoStyle.fontSize = fontSize;
		return unitInfoStyle;
	}

}
