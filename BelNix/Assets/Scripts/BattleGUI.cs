using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public enum ClassFeatureCanvas { OneOfMany, TemperedHands }
public enum ConfirmButton {Movement = 0, Standard, Minor }

public class BattleGUI : MonoBehaviour {

	static BattleGUI battleGUI;

	[SerializeField] private EventSystem eventSystem;
    // Let's grab some UI Elements from the editor
    [SerializeField] private GameObject[] CIPanels = new GameObject[3];
	[SerializeField] private GameObject consoleCanvas;
	[SerializeField] private GameObject turnOrderCanvas;
	[SerializeField] private GameObject characterInfoCanvas;
	[SerializeField] private GameObject consoleContentPanel;
	[SerializeField] private GameObject featuresContentPanel;
	[SerializeField] private GameObject turretSelectCanvas;
	[SerializeField] private GameObject turretSelectContentPanel;
	[SerializeField] private GameObject consoleMessagePrefab;
	[SerializeField] private GameObject classFeaturePrefab;
	[SerializeField] private GameObject turretSelectPrefab;
	[SerializeField] private GameObject turnOrderPrefab;
	[SerializeField] private GameObject oneOfManyCanvas;
	[SerializeField] private GameObject temperedHandsCanvas;
	[SerializeField] private GameObject[] confirmButtons;
	[SerializeField] private Text playerTurnTextObject;

    private MapGenerator mapGenerator;
    private Text atAGlanceText;
    private Text[] statsTexts;
    private Text characterInfoText;
    private Scrollbar featuresScrollBar;
    private Scrollbar consoleScrollBar;
	public Scrollbar turretScrollBar;
    private Dictionary<MovementType, GameObject> movementButtons;
    private Dictionary<StandardType, GameObject> standardButtons;
    private Dictionary<MinorType, GameObject> minorButtons;
	private Queue consoleText = new Queue();
	private GameObject[] currentClassFeatures = null;

	private const int maxNumMessages = 20;
	public bool doPlayerText;
	Color playerTurnTextColor;
	float playerTurnTextStartTime;
	const float textColorAlphaTime = .5f;
	const float textColorTime = 1.5f;
	const float textColorAlphaScale = 1.0f/textColorAlphaTime;
    
    // Numbers as indices aren't very informative. Let's use enums.
    public enum CIPanel { Glance, Stats, Skills, Buttons };
    //--------------------------------------------------------------------------------

    // Use this for initialization
    void Start()
    {
        // Some fancy stuff to make static things work in other classes
        battleGUI = this;
        GameGUI.initialize();

        // Initialize the Character Information panels, grab relevant UI elements
        atAGlanceText = GameObject.Find("Text - At a Glance").GetComponent<Text>();
        statsTexts = new Text[4];
        for (int n = 0; n < 4; n++)
        {
            statsTexts[n] = GameObject.Find("Text - Stats" + (n + 1)).GetComponent<Text>();
        }
        characterInfoText = GameObject.Find("Text - Character Info").GetComponent<Text>();
        featuresScrollBar = GameObject.Find("Scrollbar - Features").GetComponent<Scrollbar>();
        consoleScrollBar = GameObject.Find("Scrollbar - Console").GetComponent<Scrollbar>();

        // Initialize the fields relating to Action Buttons
        MinorType[] minorTypes = new MinorType[] { MinorType.Loot, MinorType.Stealth, MinorType.Mark, MinorType.TemperedHands, MinorType.Escape, MinorType.Invoke };
        MovementType[] movementTypes = new MovementType[] { MovementType.Move, MovementType.BackStep, MovementType.Recover };
        StandardType[] standardTypes = new StandardType[] { StandardType.Attack, StandardType.OverClock, StandardType.Throw, StandardType.Intimidate, StandardType.Place_Turret, StandardType.Lay_Trap, StandardType.Inventory };
        minorButtons = new Dictionary<MinorType, GameObject>();
        standardButtons = new Dictionary<StandardType, GameObject>();
        movementButtons = new Dictionary<MovementType, GameObject>();
        foreach (MinorType minorType in minorTypes)
        {
            minorButtons[minorType] = GameObject.Find("Image - " + Unit.getNameOfMinorType(minorType));
        }
        foreach (MovementType movementType in movementTypes)
        {
            movementButtons[movementType] = GameObject.Find("Image - " + Unit.getNameOfMovementType(movementType));
        }
        foreach (StandardType standardType in standardTypes)
        {
            standardButtons[standardType] = GameObject.Find("Image - " + Unit.getNameOfStandardType(standardType));
        }

        // Grab the MapGenerator for convenience
        mapGenerator = GameObject.Find("MapGenerator").GetComponent<MapGenerator>();

        // By default, the Character Info Canvas' animator idles on "Dismissed," which is visible.
        // We don't want to to be visible until the UI is revealed later, so we have to set it to "Hiding".
        GameObject.Find("Canvas - Character Info").GetComponent<Animator>().Play("CI_Panel_Hiding");
        //GameObject.Find("Canvas - Console").GetComponent<Animator>().Play("Console_Dismissed");
        foreach (GameObject panel in CIPanels)
            toggleAnimatorBool(panel.GetComponent<Animator>(), "Hidden");
    }



