using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine.UI;

public class Conversation : MonoBehaviour {
	
	public TextAsset textFile;
	public GameObject UI_Panel_Prefab;
	public Canvas canvas;
	GameObject UI_Button_Prefab;
	List<GameObject> buttons;

	//finds button child of panel prefab and creates buttons list
	void CreateUIButtonPrefab(GameObject UIPrefab){

		UI_Button_Prefab = UIPrefab.transform.FindChild ("ButtonPrefab").gameObject;
		//GameObject buttonChildPrefab = buttonChildPrefabTemp;
		//Destroy (UI_Button_Prefab);
		buttons = new List<GameObject> ();
	
	}





	//Assigns text to be displayed by a UI Prefab, most likely either the UI_Panel or a button clone
	void AssignText(GameObject UIPrefab, string text){

		Text textChild = UIPrefab.transform.FindChild ("Text").gameObject.GetComponent<Text> ();
		textChild.enabled = true; //Only affects when buttons are instantiated

		textChild.text = text;
	
	}
	




	//creates buttons to list available dialogue options 
	void InstantiateButtons(GameObject UIParentPrefab, GameObject UIButtonPrefab, TextBox currentTextBox){
	
		//GameObject buttonChildPrefabTemp = UIButtonPrefab;
	
		//buttons = new List<GameObject> ();

		for (int i = 0; i < currentTextBox.nextWindowID.Count; i++) {
				
			GameObject buttonClone = Instantiate(UIButtonPrefab) as GameObject;
			buttonClone.transform.SetParent(UIParentPrefab.transform, false);
			RectTransform rect = buttonClone.GetComponent<RectTransform>();
			rect.position = new Vector3(rect.position.x, rect.position.y - (40 * i), rect.position.z);

			Button button = buttonClone.GetComponent<Button>();
			button.enabled = true;
			Image image = buttonClone.GetComponent<Image>();
			image.enabled = true;

			TextBox t = FindTextBox(currentTextBox.nextWindowID[i]);

			AssignText(buttonClone, t.text);

			buttons.Add(buttonClone);
			//button.onClick.AddListener(delegate{DestroyButtons();});


			if(t.terminatesDialogue){

				button.onClick.AddListener(TerminateConversation);
	
			}

			else{

				button.onClick.AddListener(delegate{DisplayTextBox(FindTextBox(t.nextWindowID[0]));});

			}






		
		}




	}


	//destroys button objects and clears the buttons list.
	void DestroyButtons(){

		Debug.Log ("DestroyButtons");
		//if there are no buttons to destroy an clear, pass 
		if (buttons.Count != 0) {
				

			foreach (GameObject b in buttons) {
				
				Destroy(b);
				
				
			}
			
			buttons.Clear ();

		
		}

		else{}



	}

	TextBox FindTextBox(int windowID){

		foreach (TextBox t in textBoxes) {
				
			if(windowID == t.windowID){

				return t;
			}
		
		}


		return null;
	
	}

	void DisplayTextBox(TextBox t){

		DestroyButtons ();
		
		AssignText (UI_Panel_Prefab, t.text);
		
		InstantiateButtons (UI_Panel_Prefab, UI_Button_Prefab, t);


	}
	


	
	List<TextBox> textBoxes;

	class Conditional{
		
		public string type;
		public string condition;
		
		
		
		public Conditional(string t, string c){
			
			type = t;
			condition = c;
			
			
		}
		
	}






	class TextBox{
		
		public string text; 
		public int windowID;
		public int type;
		public List<int> nextWindowID;
		public bool terminatesDialogue;
		//public List<Conditional> conditionals;
		
		public TextBox(string t, int id, int ty, List<int> idArray, bool td){
			
			text = t; 
			windowID = id;
			type = ty;
			nextWindowID = idArray;
			terminatesDialogue = td;
			//conditionals = con;
			
		}
		
		
	}
	

	//Reads text file provided and creates a list textboxes.
	void ReadTextFile(){
		
		Debug.Log ("ReadTextFile()");
		
		textBoxes = new List<TextBox>();
		string[] nodes;
		
		
		if (textFile != null) {
			
			nodes = (textFile.text.Split( '\n' ));
			
			
			
			for (int i = 0; i < nodes.Length; i++) {
				
				if(nodes[i] != ""){
					
					textBoxes.Add(NodetoTextBox(nodes[i]));
					
				}
				
				
			}
			
			
		}
		
		else{
			
			Debug.LogError("Text file missing.");
			
		}
		
	}
	
