using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class BattleGUI : MonoBehaviour {

	static BattleGUI battleGUI;
    // Let's grab the Character Info Panels from the editor
    [SerializeField] private GameObject[] CIPanels = new GameObject[3];
	[SerializeField] private GameObject consoleCanvas;
	[SerializeField] private GameObject turnOrderCanvas;
	[SerializeField] private GameObject characterInfoCanvas;
	[SerializeField] private GameObject consoleContentPanel;
	[SerializeField] private GameObject featuresContentPanel;
	[SerializeField] private GameObject consoleMessagePrefab;
	[SerializeField] private GameObject classFeaturePrefab;
	[SerializeField] private GameObject turnOrderPrefab;
	[SerializeField] private Text playerTurnTextObject;
	Text atAGlanceText;
	Text[] statsTexts;
	Text characterInfoText;
	Scrollbar featuresScrollBar;
	Scrollbar consoleScrollBar;
	Dictionary<MovementType, GameObject> movementButtons;
	Dictionary<StandardType, GameObject> standardButtons;
	Dictionary<MinorType, GameObject> minorButtons;
	private Queue consoleText = new Queue();
	private GameObject[] currentClassFeatures = null;
	private int maxNumMessages = 20;
	public bool doPlayerText;
	Color playerTurnTextColor;
	float playerTurnTextStartTime;
	const float textColorAlphaTime = .5f;
	const float textColorTime = 1.5f;
	const float textColorAlphaScale = 1.0f/textColorAlphaTime;
    // Numbers as indices aren't very informative. Let's use enums.
    public enum CIPanel { Glance, Stats, Skills, Buttons };

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

	public void selectMovementType(string movementType) {
		GameGUI.selectMovementType(movementType);
	}
	
	public void selectStandardType(string standardType) {
		GameGUI.selectStandardType(standardType);
	}
	
	public void selectMinorType(string minorType) {
		GameGUI.selectMinorType(minorType);
	}

	public bool UIRevealed = false;
	public void toggleBattleUI()
	{
		consoleCanvas.GetComponent<Animator>().SetBool("Hidden", UIRevealed);
		turnOrderCanvas.GetComponent<Animator>().SetBool("Hidden", UIRevealed);
		characterInfoCanvas.GetComponent<Animator>().SetBool("Hidden", UIRevealed);
        GameObject.Find("Canvas - Action Bars").GetComponent<ActionBars>().setHidden(UIRevealed);
		UIRevealed = !UIRevealed;
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
		Debug.Log("Exposed" + panel.ToString());
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
		toggleAnimatorBool(consoleCanvas.GetComponent<Animator>(), "Hidden");
		writeToConsoleActually("Somebody just toggled the console", Color.black);
		GameObject.Find("Canvas - Action Bars").GetComponent<ActionBars>().adjustArmsForConsole();
		//cycleTurnOrder();
	}

	private void toggleAnimatorBool(Animator animator, string boolName)
	{
		animator.SetBool(boolName, !animator.GetBool(boolName));
	}

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
		}
		foreach (MovementType type in movementTypes) {
			battleGUI.movementButtons[type].SetActive(true);
		}
		foreach (StandardType type in standardTypes) {
			battleGUI.standardButtons[type].SetActive(true);
		}
	}

	public static void setClassFeatures(string[] classFeatureStrings) {
		if (battleGUI == null) return;
		battleGUI.setClassFeaturesActually(classFeatureStrings);
	}

	public void setClassFeaturesActually(string[] classFeatureStrings) {
		if (currentClassFeatures!=null) {
			foreach (GameObject feature in currentClassFeatures) {
				GameObject.Destroy(feature);
			}
		}
		currentClassFeatures = new GameObject[classFeatureStrings.Length];
		for (int n=0;n<classFeatureStrings.Length;n++) {
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

	private void addToPlayerOrder(Unit unit)
	{
		GameObject turnOrderEntry = (GameObject)Instantiate(turnOrderPrefab);
		turnOrderEntry.transform.GetChild(0).GetComponent<Text>().text = unit.getInitiative().ToString();
		turnOrderEntry.transform.GetChild(1).GetComponent<Text>().text = unit.name;

	}

	private void cycleTurnOrder()
	{
		// The unit on top needs to be sent to the bottom of the list, and everybody else gets to slide up.
		// Since the turn order is using a Vertical Layout group, the order is dictated by the sibling index of
		// each entry, with Panel - Character Entries as the parent. Thankfully, RectTransform has the handy SetAsLastSibling method!
		// When the top entry is set as the last sibling, the Vertical Layout group will send it to the bottom and the other entries will slide up
		GameObject.Find("Panel - Character Entries").transform.GetChild(0).GetComponent<RectTransform>().SetAsLastSibling();
	}

	// Use this for initialization
	void Start () {
		battleGUI = this;
        // By default, the Character Info Canvas' animator idles on "Dismissed," which is visible.
        // We don't want to to be visible until the UI is revealed later, so we have to set it to "Hiding".
		GameGUI.initialize();
		atAGlanceText = GameObject.Find("Text - At a Glance").GetComponent<Text>();
		statsTexts = new Text[4];
		for (int n=0;n<4;n++) {
			statsTexts[n] = GameObject.Find("Text - Stats" + (n+1)).GetComponent<Text>();
		}
		characterInfoText = GameObject.Find("Text - Character Info").GetComponent<Text>();
		featuresScrollBar = GameObject.Find("Scrollbar - Features").GetComponent<Scrollbar>();
		consoleScrollBar = GameObject.Find("Scrollbar - Console").GetComponent<Scrollbar>();
		MinorType[] minorTypes = new MinorType[] {MinorType.Loot, MinorType.Stealth, MinorType.Mark, MinorType.TemperedHands, MinorType.Escape, MinorType.Invoke};
		MovementType[] movementTypes = new MovementType[] {MovementType.Move, MovementType.BackStep, MovementType.Recover};
		StandardType[] standardTypes = new StandardType[] {StandardType.Attack, StandardType.OverClock, StandardType.Throw, StandardType.Intimidate, StandardType.Place_Turret, StandardType.Lay_Trap, StandardType.Inventory};
		minorButtons = new Dictionary<MinorType, GameObject>();
		standardButtons = new Dictionary<StandardType, GameObject>();
		movementButtons = new Dictionary<MovementType, GameObject>();
		foreach (MinorType minorType in minorTypes) {
			minorButtons[minorType] = GameObject.Find("Image - " + Unit.getNameOfMinorType(minorType));
		}
		foreach (MovementType movementType in movementTypes) {
			movementButtons[movementType] = GameObject.Find("Image - " + Unit.getNameOfMovementType(movementType));
		}
		foreach (StandardType standardType in standardTypes) {
			standardButtons[standardType] = GameObject.Find("Image - " + Unit.getNameOfStandardType(standardType));
		}
		GameObject.Find("Canvas - Character Info").GetComponent<Animator>().Play("CI_Panel_Hiding");
		GameObject.Find("Canvas - Console").GetComponent<Animator>().Play("Console_Dismissed");
		foreach(GameObject panel in CIPanels)
			toggleAnimatorBool(panel.GetComponent<Animator>(), "Hidden");
	}
	
    
	// Update is called once per frame
	void Update () {
		UnitGUI.doTabs();
		if (doPlayerText) updatePlayerTurnText();

        // C is for Character Sheet
        // V is for Class Features
        if (Input.anyKeyDown && !UIRevealed)
        {
			// Debug only: Reveal the Character UI once a key is pressed.
			toggleBattleUI();
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

	void OnGUI() {
		GameGUI.doGUI();
	}
}
