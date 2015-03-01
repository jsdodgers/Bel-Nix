﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Node : MonoBehaviour {
	
	public Editor editor;
	//public Camera camera;
	//public GameObject lineSegmentPrefab;
	
	int windowRectLeft;
	int windowRectTop;
	int windowRectWidth;
	int windowRectHeigth;
	bool isCollapsed;
	bool isConditionalDisplayed;
	
	
	
	/*public*/ string text; //Holds the text displayed 
	/*public*/ int textBoxID; //Individual box ID number.  Used to determine textbox order 
	/*public*/ int windowID; //Window ID for GUI.Window
	/*public*/ int textBoxType; // 0- Textfield for NPC response/prompt, 1- Button for player's dialogue choice
	/*public*/ bool terminatesDialogue;
	/*public*/ bool hasBeenRead; //Remembers player choice and allows for player choice to have consequence.
	
	///*public*/ int[] nextTextBoxID; //Finds the next text box in the dialogue tree by textBoxID #
	/*public*/ bool isPlayerResponse;
	
	/*public*/ List<int> nextTextBoxID = new List<int>();
	
	/*public*/ int conditionalWindowID;
	/*public*/ List<Conditional> conditionals = new List<Conditional>();
	
	
	
	
	
	//Methods and variables for player ID values
	
	//int playerGender; //Stores player characters gender. Passed through from DialogueTree.cs
	//string playerFirstName; // Stores player's first, last, and fully compiled name
	//string playerLastName;
	//string playerFullName;
	//public bool isTest; // For Testing Purposes Only
	
	string[] texts; //Holds the fragments of the unprocessed text
	string compiledText;//The string that is constantly added to
	string tempText;//Temporary chuck of processed text added to the compiled text
	
	//assigns player ID values passed from DialogueTree
	/*public void InstantiateValues(string fName, string lName, int gender){

		playerGender = gender;
		playerFirstName = fName;
		playerLastName = lName;
		playerFullName = fName + " " + lName;

	}*/
	
	
	public string Text{ 
		get{return text;}
	}
	
	public int WindowID{ 
		get{return windowID;} 
		set{windowID = value;}
	}
	
	
	//Defines "Conditional Class" with all string members specifying variable type and conditional to be met, 
	
	public class Conditional
	{
		
		public string type;
		public string condition;
		
		
		
		public Conditional(string t, string c){
			
			type = t;
			condition = c;
			
			
		}
		
	}
	
	
	
	
	
	
	
	//Rect windowRect = new Rect (windowRectLeft, windowRectTop, windowRectWidth, windowRectHeigth);
	//public Rect currentWindowRect= new Rect(50, 60, 250, 200);
	public Rect windowRect = new Rect (50, 60, 250, 200);
	Rect windowRectCollapsed = new Rect(50, 60, 100, 70);
	Rect conditionalRect = new Rect (50, 60, 250, 150);
	//Rect windowRect;
	
	public Vector2 oldscrollposition = new Vector2(0,0);
	
	void OnGUI () {
		//windowRect = GUI.Window (windowID, new Rect (windowRectLeft, windowRectTop, windowRectWidth, windowRectHeigth), WindowFunction, "Node_" + windowID);
		
		
		if(!isCollapsed){
			
			windowRect = GUI.Window (windowID, windowRect, WindowFunction, "Node_" + windowID);
			
		}
		
		if(isCollapsed){
			
			windowRectCollapsed = GUI.Window(windowID, windowRectCollapsed, WindowFunction, "Node_" + windowID);
			
		}
		
		if (isConditionalDisplayed && !isCollapsed) {
			
			
			conditionalRect = GUI.Window(conditionalWindowID, conditionalRect, ConditionalWindowFunction, "Conditionals_" + windowID);
			
			
		}
		
		//currentWindowRect = GUI.Window (windowID, new Rect (windowRectLeft, windowRectTop, windowRectWidth, windowRectHeigth), WindowFunction, "Node_" + windowID);
		
		WindowSync();
		
		
		windowRect.y -= editor.scrollPosition.y - oldscrollposition.y;
		windowRect.x -= editor.scrollPosition.x - oldscrollposition.x;
		windowRectCollapsed.y -= editor.scrollPosition.y - oldscrollposition.y;
		windowRectCollapsed.x -= editor.scrollPosition.x - oldscrollposition.x;
		
		oldscrollposition = editor.scrollPosition;
		
		
		
	}
	
	
	
	
	void WindowFunction (int windowID) {
		
		//editor.selectedWindow = windowID;
		//Debug.Log("This window is selected: " + windowID);
		//GUI.BeginGroup(new Rect(0, 0, 250, 200));
		
		if(Event.current.button == 1 && Event.current.type == EventType.MouseUp &&  (editor.currentWindow == this || editor.currentWindow == null)){
			
			GUI.BringWindowToFront(windowID);
			editor.selectedWindow = this;
			editor.currentWindow = this;
			Debug.Log("Window #" + editor.selectedWindow.windowID + " has been selected.");
			
		}
		
		if(Event.current.button == 1 && Event.current.type == EventType.MouseUp && (editor.currentWindow != this)){
			
			bool pass = false;
			
			foreach (int element in editor.currentWindow.nextTextBoxID){
				
				if(element == this.windowID){
					
					pass = true;
					Debug.Log("This node is already linked");
					
				}
				
			}
			
			
			
			if(!pass){
				
				LinkNodes(editor.currentWindow, this);
				
			}
		}
		
		
		
		terminatesDialogue = GUI.Toggle (new Rect (100, 25, 140, 20),terminatesDialogue, "Terminates Dialogue");
		isPlayerResponse = GUI.Toggle (new Rect (100, 40, 140, 20),isPlayerResponse, "isPlayerResponse");
		
		
		
		if(isCollapsed){
			
			isCollapsed = GUI.Toggle(new Rect(10, 25, 100, 25), isCollapsed, "Collapse"); 
			GUI.DragWindow(new Rect(0,0,1000,20));
			
		}
		
		
		else{ 
			
			if(GUI.Button(new Rect(10, 60, 140, 25), "Conditionals")){
				
				//display conditional GUI.box
				isConditionalDisplayed = !isConditionalDisplayed;
				
			}
			
			
			isCollapsed = GUI.Toggle(new Rect(10, 25, 140, 25), isCollapsed, "Collapse"); 
			//terminatesDialogue = GUI.Toggle(new Rect(10, 40, 140, 25), terminatesDialogue, "Terminates Dialogue");
			text = GUI.TextArea(new Rect(10, 90, 230, 100), text);	
			GUI.DragWindow(new Rect(0,0,1000,20));
			
			//GUI.EndGroup();
			
			//if (GUI.Button (new Rect(150, 30, 60, 25), "Options")) {
			
			//}
			//GUI.DragWindow();
			
		}
		
	}
	
	
	

	string type = "";
	string condition = "";

	void ConditionalWindowFunction(int conditionalID){


		if(GUI.Button(new Rect(220, 25, 20, 25), "+")){
			
			conditionals.Add (new Conditional (type, condition));
			type = "";
			condition = "";
			Debug.Log(conditionals[0].type +", " + conditionals[0].condition);
			
		}

		GUI.TextField (new Rect (10, 25, 90, 25), "Type");
		GUI.TextField (new Rect (110, 25, 90, 25), "Condition");

		type = GUI.TextArea(new Rect(10, 55, 90, 25), type);
		condition =  GUI.TextArea(new Rect(110, 55, 90, 25), condition);

		Rect r = new Rect (10, 55, 90, 25);
		Rect s = new Rect (110, 55, 90, 25);

		for (int i = 0; i < conditionals.Count; i++) {

			r.y = r.y + 30;
			GUI.TextField(r, conditionals[i].type);

			s.y = s.y + 30;
			GUI.TextField(s, conditionals[i].condition);

		
		}


	}
	

	




	void WindowSync(){
		
		if(!isCollapsed){
			
			conditionalRect.y = windowRect.y + windowRect.height;
			conditionalRect.x = windowRect.x;
			//conditionalRect.height = windowRect.height;
			//conditionalRect.width = windowRect.width;
			windowRectCollapsed.x = windowRect.x;
			windowRectCollapsed.y = windowRect.y;
			//currentWindowRect = windowRect;
			GUI.BringWindowToFront(conditionalWindowID);
			
		}
		
		else{
			
			windowRect.x = windowRectCollapsed.x;
			windowRect.y = windowRectCollapsed.y;
			//currentWindowRect = windowRectCollapsed;
			
		}
		
		//Debug.Log (windowRect.x + "       " + windowRect.y);
		
	}
	
	
	void LinkNodes(Node t1, Node t2){
		
		t1.nextTextBoxID.Add(t2.windowID);
		//DrawLine(editor.currentWindow, this);
		Debug.Log("Node " + t1.windowID + " is linked to node " + t2.windowID);
		editor.selectedWindow = null;
		editor.currentWindow = null;
		
		
	}
	
	
	
	
	
	//draws three lines to connect nodes
	public void DrawLine(Node t1, Node t2){
		
		
		//Debug.Log (t1.windowID);
		//Debug.Log (t2.windowID);
		
		//Vector3 tPos = windowRect.position;
		
		Rect rect1 = t1.windowRect;
		Rect rect2 = t2.windowRect;
		//Debug.Log(rect1 + ", " + rect2);
		
		/*float x1 = rect1.x;
		float y1 = rect1.y;

		Vector3 v1 = new Vector3 (x1, y1, Camera.main.nearClipPlane);
		Vector3 v2 = Camera.main.ScreenToWorldPoint (v1);

		float x2 = rect2.x;
		float y2 = rect2.y;
		
		Vector3 v3 = new Vector3 (x2, y2, Camera.main.nearClipPlane);
		Vector3 v4 = Camera.main.ScreenToWorldPoint (v3);
	*/	
		
		//Debug.Log (rect1.x + "    " + rect1.y);
		
		Vector3 rect1Pos = new Vector3(rect1.x, rect1.y, 1.0f);
		Vector3 rect2Pos = new Vector3 (rect2.x, rect2.y, 1.0f);
		
		Vector3 v1 = Camera.main.ScreenToWorldPoint (rect1Pos);
		Vector3 v2 = Camera.main.ScreenToWorldPoint (rect2Pos);
		
		v1.z = 1.0f;
		//Debug.Log (v1.x + "    " + v1.y);
		
		//Distance between nodes
		
		//float distanceX = v2.x - (v1.x + 250);
		//float distanceY = v2.y - (v1.y - 100);
		
		//Divide distance x 
		
		//float distanceXhalved = distanceX / 2;
		
		//instantiate segment1
		//GameObject lineSegmentclone1 = Instantiate (lineSegmentPrefab) as GameObject;
		//LineSegment lineSegment1 = lineSegmentclone1.GetComponent<LineSegment> ();
		//lineSegment1.InstantiateLineSegment (v1.x, v1.y, v2.x, v1.y);
		//lineSegment1.origin = t1;
		//lineSegment1.destination = t2;
		//editor.quad.transform.position = v1;
		//lineSegment1.transform.position = v1;
		//instantiate segment2
		//GameObject lineSegmentclone2 = Instantiate (lineSegmentPrefab) as GameObject;
		//LineSegment lineSegment2 = lineSegmentclone2.GetComponent<LineSegment> ();
		//lineSegment2.InstantiateLineSegment ((distanceXhalved), (rect1.y + 100), distanceXhalved, (distanceY));
		//lineSegment2.origin = t1;
		//lineSegment2.destination = t2;
		//instantiate segment3
		//GameObject lineSegmentclone3 = Instantiate (lineSegmentPrefab) as GameObject;
		//LineSegment lineSegment3 = lineSegmentclone3.GetComponent<LineSegment> ();
		//lineSegment3.InstantiateLineSegment ((distanceXhalved), (distanceY), distanceX, (distanceY));
		//lineSegment3.origin = t1;
		//lineSegment3.destination = t2;
		
		
		
		
	}
	
	
	
	
	
	
	public string ThisBoxToString(){
		
		string compiledString;
		string terminatesDialogueString = terminatesDialogue.ToString();
		string isPlayerResponseString = "";
		string windowIDString = windowID.ToString();
		string nextTextBoxIDString = "{";
		string conditionalsString = "(";
		
		foreach(int element in nextTextBoxID){
			
			string s = element.ToString();
	
			
			if(element == (nextTextBoxID[nextTextBoxID.Count - 1])){
				
				nextTextBoxIDString = nextTextBoxIDString + s;
				
				
			}
			
			else{nextTextBoxIDString = nextTextBoxIDString + s + "; ";}
			
			
			
			
		}
		
		nextTextBoxIDString = nextTextBoxIDString + "}";
		
		
		if (!isPlayerResponse) {
			
			isPlayerResponseString = "0";
		}
		
		else{
			isPlayerResponseString = "1";
		}



		for (int i = 0; i < conditionals.Count; i++) {

			if(i == conditionals.Count - 1){

				string s = "<" + conditionals[i].type + " : " + conditionals[i].condition + ">";
				conditionalsString = conditionalsString + s;

			}

			else{

				string s = "<" + conditionals[i].type + " : " + conditionals[i].condition + ">; ";
				conditionalsString = conditionalsString + s;

			}

			
		
		}
		
		conditionalsString = conditionalsString + ")";
		
		
		
		compiledString = "[" + text + "], " + nextTextBoxIDString + ", " + terminatesDialogueString + ", " + windowIDString + ", " + isPlayerResponseString + ", " + conditionalsString;
		
		return compiledString;
		
		
	}
	







	
	void Start(){
		
		//(-windowID -1) to give unique IDs for conditional windows (even for node 0)
		conditionalWindowID = (-windowID - 1);
		text = "Enter Text";
		
		//windowRectLeft = 50;
		//windowRectTop = 60;
		//windowRectWidth = 250;
		//windowRectHeigth = 180;
		
	}
	
	





	
	void Update(){
		
		//UpdateTransform (windowRect);
		
		
	}
	
	
	
	
}
