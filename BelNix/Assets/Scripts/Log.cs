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
	float consoleHeight = 85;
	Vector2 scrollPosition = new Vector2(0,0);
	float logX = 5.0f;
	float logY = 5.0f;
	bool needsScrollSet = false;
	bool hasScroller = false;

	float left = 150.0f;
	float right = 0.0f;

	Texture logTexture;

	// Use this for initialization
	void Start () {
		messages = new Queue<LogMessage>();
		addMessage("Welcome to Bel Nix.");
		addMessage("Welcome to Bel Nix. This is a really long message because it takes up the full screen width and some more to test if it is all scrolling and stuff properly and this one will end up being two lines to test that bingo.");
		addMessage("This is a shorter one.");
		addMessage("Another One.");
		addMessage("This is a shorter one.");
		addMessage("Another One.");
		addMessage("This is a shorter one.");
		addMessage("Another One.");
		addMessage("This is a shorter one.");
		addMessage("Another One.");
		logTexture = Resources.Load<Texture>("UI/console");
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
		float height = getHeight(left, right);
		scrollPosition = new Vector2(0.0f, Mathf.Max(0.0f, height - consoleHeight + 4.0f));
	}

	float getHeight(float left, float right) {
		GUIStyle st = getLogMessageStyle(Color.white);
		float height = 0;
		float width = Screen.width - left - right - 5.0f - 16;
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
		doGUI(left, right);
	}
	public void doGUI(float left, float right) {
		this.left = left;
		this.right = right;
		if (needsScrollSet) {
			setScrollPosition();
			needsScrollSet = false;
			hasScroller = getHeight(left, right) > consoleHeight;
		}
		if (gui.mapGenerator.isInCharacterPlacement()) return;
		GUIStyle st = getLogMessageStyle(Color.white);
		float consoleWidth = Screen.width;
		float height = getHeight(left, right);
		float width = consoleWidth - left - right - 5.0f;
		float consoleX = 0.0f;
		float x = left + 5.0f;
		float y = Screen.height - consoleHeight;
		Rect logRect = new Rect(consoleX, y, consoleWidth, consoleHeight);
		y+=2.0f;
		GUIStyle boxStyle = new GUIStyle("Box");
		boxStyle.normal.background = boxStyle.active.background = boxStyle.hover.background = logTexture as Texture2D;
		GUI.Box(logRect,"", boxStyle);
		scrollPosition = GUI.BeginScrollView(new Rect(x, y, width, consoleHeight - 4.0f), scrollPosition, new Rect(x, y, width - 16.0f, height));
		y+=logY;
		x += logX;
		foreach (LogMessage s in messages) {
			st = getLogMessageStyle(s.color);
			float h = st.CalcHeight(s.message,width - 16.0f);
			GUI.Label(new Rect(x, y, width - 16.0f, h), s.message, st);
			y += h;
		}
		GUI.EndScrollView();
	}
}