    // Update is called once per frame
    void Update()
    {
        UnitGUI.doTabs();
        if (doPlayerText) updatePlayerTurnText();

        // C is for Character Sheet
        // V is for Class Features
        if (Input.anyKeyDown && !UIRevealed)
        {
            // Debug only: Reveal the Character UI once a key is pressed.
            //toggleBattleUI();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            toggleCIPanel(CIPanel.Stats);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            toggleCIPanel(CIPanel.Skills);
        }
    }

    void OnGUI()
    {
        GameGUI.doGUI();
    }

	public static void hideTurretSelect(bool hide = true, bool selectCurrent = false) {
		battleGUI.hideTurretSelectActually(hide, selectCurrent);
	}

	public void hideTurretSelectActually(bool hide = true, bool selectCurrent = false) {
		turretSelectCanvas.SetActive(!hide);
		if (selectCurrent && lastSelected != null) {
			setTrapTurretButtonSelected(lastSelected, true);
		}
	}

	public void selectTrapOrTurret() {
		if (lastSelected == null) return;
		GameGUI.selectedTrapTurret = true;
		hideTurretSelectActually();
		mapGenerator.resetRanges();
	}
	GameObject lastSelected = null;
	public void selectTrapTurretButton(GameObject button) {
		if (lastSelected != null) setTrapTurretButtonSelected(lastSelected, false);
		MechanicalPlacementScript scr = button.GetComponent<MechanicalPlacementScript>();
		GameGUI.selectedTrap = scr.trap;
		GameGUI.selectedTurret = scr.turret;
		setTrapTurretButtonSelected(button, true);
		lastSelected = button;
	}

	public void setTrapTurretButtonSelected(GameObject button, bool selected) {
		Animator anim = button.GetComponent<Animator>();
		anim.SetBool("CurrentAction",selected);
	}
	
	public static void turnOnTurretSelect(Unit u) {
		battleGUI.turnOnTurretSelectActually(u);
	}
	
