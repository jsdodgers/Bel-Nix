using UnityEngine;
using System.Collections;

public class MyGUI : MonoBehaviour {

	GridManager gridManager;
	
	Vector2 scrollPosition = new Vector2(0.0f,0.0f);
	// Use this for initialization
	void Start () {
		GameObject spritesObject = GameObject.Find("Background");
		gridManager = spritesObject.GetComponent<GridManager>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	
	void OnGUI() {
		GUISkin skinCopy = (GUISkin)Instantiate(GUI.skin);

		float boxWidthPerc = gridManager.boxWidthPerc;
		float boxWidth = Screen.width*boxWidthPerc;
		float boxX = Screen.width*(1-boxWidthPerc);
		float boxY = 0.0f;
		float boxHeight = Screen.height;
		GUI.Box(new Rect(boxX,boxY,boxWidth,boxHeight),"");
		float scrollContentSize = boxWidth - 16.0f;
		float loadButtonWidth = scrollContentSize * .9f;
		float loadButtonX = Screen.width - boxWidth + scrollContentSize*.05f;
		float loadButtonY = scrollContentSize*.05f;
		float loadButtonHeight = 30.0f;
		float width = GUI.skin.verticalScrollbar.fixedWidth;
		scrollPosition = GUI.BeginScrollView(new Rect(boxX,boxY,boxWidth,boxHeight), scrollPosition, new Rect(boxX,boxY,scrollContentSize,boxHeight*2.0f));
		if (GUI.Button(new Rect(loadButtonX,loadButtonY,loadButtonWidth,loadButtonHeight),"Load File...")) {
			Debug.Log("Button Press");
			/* 			string path = EditorUtility.OpenFilePanel(
				"Overwrite with png",
				"",
				"png");
			Debug.Log(path);
			*/
			gridManager.loadNewBackgroundFile();
		}
		float textFieldHeight = 20.0f;
		float textLabelWidth = 15.0f;
		float textLabelX = loadButtonX;
		float textFieldWidth = loadButtonWidth - textLabelWidth;
		float textFieldX = textLabelX + textLabelWidth;
		float redY = loadButtonY + loadButtonHeight + 5.0f;
		float greenY = redY + textFieldHeight + 5.0f;
		float blueY = greenY + textFieldHeight + 5.0f;
		float red = gridManager.red;
		float green = gridManager.green;
		float blue = gridManager.blue;
		GUI.Label(new Rect(textLabelX,redY,textLabelWidth,textFieldHeight),"R");
		GUI.Label(new Rect(textLabelX,greenY,textLabelWidth,textFieldHeight),"G");
		GUI.Label(new Rect(textLabelX,blueY,textLabelWidth,textFieldHeight),"B");
		bool redParsed = float.TryParse(GUI.TextField(new Rect(textFieldX,redY,textFieldWidth,textFieldHeight),(red==0.0f?"":((int)red).ToString())),out red);
		bool greenParsed = float.TryParse(GUI.TextField(new Rect(textFieldX,greenY,textFieldWidth,textFieldHeight),(green==0.0f?"":((int)green).ToString())),out green);
		bool blueParsed = float.TryParse(GUI.TextField(new Rect(textFieldX,blueY,textFieldWidth,textFieldHeight),(blue==0.0f?"":((int)blue).ToString())),out blue);
		if (!redParsed) red = 0.0f;
		if (!greenParsed) green = 0.0f;
		if (!blueParsed) blue = 0.0f;
		red = Mathf.Clamp(red,0.0f,255.0f);
		green = Mathf.Clamp(green,0.0f,255.0f);
		blue = Mathf.Clamp(blue,0.0f,255.0f);
		gridManager.red = red;
		gridManager.green = green;
		gridManager.blue = blue;
		float colorBoxHeight = textFieldHeight;
		Texture2D colorTexture = new Texture2D((int)loadButtonWidth,(int)colorBoxHeight);
		Texture2D oldTexture = GUI.skin.box.normal.background;
		Color fillColor = new Color(red/255.0f,green/255.0f,blue/255.0f,1.0f);
		Color[] colors = colorTexture.GetPixels();
		for (int n=0;n<colors.Length;n++) {
			colors[n] = fillColor;
		}
		colorTexture.SetPixels(colors);
		colorTexture.Apply();
		GUI.skin.box.normal.background = colorTexture;
		float colorBoxY = blueY + textFieldHeight + 5.0f;
		GUI.Box(new Rect(loadButtonX, colorBoxY, loadButtonWidth, colorBoxHeight),"");
		GUI.skin.box.normal.background = oldTexture;
		float checkHeight = colorBoxHeight;
		float checkWidth = loadButtonWidth;
		float checkX = loadButtonX;
		float standableY = colorBoxY + colorBoxHeight + 5.0f;
		float passableY = standableY + checkHeight + 5.0f;
		gridManager.standable = GUI.Toggle(new Rect(checkX,standableY,checkWidth,checkHeight),gridManager.standable,"Can Stand On");
		gridManager.passable = GUI.Toggle(new Rect(checkX,passableY,checkWidth,checkHeight),gridManager.passable,"Can Pass Through");

		GUI.EndScrollView();
		
		GUI.skin = skinCopy;
	}



}
