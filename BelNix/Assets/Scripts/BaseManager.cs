using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseManager : MonoBehaviour {

	string saveName = "";
	string[] saves;
	bool saving = false;

	bool middleDraggin = false;
	bool rightDraggin = false;
	
	bool mouseLeftDown;
	bool mouseRightDown;
	bool mouseMiddleDown;

	public string[] missions = new string[]{"Test Map 1", "Test Map 2"};
	public int[] missionLevels = new int[]{4, 3};
	Vector2 savesScrollPos = new Vector2();

	GameObject hoveredObject;

	string tooltip = "";
	Dictionary<string, string> tooltips = null;

	int oldTouchCount = 0;
	Vector3 lastPos = new Vector3();
	// Use this for initialization
	void Start () {
		tooltips = new Dictionary<string, string>();
		tooltips.Add("barracks", "Barracks");
		tooltips.Add("engineering", "Create Traps and Turrets");
		tooltips.Add("exit", "Exit to Main Menu");
		tooltips.Add("map", "Open Map");
		tooltips.Add("infirmary", "Infirmary");
		tooltips.Add("newcharacter", "Create a new Character");
		int n=0;
		do {
			n++;
			saveName = "Save " + n;
		} while (Saves.hasSaveFileNamed(saveName));
	}
	
	// Update is called once per frame
	void Update () {
		handleInput();
	}

	void handleInput() {
		handleKeys();
		handleDrag();
		handleKeyPan();
		handleMouseMovement();
		handleMouseClick();
	}

	void handleMouseClick() {
		if (Input.GetMouseButtonDown(0)) {
			if (hoveredObject != null) {
				if (hoveredObject.tag == "exit")
					Application.LoadLevel(0);
				else if (hoveredObject.tag=="map") {
					loadMapScrollPos = new Vector2();
					choosingMap = true;
				}
				else if (hoveredObject.tag=="newcharacter") {
					Application.LoadLevel(1);
				}
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

	void handleKeys() {
		mouseLeftDown = Input.GetMouseButton(0);
		mouseRightDown = Input.GetMouseButton(1);
		mouseMiddleDown = Input.GetMouseButton(2);
		if (!rightDraggin) middleDraggin = ((mouseMiddleDown || (mouseLeftDown && Input.touchCount==2)) && middleDraggin) || Input.GetMouseButtonDown(2);
		if (!middleDraggin) rightDraggin = (rightDraggin && mouseRightDown) || Input.GetMouseButtonDown(1);
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
		if (saving || choosingMap) go = null;
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
	bool choosingMap = false;
	Vector2 loadMapScrollPos = new Vector2();
	void OnGUI() {
		if (!saving && !choosingMap) {
			if (GUI.Button(new Rect(0, 0, 100, 50), "Save Game")) {
				saves = Saves.getSaveFiles();
				saving = true;
				oldSaveName = saveName;
				savesScrollPos = new Vector2();
				string savesSt = "";
				foreach (string save in saves) {
					savesSt += save + "\n";
				}
				Debug.Log(savesSt);
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
		else if (choosingMap) {
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
				choosingMap = false;
			}
		}
		else {
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
				saving = false;
				saveName = oldSaveName;
			}
			if (GUI.Button(new Rect(buttonX2, buttonY, buttonWidth, buttonHeight), "Save")) {
				Saves.saveAs(saveName);
				saving = false;
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
	}

}