	public void turnOnTurretSelectActually(Unit u) {
		List<Turret> turrets = u.getTurrets();
		hideTurretSelectActually(false);
		for (int n = turretSelectContentPanel.transform.childCount-1;n>=0;n--) {
			GameObject.Destroy(turretSelectContentPanel.transform.GetChild(n).gameObject);
		}
		bool first = true;
		foreach (Turret t in turrets) {
			GameObject tsp = GameObject.Instantiate(turretSelectPrefab) as GameObject;
			Button b = tsp.GetComponent<Button>();
			b.onClick.AddListener(delegate() { selectTrapTurretButton(b.gameObject); });
			MechanicalPlacementScript scr = tsp.GetComponent<MechanicalPlacementScript>();
			scr.turret = t;
			tsp.transform.parent = turretSelectContentPanel.transform;
			Text tex = tsp.transform.FindChild("Text").GetComponent<Text>();
			Image img = tsp.transform.FindChild("Image").GetComponent<Image>();
			tex.text = turretString(t);
			img.sprite = Sprite.Create(t.inventoryTexture as Texture2D, new Rect(0, 0, 64, 64), new Vector2(.5f, .5f));
			if (first) {
				lastSelected = tsp;
				setTrapTurretButtonSelected(tsp, true);
				GameGUI.selectedTurret = t;
				first = false;
			}
		}
		turretScrollBar.value = 1;
	}
	
	public static void turnOnTrapSelect(Unit u) {
		battleGUI.turnOnTrapSelectActually(u);
	}
	
	public void turnOnTrapSelectActually(Unit u) {
		List<Trap> traps = u.getTraps();
		hideTurretSelectActually(false);
		for (int n = turretSelectContentPanel.transform.childCount-1;n>=0;n--) {
			GameObject.Destroy(turretSelectContentPanel.transform.GetChild(n).gameObject);
		}
		bool first = true;
		foreach (Trap t in traps) {
			GameObject tsp = GameObject.Instantiate(turretSelectPrefab) as GameObject;
			Button b = tsp.GetComponent<Button>();
			b.onClick.AddListener(delegate() { selectTrapTurretButton(b.gameObject); });
			MechanicalPlacementScript scr = tsp.GetComponent<MechanicalPlacementScript>();
			scr.trap = t;
			tsp.transform.parent = turretSelectContentPanel.transform;
			Text tex = tsp.transform.FindChild("Text").GetComponent<Text>();
			Image img = tsp.transform.FindChild("Image").GetComponent<Image>();
			tex.text = trapString(t);
			img.sprite = Sprite.Create(t.inventoryTexture as Texture2D, new Rect(0, 0, 64, 64), new Vector2(.5f, .5f));
			if (first) {
				lastSelected = tsp;
				setTrapTurretButtonSelected(tsp, true);
				GameGUI.selectedTrap = t;
				first = false;
			}
		}
		turretScrollBar.value = 1;
	}

	string turretString(Turret t) {
		string str = "Frame: " + t.frame.itemName + "\n" +
			"Energy Source: " + t.energySource.itemName + "\n" +
				"Gear: " + t.gear.itemName + "\n" +
				"Applicator: " + t.applicator.itemName;
		return UnitGUI.getSmallCapsString(str, 9);
	}

	string trapString(Trap t) {
		string str = "Frame: " + t.frame.itemName + "\n" +
			"Trigger: " + t.trigger.itemName + "\n" +
				"Gear: " + t.gear.itemName + "\n" +
				"Applicator: " + t.applicator.itemName;
		return UnitGUI.getSmallCapsString(str, 9);
	}

    // Trigger everything that needs to happen at the beginning of a turn
    public static void beginTurn(Unit unit)
    {
        // Update the turn order
        if (!battleGUI.mapGenerator.isInCharacterPlacement())
            battleGUI.updateTurnOrderPanel();

        // Set CharacterInformation panels
        setAtAGlanceText(unit.getAtAGlanceString());
        setStatsText(0, unit.getCharacterStatsString1());
        setStatsText(1, unit.getCharacterStatsString2());
        setStatsText(2, unit.getCharacterStatsString3());
        setStatsText(3, unit.getCharacterStatsString4());
        setCharacterInfoText(unit.getCharacterInfoString());
        setClassFeatures(unit.getClassFeatureStrings());
        disableAllButtons();
        hideActionArms();
        if (unit.team == 0 && !battleGUI.mapGenerator.isInCharacterPlacement())
        {
            // Delayed so that the collapsing animation can complete smoothly
            battleGUI.Invoke("refreshActionArms", 0.25f);
            //Debug.Log("Player is beginning their turn");
        }
        //else
        //    Debug.Log("NPC is begninning their turn");

        // Set the UI to display information based on the current unit taking their turn
        // Reveal relevant hidden UI elements (such as action bars that were hidden last turn)
    }


