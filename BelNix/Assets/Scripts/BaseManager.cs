using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.IO;
using UnityEngine.UI;

[System.Serializable]
public class BlackMarketItem {
	public EditorItem editorItem;
	public Item item;
	public void setupItem() { item = editorItem.getItem(); }
}
[System.Serializable]
public class BlackMarketSection {
	public string sectionName = "";
	public List<BlackMarketItem> items = new List<BlackMarketItem>();
	public void setupItems() { foreach (BlackMarketItem i in items) i.setupItem(); }
}

public class BaseManager : MonoBehaviour  {

    //Purse partyPurse = new Purse();
        // This should be initialized at the beginning of a new game
        // This has to be updated or added to each time a new character is created
        // This has to be added to whenever a mission ends
        // This has to be subtracted from whenever a purchase is made
	// (or upkeep is charged)
	public enum BaseState  { Save, Mission, Barracks, Infirmary, Engineering, None };
	[Space(10)]
	[Header("Black Market")]
	public List<BlackMarketSection> blackMarket = new List<BlackMarketSection>();
	public GameObject blackMarketItemPrefab;
	public RectTransform blackMarketScrollRect;
	public Transform blackMarketScrollContent;
	public Scrollbar blackMarketScrollBar;
	public GameObject blackMarketCanvas;
	public Transform blackMarketTabContent;
	public GameObject blackMarketTabPrefab;
	public Text blackMarketFundsText;
	public BlackMarketSection currentSection = null;
	
	[Space(20)]
	[SerializeField]
	private InventoryGUI inventory; 
	[SerializeField]
	private GameObject baseGUI;

	private BaseState baseState = BaseState.None;
	Character displayedCharacter = null;
	Character hoveredCharacter = null;
	Character levelingUpCharacter = null;
	public List<Character> units;

	[Space(10)]
	[Header("Inventory")]
	public Stash stash = new Stash();

	string saveName = "";
	string[] saves;
//	bool saving = false;
	bool levelup = false;
	bool middleDraggin = false;
	bool rightDraggin = false;
	const int perUnitInfirmaryCost = 2;

	bool mouseLeftDown;
	bool mouseRightDown;
	bool mouseMiddleDown;

	string[] missions = new string[] {"The Warehouse"};//, "Test Map 1"};
	int[] missionLevels = new int[] {5};//, 3};
	Vector2 savesScrollPos = new Vector2();
	Vector2 barracksScrollPos = new Vector2();

	GameObject hoveredObject;
	[SerializeField] private GameObject map;
	[SerializeField] private GameObject barracks;
	[SerializeField] private GameObject barracksEntryTemplate;
    [SerializeField] private GameObject newClassFeaturesPrompt;

	string tooltip = "";
	Dictionary<string, string> tooltips = null;

	int oldTouchCount = 0;
	Vector3 lastPos = new Vector3();
	static Texture2D barracksTexture;
	static Texture2D bottomSheetTexture;
	// Use this for initialization

	
	void Awake() {
		stash.loadStash();
		foreach (BlackMarketSection section in blackMarket) {
			section.setupItems();
		}
		currentSection = blackMarket[0];
	}
	public void openBlackMarket() {
		blackMarketCanvas.SetActive(true);
		setUpTabs();
		if (currentSection != null) {
			openSection(currentSection, currentSell);
		}
		else {
			currentSection = blackMarket[0];
			currentSell = false;
			openSection(currentSection, currentSell);
		}

	}
	public void setUpTabs() {
		for (int n=blackMarketTabContent.childCount-1;n>=0;n--) {
			GameObject.Destroy(blackMarketTabContent.GetChild(n).gameObject);
		}
		foreach (BlackMarketSection section in blackMarket) {
			GameObject sec = (GameObject)Instantiate(blackMarketTabPrefab);
			BlackMarketTabButton tabButton = sec.GetComponent<BlackMarketTabButton>();
			tabButton.setupTab(section, this, false);
			sec.transform.SetParent(blackMarketTabContent, false);
		}
		BlackMarketSection section2 = new BlackMarketSection();
		section2.sectionName = "Sell";
		GameObject sec2 = (GameObject)Instantiate(blackMarketTabPrefab);
		BlackMarketTabButton tabButton2 = sec2.GetComponent<BlackMarketTabButton>();
		tabButton2.setupTab(section2, this, true);
		sec2.transform.SetParent(blackMarketTabContent, false);
	}
	public void setCanAffordItems() {
		blackMarketFundsText.text = UnitGUI.getSmallCapsString("Funds: " + stash.moneyString(), 18);
		for (int n=0;n<blackMarketScrollContent.childCount;n++) {
			BlackMarketItemContainer ic = blackMarketScrollContent.GetChild(n).GetComponent<BlackMarketItemContainer>();
			ic.setCanAfford();
		}
	}
	public void closeBlackMarket() {
		blackMarketCanvas.SetActive(false);
	}
	bool currentSell = false;
	public void openSection(BlackMarketSection section, bool sell) {
		currentSection = section;
		currentSell = sell;
		for (int n=blackMarketScrollContent.childCount-1;n>=0;n--) {
			GameObject.Destroy(blackMarketScrollContent.GetChild(n).gameObject);
		}
		if (sell) {
			foreach (Item item in stash.items) {
				BlackMarketItem bmi = new BlackMarketItem();
				bmi.item = item;
				GameObject newItem = (GameObject)Instantiate(blackMarketItemPrefab);
				BlackMarketItemContainer container = newItem.GetComponent<BlackMarketItemContainer>();
				container.setUp(bmi, this, sell);
				newItem.transform.SetParent(blackMarketScrollContent, false);
			}
		}
		else {
			foreach (BlackMarketItem item in section.items) {
				GameObject newItem = (GameObject)Instantiate(blackMarketItemPrefab);
				BlackMarketItemContainer container = newItem.GetComponent<BlackMarketItemContainer>();
				container.setUp(item, this, sell);
				newItem.transform.SetParent(blackMarketScrollContent, false);
			}
		}
		setBlackMarketContentLayoutMinSize();
	}
	public void setBlackMarketContentLayoutMinSize() {
		blackMarketScrollContent.GetComponent<LayoutElement>().minHeight = blackMarketScrollRect.sizeDelta.y;
		blackMarketScrollBar.value = 0.9990f;
		Invoke("setBlackMarketScrollBar", 0.0108f);
	}

	public void setBlackMarketScrollBar() {
		blackMarketScrollBar.value = 1.0f;
	}
	public void buyItem(Item i) {
		stash.spendMoney(i.getPrice());
		stash.addItem(i);
		setCanAffordItems();
	}

	public void sellItem(Item i) {
		stash.addMoney(Mathf.FloorToInt(i.getBlackMarketSellPrice()));
		stash.removeItem(i);
		setCanAffordItems();
		openSection(currentSection, currentSell);
	}
	public void setInventory(Character character) {
		InventoryGUI.setupInvent(character);
		InventoryGUI.clearLootItems();
		InventoryGUI.setLootItems(stash.items, null, stash);
	}



	public bool mapShown = false;
	public void enableMap() {
		mapShown = true;
		map.SetActive(true);
	}
	public void disableMap() {
		mapShown = false;
		map.SetActive(false);
	}

	public bool barracksShown = false;
	public void enableBarracks() {
		barracksShown = true;
		barracks.SetActive(true);
	}
	public void disableBarracks() {
		barracksShown = false;
		barracks.SetActive(false);
	}
	public void initializeBarracks(List<Character> characterList) {
		enableBarracks();
		barracks.GetComponent<BarracksManager>().fillBarracks(barracksEntryTemplate, characterList);
		disableBarracks();
	}

    public bool newClassFeaturesPromptShown = false;
    private BarracksEntry entryWaitingOnFeatureSelection;
    public void enableNewClassFeaturePrompt(ClassFeature[] newFeatures, BarracksEntry originator)
    {
        entryWaitingOnFeatureSelection = originator;
        newClassFeaturesPromptShown = true;
        newClassFeaturesPrompt.SetActive(true);
        if (newFeatures.Length > 1)
            newClassFeaturesPrompt.GetComponent<NewClassFeature>().format(newFeatures[0], newFeatures[1]);
        else
            newClassFeaturesPrompt.GetComponent<NewClassFeature>().format(newFeatures[0]);
        Debug.Log(ClassFeatures.getName(newFeatures[0]));
    }

    public void disableNewClassFeaturePrompt(ClassFeature selectedFeature, int choiceNumber)
    {
        newClassFeaturesPromptShown = false;
        newClassFeaturesPrompt.SetActive(false);
        entryWaitingOnFeatureSelection.receiveClassFeatureChoice(selectedFeature, choiceNumber);
    }



	void Start ()  {
	//	Item item = new Turret(new TestFrame(), new TestApplicator(), new TestGear(), new TestEnergySource());
	//	Item item = Item.deserializeItem((ItemCode)4,"5,,124,0,Units/Turrets/TurretPlaceholder,0,11,2:Test Frame:0:0::0:65,14,0:Test Applicator:30:0:Units/Turrets/Applicator:0:0:1:1:6:0:1:5:70:0:0,15,6:Test Gear:0:0:Units/Turrets/Gear:0,12,6:Test Energy Source:0:0:Units/Turrets/EnergySource:0:2");
	//	Debug.Log(item.getItemCode() + "   " + (int)item.getItemCode() + "   \n" + item.getItemData());
	//	BinaryFormatter bf = new BinaryFormatter();
	/*	XmlSerializer bf = new XmlSerializer(item.GetType());
		using (StringWriter textWriter = new StringWriter())  {
			bf.Serialize(textWriter, item);
			Debug.Log(textWriter.ToString());
		}*/
	
		units = new List<Character>();
		string[] chars = Saves.getCharacterList();
		for (int n=0;n<chars.Length-1;n++)  {
			Character ch = new Character();
			ch.loadCharacterFromTextFile(chars[n]);
			ch.characterId = chars[n];
			units.Add(ch);
			if (ch.characterSheet.inventory.purse.money > 0) {
				stash.addMoney(ch.characterSheet.inventory.purse.money);
				ch.characterSheet.inventory.purse.money = 0;
				ch.saveCharacter();
				setCanAffordItems();
			}
		}
        InventoryGUI.setInventoryGUI(inventory);
		setCanAffordItems();
        //InventoryGUI.setupInvent();
        //InventoryGUI.setInventoryShown(false)
        initializeBarracks(units);
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
		do  {
			nn++;
			saveName = "Save " + nn;
		} while (Saves.hasSaveFileNamed(saveName));
	}

	int height = 0;
	// Update is called once per frame
	void Update ()  {
		handleInput();
		UnitGUI.doTabs();
		if (height != (int)blackMarketScrollRect.sizeDelta.y) {
			height = (int)blackMarketScrollRect.sizeDelta.y;
			setBlackMarketContentLayoutMinSize();
		}
	}

	void handleInput()  {
		handleKeys();
		handleDrag();
		handleKeyPan();
        handleMouseMovement();
        handleMouseClick();
	}