	//converts a node of the textfile to a textbox 
	TextBox NodetoTextBox(string n){
		
		n.TrimStart ();
		n.TrimEnd ();
		
		string[] firstSplit; 	//splits the line into its its basic variables, which are seperated by ','
		// within the first split array, 0 = text, 1 = next windows array, 2 = terminates dialogue, 3 = window id, 4 = nodetype;
		
		string[] textArray;		//extracts text seperated by '[]'
		string[] nextWindowsIDArray;// extracts the next nodes in the tree for this text box indicated by {}
		string terminatesDialogueString;
		string windowIDString;
		string windowTypeString;
	//	string[] conditionalsStrings;// extracts the conditionals for this text box indicated by ()

		string text = "";
		int windowID = 0;
		int windowType = 0;
		List<int> nextWindowID = new List<int> ();
		bool terminatesDialogue = false;
		//List<Conditional> conditionals = new List<Conditional>();
		
		firstSplit = n.Split (new char[] {','});
		
		textArray = firstSplit [0].Split (new char[] {'[',']'}, System.StringSplitOptions.RemoveEmptyEntries);
		nextWindowsIDArray = firstSplit [1].Split (new char[] {'{','}'}, System.StringSplitOptions.RemoveEmptyEntries);
		terminatesDialogueString = firstSplit [2].Trim();
		windowIDString = firstSplit [3].Trim();
		windowTypeString = firstSplit [4].Trim();
		//conditionalsStrings = firstSplit [5].Split (new char[]{'(',')'}, System.StringSplitOptions.RemoveEmptyEntries);

		
		//builds textarray into the text to be displayed by this node
		for (int i = 0; i < textArray.Length; i++) {
			
			text = text + textArray[i];
			
		}
		
		text.TrimStart ();
		text.TrimEnd ();
		
		
		//builds the list of nodes that will succeed this node
		for (int i = 0; i < nextWindowsIDArray.Length; i++) {
			
			//Debug.Log(i + " : " + nextWindowsIDArray[i]);
			
			if(nextWindowsIDArray[i] != ""){
				
				string[] tempArray = nextWindowsIDArray[i].Split(new char[]{';', ' '}, System.StringSplitOptions.RemoveEmptyEntries);
				
				for(int j = 0; j < tempArray.Length; j++){
					//Debug.Log(j + " : " + tempArray[j]);
					
					if(tempArray[j] != ""){
						tempArray[j].Trim();
						//Debug.Log(tempArray[j]);
						int tempID = int.Parse(tempArray[j]);
						nextWindowID.Add(tempID);
						
					}
					
					
				}
				
				
			}
			
		}


		/*for (int i = 0; i < conditionalsStrings.Length; i++) {
			
			//Debug.Log(i + " : " + nextWindowsIDArray[i]);
			
			if(conditionalsStrings[i] != ""){
				
				string[] tempArray0 = conditionalsStrings[i].Split(new char[]{';', ' '}, System.StringSplitOptions.RemoveEmptyEntries);
				
				for(int j = 0; j < tempArray0.Length; j++){


					//Debug.Log(j + " : " + tempArray[j]);

					string[] tempArray1 = conditionalsStrings[j].Split(new char[]{'<', '>'}, System.StringSplitOptions.RemoveEmptyEntries);

					for(int l = 0; l < tempArray0.Length; l++){
						
						
						//Debug.Log(j + " : " + tempArray[j]);
						
						string[] tempArray2 = conditionalsStrings[l].Split(new char[]{'<', '>'}, System.StringSplitOptions.RemoveEmptyEntries);
						
						if(tempArray1[l] != ""){
							tempArray1[l].Trim();
							//Debug.Log(tempArray[j]);
							int tempID = int.Parse(tempArray1[l]);
							//conditionals.Add(tempID);
							
						}
						
						
					}
					
					
				}
				
				
			}
			
		}*/
		




		
		
		//determines whether this node terminates dialogue
		if (terminatesDialogueString == "False") {
			
			terminatesDialogue = false;
			
		}
		
		else if(terminatesDialogueString == "True"){
			
			terminatesDialogue = true;
			
		}
		
		else{
			Debug.LogError("Cannot create TextBox: TerminatesDialogueString format invalid");
		}
		
		
		windowID = int.Parse(windowIDString);
		windowType = int.Parse (windowTypeString);
		
		
		Debug.Log ("Text: " + text);
		
		foreach(int element in nextWindowID) {
			
			Debug.Log("nextWindowID: " + element);
			
		}
		
		Debug.Log ("Terminates Dialogue: " + terminatesDialogue);
		Debug.Log ("Window ID: " + windowID);
		Debug.Log ("WindowType: " + windowType);
		
		
		
		TextBox t = new TextBox (text, windowID, windowType, nextWindowID, terminatesDialogue);
		
		
		return t;
		
	}
	
	
	
	void InitiateConversation(){
		canvas.enabled = true;
		CreateUIButtonPrefab (UI_Panel_Prefab);
		ReadTextFile ();
		DisplayTextBox (FindTextBox (0));
		
	
	
	}

	void TerminateConversation(){

		textBoxes.Clear();
		DestroyButtons();
		canvas.enabled = false;

	}
	




	
	void Start(){
		
		//ReadTextFile ();
		//AssignText (UI_Panel_Prefab, "Hello");
		//InstantiateButtons (UI_Panel_Prefab, UI_Button_Prefab, 1);
		//DestroyButtons ();
		InitiateConversation ();
		
	}
	
	
	
}