    // Display in the UI whose turn it is
	void updatePlayerTurnText() {
		float t = Time.time;
		if (t - playerTurnTextStartTime > textColorTime) {
			doPlayerText = false;
			return;
		}
	/*	if (t - playerTurnTextStartTime <= textColorAlphaTime) {
			Debug.Log(textColorAlphaScale + "  " + playerTurnTextColor.a + "  " + Time.deltaTime + "  " + (Time.deltaTime * textColorAlphaScale));
			playerTurnTextColor.a += Time.deltaTime*textColorAlphaScale;
		}*/
		if (t - playerTurnTextStartTime >= textColorTime - textColorAlphaTime) {
			playerTurnTextColor.a -= Time.deltaTime * textColorAlphaScale;
		}
		playerTurnTextObject.color = playerTurnTextColor;
	}

	public static void setPlayerTurnText(string text, Color color) {
		writeToConsole(text, color);
		battleGUI.playerTurnTextObject.text = text;
		battleGUI.playerTurnTextColor = color;
		battleGUI.playerTurnTextColor.a = 1.0f;
		battleGUI.playerTurnTextStartTime = Time.time;
		battleGUI.doPlayerText = true;
	}
    //--------------------------------------------------------------------------------

    // Provide some hooks to the backend for the buttons to grab onto
	public void selectMovementType(string movementType) {
		GameGUI.selectMovementType(movementType);
	}
	
	public void selectStandardType(string standardType) {
		GameGUI.selectStandardType(standardType);
	}
	
	public void selectMinorType(string minorType) {
		GameGUI.selectMinorType(minorType);
	}
    //--------------------------------------------------------------------------------

	public static void selectMovementType(MovementType type, bool selected = true) {
		if (type == MovementType.None) return;
	//	battleGUI.eventSystem.SetSelectedGameObject();
		Debug.Log(type + "    " + battleGUI.movementButtons[type].transform.GetChild(0).name);
		battleGUI.movementButtons[type].transform.GetChild(0).GetComponent<Animator>().SetBool("CurrentAction",selected);
	}

	public static void selectStandardType(StandardType type, bool selected = true) {
		if (type == StandardType.None) return;
		//	battleGUI.eventSystem.SetSelectedGameObject(battleGUI.standardButtons[type].transform.GetChild(0).gameObject);
		battleGUI.standardButtons[type].transform.GetChild(0).GetComponent<Animator>().SetBool("CurrentAction",selected);
	}

	public static void selectMinorType(MinorType type, bool selected = true) {
		if (type == MinorType.None) return;
		//	battleGUI.eventSystem.SetSelectedGameObject(battleGUI.minorButtons[type].transform.GetChild(0).gameObject);
		battleGUI.minorButtons[type].transform.GetChild(0).GetComponent<Animator>().SetBool("CurrentAction",selected);
	}

	public static void setConfirmButtonShown(ConfirmButton confirmButton, bool shown) {
		battleGUI.confirmButtons[(int)confirmButton].SetActive(shown);
	}

    // Some handy methods for controlling the GUI
	public bool UIRevealed = false;
    public static void toggleUI()
    {
        battleGUI.toggleBattleUI();
    }
	public void toggleBattleUI()
	{
		consoleCanvas.GetComponent<Animator>().SetBool("Hidden", UIRevealed);
		turnOrderCanvas.GetComponent<Animator>().SetBool("Hidden", UIRevealed);
		setCharacterInfoVisibility(!UIRevealed);
        GameObject.Find("Canvas - Action Bars").GetComponent<ActionBars>().setHidden(UIRevealed);
		UIRevealed = !UIRevealed;
	}

	public static void showClassFeatureCanvas(ClassFeatureCanvas canvas) {
		battleGUI.setClassFeatureCanvasShown(canvas, true);
	}

