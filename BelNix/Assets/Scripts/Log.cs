using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LogMessage {
	public GUIContent message;
	public Color color;
	public LogMessage(string message) {
		this.message = new GUIContent(message);
	}
	public LogMessage(string message, Color color) : this(message) {
		this.color = color;
	}
}

public class Log : MonoBehaviour {

	public GameGUI gui;
	Queue<LogMessage> messages;
	float consoleWidth = 500;
	float consoleHeight = 200;
	Vector2 scrollPosition = new Vector2(0,0);
	float logX = 5.0f;
	float logY = 5.0f;
	bool needsScrollSet = false;
	bool hasScroller = false;

	// Use this for initialization
	void Start () {
		messages = new Queue<LogMessage>();
		addMessage("Welcome to Bel Nix.");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void addMessage(string message) {
		addMessage(message, Color.white);
	}

	public void addMessage(string message, Color c) {
		messages.Enqueue(new LogMessage(message,c));
		needsScrollSet = true;
	}

	static GUIStyle logMessageStyle;
	GUIStyle getLogMessageStyle(Color c) {
		if (logMessageStyle==null) {
			logMessageStyle = new GUIStyle("Label");
			logMessageStyle.fontSize = 13;
			logMessageStyle.wordWrap = true;
			logMessageStyle.padding = new RectOffset(0, 0, 0, 0);
		}
		logMessageStyle.normal.textColor = logMessageStyle.active.textColor = logMessageStyle.hover.textColor = c;
		return logMessageStyle;
	}

	void setScrollPosition() {
		float height = getHeight();
		scrollPosition = new Vector2(0.0f, Mathf.Max(0.0f, height - consoleHeight));
	}

	float getHeight() {
		GUIStyle st = getLogMessageStyle(Color.white);
		float height = 0;
		float width = consoleWidth - 16;
		foreach (LogMessage s in messages) {
			height += st.CalcHeight(s.message,width);
		}
		return height + logY*2;
	}

	public bool mouseIsOnGUI() {
		Vector2 mousePos = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
		if (hasScroller) {
			return (new Rect(Screen.width - 16.0f,Screen.height - consoleHeight, 16.0f, consoleHeight)).Contains(mousePos);
		}
		return false;
	}

	void OnGUI() {
		if (needsScrollSet) {
			setScrollPosition();
			needsScrollSet = false;
			hasScroller = getHeight() > consoleHeight;
		}
		if (gui.mapGenerator.isInCharacterPlacement()) return;
		GUIStyle st = getLogMessageStyle(Color.white);
		float height = getHeight();
		float width = consoleWidth - 16;
		float x = Screen.width - consoleWidth;
		float y = Screen.height - consoleHeight;
		Rect logRect = new Rect(x, y, consoleWidth, consoleHeight);
		GUI.Box(logRect,"");
		scrollPosition = GUI.BeginScrollView(logRect, scrollPosition, new Rect(x, y, width, height));
		y+=logY;
		x += logX;
		foreach (LogMessage s in messages) {
			st = getLogMessageStyle(s.color);
			float h = st.CalcHeight(s.message,width);
			GUI.Label(new Rect(x, y, width, h), s.message, st);
			y += h;
		}
		GUI.EndScrollView();
	}
}
