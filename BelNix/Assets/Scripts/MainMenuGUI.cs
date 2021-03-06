﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class MainMenuGUI : MonoBehaviour  {

	// Let's get all of our buttons into one place.
	// [SerializeField] lets us expose this *private* Button array to the Unity Editor,
	// where we'll assign the New Game, Load Game, Options, and Exit buttons to slots 0 through 3.
	// Let's also include a prefab for save entries that we'll instantiate later
	[SerializeField] private GameObject[] buttons = new GameObject[4];
	[SerializeField] private GameObject saveEntry;
	private string loadingName = "";
	private string[] saves;
	private bool loading = false;
	private Vector2 loadingScrollPos = new Vector2();

	//public Texture splashArt;
	/*
	static float boxX = Screen.width/4.0f;
	static float boxY = Screen.height/2.0f;
	static float boxHeight = 250.0f;
	static float boxWidth = 200.0f;
	static float buttX = boxX = 20.0f;
	static float buttWidth = 200.0f;
	*/

	// Use this for initialization
	void Start ()  {
		// When the main level loads, check if there are any save files. If there are,
		// then enable the Load Game button, populate the list of save files in the UI,
		// and set Load Game to be the default selection instead of New Game
		saves = Saves.getSaveFiles();
		populateSaves();
		enableLoadButton();
	}

	// If there aren't any saves, this does nothing, otherwise it enables and sets focus to the Load Game button
	public void enableLoadButton()  {
		if(saves.Length > 0)  {
			buttons[1].GetComponent<Button>().interactable = true;
	//		GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(buttons[1]);
		}
	}

	public void resetFocus()  {
	/*	if(saves.Length > 0)
			GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(buttons[1]);
		else
			GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(buttons[0]);*/
	}

	public void disableButtons()  {
		// Run through all four buttons and disable them
		foreach(GameObject g in buttons)
			g.GetComponent<Button>().interactable = false;
	}


	public void newGame()  {
		Saves.removeFilesFromCurrentSaveFile();
		//Load into Character Creation
		PlayerPrefs.SetInt("playercreatefrom", Application.loadedLevel);
		Application.LoadLevel(1);
	}

	//public void options()
	// {
		//GameObject.Find("Panel - Load Game").GetComponent<Animator>().SetBool(	"Exposed", false);
		//GameObject.Find("Canvas - Options").GetComponent<Animator>().SetBool(	"Exposed", true);
		//disableButtons();
	//}

	// Pass in a panel that uses the UI Panel Controller. cancel() will dismiss it.
	//public void cancel(GameObject panel)
	// {
	//	panel.GetComponent<Animator>().SetBool("Exposed", false);
	//	enableButtons();
	//}

	public void quit()  {
		Application.Quit();
	}

	private void enableButtons()  {
		// Run through all four buttons except Load Game and enable them. Put focus on New Game
		for(int i = 0; i < buttons.Length; i++)  {
			if(i != 1)	// 1 is Load Game
				buttons[i].GetComponent<Button>().interactable = true;
		}
	//	GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(buttons[0]);
		// If there are no saves, keep Load Game disabled, otherwise enable it and set focus to it
		enableLoadButton();
	}
	private void populateSaves()  {
		// Step 1: need to format the canvas to fit the number of saves and look correct
		GameObject savedGameCanvas = GameObject.Find("Canvas - Content");
		RectTransform savedGameCanvasRect =  GameObject.Find("Canvas - Content").GetComponent<RectTransform>();
		// Set the canvas height to fit all of the save files
		// height should be (#saves * buttonheight) + ((#saves-1) * inter-entry padding) + top padding + bottom padding
		int numSaves = saves.Length;
		float buttonHeight = 60;	// Currently the height of each button. Later I should grab this from the prefab.
		float topPadding = savedGameCanvas.GetComponent<VerticalLayoutGroup>().padding.top;
		float botPadding = savedGameCanvas.GetComponent<VerticalLayoutGroup>().padding.bottom;
		float newHeight = 	(numSaves * buttonHeight) + 	// cumulative button height
							topPadding + botPadding + 		// padding at the top and bottom
							((numSaves-1) * savedGameCanvas.GetComponent<VerticalLayoutGroup>().spacing);	// cumulative spacing between buttons
		if(newHeight < GameObject.Find("ScrollView - Save Files").GetComponent<RectTransform>().rect.height)  {
			newHeight = GameObject.Find("ScrollView - Save Files").GetComponent<RectTransform>().rect.height;
		}
		savedGameCanvasRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
		savedGameCanvasRect.anchoredPosition = new Vector2(savedGameCanvasRect.anchoredPosition.x, newHeight/-2);

		//Step 2: We need to instantiate a button for each save and add them as children of the canvas.
		// While we're at it, we'll also set the text field of the save entry to the save name.
		for(int i = 0; i < saves.Length; i++)  {
			GameObject newSaveEntry = (GameObject)Instantiate(saveEntry);
			newSaveEntry.transform.SetParent(savedGameCanvas.transform);
			newSaveEntry.transform.GetChild(1).GetComponentInChildren<Text>().text = saves[i];
		}
	}

	// Update is called once per frame
	void Update ()  {
		// If the escape key is pressed, it should dismiss the load game or options panel, or quit
		if(Input.GetKeyDown(KeyCode.Escape))  {
			GameObject loadCanvas = GameObject.Find("Canvas - Load Game");
			GameObject optionsCanvas = GameObject.Find("Canvas - Options");
			if(loadCanvas.GetComponent<CanvasGroup>().alpha == 1)
			 {
				loadCanvas.GetComponent<Animator>().SetTrigger("Dismissed");
				enableButtons();
			}
			else if(optionsCanvas.GetComponent<CanvasGroup>().alpha == 1)
			 {
				optionsCanvas.GetComponent<Animator>().SetTrigger("Dismissed");
				enableButtons();
			}
			else quit();
		}
		// With how the Unity 4.6 UI works, if you click somewhere besides a button, you'll drop focus
		// on the button and no longer be able to use the arrow keys to navigate the buttons until you click on one again.
		// Solution: on Mouse up (any mouse key), reset the focus to New Game (or Load Game if available)
		//else if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2))
		//	resetFocus();
	}
}
