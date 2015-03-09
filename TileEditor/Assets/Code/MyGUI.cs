using UnityEngine;
using System.Collections;

public class MyGUI : MonoBehaviour {

	GridManager gridManager;

	public int mapMode = 0;			// 0=Tile	1=Visibility
	public int visibilityMode = 0; // 0=Place	1=Edit
	
	Vector2 scrollPosition = new Vector2(0.0f,0.0f);
	// Use this for initialization
	void Start () {
		GameObject spritesObject = GameObject.Find("Background");
		gridManager = spritesObject.GetComponent<GridManager>();
		colorChanged = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public bool editWhenPlaced = false;

	bool actuallyZeroWallV = true;
	bool actuallyZeroStartX = true;
	bool actuallyZeroStartY = true;
	bool actuallyZeroEndX = true;
	bool actuallyZeroEndY = true;

	bool actuallyZeroRed = true;
	bool actuallyZeroGreen = true;
	bool actuallyZeroBlue = true;
	bool actuallyZeroUp = true;
	bool actuallyZeroDown = true;
	bool actuallyZeroLeft = true;
	bool actuallyZeroRight = true;
	bool actuallyZeroUpV = true;
	bool actuallyZeroDownV = true;
	bool actuallyZeroLeftV = true;
	bool actuallyZeroRightV = true;
	bool actuallyZeroTrigger = true;
	bool actuallyZeroAction = true;
	public bool colorChanged = true;
	public float checkExtraX = 20.0f;

	GUIStyle boxStyle = null;
	
	void createStyle() {
		if (boxStyle == null) {
			boxStyle = new GUIStyle(GUI.skin.box);
		}
	}

	void OnGUI() {
		GUISkin skinCopy = (GUISkin)Instantiate(GUI.skin);

		float boxWidthPerc = gridManager.boxWidthPerc;
		float boxWidth = Screen.width*boxWidthPerc;
		float boxX = Screen.width*(1-boxWidthPerc);
		float boxY = 0.0f;
		//checkExtraX = 20.0f;
		float checkLeftX = boxX - checkExtraX + 5.0f;
		float boxHeight = Screen.height;
		GUI.Box(new Rect(boxX - checkExtraX,boxY,boxWidth + checkExtraX,boxHeight),"");
		float scrollContentSize = boxWidth - 16.0f;
		float loadButtonWidth = scrollContentSize * .9f;
		float loadButtonX = Screen.width - boxWidth + scrollContentSize*.05f;
		float loadButtonY = scrollContentSize*.05f;
		float visibilityModeHeight = 20.0f;
		float loadButtonHeight = 30.0f;
		float width = GUI.skin.verticalScrollbar.fixedWidth;
		float textFieldHeight = 20.0f;
		float longTextLabelW = 15.0f;
		float textLabelX = loadButtonX;
		float textFieldWidth = loadButtonWidth - longTextLabelW;
		float textFieldX = textLabelX + longTextLabelW;

		
		float importButtonY = loadButtonY + loadButtonHeight + 5.0f;
		float redY = importButtonY + loadButtonHeight + 5.0f;//editTypeY + loadButtonHeight + 5.0f;
		float greenY = redY + textFieldHeight + 5.0f;
		float blueY = greenY + textFieldHeight + 5.0f;
		float red = gridManager.red;
		float green = gridManager.green;
		float blue = gridManager.blue;
		float colorBoxHeight = textFieldHeight;
		float colorBoxY = blueY + textFieldHeight + 5.0f;
		float editTypeY = colorBoxY + colorBoxHeight + 5.0f;//importButtonY + loadButtonHeight + 5.0f;

		float visibilityModeY = editTypeY + loadButtonHeight + 5.0f;
		float wallBothWaysY = visibilityModeY + visibilityModeHeight + 5.0f;
		float wallVisibilityY = wallBothWaysY + textFieldHeight + 5.0f;
		float wallRangedY = wallVisibilityY + textFieldHeight + 5.0f;
		float wallMeleeY = wallRangedY + textFieldHeight + 5.0f;
		float editPlacedY = wallMeleeY + textFieldHeight + 5.0f;
		float startTitleY = wallMeleeY + textFieldHeight + 5.0f;
		float startXY = startTitleY + textFieldHeight + 5.0f;
		float startYY = startXY + textFieldHeight + 5.0f;
		float endTitleY = startYY + textFieldHeight + 5.0f;
		float endXY = endTitleY + textFieldHeight + 5.0f;
		float endYY = endXY + textFieldHeight + 5.0f;
		float flipY = endYY + textFieldHeight + 10.0f;
		float deleteY = flipY + loadButtonHeight + 5.0f;

		float checkHeight = colorBoxHeight;
		float checkWidth = loadButtonWidth;
		float checkX = loadButtonX;
		float standableY = editTypeY + loadButtonHeight + 5.0f;//colorBoxY + colorBoxHeight + 5.0f;
		float startingPointY = standableY + checkHeight + 5.0f;
		float turnY = startingPointY + checkHeight + 5.0f;
		float passableY = turnY + checkHeight + 5.0f;

		float longTextLabelWidth = 50.0f;
		float longTextFieldWidth = loadButtonWidth - longTextLabelWidth;
		float longTextFieldX = textLabelX + longTextLabelWidth;
		float longerTextLabelWidth = 65.0f;
		float longerTextFieldWidth = loadButtonWidth - longerTextLabelWidth;
		float longerTextFieldX = textLabelX + longerTextLabelWidth;
		float passableUpY = passableY + textFieldHeight + 5.0f;
		float passableRightY = passableUpY + textFieldHeight + 5.0f;
		float passableDownY = passableRightY + textFieldHeight + 5.0f;
		float passableLeftY = passableDownY + textFieldHeight + 5.0f;
		float visibleY = passableLeftY + textFieldHeight + 5.0f;
		float visibilityUpY = visibleY + textFieldHeight + 5.0f;
		float visibilityRightY = visibilityUpY + textFieldHeight + 5.0f;
		float visibilityDownY = visibilityRightY + textFieldHeight + 5.0f;
		float visibilityLeftY = visibilityDownY + textFieldHeight + 5.0f;
		float triggerY = visibilityLeftY + textFieldHeight + 15.0f;
		float actionY = triggerY + textFieldHeight + 5.0f;


		float allCheckY = actionY + checkHeight + 10.0f;
		float printY = (mapMode == 0 ? allCheckY + checkHeight + 10.0f : (visibilityMode==0 ? editPlacedY + textFieldHeight + 5.0f : deleteY + loadButtonHeight + 5.0f));

		float endY = printY + loadButtonHeight + 5.0f;

		scrollPosition = GUI.BeginScrollView(new Rect(boxX-checkExtraX,boxY,boxWidth + checkExtraX,boxHeight), scrollPosition, new Rect(boxX-checkExtraX,boxY,scrollContentSize + checkExtraX,endY + 0.0f*boxHeight*2.0f/2.0f));
		if (GUI.Button(new Rect(loadButtonX,loadButtonY,loadButtonWidth,loadButtonHeight),"Load File...")) {
			//Debug.Log("Button Press");
			/* 			string path = EditorUtility.OpenFilePanel(
				"Overwrite with png",
				"",
				"png");
			Debug.Log(path);
			*/
			gridManager.loadNewBackgroundFile();
		}
		
	//	GUI.enabled = gridManager.imageFileName != null && !gridManager.imageFileName.Equals("");
		if (GUI.Button(new Rect(loadButtonX,importButtonY,loadButtonWidth,loadButtonHeight),"Import Tile Map")) {
			StartCoroutine(gridManager.importGrid());
		}
		GUI.Label(new Rect(textLabelX,redY,longTextLabelW,textFieldHeight),"R");
		GUI.Label(new Rect(textLabelX,greenY,longTextLabelW,textFieldHeight),"G");
		GUI.Label(new Rect(textLabelX,blueY,longTextLabelW,textFieldHeight),"B");
		GUI.SetNextControlName("red");string redS = GUI.TextField(new Rect(textFieldX,redY,textFieldWidth,textFieldHeight),(red==0.0f?(actuallyZeroRed||!GUI.GetNameOfFocusedControl().Equals("red")?"0":""):((int)red).ToString()));
		GUI.SetNextControlName("green");string greenS = GUI.TextField(new Rect(textFieldX,greenY,textFieldWidth,textFieldHeight),(green==0.0f?(actuallyZeroGreen||!GUI.GetNameOfFocusedControl().Equals("green")?"0":""):((int)green).ToString()));
		GUI.SetNextControlName("blue");string blueS = GUI.TextField(new Rect(textFieldX,blueY,textFieldWidth,textFieldHeight),(blue==0.0f?(actuallyZeroBlue||!GUI.GetNameOfFocusedControl().Equals("blue")?"0":""):((int)blue).ToString()));
		bool redParsed = float.TryParse(redS,out red);
		bool greenParsed = float.TryParse(greenS,out green);
		bool blueParsed = float.TryParse(blueS,out blue);
		//		if (!redParsed) red = 0.0f;
		//		if (!greenParsed) green = 0.0f;
		//		if (!blueParsed) blue = 0.0f;
		actuallyZeroRed = (red==0.0f && (redS.Length>0 && (redS.ToCharArray()[0]=='0' || redS.ToCharArray()[redS.ToCharArray().Length-1]=='0')));
		actuallyZeroGreen = (green==0.0f && (greenS.Length>0 && (greenS.ToCharArray()[0]=='0' || greenS.ToCharArray()[greenS.ToCharArray().Length-1]=='0')));
		actuallyZeroBlue = (blue==0.0f && (blueS.Length>0 && (blueS.ToCharArray()[0]=='0' || blueS.ToCharArray()[blueS.ToCharArray().Length-1]=='0')));
		red = Mathf.Clamp(red,0.0f,255.0f);
		green = Mathf.Clamp(green,0.0f,255.0f);
		blue = Mathf.Clamp(blue,0.0f,255.0f);
		if (red != gridManager.red || blue!=gridManager.blue || green!=gridManager.green) colorChanged = true;
		gridManager.red = red;
		gridManager.green = green;
		gridManager.blue = blue;
		createStyle();
		if (colorChanged) {
			boxStyle.normal.background = makeTex((int)loadButtonWidth, (int)colorBoxHeight, new Color(red/255.0f, green/255.0f, blue/255.0f, 1.0f));
			if (mapMode == 1 && visibilityMode == 1 && gridManager.currentWall != null) {
				gridManager.currentWall.setColor(new Color(red/255.0f, green/255.0f, blue/255.0f));
			}
		}
		GUI.Box(new Rect(loadButtonX, colorBoxY, loadButtonWidth, colorBoxHeight),"", boxStyle);

		
		int oldMapMode = mapMode;
		mapMode = GUI.SelectionGrid(new Rect(loadButtonX, editTypeY, loadButtonWidth, loadButtonHeight), mapMode, new string[]{"Tiles","Visibility"}, 2);
		if (mapMode != oldMapMode) {
			gridManager.shiftDraggin = false;
			gridManager.wasShiftDraggin = false;
			if (mapMode == 0) {
				gridManager.showGrids();
			} 
			else {
				gridManager.showWalls();
			}
		}
		//	GUI.enabled = true;
		if (mapMode == 1) {
			visibilityMode = GUI.SelectionGrid(new Rect(loadButtonX, visibilityModeY, loadButtonWidth, visibilityModeHeight), visibilityMode, new string[]{"Place", "Edit"}, 2);
		
			GUI.enabled = visibilityMode == 0 || gridManager.currentWall != null;
			bool bothWays = (visibilityMode == 0 || gridManager.currentWall == null ? gridManager.wallBothWays : gridManager.currentWall.bothWays);
			bool newBothWays = GUI.Toggle(new Rect(checkX,wallBothWaysY,checkWidth,checkHeight),bothWays,"Blocked Both Ways");
			if (visibilityMode == 0) {
				gridManager.wallBothWays = newBothWays;
			}
			GUI.Label(new Rect(textLabelX,wallVisibilityY,longerTextLabelWidth,textFieldHeight),"Visibility:");
			int wallVisibility = (visibilityMode == 0 || gridManager.currentWall == null ? gridManager.wallVisibility : gridManager.currentWall.visibility);
			GUI.SetNextControlName("wallvisibility");string wallVS = GUI.TextField(new Rect(longerTextFieldX,wallVisibilityY,longerTextFieldWidth,textFieldHeight),(wallVisibility==0?(actuallyZeroWallV||!GUI.GetNameOfFocusedControl().Equals("wallvisibility")?"0":""):((int)wallVisibility).ToString()));
			int newWallVisibility = wallVisibility;
			bool wallVParsed = int.TryParse(wallVS,out newWallVisibility);
			//		if (!upParsed)
			actuallyZeroWallV = (newWallVisibility==0 && (wallVS.Length>0 && (wallVS.ToCharArray()[0]=='0' || wallVS.ToCharArray()[wallVS.ToCharArray().Length-1]=='0')));
			if (visibilityMode == 0) {
				gridManager.wallVisibility = newWallVisibility;
			}
			bool ranged = (visibilityMode == 0 || gridManager.currentWall == null ? gridManager.wallRange : gridManager.currentWall.canRange);
			bool newRanged = GUI.Toggle(new Rect(checkX,wallRangedY,checkWidth,checkHeight),ranged,"Can Range Through");
			if (visibilityMode == 0) {
				gridManager.wallRange = newRanged;
			}
			bool melee = (visibilityMode == 0 || gridManager.currentWall == null ? gridManager.wallMelee : gridManager.currentWall.canMelee);
			bool newMelee = GUI.Toggle(new Rect(checkX,wallMeleeY,checkWidth,checkHeight),melee,"Can Melee Through");
			if (visibilityMode == 0) {
				gridManager.wallMelee = newMelee;
			}
			if (visibilityMode == 0) {
				editWhenPlaced = GUI.Toggle(new Rect(checkX, editPlacedY, checkWidth, checkHeight), editWhenPlaced, "Edit When Placed");
			}
			else {
				GUI.Label(new Rect(textLabelX,startTitleY,loadButtonWidth,textFieldHeight),"Start:");
				GUI.Label(new Rect(textLabelX,endTitleY,loadButtonWidth,textFieldHeight),"End:");
				GUI.Label(new Rect(textLabelX,startXY,longTextLabelWidth,textFieldHeight),"X:");
				GUI.Label(new Rect(textLabelX,startYY,longTextLabelWidth,textFieldHeight),"Y:");
				GUI.Label(new Rect(textLabelX,endXY,longTextLabelWidth,textFieldHeight),"X:");
				GUI.Label(new Rect(textLabelX,endYY,longTextLabelWidth,textFieldHeight),"Y:");
				/*
				v3.x += gridX/2.0f;
				v3.y += gridY/2.0f;
				v3.y = gridY - v3.y;
*/
				int currentStartX = (int)(gridManager.currentWall == null ? 0 : (gridManager.currentWall.startPos.x + gridManager.gridX/2.0f) * gridManager.tileSize);
				int currentStartY = (int)(gridManager.currentWall == null ? 0 : (gridManager.gridY - (gridManager.currentWall.startPos.y + gridManager.gridY/2.0f)) * gridManager.tileSize);
				int currentEndX = (int)(gridManager.currentWall == null ? 0 : (gridManager.currentWall.endPos.x + gridManager.gridX/2.0f) * gridManager.tileSize);
				int currentEndY =(int)( gridManager.currentWall == null ? 0 : (gridManager.gridY - (gridManager.currentWall.endPos.y + gridManager.gridY/2.0f)) * gridManager.tileSize);
				GUI.SetNextControlName("startx");string startXS = GUI.TextField(new Rect(textFieldX,startXY,textFieldWidth,textFieldHeight),(currentStartX==0?(actuallyZeroStartX||!GUI.GetNameOfFocusedControl().Equals("startx")?"0":""):((int)currentStartX).ToString()));
				GUI.SetNextControlName("starty");string startYS = GUI.TextField(new Rect(textFieldX,startYY,textFieldWidth,textFieldHeight),(currentStartY==0?(actuallyZeroStartY||!GUI.GetNameOfFocusedControl().Equals("starty")?"0":""):((int)currentStartY).ToString()));
				GUI.SetNextControlName("endx");string endXS = GUI.TextField(new Rect(textFieldX,endXY,textFieldWidth,textFieldHeight),(currentEndX==0?(actuallyZeroEndX||!GUI.GetNameOfFocusedControl().Equals("endx")?"0":""):((int)currentEndX).ToString()));
				GUI.SetNextControlName("endy");string endYS = GUI.TextField(new Rect(textFieldX,endYY,textFieldWidth,textFieldHeight),(currentEndY==0?(actuallyZeroEndY||!GUI.GetNameOfFocusedControl().Equals("endy")?"0":""):((int)currentEndY).ToString()));
				int cx,cy,cx2,cy2;
				bool sxParsed = int.TryParse(startXS,out cx);
				bool syParsed = int.TryParse(startYS,out cy);
				bool exParsed = int.TryParse(endXS,out cx2);
				bool eyParsed = int.TryParse(endYS,out cy2);
				actuallyZeroStartX = (cx==0.0f && (startXS.Length>0 && (startXS.ToCharArray()[0]=='0' || startXS.ToCharArray()[startXS.ToCharArray().Length-1]=='0')));
				actuallyZeroStartY = (cy==0.0f && (startYS.Length>0 && (startYS.ToCharArray()[0]=='0' || startYS.ToCharArray()[startYS.ToCharArray().Length-1]=='0')));
				actuallyZeroEndX = (cx2==0.0f && (endXS.Length>0 && (endXS.ToCharArray()[0]=='0' || endXS.ToCharArray()[endXS.ToCharArray().Length-1]=='0')));
				actuallyZeroEndY = (cy2==0.0f && (endYS.Length>0 && (endYS.ToCharArray()[0]=='0' || endYS.ToCharArray()[endYS.ToCharArray().Length-1]=='0')));
				if (gridManager.currentWall != null) {
					if (cx != currentStartX || cy != currentStartY) {
						gridManager.currentWall.setStart(((float)cx)/gridManager.tileSize - gridManager.gridX/2.0f, gridManager.gridY / 2.0f - ((float)cy)/gridManager.tileSize);
					}
					if (cx2 != currentEndX || cy2 != currentEndY) {
						gridManager.currentWall.setEnd(((float)cx2)/gridManager.tileSize - gridManager.gridX/2.0f, gridManager.gridY / 2.0f - ((float)cy2)/gridManager.tileSize);
					}
					if (bothWays != newBothWays) {
						gridManager.currentWall.setBothWays(newBothWays);
					}
					if (wallVisibility != newWallVisibility) {
						gridManager.currentWall.visibility = newWallVisibility;
					}
					if (ranged != newRanged) {
						gridManager.currentWall.canRange = newRanged;
					}
					if (melee != newMelee) {
						gridManager.currentWall.canMelee = newMelee;
					}
				}
				if (GUI.Button(new Rect(loadButtonX, flipY, loadButtonWidth, loadButtonHeight), "Flip")) {
					Vector3 start = gridManager.currentWall.startPos;
					Vector3 end = gridManager.currentWall.endPos;
					gridManager.currentWall.setStart(end.x, end.y);
					gridManager.currentWall.setEnd(start.x, start.y);
				}
				if (GUI.Button(new Rect(loadButtonX, deleteY, loadButtonWidth, loadButtonHeight), "Delete")) {
					GameObject.Destroy(gridManager.currentWall.gameObject);
					gridManager.currentWall = null;
				}
			}
			GUI.enabled = true;
		
		
		}
		else if (mapMode == 0) {
			bool allColors = gridManager.doingAllColors();
			bool allEverything = gridManager.doingAll();
			gridManager.doRed = GUI.Toggle(new Rect(checkLeftX, redY, checkExtraX, textFieldHeight), gridManager.doRed, "");
			gridManager.doGreen = GUI.Toggle(new Rect(checkLeftX, greenY, checkExtraX, textFieldHeight), gridManager.doGreen, "");
			gridManager.doBlue = GUI.Toggle(new Rect(checkLeftX, blueY, checkExtraX, textFieldHeight), gridManager.doBlue, "");

		//	Texture2D colorTexture = new Texture2D((int)loadButtonWidth,(int)colorBoxHeight);
		//	Texture2D oldTexture = GUI.skin.box.normal.background;
		//	Color fillColor = new Color(red/255.0f,green/255.0f,blue/255.0f,1.0f);
		//	Color[] colors = colorTexture.GetPixels();
		//	for (int n=0;n<colors.Length;n++) {
		//		colors[n] = fillColor;
		//	}
		//	colorTexture.SetPixels(colors);
		//	colorTexture.Apply();
		//	GUI.skin.box.normal.background = colorTexture;
			bool allChecked = GUI.Toggle(new Rect(checkLeftX, colorBoxY, checkExtraX, textFieldHeight), allColors, "");
			if (allChecked != allColors) {
				gridManager.doRed = allChecked;
				gridManager.doGreen = allChecked;
				gridManager.doBlue = allChecked;
			}
		//	GUI.skin.box.normal.background = oldTexture;
			gridManager.standable = GUI.Toggle(new Rect(checkX,standableY,checkWidth,checkHeight),gridManager.standable,"Can Stand On");
			gridManager.doStand = GUI.Toggle(new Rect(checkLeftX, standableY, checkExtraX, textFieldHeight), gridManager.doStand, "");
			gridManager.startingPoint = GUI.Toggle(new Rect(checkX, startingPointY,checkWidth,checkHeight),gridManager.startingPoint,"Starting Point");
			gridManager.doStartingPoint = GUI.Toggle(new Rect(checkLeftX, startingPointY, checkExtraX, textFieldHeight), gridManager.doStartingPoint, "");
			gridManager.canTurn = GUI.Toggle(new Rect(checkX, turnY,checkWidth,checkHeight),gridManager.canTurn,"Can Turn");
			gridManager.doTurn = GUI.Toggle(new Rect(checkLeftX, turnY, checkExtraX, textFieldHeight), gridManager.doTurn, "");
			//	gridManager.passable = GUI.Toggle(new Rect(checkX,passableY,checkWidth,checkHeight),gridManager.passable,"Can Pass Through");
			GUI.Label(new Rect(textLabelX,passableY,loadButtonWidth,textFieldHeight),"Passability:");
			GUI.Label(new Rect(textLabelX,visibleY,loadButtonWidth,textFieldHeight),"Visibility:");
			GUI.Label(new Rect(textLabelX,passableUpY,longTextLabelWidth,textFieldHeight),"Up");
			GUI.Label(new Rect(textLabelX,passableRightY,longTextLabelWidth,textFieldHeight),"Right");
			GUI.Label(new Rect(textLabelX,passableDownY,longTextLabelWidth,textFieldHeight),"Down");
			GUI.Label(new Rect(textLabelX,passableLeftY,longTextLabelWidth,textFieldHeight),"Left");
			GUI.Label(new Rect(textLabelX,visibilityUpY,longTextLabelWidth,textFieldHeight),"Up");
			GUI.Label(new Rect(textLabelX,visibilityRightY,longTextLabelWidth,textFieldHeight),"Right");
			GUI.Label(new Rect(textLabelX,visibilityDownY,longTextLabelWidth,textFieldHeight),"Down");
			GUI.Label(new Rect(textLabelX,visibilityLeftY,longTextLabelWidth,textFieldHeight),"Left");
			GUI.Label(new Rect(textLabelX,triggerY,longTextLabelWidth,textFieldHeight),"Trigger:");
			GUI.Label(new Rect(textLabelX,actionY,longTextLabelWidth,textFieldHeight),"Action:");
			GUI.SetNextControlName("up");string upS = GUI.TextField(new Rect(longTextFieldX,passableUpY,longTextFieldWidth,textFieldHeight),(gridManager.passableUp==0?(actuallyZeroUp||!GUI.GetNameOfFocusedControl().Equals("up")?"0":""):((int)gridManager.passableUp).ToString()));
			GUI.SetNextControlName("right");string rightS = GUI.TextField(new Rect(longTextFieldX,passableRightY,longTextFieldWidth,textFieldHeight),(gridManager.passableRight==0?(actuallyZeroRight||!GUI.GetNameOfFocusedControl().Equals("right")?"0":""):((int)gridManager.passableRight).ToString()));
			GUI.SetNextControlName("down");string downS = GUI.TextField(new Rect(longTextFieldX,passableDownY,longTextFieldWidth,textFieldHeight),(gridManager.passableDown==0?(actuallyZeroDown||!GUI.GetNameOfFocusedControl().Equals("down")?"0":""):((int)gridManager.passableDown).ToString()));
			GUI.SetNextControlName("left");string leftS = GUI.TextField(new Rect(longTextFieldX,passableLeftY,longTextFieldWidth,textFieldHeight),(gridManager.passableLeft==0?(actuallyZeroLeft||!GUI.GetNameOfFocusedControl().Equals("left")?"0":""):((int)gridManager.passableLeft).ToString()));
			GUI.SetNextControlName("upV");string upVS = GUI.TextField(new Rect(longTextFieldX,visibilityUpY,longTextFieldWidth,textFieldHeight),(gridManager.visibleUp==0?(actuallyZeroUpV||!GUI.GetNameOfFocusedControl().Equals("upV")?"0":""):((int)gridManager.visibleUp).ToString()));
			GUI.SetNextControlName("rightV");string rightVS = GUI.TextField(new Rect(longTextFieldX,visibilityRightY,longTextFieldWidth,textFieldHeight),(gridManager.visibleRight==0?(actuallyZeroRightV||!GUI.GetNameOfFocusedControl().Equals("rightV")?"0":""):((int)gridManager.visibleRight).ToString()));
			GUI.SetNextControlName("downV");string downVS = GUI.TextField(new Rect(longTextFieldX,visibilityDownY,longTextFieldWidth,textFieldHeight),(gridManager.visibleDown==0?(actuallyZeroDownV||!GUI.GetNameOfFocusedControl().Equals("downV")?"0":""):((int)gridManager.visibleDown).ToString()));
			GUI.SetNextControlName("leftV");string leftVS = GUI.TextField(new Rect(longTextFieldX,visibilityLeftY,longTextFieldWidth,textFieldHeight),(gridManager.visibleLeft==0?(actuallyZeroLeftV||!GUI.GetNameOfFocusedControl().Equals("leftV")?"0":""):((int)gridManager.visibleLeft).ToString()));
			GUI.SetNextControlName("trigger");string triggerS = GUI.TextField(new Rect(longTextFieldX,triggerY,longTextFieldWidth,textFieldHeight),(gridManager.trigger==0?(actuallyZeroTrigger||!GUI.GetNameOfFocusedControl().Equals("trigger")?"0":""):((int)gridManager.trigger).ToString()));
			GUI.SetNextControlName("action");string actionS = GUI.TextField(new Rect(longTextFieldX,actionY,longTextFieldWidth,textFieldHeight),(gridManager.action==0?(actuallyZeroAction||!GUI.GetNameOfFocusedControl().Equals("action")?"0":""):((int)gridManager.action).ToString()));
			gridManager.doUp = GUI.Toggle(new Rect(checkLeftX, passableUpY, checkExtraX, textFieldHeight), gridManager.doUp, "");
			gridManager.doRight = GUI.Toggle(new Rect(checkLeftX, passableRightY, checkExtraX, textFieldHeight), gridManager.doRight, "");
			gridManager.doDown = GUI.Toggle(new Rect(checkLeftX, passableDownY, checkExtraX, textFieldHeight), gridManager.doDown, "");
			gridManager.doLeft = GUI.Toggle(new Rect(checkLeftX, passableLeftY, checkExtraX, textFieldHeight), gridManager.doLeft, "");
			gridManager.doUpV = GUI.Toggle(new Rect(checkLeftX, visibilityUpY, checkExtraX, textFieldHeight), gridManager.doUpV, "");
			gridManager.doRightV = GUI.Toggle(new Rect(checkLeftX, visibilityRightY, checkExtraX, textFieldHeight), gridManager.doRightV, "");
			gridManager.doDownV = GUI.Toggle(new Rect(checkLeftX, visibilityDownY, checkExtraX, textFieldHeight), gridManager.doDownV, "");
			gridManager.doLeftV = GUI.Toggle(new Rect(checkLeftX, visibilityLeftY, checkExtraX, textFieldHeight), gridManager.doLeftV, "");
			gridManager.doTrigger = GUI.Toggle(new Rect(checkLeftX, triggerY, checkExtraX, textFieldHeight), gridManager.doTrigger, "");
			gridManager.doAction = GUI.Toggle(new Rect(checkLeftX, actionY, checkExtraX, textFieldHeight), gridManager.doAction, "");
			bool upParsed = int.TryParse(upS,out gridManager.passableUp);
			bool rightParsed = int.TryParse(rightS,out gridManager.passableRight);
			bool downParsed = int.TryParse(downS,out gridManager.passableDown);
			bool leftParsed = int.TryParse(leftS,out gridManager.passableLeft);
			bool upParsedV = int.TryParse(upVS,out gridManager.visibleUp);
			bool rightParsedV = int.TryParse(rightVS,out gridManager.visibleRight);
			bool downParsedV = int.TryParse(downVS,out gridManager.visibleDown);
			bool leftParsedV = int.TryParse(leftVS,out gridManager.visibleLeft);

			bool triggerParsed = int.TryParse(triggerS,out gridManager.trigger);
			bool actionParsed = int.TryParse(actionS,out gridManager.action);
			//		if (!upParsed)
			actuallyZeroUp = (gridManager.passableUp==0.0f && (upS.Length>0 && (upS.ToCharArray()[0]=='0' || upS.ToCharArray()[upS.ToCharArray().Length-1]=='0')));
			actuallyZeroRight = (gridManager.passableRight==0.0f && (rightS.Length>0 && (rightS.ToCharArray()[0]=='0' || rightS.ToCharArray()[rightS.ToCharArray().Length-1]=='0')));
			actuallyZeroDown = (gridManager.passableDown==0.0f && (downS.Length>0 && (downS.ToCharArray()[0]=='0' || downS.ToCharArray()[downS.ToCharArray().Length-1]=='0')));
			actuallyZeroLeft = (gridManager.passableLeft==0.0f && (leftS.Length>0 && (leftS.ToCharArray()[0]=='0' || leftS.ToCharArray()[leftS.ToCharArray().Length-1]=='0')));
			actuallyZeroUpV = (gridManager.visibleUp==0.0f && (upVS.Length>0 && (upVS.ToCharArray()[0]=='0' || upVS.ToCharArray()[upVS.ToCharArray().Length-1]=='0')));
			actuallyZeroRightV = (gridManager.visibleRight==0.0f && (rightVS.Length>0 && (rightVS.ToCharArray()[0]=='0' || rightVS.ToCharArray()[rightVS.ToCharArray().Length-1]=='0')));
			actuallyZeroDownV = (gridManager.visibleDown==0.0f && (downVS.Length>0 && (downVS.ToCharArray()[0]=='0' || downVS.ToCharArray()[downVS.ToCharArray().Length-1]=='0')));
			actuallyZeroLeftV = (gridManager.visibleLeft==0.0f && (leftVS.Length>0 && (leftVS.ToCharArray()[0]=='0' || leftVS.ToCharArray()[leftVS.ToCharArray().Length-1]=='0')));
			actuallyZeroTrigger = (gridManager.trigger==0.0f && (triggerS.Length>0 && (triggerS.ToCharArray()[0]=='0' || triggerS.ToCharArray()[triggerS.ToCharArray().Length-1]=='0')));
			actuallyZeroAction = (gridManager.action==0.0f && (actionS.Length>0 && (actionS.ToCharArray()[0]=='0' || actionS.ToCharArray()[actionS.ToCharArray().Length-1]=='0')));

			allChecked = GUI.Toggle(new Rect(checkLeftX, allCheckY, checkWidth, textFieldHeight), allEverything, "Select All");
			if (allChecked != allEverything) {
				gridManager.doRed = allChecked;
				gridManager.doGreen = allChecked;
				gridManager.doBlue = allChecked;
				gridManager.doStand = allChecked;
				gridManager.doLeft = allChecked;
				gridManager.doRight = allChecked;
				gridManager.doUp = allChecked;
				gridManager.doDownV = allChecked;
				gridManager.doLeftV = allChecked;
				gridManager.doRightV = allChecked;
				gridManager.doUpV = allChecked;
				gridManager.doStartingPoint = allChecked;
				gridManager.doDown = allChecked;
				gridManager.doTrigger = allChecked;
				gridManager.doAction = allChecked;
				gridManager.doTurn = allChecked;
			}

		}
		GUI.enabled = gridManager.imageFileName != null && !gridManager.imageFileName.Equals("");
		if (GUI.Button(new Rect(loadButtonX,printY,loadButtonWidth,loadButtonHeight),"Save Tile Map")) {
			gridManager.printGrid();
		}
		GUI.enabled = true;
		GUI.EndScrollView();

		if (gridManager.displayH || gridManager.displayHTime>0) {
			float lW = 200.0f;
			float lH = 100.0f;
			float lX = (Screen.width - lW)/2.0f;
			float lY = (Screen.width - lH)/2.0f;
			GUI.Label(new Rect(lX,lY,lW,lH),"Shift+Control+Alt+" + (gridManager.displayI?"I":"H") + " is pressed!");
		}

		GUI.skin = skinCopy;
	}


	
	Texture2D makeTex( int width, int height, Color col )
	{
		Color[] pix = new Color[width * height];
		for( int i = 0; i < pix.Length; ++i )
		{
			pix[ i ] = col;
		}
		Texture2D result = new Texture2D( width, height );
		result.SetPixels( pix );
		result.Apply();
		colorChanged = false;
		Resources.UnloadUnusedAssets();
		return result;
	}



}
