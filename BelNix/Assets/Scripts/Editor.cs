using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;


public class Editor : MonoBehaviour  {
	
	public GameObject nodePrefab;
	//public GameObject lineSegmentPrefab;
	public Camera mainCamera;
	
	
	public List<Node> nodes = new List<Node> ();
	
	public Node currentWindow; //holds the current window
	public Node selectedWindow; //tells the editor that a window is being clicked on
	
	int totalNodes;
	string filename;
	
	
	
	
	
	
	void InstantiateTextBox() {
		
		Debug.Log ("Total Nodes = " + totalNodes);
		
		GameObject textBoxClone = Instantiate (nodePrefab) as GameObject;
		
		Node t = textBoxClone.GetComponent<Node> ();
		
		t.editor = this;
		//	t.camera = mainCamera.GetComponent<Camera> ();
		//t.lineSegmentPrefab = lineSegmentPrefab;
		t.oldscrollposition = scrollPosition;
		t.WindowID = totalNodes;
		//Debug.Log ("WindowID  " + t.WindowID);
		
		nodes.Add (t);
		
		if(totalNodes != 0) {
			
			int i = totalNodes - 1;
			
			t.windowRect.x = nodes[i].windowRect.x + 300;
			t.windowRect.y = nodes[i].windowRect.y;
			
		}
		//Debug.Log (textboxes [totalTextBoxes]);
		
		
		totalNodes++;
		
		
	}
	
	
	
	
	
	
	
	
	
	
	
	public Vector2 scrollPosition = Vector2.zero;
	
	void OnGUI() {
		
		//GUILayout.BeginArea(new Rect(0, 0, 10000, 10000));
		
		//GUI.Box(new Rect(0,0, Screen.width, Screen.height), "this is a box");
		
		//GUI.BeginGroup(new Rect(0,0, 100000, 100000));
		
		//GUI.depth = totalTextBoxes;
		//Debug.Log(GUI.depth);
		
		if(Event.current.button == 1 && Event.current.type == EventType.MouseUp) {
			
			if(selectedWindow == null) {
				
				currentWindow = null;
				Debug.Log("Selected Window Cleared");
				
				
			}
			
			/*else if(selectedWindow == currentWindow) {



			}*/
			
			
			else {
				selectedWindow = null;
			}
			
		}
		
		scrollPosition = GUI.BeginScrollView(new Rect(0, 0, Screen.width, Screen.height), scrollPosition, new Rect(0, 0, 10000, 10000));
		
		if(GUI.Button(new Rect(10, 10, 40, 25), "Save")) {
			
			WriteAllText(filename);
			
		}
		
		if(GUI.Button(new Rect(10, 35, 80, 25), "Add Node")) {
			
			InstantiateTextBox();
			
		}

		if (GUI.Button (new Rect (10, 60, 80, 25), "mybutton"))  {
				
			foreach(Node element in nodes) {

				Debug.Log(element.WindowID);

			}
		
		}
		
		filename = GUI.TextField(new Rect(60, 10, 60, 25), filename);
		
		
		
		GUI.EndScrollView();
		
		//GUILayout.EndArea();
		//GUI.EndGroup();
		
	}
	
	
	
	
	
	//Saves project under file name in the Saves folder within Assets
	void WriteAllText(string filename) {
		
		if (filename != "")  {
			
			string path = Application.dataPath + "/Saves/" + filename + ".txt";
			
			StreamWriter sw = new StreamWriter (path, false);
			
			foreach (Node element in nodes) {
				
				sw.WriteLine (element.ThisBoxToString());
			}
			
			sw.Close ();
			/*StreamWriter sw = new StreamWriter (path, false);
			
			sw.WriteLine (s);
			sw.Close ();*/
			
			
		}
		
		
		
		else  {
			
			Debug.Log("File Name Required");
			
		}
		
		
		
		
		
		//File.WriteAllText(path, newString);
		
		
		
		
		
	}
	
	
	
	
	
	
	
	void Start ()  {
		
		filename = "";
		totalNodes = 0;
		
		
	}
	
	
	
	void Update() {
		
		
		
		
		
	}
	
}