	public static void hideClassFeatureCanvas(ClassFeatureCanvas canvas) {
		battleGUI.setClassFeatureCanvasShown(canvas, false);
	}

	public void setClassFeatureCanvasShown(ClassFeatureCanvas canvas, bool shown) {
		switch (canvas) {
		case ClassFeatureCanvas.OneOfMany:
			oneOfManyCanvas.SetActive(shown);
			break;
		case ClassFeatureCanvas.TemperedHands:
			temperedHandsCanvas.SetActive(shown);
			break;
		}
	}

    public static void setCharacterInfoVisibility(bool isVisible)
    {
        battleGUI.characterInfoCanvas.GetComponent<Animator>().SetBool("Hidden", !isVisible);
    }

    public void toggleCIPanel(GameObject panel)
    {
        switch(panel.name)
        {
            case "Panel - Character Stats":
                toggleCIPanel(CIPanel.Stats);
                break;
            case "Panel - Class Features":
                toggleCIPanel(CIPanel.Skills);
                break;
            default:
                break;
        }
    }

    private void toggleCIPanel(CIPanel panel)
    {
		//Debug.Log("Exposed" + panel.ToString());
		toggleAnimatorBool(CIPanels[(int)panel].GetComponent<Animator>(), "Exposed" + panel.ToString());

        if (panel == CIPanel.Skills)
        {
            if(CIPanels[(int)CIPanel.Stats].GetComponent<Animator>().GetBool("ExposedStats"))
				toggleAnimatorBool(CIPanels[(int)CIPanel.Stats].GetComponent<Animator>(), "ExposedStats");
        }
        else if (panel == CIPanel.Stats)
        {
			if(CIPanels[(int)CIPanel.Skills].GetComponent<Animator>().GetBool("ExposedSkills"))
				toggleAnimatorBool(CIPanels[(int)CIPanel.Skills].GetComponent<Animator>(), "ExposedSkills");
        }
    }

	public void toggleConsole()
	{
		toggleAnimatorBool(consoleCanvas.GetComponent<Animator>(), "Dismissed");
		writeToConsoleActually("Somebody just toggled the console", Color.black);
		GameObject.Find("Canvas - Action Bars").GetComponent<ActionBars>().adjustArmsForConsole();
		//cycleTurnOrder();
	}

    public void toggleTurnOrderMinimize()
    {
        toggleAnimatorBool(GameObject.Find("Canvas - Turn Order").GetComponent<Animator>(), "Revealed");
    }
	private void toggleAnimatorBool(Animator animator, string boolName)
	{
		animator.SetBool(boolName, !animator.GetBool(boolName));
	}
    //--------------------------------------------------------------------------------


    // Set the text for the Character Information panel in the upper right corner
	public static void setAtAGlanceText(string text) {
		if (battleGUI==null) return;
		battleGUI.atAGlanceText.text = text;
	}

	public static void setStatsText(int statNum, string text) {
		if (battleGUI==null) return;
		battleGUI.statsTexts[statNum].text = text;
	}

	public static void setCharacterInfoText(string text) {
		if (battleGUI == null) return;
		battleGUI.characterInfoText.text = text;
	}

    public static void setClassFeatures(string[] classFeatureStrings)
    {
        if (battleGUI == null) return;
        battleGUI.setClassFeaturesActually(classFeatureStrings);
    }

    public void setClassFeaturesActually(string[] classFeatureStrings)
    {
        if (currentClassFeatures != null)
        {
            foreach (GameObject feature in currentClassFeatures)
            {
                GameObject.Destroy(feature);
            }
        }
        currentClassFeatures = new GameObject[classFeatureStrings.Length];
        for (int n = 0; n < classFeatureStrings.Length; n++)
        {
            GameObject textToAdd = (GameObject)Instantiate(classFeaturePrefab);
            Text textComponent = textToAdd.GetComponent<Text>();
            textComponent.text = classFeatureStrings[n];
            textToAdd.transform.SetParent(featuresContentPanel.transform);
            currentClassFeatures[n] = textToAdd;
        }
        //	Debug.Log(featuresScrollBar.value);
        //	featuresScrollBar.value = 1;
        //	Debug.Log(featuresScrollBar.value);
        //		featuresScrollBar.
    }
    //--------------------------------------------------------------------------------

