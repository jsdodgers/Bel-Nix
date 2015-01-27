using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class BattleGUI : MonoBehaviour {

	static BattleGUI battleGUI;
    // Let's grab the Character Info Panels from the editor
    [SerializeField] private GameObject[] CIPanels = new GameObject[3];
	[SerializeField] private GameObject consoleCanvas;
	[SerializeField] private GameObject turnOrderCanvas;
	[SerializeField] private GameObject characterInfoCanvas;
	[SerializeField] private GameObject consoleContentPanel;
	[SerializeField] private GameObject consoleMessagePrefab;
	[SerializeField] private GameObject turnOrderPrefab;
	Text atAGlanceText;
	Text[] statsTexts;
	private Queue consoleText = new Queue();
	private int maxNumMessages = 20;
    // Numbers as indices aren't very informative. Let's use enums.
    public enum CIPanel { GLANCE, STATS, SKILLS, BUTTONS };
	
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
                toggleCIPanel(CIPanel.STATS);
                break;
            case "Panel - Class Features":
                toggleCIPanel(CIPanel.SKILLS);
                break;
            default:
                break;
        }
    }
    private void toggleCIPanel(CIPanel panel)
    {
		toggleAnimatorBool(CIPanels[(int)panel].GetComponent<Animator>(), "Exposed");

        if (panel == CIPanel.SKILLS)
        {
            if(CIPanels[(int)CIPanel.STATS].GetComponent<Animator>().GetBool("Exposed"))
				toggleAnimatorBool(CIPanels[(int)CIPanel.STATS].GetComponent<Animator>(), "Exposed");
        }
        else if (panel == CIPanel.STATS)
        {
			if(CIPanels[(int)CIPanel.SKILLS].GetComponent<Animator>().GetBool("Exposed"))
				toggleAnimatorBool(CIPanels[(int)CIPanel.SKILLS].GetComponent<Animator>(), "Exposed");
        }
    }

	public void toggleConsole()
	{
		toggleAnimatorBool(consoleCanvas.GetComponent<Animator>(), "Hidden");
		writeToConsoleActually("Somebody just toggled the console", Color.black);
		//GameObject.Find("Image - Minor Arm").GetComponent<Animator>().SetBool("Console Expanded", !consoleCanvas.GetComponent<Animator>().GetBool("Hidden"));
		//GameObject.Find("Image - Movement Arm").GetComponent<Animator>().SetBool("Console Expanded", !consoleCanvas.GetComponent<Animator>().GetBool("Hidden"));
		//GameObject.Find("Image - Standard Arm").GetComponent<Animator>().SetBool("Console Expanded", !consoleCanvas.GetComponent<Animator>().GetBool("Hidden"));
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
        GameObject.Find("Scrollbar - Console").GetComponent<Scrollbar>().value = 0;

        // Adjust the padding on the top of the console if needed. This buffer initially inflates the panel to fit the
        // console, but as text is added, it's no longer needed and it adds a strange empty space if you scroll up.
        consoleContentPanel.GetComponent<VerticalLayoutGroup>().padding.top -= 40; 
        if (consoleContentPanel.GetComponent<VerticalLayoutGroup>().padding.top < 25)
            consoleContentPanel.GetComponent<VerticalLayoutGroup>().padding.top = 25;

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
		GameObject.Find("Canvas - Character Info").GetComponent<Animator>().Play("CI_Panel_Hiding");
		GameObject.Find("Canvas - Console").GetComponent<Animator>().Play("Console_Dismissed");
		foreach(GameObject panel in CIPanels)
			toggleAnimatorBool(panel.GetComponent<Animator>(), "Hidden");
	}
	
    
	// Update is called once per frame
	void Update () {
		UnitGUI.doTabs();
        // C is for Character Sheet
        // V is for Class Features
        if (Input.anyKeyDown && !UIRevealed)
        {
			// Debug only: Reveal the Character UI once a key is pressed.
			toggleBattleUI();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            toggleCIPanel(CIPanel.STATS);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            toggleCIPanel(CIPanel.SKILLS);
        }
	}

	void OnGUI() {
		GameGUI.doGUI();
	}
}