	void handleMouseClick()  {
        if(EventSystem.current.IsPointerOverGameObject())
            return;
        Vector2 mouse = Input.mousePosition;
		mouse.y = Screen.height - mouse.y;
		if (Input.GetMouseButtonDown(0))  {
			if (hoveredObject != null)  {
				if (hoveredObject.tag == "exit")
					Application.LoadLevel(0);
				else if (hoveredObject.tag=="map")  {
					enableMap();
					//loadMapScrollPos = new Vector2();
					//baseState = BaseState.Mission;
				}
				else if (hoveredObject.tag=="barracks")  {
					//barracksScrollPos = new Vector2();
					//displayedCharacter = null;
					//baseState = BaseState.Barracks;
                   enableBarracks();
				}
				else if (hoveredObject.tag=="newcharacter")  {
					PlayerPrefs.SetInt("playercreatefrom", Application.loadedLevel);
					Application.LoadLevel(1);
				}
				else if (hoveredObject.tag=="infirmary")  {
					baseState = BaseState.Infirmary;
				}
				else if (hoveredObject.tag=="engineering")  {
					barracksScrollPos = new Vector2();
					displayedCharacter = null;
					baseState = BaseState.Engineering;
				}
			}
		
			if (!levelup && !(UnitGUI.containsMouse(mouse) && displayedCharacter!=null) && levelingUpCharacter == null)  {
				if (baseState==BaseState.Barracks)  {
					if (hoveredCharacter == displayedCharacter) displayedCharacter = null;
					else if (hoveredCharacter != null) displayedCharacter = hoveredCharacter;
				}
				else if (baseState == BaseState.Engineering)  {
					if (displayedCharacter == null) displayedCharacter = hoveredCharacter;
				}
			}
			selectItem(displayedCharacter);
		}
		if (Input.GetMouseButtonUp(0))  {
			deselectItem(displayedCharacter);
		}
		if (UnitGUI.containsMouse(mouse) && Input.GetMouseButtonDown(0) && !rightDraggin && !middleDraggin)  {
		/*	if (UnitGUI.inventoryOpen && displayedCharacter != null)  {
				UnitGUI.selectItem(displayedCharacter);
				//		selectedUnit.selectItem();
			}*/
		}
		if (Input.GetMouseButtonUp(0) && !rightDraggin && !middleDraggin)  {
		/*	if (UnitGUI.inventoryOpen && displayedCharacter != null)  {
				//				selectedUnit.deselectItem();
				UnitGUI.deselectItem(displayedCharacter);
			}*/
		}
        newClassFeaturesPrompt.SetActive(false);
	}

	
	void handleKeyPan()  {
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
	void handleKeys()  {
		shiftDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		altDown = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
		controlDown = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
		commandDown = Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand);
		mouseLeftDown = Input.GetMouseButton(0);
		mouseRightDown = Input.GetMouseButton(1);
		mouseMiddleDown = Input.GetMouseButton(2);
		if (!rightDraggin) middleDraggin = ((mouseMiddleDown || (mouseLeftDown && Input.touchCount==2)) && middleDraggin) || Input.GetMouseButtonDown(2);
		if (!middleDraggin) rightDraggin = (rightDraggin && mouseRightDown) || Input.GetMouseButtonDown(1);
		if (baseState==BaseState.Barracks && displayedCharacter!=null)  {
			if (Input.GetKeyDown(KeyCode.C))  {
				UnitGUI.clickTab(Tab.C);
			}
			if (Input.GetKeyDown(KeyCode.V))  {
				UnitGUI.clickTab(Tab.V);
			}
			if (Input.GetKeyDown(KeyCode.B))  {
				UnitGUI.clickTab(Tab.B);
			}
		}
		if (shiftDown && controlDown && (altDown || commandDown))  {
			if (displayedCharacter != null)  {
				if ((commandDown && Input.GetKey(KeyCode.Minus)) || Input.GetKeyDown(KeyCode.Minus))  {
					if (expChanged != null && expChanged != displayedCharacter) expChanged.saveCharacter();
					expChanged = displayedCharacter;
					displayedCharacter.characterSheet.characterProgress.setExperience(Mathf.Max(0,displayedCharacter.characterSheet.characterProgress.getCharacterExperience()-100));
					displayedCharacter.saveCharacter();
				}
				if ((commandDown && Input.GetKey(KeyCode.Equals)) || Input.GetKeyDown(KeyCode.Equals))  {
					if (expChanged != null && expChanged != displayedCharacter) expChanged.saveCharacter();
					expChanged = displayedCharacter;
					displayedCharacter.characterSheet.characterProgress.addExperience(100);
					displayedCharacter.saveCharacter();
				}
				if ((commandDown && Input.GetKey(KeyCode.Alpha9)) || Input.GetKeyDown(KeyCode.Alpha9))  {
					if (expChanged != null && expChanged != displayedCharacter) expChanged.saveCharacter();
					expChanged = displayedCharacter;
					//displayedCharacter.characterProgress.setExperience(Mathf.Max(0,displayedCharacter.characterProgress.getCharacterExperience()-100));
					displayedCharacter.characterSheet.characterProgress.setLevel(displayedCharacter.characterSheet.characterProgress.getCharacterLevel()-1);
					displayedCharacter.saveCharacter();
				}
			}
			if ((commandDown && Input.GetKey (KeyCode.RightBracket)) || Input.GetKeyDown(KeyCode.RightBracket))  {
			//	units[0].characterSheet.inventory.purse.receiveMoney(10);
			//	units[0].saveCharacter();
				stash.addMoney(10);
				setCanAffordItems();
			}
			if ((commandDown && Input.GetKey (KeyCode.LeftBracket)) || Input.GetKeyDown(KeyCode.LeftBracket))  {
			//	units[0].characterSheet.inventory.purse.spendMoney(10);
			//	units[0].saveCharacter();
				stash.spendMoney(10);
				setCanAffordItems();
			}
		}
		if (expChanged!=null)  {
			if (Input.GetKeyUp(KeyCode.Equals) || Input.GetKeyUp(KeyCode.Plus))  {
				expChanged.saveCharacter();
				expChanged = null;
			}
		}
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (InventoryGUI.isShown) {
				InventoryGUI.selectedCharacter.saveCharacter();
				InventoryGUI.setInventoryShown(false);
			}
			else if (barracksShown) {
				disableBarracks();
			}

		}
	}

	void handleDrag()  {
		return;
		var mPos = Input.mousePosition;
		mPos.z = 10.0f;
		Vector3 pos1 = Camera.main.ScreenToWorldPoint(mPos);
		if (((middleDraggin && Input.touchCount == oldTouchCount) || rightDraggin))  {//  && Input.mousePosition.x < Screen.width*(1-boxWidthPerc))  {
			//= mainCamera.WorldToScreenPoint(cameraTransform.position);
			if (!Input.GetMouseButtonDown(0))  {
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

	void handleMouseMovement()  {
        if (hoveredObject != null && EventSystem.current.IsPointerOverGameObject())
        {
            hoveredObject.transform.localScale = new Vector3(1, 1, 1);
            return;
        }

        bool old = hoveredObject==null;
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 100.0f, 1<<13);
		//		Physics2D.Ray
		GameObject go = null;
		if (hit.collider != null) go = hit.collider.gameObject;
		if (baseState != BaseState.None) go = null;
		if (go != hoveredObject)  {
			if (hoveredObject != null)  {
				hoveredObject.transform.localScale = new Vector3(1, 1, 1);
			}
			hoveredObject = go;
			if (hoveredObject != null)  {
				hoveredObject.transform.localScale = new Vector3(scale, scale, scale);
			}
		}
        if (go != null) {
            //Dictionary<string, string> dict = tooltips;
            //GameObject gam = go;
            //string tag = gam.tag;
            //Debug.Log(dict);
            tooltip = tooltips[go.tag];
        }
        else tooltip = "";
		if (go==null && !old) handleMouseMovement();
	}

	static GUIStyle saveButtonsStyle = null;
	public static GUIStyle getSaveButtonsStyle()  {
		if (saveButtonsStyle==null)  {
			saveButtonsStyle = new GUIStyle("Button");
			saveButtonsStyle.active.background = saveButtonsStyle.hover.background = saveButtonsStyle.normal.background = null;
			saveButtonsStyle.alignment = TextAnchor.MiddleLeft;
		}
		return saveButtonsStyle;
	}

	string oldSaveName;
//	bool choosingMap = false;
	Vector2 loadMapScrollPos = new Vector2();
	void OnGUI()  {
		hoveredCharacter = null;
		if (baseState == BaseState.None)  {
			if (GUI.Button(new Rect(0, 0, 150, 75), "Save Game"))  {
				saves = Saves.getSaveFiles();
				baseState = BaseState.Save;
				oldSaveName = saveName;
				savesScrollPos = new Vector2();
				string savesSt = "";
				foreach (string save in saves)  {
					savesSt += save + "\n";
				}
			}
			if (GUI.Button(new Rect(Screen.width/4, 0, 150, 75), "Black Market")) {
				openBlackMarket();
			}
	//		Vector3 mousePos = Input.mousePosition;
	//		mousePos.y = Screen.height - mousePos.y;
			GUIContent toolContent = new GUIContent(tooltip);
			GUIStyle st = GUI.skin.label;
			st.fontSize = 24;
			Vector2 size = st.CalcSize(toolContent);
			float x = Screen.width/2 - size.x/2;
	//		if (x + size.x + 5.0f > Screen.width) x = mousePos.x - size.x;//x = Screen.width - size.x - 5.0f;
	//		float y = mousePos.y - size.y;
			float y = 80.0f;	
			GUI.Label(new Rect(x, y, size.x, size.y), toolContent, st);
		}
		else if (baseState == BaseState.Mission)  {
			float boxHeight = 250.0f;
			float boxWidth = 200.0f;
			float boxX = (Screen.width - boxWidth)/2.0f;
			float boxY = (Screen.height - boxHeight)/2.0f;
			float buttX = boxX + 20.0f;
			float buttWidth = boxWidth - 20.0f*2.0f;
			GUI.Box(new Rect(boxX, boxY, boxWidth, boxHeight), "Select Mission");
			GUI.BeginScrollView(new Rect(boxX, boxY + 25.0f, boxWidth, 40.0f * 4), loadMapScrollPos, new Rect(boxX, boxY + 25.0f, boxWidth - 16.0f, 40.0f * missions.Length));
			for (int n=0;n<Mathf.Min(missions.Length, missionLevels.Length); n++)  {
				if (GUI.Button(new Rect(buttX, boxY + 25.0f + 40.0f * n, buttWidth, 40.0f), missions[n]))  {
					Application.LoadLevel(missionLevels[n]);
				}
			}
			GUI.EndScrollView();
			if (GUI.Button(new Rect(buttX, boxY + 25.0f + 40.0f * 4 + 10.0f, buttWidth, 40.0f), "Cancel"))  {
				baseState = BaseState.None;
			}
		}
		else if (baseState == BaseState.Save)  {
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
			if (GUI.Button(new Rect(buttonX1, buttonY, buttonWidth, buttonHeight), "Cancel"))  {
				baseState = BaseState.None;
				saveName = oldSaveName;
			}
			if (GUI.Button(new Rect(buttonX2, buttonY, buttonWidth, buttonHeight), "Save"))  {
				Saves.saveAs(saveName);
				baseState = BaseState.None;
			}
			float textFieldHeight = 25.0f;
			saveName = GUI.TextField(new Rect(x + 5.0f, y + 5.0f, width - 10.0f, textFieldHeight), saveName);
			float savesHeight = 0.0f;
			GUIStyle st = getSaveButtonsStyle();
			foreach (string save in saves)  {
				savesHeight += st.CalcSize(new GUIContent(save)).y;
			}
			y += 5.0f + textFieldHeight + 5.0f;
			float scrollHeight = buttonY - y - 5.0f;
			float scrollX = x + 5.0f;
			float scrollWidth = width - (scrollX - x) * 2.0f;
			savesScrollPos = GUI.BeginScrollView(new Rect(scrollX, y, scrollWidth, scrollHeight), savesScrollPos, new Rect(scrollX, y, scrollWidth - 16.0f, savesHeight));
			foreach (string save in saves)  {
				GUIContent gc = new GUIContent(save);
				float h = st.CalcSize(gc).y;
				if (GUI.Button(new Rect(scrollX, y, scrollWidth, h), gc, st))  {
					saveName = save;
				}
				y += h;
			}
			GUI.EndScrollView();
		}
		else if (baseState == BaseState.Infirmary)  {
			Vector2 boxSize = new Vector2(200.0f, 300.0f);
			Vector2 boxOrigin = new Vector2((Screen.width - boxSize.x)/2.0f, (Screen.height - boxSize.y)/2.0f);
			float y = boxOrigin.y;
			GUI.Box(new Rect(boxOrigin.x, boxOrigin.y, boxSize.x, boxSize.y), "Infirmary");
			y += 30.0f;
			float x = boxOrigin.x + 5.0f;
			int cost = perUnitInfirmaryCost * units.Count;
			bool enabled = stash.canAfford(cost);//units[0].characterSheet.inventory.purse.enoughMoney(cost);
			bool enabled2 = false;
			foreach (Character c in units)  {
				CombatScores cs = c.characterSheet.combatScores;
				if (cs.getCurrentHealth() < cs.getMaxHealth() || cs.getCurrentComposure() < cs.getMaxComposure())  {
					enabled2 = true;
					break;
				}
			}
		//	Purse p = units[0].characterSheet.inventory.purse;
			GUIContent infContent = new GUIContent((enabled2 ? (enabled ? "Rest for the day.\nIt will cost " + Purse.moneyString(cost) + " to rest.\n\n" : "You do not have enough money to rest. \nIt will cost " + cost + "c to rest.\n\n") : "All of your units are fully rested.\n\n") + "Purse: " + stash.moneyString());
			float infSize = GUI.skin.label.CalcHeight(infContent, boxSize.x - 10.0f);
			GUI.Label(new Rect(x, y, boxSize.x - 10.0f, infSize), infContent);
			y += infSize + 5.0f;
			Vector2 buttonSize = new Vector2(90.0f, 40.0f);
			if (GUI.Button(new Rect(Screen.width/2.0f - buttonSize.x - 5.0f, boxOrigin.y + boxSize.y - buttonSize.y - 20.0f, buttonSize.x, buttonSize.y), "Cancel"))  {
				baseState = BaseState.None;
			}
			GUI.enabled = enabled && enabled2;
			if (GUI.Button(new Rect((Screen.width)/2.0f + 5.0f, boxOrigin.y + boxSize.y - buttonSize.y - 20.0f, buttonSize.x, buttonSize.y), "Rest"))  {
				for (int n=0;n<units.Count;n++)  {
					Character c = units[n];
					int health = c.characterSheet.combatScores.getCurrentHealth();
					int maxHealth = c.characterSheet.combatScores.getMaxHealth();
					bool changed = false;
					if (n==0)  {
						//c.characterSheet.inventory.purse.spendMoney(cost);
						stash.spendMoney(cost);
						setCanAffordItems();
						changed = true;
					}
					if (health < maxHealth)  {
						if (health < 0) c.characterSheet.combatScores.addHealth(1);
						else c.characterSheet.combatScores.setHealth(maxHealth);
						changed = true;
					}
					if (c.characterSheet.combatScores.getCurrentComposure() < c.characterSheet.combatScores.getMaxComposure())  {
						c.characterSheet.combatScores.setComposure(c.characterSheet.combatScores.getMaxComposure());
						changed = true;
					}
					if (changed)  {
						c.saveCharacter();
					}
				}
			}
		}
		else if (baseState == BaseState.Engineering)  {
			int numHeight = 8;
			float topHeight = 20.0f;
			float buttonHeight = 40.0f;
			float buttonWidth = 200.0f;
			float bottomHeight = buttonHeight + 5.0f*2;
			Vector2 eachSize = new Vector2(476, 79);
			Vector2 totalSize = new Vector2(eachSize.x + 20.0f*2, eachSize.y * numHeight + topHeight + bottomHeight);
			while (totalSize.y > Screen.height - 50.0f && numHeight > 2)  {
				numHeight--;
				totalSize = new Vector2(eachSize.x + 20.0f*2, eachSize.y * numHeight + topHeight + bottomHeight);
			}
			Vector2 boxOrigin = new Vector2((Screen.width - totalSize.x)/2.0f, (Screen.height - totalSize.y)/2.0f);
			GUI.Box(new Rect(boxOrigin.x, boxOrigin.y, totalSize.x, totalSize.y), "Workbench");


			List<Character> engineerUnits = new List<Character>();
			foreach (Character c in units)  {
				if (c.characterSheet.characterProgress.getCharacterClass().getClassName()==ClassName.Engineer)  {
					engineerUnits.Add(c);
				}
			}
			GUI.Label(new Rect(boxOrigin.x + 20.0f, boxOrigin.y + topHeight + 25.0f, eachSize.x, eachSize.y), "Select a unit to create traps and turrets:");


			Rect scrollRect = new Rect(boxOrigin.x + 20.0f, boxOrigin.y + topHeight + eachSize.y, eachSize.x + 16.0f, eachSize.y * (numHeight - 1));
			bool inScroll = scrollRect.Contains(Event.current.mousePosition);
		/*	if (UnitGUI.containsMouse(Event.current.mousePosition) && displayedCharacter!=null)  {
				GUI.enabled = false;
			}*/
			barracksScrollPos = GUI.BeginScrollView(scrollRect, barracksScrollPos, new Rect(boxOrigin.x + 20.0f, boxOrigin.y + topHeight + eachSize.y, eachSize.x, eachSize.y * engineerUnits.Count));
			GUI.enabled = true;
			for (int n=0;n<engineerUnits.Count;n++)  {
				Character u = engineerUnits[n];
				Rect totalRect = new Rect(boxOrigin.x + 20.0f, boxOrigin.y + topHeight + (n+1)*eachSize.y, eachSize.x, eachSize.y);
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
				GUI.Label(new Rect(boxOrigin.x + 25.0f, boxOrigin.y + topHeight + (n+1)*eachSize.y, eachSize.x - 5.0f, eachSize.y), infoCont, st);
				string exp = UnitGUI.getSmallCapsString("Level", smallSize) + ":" + startSize + u.characterSheet.characterProgress.getCharacterLevel() + endSize + "\n" +
					UnitGUI.getSmallCapsString("Experience", smallSize) + ":" + startSize + u.characterSheet.characterProgress.getCharacterExperience() + "/" + (u.characterSheet.characterProgress.getCharacterLevel()*100) + endSize + "\n" +
						UnitGUI.getSmallCapsString("Health", smallSize) + ":" + startSize + u.characterSheet.combatScores.getCurrentHealth() + "/" + u.characterSheet.combatScores.getMaxHealth() + endSize + "\n" +
						UnitGUI.getSmallCapsString("Composure", smallSize) + ":" + startSize + u.characterSheet.combatScores.getCurrentComposure() + "/" + u.characterSheet.combatScores.getMaxComposure() + endSize;
				GUIContent expCont = new GUIContent(exp);
				Vector2 expSize = st.CalcSize(expCont);
				float expWidth = 150.0f;
				float expX = boxOrigin.x + 20.0f + eachSize.x - expWidth;
				float y = boxOrigin.y + topHeight + (n+1)*eachSize.y;
				GUI.Label(new Rect(expX, y, expWidth, eachSize.y), expCont, st);
				Vector2 levelUpSize = new Vector2(70.0f, expSize.y/2.0f - 15.0f);
				Rect levelUpRect = new Rect(expX - levelUpSize.x - 5.0f, y + 10.0f, levelUpSize.x, levelUpSize.y);

				if (inScroll)  {
					if (totalRect.Contains(Event.current.mousePosition)) hoveredCharacter = u;
				}
			}
			if (UnitGUI.containsMouse(Event.current.mousePosition) && displayedCharacter!=null)  {
				GUI.enabled = false;
			}
			GUI.EndScrollView();
			GUI.enabled = true;
			if (GUI.Button(new Rect((Screen.width - buttonWidth)/2.0f, boxOrigin.y + totalSize.y - 5.0f - buttonHeight, buttonWidth, buttonHeight), "Cancel"))  {
				baseState = BaseState.None;
				displayedCharacter = null;
			}
			/*
			if (displayedCharacter != null)  {
				UnitGUI.drawGUI(displayedCharacter, null, null);
			}
			else if (hoveredCharacter != null)  {
				UnitGUI.drawGUI(hoveredCharacter, null, null);
			}*/
			if (displayedCharacter != null)  {
			//	UnitGUI.drawGUI(displayedCharacter, null, null);
				drawWorkbenchGUI();
			}






		}
		else if (baseState == BaseState.Barracks)  {
			int numHeight = 8;
			float topHeight = 20.0f;
			float buttonHeight = 40.0f;
			float buttonWidth = 200.0f;
			float bottomHeight = buttonHeight + 5.0f*2;
			Vector2 eachSize = new Vector2(476, 79);
			Vector2 totalSize = new Vector2(eachSize.x + 20.0f*2, eachSize.y * numHeight + topHeight + bottomHeight);
			while (totalSize.y > Screen.height - 50.0f && numHeight > 1)  {
				numHeight--;
				totalSize = new Vector2(eachSize.x + 20.0f*2, eachSize.y * numHeight + topHeight + bottomHeight);
			}
			Vector2 boxOrigin = new Vector2((Screen.width - totalSize.x)/2.0f, (Screen.height - totalSize.y)/2.0f);
			GUI.Box(new Rect(boxOrigin.x, boxOrigin.y, totalSize.x, totalSize.y), "Barracks");
			Rect scrollRect = new Rect(boxOrigin.x + 20.0f, boxOrigin.y + topHeight, eachSize.x + 16.0f, eachSize.y * numHeight);
			bool inScroll = scrollRect.Contains(Event.current.mousePosition);
			if (UnitGUI.containsMouse(Event.current.mousePosition) && displayedCharacter!=null)  {
				GUI.enabled = false;
			}
			barracksScrollPos = GUI.BeginScrollView(scrollRect, barracksScrollPos, new Rect(boxOrigin.x + 20.0f, boxOrigin.y + topHeight, eachSize.x, eachSize.y * units.Count));
			GUI.enabled = true;
			for (int n=0;n<units.Count;n++)  {
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
				if (u.characterSheet.characterProgress.canLevelUp())  {
					//if ((UnitGUI.containsMouse(Event.current.mousePosition) || guiContainsMouse()) && displayedCharacter!=null)  {
					if (levelingUpCharacter != null)  {
						GUI.enabled = false;
					}
					if (GUI.Button(levelUpRect, "Level Up!") && levelingUpCharacter == null)  {
						levelingUpCharacter = u;
						abilityScorePointsAvailable = 1;
						skillPointsAvailable = 1;

                        AbilityScores unitAbilityScores = u.characterSheet.abilityScores;
                        sturdyScore = unitAbilityScores.getSturdy();
                        perceptionScore = unitAbilityScores.getPerception(0);
                        techniqueScore  = unitAbilityScores.getTechnique();
                        wellVersedScore = unitAbilityScores.getWellVersed();

                        SkillScores unitSkillScores = u.characterSheet.skillScores;
                        athleticsSkill  = unitSkillScores.scores[0];
                        meleeSkill      = unitSkillScores.scores[1];
                        rangedSkill     = unitSkillScores.scores[2];
                        stealthSkill    = unitSkillScores.scores[3];
                        mechanicalSkill = unitSkillScores.scores[4];
                        medicinalSkill  = unitSkillScores.scores[5];
                        historicalSkill = unitSkillScores.scores[6];
                        politicalSkill  = unitSkillScores.scores[7];

						possibleFeatures = u.characterSheet.characterProgress.getCharacterClass().getPossibleFeatures(u.characterSheet.characterProgress.getCharacterLevel()+1);
						page = 0;
						selectedFeature = -1;
						selectedWeaponFocus = -1;
						selectedRace = -1;
						Debug.Log("Level Up!!");
					}
					GUI.enabled = true;
					if (levelUpRect.Contains(Event.current.mousePosition)) haslevelup = true;
				}
				if (!haslevelup && inScroll)  {
					if (totalRect.Contains(Event.current.mousePosition)) hoveredCharacter = u;
				}
			}
			if (UnitGUI.containsMouse(Event.current.mousePosition) && displayedCharacter!=null)  {
				GUI.enabled = false;
			}
			GUI.EndScrollView();
			GUI.enabled = true;
			if (GUI.Button(new Rect((Screen.width - buttonWidth)/2.0f, boxOrigin.y + totalSize.y - 5.0f - buttonHeight, buttonWidth, buttonHeight), "Cancel"))  {
				baseState = BaseState.None;
				displayedCharacter = null;
			}
		/*	if (displayedCharacter != null)  {
				UnitGUI.drawGUI(displayedCharacter, null, null);
			}*/
			if (levelingUpCharacter != null)  {
				drawLevelUpGUI();
			}
		}
	}
	
	public const float inventoryCellSize = 24.0f;
	const float inventoryLineThickness = 2.0f;
	public const float change = inventoryCellSize - inventoryLineThickness;
	public const float change2 = inventoryCellSize - inventoryLineThickness/2.0f;

	public static Vector2 getInventorySlotPos(InventorySlot slot, float baseX, float baseY)  {
		switch (slot)  {
		case InventorySlot.Frame:
			return new Vector2(baseX, baseY);
		case InventorySlot.Applicator:
			return new Vector2(baseX + change2*2 + 10.0f, baseY);
		case InventorySlot.Gear:
			return new Vector2(baseX + change2*4 + 20.0f, baseY);
		case InventorySlot.TriggerEnergySource:
			return new Vector2(baseX + change2*6 + 30.0f, baseY);
		case InventorySlot.TrapTurret:
			return new Vector2(baseX + change2*6 + 30.0f, baseY + change2*2 + 10.0f);
		case InventorySlot.Zero:
			return new Vector2(baseX, baseY);
		case InventorySlot.One:
			return new Vector2(baseX + change, baseY);
		case InventorySlot.Two:
			return new Vector2(baseX + change*2, baseY);
		case InventorySlot.Three:
			return new Vector2(baseX + change*3, baseY);
		case InventorySlot.Four:
			return new Vector2(baseX, baseY + change);
		case InventorySlot.Five:
			return new Vector2(baseX + change, baseY + change);
		case InventorySlot.Six:
			return new Vector2(baseX + change*2, baseY + change);
		case InventorySlot.Seven:
			return new Vector2(baseX + change*3, baseY + change);
		case InventorySlot.Eight:
			return new Vector2(baseX, baseY + change*2);
		case InventorySlot.Nine:
			return new Vector2(baseX + change, baseY + change*2);
		case InventorySlot.Ten:
			return new Vector2(baseX + change*2, baseY + change*2);
		case InventorySlot.Eleven:
			return new Vector2(baseX + change*3, baseY + change*2);
		case InventorySlot.Twelve:
			return new Vector2(baseX, baseY + change*3);
		case InventorySlot.Thirteen:
			return new Vector2(baseX + change, baseY + change*3);
		case InventorySlot.Fourteen:
			return new Vector2(baseX + change*2, baseY + change*3);
		case InventorySlot.Fifteen:
			return new Vector2(baseX + change*3, baseY + change*3);	
		default:
			return new Vector2();
		}
	}

	
	public static Rect getInventorySlotRect(InventorySlot slot, float baseX, float baseY)  {
		Vector2 v = getInventorySlotPos(slot, baseX, baseY);
		switch (slot)  {
		case InventorySlot.Head:
		case InventorySlot.Shoulder:
		case InventorySlot.Back:
		case InventorySlot.Chest:
		case InventorySlot.Glove:
		case InventorySlot.RightHand:
		case InventorySlot.LeftHand:
		case InventorySlot.Pants:
		case InventorySlot.Boots:
		case InventorySlot.Frame:
		case InventorySlot.Applicator:
		case InventorySlot.Gear:
		case InventorySlot.TriggerEnergySource:
		case InventorySlot.TrapTurret:
			return new Rect(v.x, v.y, inventoryCellSize*2, inventoryCellSize*2);
		case InventorySlot.Zero:
		case InventorySlot.One:
		case InventorySlot.Two:
		case InventorySlot.Three:
		case InventorySlot.Four:
		case InventorySlot.Five:
		case InventorySlot.Six:
		case InventorySlot.Seven:
		case InventorySlot.Eight:
		case InventorySlot.Nine:
		case InventorySlot.Ten:
		case InventorySlot.Eleven:
		case InventorySlot.Twelve:
		case InventorySlot.Thirteen:
		case InventorySlot.Fourteen:
		case InventorySlot.Fifteen:
			return new Rect(v.x, v.y, inventoryCellSize, inventoryCellSize);
		default:
			return new Rect();
		}
	}




	
	public float inventX = 0.0f;
	public float inventY = 0.0f;
	public float trapsTurretsX = 0.0f;
	public float trapsTurretsY = 0.0f;
	public Frame frame;
	public Applicator applicator;
	public Gear gear;
	public Item energySourceOrTrigger;
	public Item selectedItem;
	public Turret turret;
	public Trap trap;
	public InventorySlot selectedItemWasInSlot;
	public Vector3 selectedMousePos = new Vector3();
	public Vector2 selectedItemPos = new Vector2();
	public Vector2 selectedCell = new Vector2();
	public void selectItem(Character characterSheet)  {
		selectItem(characterSheet, null, null);
	}
	public void selectItem(Character characterSheet, MapGenerator mapGenerator, Unit u)  {
		Debug.Log("Select Item");
		Vector3 mousePos = Input.mousePosition;
		mousePos.y = Screen.height - mousePos.y;
		foreach (InventorySlot slot in UnitGUI.inventorySlots)  {
			Rect r = getInventorySlotRect(slot, inventX, inventY);
			if (r.Contains(mousePos))  {
				Vector2 v = UnitGUI.getIndexOfSlot(slot);
				//				Debug.Log(v);
				int ind = UnitGUI.getLinearIndexFromIndex(v);
				InventoryItemSlot sl = displayedCharacter.characterSheet.inventory.inventory[ind];
				InventoryItemSlot slR = sl.itemSlot;
				if (slR==null) break;
				//	Item i = slR.item;
				Vector2 itemSlot = Inventory.getSlotForIndex(ind);
				ItemReturn ir = characterSheet.characterSheet.inventory.removeItemFromSlot(itemSlot);
				selectedItem = ir.item;
				if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))  {
					if (selectedItem.stackSize()>1)  {
						characterSheet.characterSheet.inventory.insertItemInSlot(selectedItem, itemSlot - ir.slot);
						selectedItem = selectedItem.popStack();
					}
				}
				selectedCell = ir.slot;
				selectedMousePos = mousePos;
				//				selectedItemPos = getInventorySlotPos();
				selectedItemPos = getInventorySlotPos(UnitGUI.inventorySlots[slR.index], inventX, inventY);
				selectedItemWasInSlot = UnitGUI.inventorySlots[slR.index];
				break;
			}
		}
		foreach (InventorySlot slot in UnitGUI.trapTurretSlots)  {
			Rect r = getInventorySlotRect(slot, trapsTurretsX, trapsTurretsY);
			if (r.Contains(mousePos))  {
				Item i = null;
				switch (slot)  {
				case InventorySlot.Frame:
					i = frame;
					break;
				case InventorySlot.Applicator:
					i = applicator;
					break;
				case InventorySlot.Gear:
					i = gear;
					break;
				case InventorySlot.TriggerEnergySource:
					i = energySourceOrTrigger;
					break;
				default:
					break;
				}
				if (i==null)break;
				Vector2 size = i.getSize();
				float x = r.x + (r.width - size.x*inventoryCellSize)/2.0f;
				float y = r.y + (r.height - size.y*inventoryCellSize)/2.0f;
				float width = size.x*inventoryCellSize;
				float height = size.y*inventoryCellSize;
				Rect r2 = new Rect(x, y, width, height);
				float inventW = inventoryCellSize;
				float inventH = inventoryCellSize;
				if (!r2.Contains(mousePos))  {
					inventW = inventW * r.width/r2.width;
					inventH = inventH * r.height/r2.height;
					r2 = r;
				}
				selectedCell = new Vector2((int)((mousePos.x - r2.x)/inventW), (int)((mousePos.y - r2.y)/inventH));
				foreach (Vector2 cell in i.getShape())  {
					if (cell.x == selectedCell.x && cell.y == selectedCell.y)  {
						selectedItemPos = new Vector2(mousePos.x - (mousePos.x - r2.x)/(inventW/inventoryCellSize), mousePos.y - (mousePos.y - r2.y)/(inventH/inventoryCellSize));
						selectedMousePos = mousePos;
						selectedItem = i;
						selectedItemWasInSlot = slot;
						switch (slot)  {
						case InventorySlot.Frame:
							frame = null;
							break;
						case InventorySlot.Applicator:
							applicator = null;
							break;
						case InventorySlot.Gear:
							gear = null;
							break;
						case InventorySlot.TriggerEnergySource:
							energySourceOrTrigger = null;
							break;
						default:
							break;
						}
						trap = null;
						turret = null;
						break;
					}
				}
				if (selectedItem != null)  {
					break;
				}
			}
		}
		bool doTrapsTurrets = true;
		while (doTrapsTurrets)  {
			doTrapsTurrets = false;
			InventorySlot slot = InventorySlot.TrapTurret;
			Rect r = getInventorySlotRect(slot, trapsTurretsX, trapsTurretsY);
			if (r.Contains(mousePos))  {
				Item i = (trapOrTurret==0 ? (Item)turret : (Item)trap);
				if (i==null)break;
				Vector2 size = i.getSize();
				float x = r.x + (r.width - size.x*inventoryCellSize)/2.0f;
				float y = r.y + (r.height - size.y*inventoryCellSize)/2.0f;
				float width = size.x*inventoryCellSize;
				float height = size.y*inventoryCellSize;
				Rect r2 = new Rect(x, y, width, height);
				float inventW = inventoryCellSize;
				float inventH = inventoryCellSize;
				if (!r2.Contains(mousePos))  {
					inventW = inventW * r.width/r2.width;
					inventH = inventH * r.height/r2.height;
					r2 = r;
				}
				selectedCell = new Vector2((int)((mousePos.x - r2.x)/inventW), (int)((mousePos.y - r2.y)/inventH));
				foreach (Vector2 cell in i.getShape())  {
					if (cell.x == selectedCell.x && cell.y == selectedCell.y)  {
						selectedItemPos = new Vector2(mousePos.x - (mousePos.x - r2.x)/(inventW/inventoryCellSize), mousePos.y - (mousePos.y - r2.y)/(inventH/inventoryCellSize));
						selectedMousePos = mousePos;
						selectedItem = i;
						selectedItemWasInSlot = slot;
						trap = null;
						turret = null;
						break;
					}
				}
				if (selectedItem != null)  {
					break;
				}
			}
		}
		/*
		if (!GameGUI.looting || mousePos.x < groundX || mousePos.y < groundY || mousePos.x > groundX + groundWidth || mousePos.y > groundY + groundHeight) return;
		Vector2 scrollOff = UnitGUI.groundScrollPosition;
		float div = 20.0f;
		float y = div + UnitGUI.groundY - scrollOff.y;
		float mid = UnitGUI.groundX + UnitGUI.groundWidth/2.0f + scrollOff.x;
		//	mousePos.y += groundScrollPosition.y;
		selectedItem = null;
		if (mapGenerator != null)  {
			List<Item> groundItems = mapGenerator.tiles[(int)u.position.x,(int)-u.position.y].getReachableItems();
			foreach (Item i in groundItems)  {
				if (i.inventoryTexture==null) continue;
				//	Debug.Log(mousePos.x + "  " + mousePos.y + "       " + mid + "  " + y);
				Vector2 size = i.getSize();
				float x = mid - size.x*inventoryCellSize/2.0f;
				Rect r = new Rect(x, y, size.x*inventoryCellSize, size.y*UnitGUI.inventoryCellSize);
				if (r.Contains(mousePos))  {
					//	Debug.Log(i);
					selectedCell = new Vector2((int)((mousePos.x - x)/inventoryCellSize), (int)((mousePos.y - y)/inventoryCellSize));
					foreach (Vector2 cell in i.getShape())  {
						if (cell.x == selectedCell.x && cell.y == selectedCell.y)  {
							selectedItemPos = new Vector2(x, y);
							selectedMousePos = mousePos;
							selectedItem = i;
							selectedItemWasInSlot = InventorySlot.None;
						}
					}
					Debug.Log(selectedCell);
					if (selectedItem!=null)  {
						break;
					}
				}
				y += size.y*UnitGUI.inventoryCellSize + div;
			}
		}*/
	}
	public void deselectItem(Character characterSheet)  {
		deselectItem(characterSheet, null, null);
	}
	public void removeTurretTrapItemsPossibly()  {
		if (selectedItemWasInSlot==InventorySlot.TrapTurret)  {
			frame = null;
			applicator = null;
			gear = null;
			energySourceOrTrigger = null;
			displayedCharacter.saveCharacter();
		}
	}
	public void deselectItem(Character characterSheet, MapGenerator mapGenerator, Unit u)  {
		if (selectedItem == null) return;
		Vector3 mousePos = Input.mousePosition;
		mousePos.y = Screen.height - mousePos.y;
		Tile t = (mapGenerator != null && u!=null ? mapGenerator.tiles[(int)u.position.x,(int)-u.position.y] : null);
		foreach (InventorySlot slot in UnitGUI.inventorySlots)  {
			Rect r = getInventorySlotRect(slot, inventX, inventY);
			if (r.Contains(mousePos))  {
				Vector2 v2 = UnitGUI.getIndexOfSlot(slot);
				Vector2 v = v2 - selectedCell;
				Debug.Log(v);
				if (characterSheet.characterSheet.inventory.canInsertItemInSlot(selectedItem, v))  {
					if (selectedItemWasInSlot == InventorySlot.None)  {
						t.removeItem(selectedItem,1);
						//u.useMinor(false,false);
					}
					characterSheet.characterSheet.inventory.insertItemInSlot(selectedItem, v);
					selectedItem = null;
					removeTurretTrapItemsPossibly();
					return;
				}
				else  {
					InventoryItemSlot invSlot = characterSheet.characterSheet.inventory.inventory[Inventory.getIndexForSlot(v2)];
					Item invSlotItem = invSlot.getItem();
					if (invSlotItem != null && characterSheet.characterSheet.inventory.itemCanStackWith(invSlotItem, selectedItem))  {
						if (selectedItemWasInSlot == InventorySlot.None)  {
							t.removeItem(selectedItem,1);
							//u.useMinor(false,false);
						}
						characterSheet.characterSheet.inventory.stackItemWith(invSlotItem,selectedItem);
						selectedItem = null;
						removeTurretTrapItemsPossibly();
						return;
					}
				}
				break;
			}
		}
		foreach (InventorySlot slot in UnitGUI.trapTurretSlots)  {
			Rect r = getInventorySlotRect(slot, trapsTurretsX, trapsTurretsY);
			if (r.Contains(mousePos))  {
			//	Vector2 v2 = UnitGUI.getIndexOfSlot(slot);
				bool removed = false;
				switch (slot)  {
				case InventorySlot.Frame:
					if (selectedItem is Frame && frame==null)  {
						frame = (Frame)selectedItem;
						removed = true;
					}
					break;
				case InventorySlot.Applicator:
					if (selectedItem is Applicator && applicator == null)  {
						applicator = (Applicator)selectedItem;
						removed = true;
					}
					break;
				case InventorySlot.Gear:
					if (selectedItem is Gear && gear == null)  {
						gear = (Gear)selectedItem;
						removed = true;
					}
					break;
				case InventorySlot.TriggerEnergySource:
					if ((trapOrTurret==1 ? selectedItem is Trigger : selectedItem is EnergySource) && energySourceOrTrigger == null)  {
						energySourceOrTrigger = selectedItem;
						removed = true;
					}
					break;
				default:
					break;
				}
				if (removed)  {
					selectedItem = null;
					if (frame!=null && applicator!=null && gear!=null && (energySourceOrTrigger!=null && (trapOrTurret==1 ? (energySourceOrTrigger is Trigger) : (energySourceOrTrigger is EnergySource))))  {
						if (trapOrTurret==0)  {
							turret = new Turret(displayedCharacter.characterId, frame, applicator, gear, (EnergySource)energySourceOrTrigger);
						}
						else  {
							trap = new Trap(displayedCharacter.characterId, frame, applicator, gear, (Trigger)energySourceOrTrigger);
						}
					}
					return;
				}
			}
		}
/*		if (GameGUI.looting && !(mousePos.x < groundX || mousePos.y < groundY || mousePos.x > groundX + groundWidth || mousePos.y > groundY + groundHeight))  {
			if (selectedItemWasInSlot!=InventorySlot.None && selectedItem!=null)  {
				while (selectedItem.stackSize() > 1) t.addItem(selectedItem.popStack());
				t.addItem(selectedItem);
				u.minorsLeft--;
				//		characterSheet.characterSheet.inventory.removeItemFromSlot(getInventorySlotPos(selectedItemWasInSlot));
			}
		}
		else*/ 
		if (selectedItemWasInSlot!=InventorySlot.None)  {
			switch (selectedItemWasInSlot)  {
			case InventorySlot.Frame:
				frame = (Frame)selectedItem;
				break;
			case InventorySlot.Applicator:
				applicator = (Applicator)selectedItem;
				break;
			case InventorySlot.Gear:
				gear = (Gear)selectedItem;
				break;
			case InventorySlot.TriggerEnergySource:
				energySourceOrTrigger = selectedItem;
				break;
			case InventorySlot.TrapTurret:
				if (selectedItem is Turret)  {
					turret = (Turret)selectedItem;
				}
				else if (selectedItem is Trap)  {
					trap = (Trap)selectedItem;
				}
				break;
			default:
				if (displayedCharacter.characterSheet.inventory.canInsertItemInSlot(selectedItem, UnitGUI.getIndexOfSlot(selectedItemWasInSlot)))  {
					displayedCharacter.characterSheet.inventory.insertItemInSlot(selectedItem, UnitGUI.getIndexOfSlot(selectedItemWasInSlot));
				}
				else  {
					int slot1 = UnitGUI.getLinearIndexFromIndex(UnitGUI.getIndexOfSlot(selectedItemWasInSlot));
					if (slot1 > -1 && characterSheet.characterSheet.inventory.itemCanStackWith(displayedCharacter.characterSheet.inventory.inventory[slot1].getItem(),selectedItem))  {
						displayedCharacter.characterSheet.inventory.stackItemWith(displayedCharacter.characterSheet.inventory.inventory[slot1].getItem(),selectedItem);
					}
				}
				break;
			}
		}
		selectedItem = null;
	}




	int trapOrTurret = 0;
	public void drawWorkbenchGUI()  {
		float xOrig = (Screen.width - backgroundSize.x)/2.0f;
		float yOrig = (Screen.height - backgroundSize.y)/2.0f;
		float boxX = xOrig;
		float boxY = yOrig;
		GUI.DrawTexture(new Rect(xOrig, yOrig, backgroundSize.x, backgroundSize.y), bottomSheetTexture);
		yOrig +=30.0f;

		GUIStyle st = UnitGUI.getCourierStyle(24);
		string title = UnitGUI.getSmallCapsString(displayedCharacter.characterSheet.personalInformation.getCharacterName().fullName() + ":", 17);
		GUIContent titleContent = new GUIContent(title);
		Vector2 titleSize = st.CalcSize(titleContent);
		float y = yOrig;
		float x = xOrig + 30.0f;
		GUI.Label(new Rect(boxX + (backgroundSize.x - titleSize.x)/2.0f, y, titleSize.x, titleSize.y), titleContent, st);
		y += titleSize.y + 5.0f;
		int oldTr = trapOrTurret;
		trapOrTurret = GUI.SelectionGrid(new Rect(x, y, 218.0f, 25.0f), trapOrTurret, new string[] {"Turret","Trap"}, 2);
		if (oldTr != trapOrTurret && frame!=null)  {
			if ((trapOrTurret == 0 && turret == null) || (trapOrTurret==1 && trap==null))  {
				if (frame!=null && applicator!=null && gear!=null && (energySourceOrTrigger!=null && (trapOrTurret==1 ? (energySourceOrTrigger is Trigger) : (energySourceOrTrigger is EnergySource))))  {
					if (trapOrTurret==0)  {
						turret = new Turret(displayedCharacter.characterId, frame, applicator, gear, (EnergySource)energySourceOrTrigger);
					}
					else  {
						trap = new Trap(displayedCharacter.characterId, frame, applicator, gear, (Trigger)energySourceOrTrigger);
					}
				}
			}
		}


		y += 80.0f;
		trapsTurretsX = x;
		trapsTurretsY = y;
		y += 100.0f;
		inventX = x;
		inventY = y;
		
		Vector3 mousePos = Input.mousePosition;
		mousePos.y = Screen.height - mousePos.y;

		foreach (InventorySlot slot in UnitGUI.inventorySlots)  {
			Rect r = getInventorySlotRect(slot, inventX, inventY);
			if (r.Contains(mousePos))  {
				GUI.DrawTexture(r, UnitGUI.getInventoryHoverBackground());
				if (selectedItem!=null)  {
					Vector2 startPos = UnitGUI.getIndexOfSlot(slot);
					foreach(Vector2 cell in selectedItem.getShape())  {
						Vector2 pos = startPos;
						pos.x += cell.x - selectedCell.x;
						pos.y += cell.y - selectedCell.y;
						if (pos.x == startPos.x && pos.y == startPos.y) continue;
						//	Debug.Log(startPos + "   " + pos);
						InventorySlot newSlot = UnitGUI.getInventorySlotFromIndex(pos);
						if (newSlot != InventorySlot.None)  {
							Rect r2 = getInventorySlotRect(newSlot, inventX, inventY);
							GUI.DrawTexture(r2, UnitGUI.getInventoryHoverBackground());
						}
					}
				}
				break;
			}
		}
		bool doTrapTurret = true;
		if (doTrapTurret)  {
			InventorySlot slot = InventorySlot.TrapTurret;
			Rect r = getInventorySlotRect(slot, trapsTurretsX, trapsTurretsY);
			if (r.Contains(mousePos))  {
				GUI.DrawTexture(r, UnitGUI.getArmorHoverBackground());
			}
			GUI.DrawTexture(new Rect(r.x,r.y,inventoryLineThickness, inventoryCellSize),UnitGUI.getInventoryLineTall());
			GUI.DrawTexture(new Rect(r.x,r.y + inventoryCellSize,inventoryLineThickness, inventoryCellSize),UnitGUI.getInventoryLineTall());
			GUI.DrawTexture(new Rect(r.x + inventoryCellSize*2 - inventoryLineThickness,r.y,inventoryLineThickness, inventoryCellSize),UnitGUI.getInventoryLineTall());
			GUI.DrawTexture(new Rect(r.x + inventoryCellSize*2 - inventoryLineThickness,r.y+ inventoryCellSize,inventoryLineThickness, inventoryCellSize),UnitGUI.getInventoryLineTall());
			
			GUI.DrawTexture(new Rect(r.x,r.y,inventoryCellSize, inventoryLineThickness),UnitGUI.getInventoryLineWide());
			GUI.DrawTexture(new Rect(r.x + inventoryCellSize,r.y,inventoryCellSize, inventoryLineThickness),UnitGUI.getInventoryLineWide());
			GUI.DrawTexture(new Rect(r.x,r.y + inventoryCellSize*2 - inventoryLineThickness,inventoryCellSize, inventoryLineThickness),UnitGUI.getInventoryLineWide());
			GUI.DrawTexture(new Rect(r.x + inventoryCellSize,r.y + inventoryCellSize*2 - inventoryLineThickness,inventoryCellSize, inventoryLineThickness),UnitGUI.getInventoryLineWide());
			Item i = (trapOrTurret==0 ? (Item)turret : (Item)trap);
			if (i != null && i.inventoryTexture != null)  {
				float w = i.getSize().x*inventoryCellSize;
				float h = i.getSize().y*inventoryCellSize;
				x = r.x;
				y = r.y;
				if (h > r.height) y = r.y + r.height - h;
				else y = r.y + (r.height - h)/2.0f;
				x = r.x + (r.width - w)/2.0f;
//				GUI.DrawTexture(new Rect(x, y, w, h), i.inventoryTexture);
			}
		}
		foreach (InventorySlot slot in UnitGUI.trapTurretSlots)  {
			Rect r = getInventorySlotRect(slot, trapsTurretsX, trapsTurretsY);
			if (r.Contains(mousePos))  {
				GUI.DrawTexture(r, UnitGUI.getArmorHoverBackground());
				break;
			}

		}
		foreach (InventorySlot slot in UnitGUI.trapTurretSlots)  {
			Rect r = getInventorySlotRect(slot, trapsTurretsX, trapsTurretsY);
			if (slot==InventorySlot.TriggerEnergySource && energySourceOrTrigger != null && (trapOrTurret==1 ? (energySourceOrTrigger is EnergySource) : (energySourceOrTrigger is Trigger)))  {
				GUI.DrawTexture(r, UnitGUI.getArmorRedBackground());
			}
			GUI.DrawTexture(new Rect(r.x,r.y,inventoryLineThickness, inventoryCellSize),UnitGUI.getInventoryLineTall());
			GUI.DrawTexture(new Rect(r.x,r.y + inventoryCellSize,inventoryLineThickness, inventoryCellSize),UnitGUI.getInventoryLineTall());
			GUI.DrawTexture(new Rect(r.x + inventoryCellSize*2 - inventoryLineThickness,r.y,inventoryLineThickness, inventoryCellSize),UnitGUI.getInventoryLineTall());
			GUI.DrawTexture(new Rect(r.x + inventoryCellSize*2 - inventoryLineThickness,r.y+ inventoryCellSize,inventoryLineThickness, inventoryCellSize),UnitGUI.getInventoryLineTall());
			
			GUI.DrawTexture(new Rect(r.x,r.y,inventoryCellSize, inventoryLineThickness),UnitGUI.getInventoryLineWide());
			GUI.DrawTexture(new Rect(r.x + inventoryCellSize,r.y,inventoryCellSize, inventoryLineThickness),UnitGUI.getInventoryLineWide());
			GUI.DrawTexture(new Rect(r.x,r.y + inventoryCellSize*2 - inventoryLineThickness,inventoryCellSize, inventoryLineThickness),UnitGUI.getInventoryLineWide());
			GUI.DrawTexture(new Rect(r.x + inventoryCellSize,r.y + inventoryCellSize*2 - inventoryLineThickness,inventoryCellSize, inventoryLineThickness),UnitGUI.getInventoryLineWide());
			Item i = null;
			string s = "";
			int largeSize = 14;
			int smallSize = 10;
			switch (slot)  {
			case InventorySlot.Frame:
				i = frame;
				s = UnitGUI.getSmallCapsString("Frame", smallSize);
				break;
			case InventorySlot.Applicator:
				i = applicator;
				s = UnitGUI.getSmallCapsString("Applicator", smallSize);
				break;
			case InventorySlot.Gear:
				i = gear;
				s = UnitGUI.getSmallCapsString("Gear", smallSize);
				break;
			case InventorySlot.TriggerEnergySource:
				i = energySourceOrTrigger;
				s = UnitGUI.getSmallCapsString((trapOrTurret==0 ? "Energy\nSource" : "Trigger"), smallSize);
				break;
			default:
				break;
			}
			GUIContent cont = new GUIContent(s);
			GUIStyle sty = UnitGUI.getCourierStyle(largeSize);
			Vector2 size = sty.CalcSize(cont);
			GUI.Label(new Rect(r.x + (r.width - size.x)/2.0f, r.y - size.y, size.x, size.y), cont, sty);
			//displayedCharacter.characterSheet.characterLoadout.getItemInSlot(slot);
			if (i != null && i.inventoryTexture != null)  {
				float w = i.getSize().x*inventoryCellSize;
				float h = i.getSize().y*inventoryCellSize;
				x = r.x;
				y = r.y;
				if (h > r.height) y = r.y + r.height - h;
				else y = r.y + (r.height - h)/2.0f;
				x = r.x + (r.width - w)/2.0f;
//				GUI.DrawTexture(new Rect(x, y, w, h), i.inventoryTexture);
			}
		}
		foreach (InventorySlot slot in UnitGUI.inventorySlots)  {
			Rect r = getInventorySlotRect(slot, inventX, inventY);
			GUI.DrawTexture(new Rect(r.x,r.y,inventoryLineThickness, inventoryCellSize),UnitGUI.getInventoryLineTall());
			GUI.DrawTexture(new Rect(r.x + inventoryCellSize - inventoryLineThickness,r.y,inventoryLineThickness, inventoryCellSize),UnitGUI.getInventoryLineTall());
			
			GUI.DrawTexture(new Rect(r.x,r.y,inventoryCellSize, inventoryLineThickness),UnitGUI.getInventoryLineWide());
			GUI.DrawTexture(new Rect(r.x,r.y + inventoryCellSize - inventoryLineThickness,inventoryCellSize, inventoryLineThickness),UnitGUI.getInventoryLineWide());
		}
		GUIStyle stackSt = UnitGUI.getStackStyle();
		foreach (InventorySlot slot in UnitGUI.inventorySlots)  {
			Vector2 vec = UnitGUI.getIndexOfSlot(slot);
			int ind = UnitGUI.getLinearIndexFromIndex(vec);
			InventoryItemSlot isl = displayedCharacter.characterSheet.inventory.inventory[ind];
			Item i = isl.item;
			if (i == null) continue;
			Vector2 origin = getInventorySlotPos(slot, inventX, inventY);
			Vector2 size = i.getSize();
//			GUI.DrawTexture(new Rect(origin.x,origin.y, size.x*inventoryCellSize,size.y*inventoryCellSize),i.inventoryTexture);
			if (i.stackSize()>1)  {
				Vector2 bottomRight = i.getBottomRightCell();
				bottomRight.x *= inventoryCellSize - inventoryLineThickness;
				bottomRight.y *= inventoryCellSize - inventoryLineThickness;
				Vector2 stackPos = origin + bottomRight;
				GUIContent content = new GUIContent("" + i.stackSize());
				GUI.Label(new Rect(stackPos.x,stackPos.y,inventoryCellSize,inventoryCellSize),content,stackSt);
			}
		}/*
		if (mapGenerator != null)  {
			List<Item> groundItems = mapGenerator.tiles[(int)position.x,(int)-position.y].getReachableItems();
			//	Debug.Log("ground Items: " + groundItems.Count + "   " + groundItems);
			float div = 20.0f;
			float height = div;
			foreach (Item i in groundItems)  {
				if (i.inventoryTexture==null) continue;
				height += i.getSize().y*inventoryCellSize + div;
			}
			groundScrollPosition = GUI.BeginScrollView(new Rect(groundX, groundY, groundWidth, groundHeight), groundScrollPosition, new Rect(groundX, groundY, groundWidth-20.0f, height));
			y = div + groundY;
			float mid = groundX + groundWidth/2.0f;
			foreach (Item i in groundItems)  {
				if (i.inventoryTexture==null) continue;
				Vector2 size = i.getSize();
				if (i!=selectedItem)  {
					GUI.DrawTexture(new Rect(mid - size.x*inventoryCellSize/2.0f, y, size.x*inventoryCellSize, size.y*inventoryCellSize), i.inventoryTexture);
				}
				y += size.y*inventoryCellSize + div;
			}
			GUI.EndScrollView();
		}*/
		if (selectedItem != null)  {
			Vector2 size = selectedItem.getSize();
			Vector2 pos = selectedItemPos;
			pos.y += (mousePos.y - selectedMousePos.y);
			pos.x += (mousePos.x - selectedMousePos.x);
//			GUI.DrawTexture(new Rect(pos.x, pos.y,size.x*inventoryCellSize, size.y*inventoryCellSize), selectedItem.inventoryTexture);
			if (selectedItem.stackSize()>1)  {
				Vector2 bottomRight = selectedItem.getBottomRightCell();
				bottomRight.x *= inventoryCellSize - inventoryLineThickness;
				bottomRight.y *= inventoryCellSize - inventoryLineThickness;
				Vector2 stackPos = pos + bottomRight;
				GUIContent content = new GUIContent("" + selectedItem.stackSize());
				GUI.Label(new Rect(stackPos.x,stackPos.y,inventoryCellSize,inventoryCellSize),content,stackSt);
			}
		}






		Vector2 cancelButtonSize = new Vector2(100.0f, 40.0f);
		float buttonY = boxY + backgroundSize.y - cancelButtonSize.y - 40.0f;
		if (GUI.Button(new Rect((Screen.width - cancelButtonSize.x)/2.0f, buttonY, cancelButtonSize.x, cancelButtonSize.y), "Done"))  {
			displayedCharacter = null;
		}

	}

					
	public bool guiContainsMouse()  {
		Vector3 mousePos = Input.mousePosition;
		mousePos.y = Screen.height - mousePos.y;
		if (baseState == BaseState.Barracks && levelingUpCharacter != null)  {
			Rect levelingUpRect = new Rect((Screen.width - backgroundSize.x)/2.0f, (Screen.height - backgroundSize.y)/2.0f, backgroundSize.x, backgroundSize.y);
			if (levelingUpRect.Contains(mousePos)) return true;
		}
		return false;
	}

	
	int calculateBoxHeight(int n)  {
		int height = 0;
		
		height = 20 * n;
		
		return height;
	}
	
	int calculateMod(int abilityScore)  {
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
	int selectedRace = -1;
	ClassFeature[] possibleFeatures;
	public bool canGoNextPage()  {
		switch (page)  {
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


	public ClassFeature getSelectedFeature()  {
		if (possibleFeatures.Length==0) return ClassFeature.None;
		return (possibleFeatures.Length==1 ? possibleFeatures[0] : possibleFeatures[selectedFeature]);
	}

	public bool hasFinishedAllSelections()  {
		if (possibleFeatures.Length == 0) return true;
		if (possibleFeatures.Length > 1 && selectedFeature < 0) return false;
		ClassFeature feature = getSelectedFeature();
		switch (feature)  {
		case ClassFeature.Weapon_Focus:
			return selectedWeaponFocus >= 0;
		case ClassFeature.Favored_Race:
			return selectedRace >= 0;
		default:
			return true;
		}
	}
	
	int setSkillDecreaseButton(int skill, Rect r, int skillLowerBound)  {
		if(skill == skillLowerBound)  {
			GUI.enabled = false;
		}
		if(GUI.Button(r, "<"))  {
			skillPointsAvailable++;
			skill--;
		}
		GUI.enabled = true;

		return skill;
	}
	
	int setSkillIncreaseButton(int skill, Rect r)  {
		if(skillPointsAvailable == 0)  {
			GUI.enabled = false;
		}
		if(GUI.Button(r, ">"))  {
			skillPointsAvailable--;
			skill++;
		}
		GUI.enabled = true;
		
		return skill;
	}
		
	static Vector2 backgroundSize = new Vector2(500.0f, 500.0f);

	public void drawLevelUpGUI()  {
		float xOrig = (Screen.width - backgroundSize.x)/2.0f;
		float yOrig = (Screen.height - backgroundSize.y)/2.0f;
		float boxX = xOrig;
		float boxY = yOrig;
		GUI.DrawTexture(new Rect(xOrig, yOrig, backgroundSize.x, backgroundSize.y), bottomSheetTexture);
		yOrig +=50.0f;
		switch (page)  {
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
		if (GUI.Button(new Rect(boxX + 30.0f, buttonY, cancelButtonSize.x, cancelButtonSize.y), "Cancel"))  {
			levelingUpCharacter = null;
		}
		if (page==0) GUI.enabled = false;
		if (GUI.Button(new Rect(boxX + backgroundSize.x - otherButtonSize.x*2 - 30.0f, buttonY, otherButtonSize.x, otherButtonSize.y), "Back"))  {
			page--;
		}
		GUI.enabled = canGoNextPage();
		if (GUI.Button(new Rect(boxX + backgroundSize.x - otherButtonSize.x - 30.0f, buttonY, otherButtonSize.x, otherButtonSize.y), (page==2?"Finish":"Next")))  {
			if (page == 2)  {
				levelingUpCharacter.characterSheet.skillScores.scores = new int[] {athleticsSkill, meleeSkill, rangedSkill, stealthSkill, mechanicalSkill, medicinalSkill, historicalSkill, politicalSkill}; 
				levelingUpCharacter.characterSheet.abilityScores.setScores(sturdyScore, perceptionScore, techniqueScore, wellVersedScore);
				if (selectedFeature != -1)  {
					int[] oldFeatures = levelingUpCharacter.characterSheet.characterProgress.getCharacterClass().chosenFeatures;
					int[] newFeatures = new int[oldFeatures.Length+1];
					for (int n=0;n<oldFeatures.Length;n++) newFeatures[n] = oldFeatures[n];
					newFeatures[newFeatures.Length-1] = selectedFeature;
					levelingUpCharacter.characterSheet.characterProgress.getCharacterClass().chosenFeatures = newFeatures;
				}
			//	if (possibleFeatures.Length>=1)  {
				ClassFeature feature = getSelectedFeature();
				switch (feature)  {
				case ClassFeature.Weapon_Focus:
					levelingUpCharacter.characterSheet.characterProgress.setWeaponFocus(selectedWeaponFocus + 1);
					break;
				case ClassFeature.Favored_Race:
					levelingUpCharacter.characterSheet.characterProgress.setFavoredRace(selectedRace + 1);
					break;
				default:
					break;
				}
			//	}
				levelingUpCharacter.characterSheet.characterProgress.incrementLevel();
				levelingUpCharacter.saveCharacter();
				levelingUpCharacter = null;
			}
			else  {
				page++;
			}
		}
	}
	public void drawFeatureGUI(float xOrig, float yOrig, Vector2 backgroundSize)  {
		GUIStyle st = UnitGUI.getCourierStyle(24);
		string title = UnitGUI.getSmallCapsString((possibleFeatures.Length==0 ? "No Class Feature Learned" : "Class Feature Learned:"), 17);
		GUIContent titleContent = new GUIContent(title);
		Vector2 titleSize = st.CalcSize(titleContent);
		float y = yOrig;
		float x = xOrig + 30.0f;
		GUI.Label(new Rect(xOrig + (backgroundSize.x - titleSize.x)/2.0f, y, titleSize.x, titleSize.y), titleContent, st);
		y += titleSize.y + 30.0f;
		Vector2 buttonSize = new Vector2(150.0f, 30.0f);
		if (possibleFeatures.Length>1)  {
			st.fontSize = 20;
			string featureSelectTitle = UnitGUI.getSmallCapsString("Select a Feature:", 14);
			GUIContent featureSelectTitleContent = new GUIContent(featureSelectTitle);
			Vector2 featureSelectTitleSize = st.CalcSize(featureSelectTitleContent);
			GUI.Label(new Rect(x, y, featureSelectTitleSize.x, featureSelectTitleSize.y), featureSelectTitleContent, st);
			y += featureSelectTitleSize.y;
			for (int n=0;n<possibleFeatures.Length;n++)  {
				if (GUI.Button(new Rect(x, y, buttonSize.x, buttonSize.y), ClassFeatures.getName(possibleFeatures[n])))  {
					if (selectedFeature == n) selectedFeature = -1;
					else selectedFeature = n;
				}
				y += buttonSize.y;
			}
			y += 10.0f;
		}
		if (possibleFeatures.Length==1 || selectedFeature >=0)  {
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
			if (feature == ClassFeature.Weapon_Focus)  {
				st.fontSize = 20;
				GUIContent selectTitleString = new GUIContent(UnitGUI.getSmallCapsString("Select a Weapon Focus:", 14));
				Vector2 selectTitleSize = st.CalcSize(selectTitleString);
				GUI.Label(new Rect(x, y, selectTitleSize.x, selectTitleSize.y), selectTitleString, st);
				y += selectTitleSize.y + 5.0f;
				GUIContent selectedString = new GUIContent(UnitGUI.getSmallCapsString("Selected", 14));
				Vector2 selectedSize = st.CalcSize(selectedString);
				string[] focuses = new string[] {"Piercing","Slashing","Crushing"};
				for (int n=0;n<focuses.Length;n++)  {
					if (GUI.Button(new Rect(x, y, buttonSize.x, buttonSize.y), focuses[n]))  {
						if (selectedWeaponFocus == n) selectedWeaponFocus = -1;
						else selectedWeaponFocus = n;
					}
					if (selectedWeaponFocus == n) GUI.Label(new Rect(x + buttonSize.x + 5.0f, y + (buttonSize.y - selectedSize.y)/2.0f, selectedSize.x, selectedSize.y), selectedString, st);
					y += buttonSize.y;
				}
			}
			else if (feature == ClassFeature.Favored_Race)  {
				st.fontSize = 20;
				GUIContent selectTitleString = new GUIContent(UnitGUI.getSmallCapsString("Select a Favored Race:", 14));
				Vector2 selectTitleSize = st.CalcSize(selectTitleString);
				GUI.Label(new Rect(x, y, selectTitleSize.x, selectTitleSize.y), selectTitleString, st);
				y += selectTitleSize.y + 5.0f;
				GUIContent selectedString = new GUIContent(UnitGUI.getSmallCapsString("Selected", 14));
				Vector2 selectedSize = st.CalcSize(selectedString);
				string[] focuses = new string[] {"Berrid","Ashpian","Rorrul"};
				for (int n=0;n<focuses.Length;n++)  {
					if (GUI.Button(new Rect(x, y, buttonSize.x, buttonSize.y), focuses[n]))  {
						if (selectedRace == n) selectedRace = -1;
						else selectedRace = n;
					}
					if (selectedRace == n) GUI.Label(new Rect(x + buttonSize.x + 5.0f, y + (buttonSize.y - selectedSize.y)/2.0f, selectedSize.x, selectedSize.y), selectedString, st);
					y += buttonSize.y;
				}
			}
		}
	}

	public void drawSkillScoresGUI(float xOrig, float yOrig)  {
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
		athleticsSkill = setSkillDecreaseButton(athleticsSkill, new Rect(x, y, cellWidth, cellHeight), levelingUpCharacter.characterSheet.skillScores.scores[0]);
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (athleticsSkill + levelingUpCharacter.characterSheet.characterProgress.getCharacterClass().getClassModifiers().getAthleticsModifier()).ToString());
		x += cellWidth*2;
		athleticsSkill = setSkillIncreaseButton(athleticsSkill, new Rect(x, y, cellWidth, cellHeight));
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth, cellHeight*2), "+");
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight*2), calculateMod(sturdyScore).ToString());
		x += cellWidth*2;
		GUI.Box(new Rect(x, y, cellWidth, cellHeight*2), "=");
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (athleticsSkill + calculateMod(sturdyScore) + levelingUpCharacter.characterSheet.characterProgress.getCharacterClass().getClassModifiers().getAthleticsModifier()).ToString());
		y += cellHeight;
		x = xOrig + cellWidth*4;
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight), "Melee:");
		x += cellWidth*4;
		meleeSkill = setSkillDecreaseButton(meleeSkill, new Rect(x, y, cellWidth, cellHeight), levelingUpCharacter.characterSheet.skillScores.scores[1]);
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (meleeSkill + levelingUpCharacter.characterSheet.characterProgress.getCharacterClass().getClassModifiers().getMeleeModifier()).ToString());
		x += cellWidth*2;
		meleeSkill = setSkillIncreaseButton(meleeSkill, new Rect(x, y, cellWidth, cellHeight));
		x += cellWidth*5;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (meleeSkill + calculateMod(sturdyScore) + levelingUpCharacter.characterSheet.characterProgress.getCharacterClass().getClassModifiers().getMeleeModifier()).ToString());
		y += cellHeight;
		x = xOrig;
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight*2), "Prowess:");
		x += cellWidth*4;
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight), "Ranged:");
		x += cellWidth*4;
		rangedSkill = setSkillDecreaseButton(rangedSkill, new Rect(x, y, cellWidth, cellHeight), levelingUpCharacter.characterSheet.skillScores.scores[2]);
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (rangedSkill + levelingUpCharacter.characterSheet.characterProgress.getCharacterClass().getClassModifiers().getRangedModifier()).ToString());
		x += cellWidth*2;
		rangedSkill = setSkillIncreaseButton(rangedSkill, new Rect(x, y, cellWidth, cellHeight));
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth, cellHeight*2), "+");
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight*2), calculateMod(perceptionScore).ToString());
		x += cellWidth*2;
		GUI.Box(new Rect(x, y, cellWidth, cellHeight*2), "=");
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (rangedSkill + calculateMod(perceptionScore) + levelingUpCharacter.characterSheet.characterProgress.getCharacterClass().getClassModifiers().getRangedModifier()).ToString());
		x = xOrig + cellWidth*4;
		y += cellHeight;
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight), "Stealth:");
		x += cellWidth*4;
		stealthSkill = setSkillDecreaseButton(stealthSkill, new Rect(x, y, cellWidth, cellHeight), levelingUpCharacter.characterSheet.skillScores.scores[3]);
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (stealthSkill + levelingUpCharacter.characterSheet.characterProgress.getCharacterClass().getClassModifiers().getStealthModifier()).ToString());
		x += cellWidth*2;
		stealthSkill = setSkillIncreaseButton(stealthSkill, new Rect(x, y, cellWidth, cellHeight));
		x += cellWidth*5;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (stealthSkill + calculateMod(perceptionScore) + levelingUpCharacter.characterSheet.characterProgress.getCharacterClass().getClassModifiers().getStealthModifier()).ToString());
		y += cellHeight;
		x = xOrig;
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight*2), "Mastery:");
		x += cellWidth*4;
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight), "Mechanical:");
		x += cellWidth*4;
		mechanicalSkill = setSkillDecreaseButton(mechanicalSkill, new Rect(x, y, cellWidth, cellHeight), levelingUpCharacter.characterSheet.skillScores.scores[4]);
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (mechanicalSkill + levelingUpCharacter.characterSheet.characterProgress.getCharacterClass().getClassModifiers().getMechanicalModifier()).ToString());
		x += cellWidth*2;
		mechanicalSkill = setSkillIncreaseButton(mechanicalSkill, new Rect(x, y, cellWidth, cellHeight));
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth, cellHeight*2), "+");
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight*2), calculateMod(techniqueScore).ToString());
		x += cellWidth*2;
		GUI.Box(new Rect(x, y, cellWidth, cellHeight*2), "=");
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (mechanicalSkill + calculateMod(techniqueScore) + levelingUpCharacter.characterSheet.characterProgress.getCharacterClass().getClassModifiers().getMechanicalModifier()).ToString());
		y += cellHeight;
		x = xOrig + cellWidth*4;
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight), "Medicinal:");
		x += cellWidth*4;
		medicinalSkill = setSkillDecreaseButton(medicinalSkill, new Rect(x, y, cellWidth, cellHeight), levelingUpCharacter.characterSheet.skillScores.scores[5]);
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (medicinalSkill + levelingUpCharacter.characterSheet.characterProgress.getCharacterClass().getClassModifiers().getMedicinalModifier()).ToString());
		x += cellWidth*2;
		medicinalSkill = setSkillIncreaseButton(medicinalSkill, new Rect(x, y, cellWidth, cellHeight));
		x += cellWidth*5;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (medicinalSkill + calculateMod(techniqueScore) + levelingUpCharacter.characterSheet.characterProgress.getCharacterClass().getClassModifiers().getMedicinalModifier()).ToString());
		y += cellHeight;
		x = xOrig;
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight*2), "Knowledge:");
		x += cellWidth*4;
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight), "Historical:");
		x += cellWidth*4;
		historicalSkill = setSkillDecreaseButton(historicalSkill, new Rect(x, y, cellWidth, cellHeight), levelingUpCharacter.characterSheet.skillScores.scores[6]);
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (historicalSkill + levelingUpCharacter.characterSheet.characterProgress.getCharacterClass().getClassModifiers().getHistoricalModifier()).ToString());
		x += cellWidth*2;
		historicalSkill = setSkillIncreaseButton(historicalSkill, new Rect(x, y, cellWidth, cellHeight));
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth, cellHeight*2), "+");
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight*2), calculateMod(wellVersedScore).ToString());
		x += cellWidth*2;
		GUI.Box(new Rect(x, y, cellWidth, cellHeight*2), "=");
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (historicalSkill + calculateMod(wellVersedScore) + levelingUpCharacter.characterSheet.characterProgress.getCharacterClass().getClassModifiers().getHistoricalModifier()).ToString());
		y += cellHeight;
		x = xOrig + cellWidth*4;
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight), "Political:");
		x += cellWidth*4;
		politicalSkill = setSkillDecreaseButton(politicalSkill, new Rect(x, y, cellWidth, cellHeight), levelingUpCharacter.characterSheet.skillScores.scores[7]);
		x += cellWidth;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (politicalSkill + levelingUpCharacter.characterSheet.characterProgress.getCharacterClass().getClassModifiers().getPoliticalModifier()).ToString());
		x += cellWidth*2;
		politicalSkill = setSkillIncreaseButton(politicalSkill, new Rect(x, y, cellWidth, cellHeight));
		x += cellWidth*5;
		GUI.Box(new Rect(x, y, cellWidth*2, cellHeight), (politicalSkill + calculateMod(wellVersedScore) + levelingUpCharacter.characterSheet.characterProgress.getCharacterClass().getClassModifiers().getPoliticalModifier()).ToString());
	}

	public void drawAbilityScoresGUI(float xOrig, float yOrig)  {
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
		if(sturdyScore == levelingUpCharacter.characterSheet.abilityScores.getSturdy())  {
			GUI.enabled = false;
		}
		if(GUI.Button(new Rect(x, y, cellWidth, cellHeight), "<"))  {
			abilityScorePointsAvailable++;
			sturdyScore--;
		}
		x += cellWidth;
		GUI.enabled = true;
		
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight), sturdyScore.ToString());
		x += cellWidth*4;
		if(abilityScorePointsAvailable == 0)  {
			GUI.enabled = false;
		}
		if(GUI.Button(new Rect(x, y, cellWidth, cellHeight), ">"))  {
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
		if(perceptionScore == levelingUpCharacter.characterSheet.abilityScores.getPerception(0))  {
			GUI.enabled = false;
		}
		if(GUI.Button(new Rect(x, y, cellWidth, cellHeight), "<"))  {
			abilityScorePointsAvailable++;
			perceptionScore--;
		}
		x += cellWidth;
		GUI.enabled = true;
		
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight), perceptionScore.ToString());
		x += cellWidth*4;
		if(abilityScorePointsAvailable == 0)  {
			GUI.enabled = false;
		}
		if(GUI.Button(new Rect(x, y, cellWidth, cellHeight), ">"))  {
			abilityScorePointsAvailable--;
			perceptionScore++;
		}
		x += cellWidth;
		GUI.enabled = true;
		GUI.Box(new Rect(x, y, cellWidth*5, cellHeight), (sturdyScore + perceptionScore + levelingUpCharacter.characterSheet.characterProgress.getCharacterClass().getClassModifiers().getHealthModifier() + levelingUpCharacter.characterSheet.personalInformation.getCharacterRace().getHealthModifier()).ToString());
		y += cellHeight;
		x = xOrig;
		
		GUI.Box(new Rect(x, y, cellWidth*5, cellHeight), "Technique:");
		x += cellWidth*5;
		if(techniqueScore == levelingUpCharacter.characterSheet.abilityScores.getTechnique())  {
			GUI.enabled = false;
		}
		if(GUI.Button(new Rect(x, y, cellWidth, cellHeight), "<"))  {
			abilityScorePointsAvailable++;
			techniqueScore--;
		}
		x += cellWidth;
		GUI.enabled = true;
		
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight), techniqueScore.ToString());
		x += cellWidth*4;
		if(abilityScorePointsAvailable == 0)  {
			GUI.enabled = false;
		}
		if(GUI.Button(new Rect(x, y, cellWidth, cellHeight), ">"))  {
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
		if(wellVersedScore == levelingUpCharacter.characterSheet.abilityScores.getWellVersed())  {
			GUI.enabled = false;
		}
		if(GUI.Button(new Rect(x, y, cellWidth, cellHeight), "<"))  {
			abilityScorePointsAvailable++;
			wellVersedScore--;
		}
		x += cellWidth;
		GUI.enabled = true;
		
		GUI.Box(new Rect(x, y, cellWidth*4, cellHeight), wellVersedScore.ToString());
		x += cellWidth*4;
		if(abilityScorePointsAvailable == 0)  {
			GUI.enabled = false;
		}
		if(GUI.Button(new Rect(x, y, cellWidth, cellHeight), ">"))  {
			abilityScorePointsAvailable--;
			wellVersedScore++;
		}
		x += cellWidth;
		GUI.enabled = true;

		GUI.Box(new Rect(x, y, cellWidth*5, cellHeight), (techniqueScore + wellVersedScore + levelingUpCharacter.characterSheet.characterProgress.getCharacterClass().getClassModifiers().getComposureModifier() + levelingUpCharacter.characterSheet.personalInformation.getCharacterRace().getComposureModifier()).ToString());
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
	static GUIStyle getUnitInfoStyle(int fontSize)  {
		if (unitInfoStyle==null)  {
			unitInfoStyle = new GUIStyle("Label");
			unitInfoStyle.font = Resources.Load<Font>("Fonts/Courier New");
			unitInfoStyle.active.textColor = unitInfoStyle.normal.textColor = unitInfoStyle.hover.textColor = Color.black;
			unitInfoStyle.hover.textColor = Color.red;
		}
		unitInfoStyle.fontSize = fontSize;
		return unitInfoStyle;
	}

}