    // Write text to the Console
	public static void writeToConsole(string message) {
		writeToConsole(message, Color.black);
	}
	
	public static void writeToConsole(string message, Color color) {
		if (battleGUI == null) return;
		battleGUI.writeToConsoleActually(message, color);
	}

	private void writeToConsoleActually(string message, Color color)
	{
        // Get the number of messages written on the console.
		int numMessages = consoleText.Count;

        // Create a text entry, pop an entry off the queue if necessary, then add the new one.
		GameObject textToAdd = (GameObject)Instantiate(consoleMessagePrefab);
		if(numMessages >= maxNumMessages)
		{
			GameObject deleteThis = (GameObject)consoleText.Dequeue();
			GameObject.Destroy(deleteThis);
		}
		consoleText.Enqueue(textToAdd);

        // Set the text of the message
		Text textComponent = textToAdd.GetComponent<Text>();
		textComponent.text = UnitGUI.getSmallCapsString(message, textComponent.fontSize - 4);
		textToAdd.transform.SetParent(consoleContentPanel.transform);
		textComponent.color = color;

        // Move the console scrollbar to the bottom to show the new message
        consoleScrollBar.value = 0;

        // Adjust the padding on the top of the console if needed. This buffer initially inflates the panel to fit the
        // console, but as text is added, it's no longer needed and it adds a strange empty space if you scroll up.
        consoleContentPanel.GetComponent<VerticalLayoutGroup>().padding.top -= 40; 
        if (consoleContentPanel.GetComponent<VerticalLayoutGroup>().padding.top < 25)
            consoleContentPanel.GetComponent<VerticalLayoutGroup>().padding.top = 25;

	}
    //--------------------------------------------------------------------------------


    // Manage the available Action Buttons
	public static void disableAllButtons() {
		if (battleGUI == null) return;
		foreach (GameObject button in battleGUI.minorButtons.Values) {
			button.SetActive(false);
		}
		foreach (GameObject button in battleGUI.standardButtons.Values) {
			button.SetActive(false);
		}
		foreach (GameObject button in battleGUI.movementButtons.Values) {
			button.SetActive(false);
		}
	}

	public static void enableButtons(MinorType[] minorTypes, MovementType[] movementTypes, StandardType[] standardTypes) {
		if (battleGUI == null) return;
		foreach (MinorType type in minorTypes) {
			battleGUI.minorButtons[type].SetActive(true);
			Button b;
		}
		foreach (MovementType type in movementTypes) {
			battleGUI.movementButtons[type].SetActive(true);
		}
		foreach (StandardType type in standardTypes) {
			battleGUI.standardButtons[type].SetActive(true);
		}
	}
    public static void hideActionArms()
    {
		hideMinorArm();
		hideStandardArm();
		hideMovementArm();
	}
	public static void hideMinorArm(bool hidden = true) {
        GameObject.Find("Image - Minor Arm").GetComponent<Animator>().SetBool("Hidden", hidden);
	}
	public static void hideStandardArm(bool hidden = true) {
        GameObject.Find("Image - Standard Arm").GetComponent<Animator>().SetBool("Hidden", hidden);
	}
	public static void hideMovementArm(bool hidden = true) {
        GameObject.Find("Image - Movement Arm").GetComponent<Animator>().SetBool("Hidden", hidden);
    }
    private void refreshActionArms()
    {
        Unit unit = mapGenerator.getCurrentUnit();
		enableButtons(unit.getMinorTypes(), unit.getMovementTypes(), unit.getStandardTypes());
		hideMinorArm(false);
		hideStandardArm(false);
		hideMovementArm(false);
		unit.chooseNextBestActionType();
    }
    //--------------------------------------------------------------------------------

	
    // Manage the Turn Order panel
    public void updateTurnOrderPanel()
    {
        // Grab the Turn Order Panel (container/parent for the player entries) for convenience
        GameObject turnOrderPanel = GameObject.Find("Panel - Character Entries");

        // if the Turn Order panel already has children, then new character(s) has/have entered the fray.
        if (turnOrderPanel.transform.childCount > 0)
        {
            List<GameObject> characterEntries = new List<GameObject>();
            // Handle this by simpy clearing the turn order before moving on 
            for (int i = turnOrderPanel.transform.childCount - 1; i >= 0; i--)
            {
                characterEntries.Add(turnOrderPanel.transform.GetChild(i).gameObject);
            }
            turnOrderPanel.transform.DetachChildren();
            foreach (GameObject character in characterEntries)
            {
                Destroy(character);
            }
        }

        // Get the set of characters to add to the turn order
        List<Unit> characters = mapGenerator.priorityOrder;

        // Add the characters to the Turn Order panel according to their sorted order
        foreach (Unit c in characters)
            addToPlayerOrder(c);

        // If necessary, cycle the Turn Order panel until the player whose turn it is rises to the top
        // (this is only likely to matter if new characters enter the fray after combat has already started)
        //Debug.Log(turnOrderPanel.transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text);
        //Debug.Log(mapGenerator.getCurrentUnit().characterSheet.personalInfo.getCharacterName().fullName());
        for (int i = 0; i < turnOrderPanel.transform.childCount; i++)
        {
            if (turnOrderPanel.transform.GetChild(0).transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text
            != mapGenerator.getCurrentUnit().characterSheet.personalInfo.getCharacterName().fullName())
            {
                cycleTurnOrder();

                //Debug.Log("Yo.");
            }
            else break;
        }
    }
    
    private void addToPlayerOrder(Unit unit)
	{
        GameObject turnOrderPanel = GameObject.Find("Panel - Character Entries");
		GameObject turnOrderEntry = (GameObject)Instantiate(turnOrderPrefab);
        List<Unit> activatedCharacters = new List<Unit>(mapGenerator.priorityOrder);
        // If the unit is not participating in combat (they're not activated), take it out.
        foreach (Unit enemy in mapGenerator.nonAlertEnemies)
        {
            activatedCharacters.Remove(enemy);
        }
        // If it wasn't taken out, add it to the turn order.
        if (activatedCharacters.Contains(unit))
        {
            turnOrderEntry.transform.SetParent(turnOrderPanel.transform);
            turnOrderEntry.transform.SetAsLastSibling();
            // Set turn order number (the box on the left) based on their position in the list of activated units
            turnOrderEntry.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = (activatedCharacters.IndexOf(unit) + 1).ToString();
            // Set the name (the box on the right) to the unit's full name
            turnOrderEntry.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = unit.characterSheet.personalInfo.getCharacterName().fullName();
            if (unit.deadOrDyingOrUnconscious())
                turnOrderEntry.GetComponent<CanvasGroup>().alpha = 0.5f;
            else
                turnOrderEntry.GetComponent<CanvasGroup>().alpha = 1.0f;
        }

	}

	private void cycleTurnOrder()
	{
		// The unit on top needs to be sent to the bottom of the list, and everybody else gets to slide up.
		// Since the turn order is using a Vertical Layout group, the order is dictated by the sibling index of
		// each entry, with Panel - Character Entries as the parent. Thankfully, RectTransform has the handy SetAsLastSibling method!
		// When the top entry is set as the last sibling, the Vertical Layout group will send it to the bottom and the other entries will slide up
		GameObject.Find("Panel - Character Entries").transform.GetChild(0).GetComponent<RectTransform>().SetAsLastSibling();
	}
    //--------------------------------------------------------------------------------


	
}
