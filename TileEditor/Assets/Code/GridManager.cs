using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class GridManager : MonoBehaviour {

	public Wall currentWall;
	public int selectedEdges = 0; // 0 = Both,	1 = Start,	2 = End,	-1 = None
	public int tileSize = 64;

	public Vector3 oldStartPos;
	public Vector3 oldEndPos;

	Transform cameraTransform;
	Camera mainCamera;
	SpriteRenderer sprend;
	Sprite spr;
	GameObject grids;
//	ArrayList gridsArray = new ArrayList();
	Tile[,] gridsArray;
	GameObject lines;
	GameObject walls;
	ArrayList linesArray;
	GameObject gridPrefab;
	GameObject mouseOver;
	GameObject mouseOver2;
	MyGUI gui;

	float cameraOriginalSize;
	public float boxWidthPerc = .2f;
	
	bool mouseLeftDown;
	bool mouseRightDown;
	bool mouseMiddleDown;

	bool shiftDown;
	bool altDown;
	bool controlDown;
	bool hDown;
	bool iDown;
	bool lDown;
	bool sDown;
	bool spaceDown;
	bool escapeDown;

	public bool shiftDraggin = false;
	bool middleDraggin = false;
	bool normalDraggin = false;
	bool rightDraggin = false;
	public bool wasShiftDraggin = false;
	public bool shiftDragginCancelled = false;
	Vector2 startSquare;
	Vector2 lastPos;
	Vector3 startSquareActual;

	public int gridX = 0;
	public int gridY = 0;

	public bool wallBothWays = false;
	public int wallVisibility = 0;
	public bool wallRange = false;
	public bool wallMelee = false;

	public bool displayH;
	public bool displayI;
	public bool displayS;
	public bool displayL;
	public int displayHTime = 0;

	public float red = 0.0f;
	public float green = 0.0f;
	public float blue = 0.0f;

//	public bool passable = true;
	public bool standable = true;
	
	
	public int passableRight;
	public int passableLeft;
	public int passableDown;
	public int passableUp;

	public int visibleUp;
	public int visibleDown;
	public int visibleLeft;
	public int visibleRight;

	public bool startingPoint;
	public bool canTurn;

	public int trigger;
	public int action; 


	public bool doRed = true;
	public bool doGreen = true;
	public bool doBlue = true;
	public bool doStand = true;
	public bool doUp = true;
	public bool doRight = true;
	public bool doDown = true;
	public bool doLeft = true;
	public bool doUpV = true;
	public bool doRightV = true;
	public bool doDownV = true;
	public bool doLeftV = true;
	public bool doStartingPoint = true;
	public bool doTrigger = true;
	public bool doAction = true;
	public bool doTurn = true;

	public void showWalls() {
		walls.SetActive(true);
		grids.SetActive(false);
	}

	public void showGrids() {
		grids.SetActive(true);
		walls.SetActive(false);
	}

	public bool doingAll() {
		return doingAllColors() && doStand && doUp && doRight && doDown && doLeft && doTrigger && doAction && doUpV && doRightV && doLeftV && doDownV && doStartingPoint && doTurn;
	}

	public bool doingAllColors() {
		return doRed && doGreen && doBlue;
	}

	public string imageFileName = "";
	
	// Use this for initialization
	void Start () {
		GameObject gu = GameObject.Find("GUI");
		gui = gu.GetComponent<MyGUI>();
		GameObject mainCameraObj = GameObject.Find("Main Camera");
		cameraTransform = mainCameraObj.transform;
		mainCamera = mainCameraObj.GetComponent<Camera>();
		cameraOriginalSize = mainCamera.orthographicSize;
		sprend = (SpriteRenderer)transform.GetComponent("SpriteRenderer");
		spr = sprend.sprite;
		grids = GameObject.Find("Grids");
		walls = GameObject.Find("Walls");
		gridsArray = new Tile[gridX,gridY];
		gridPrefab = (GameObject)Resources.Load("Sprite/Square_" + tileSize);
		lines = GameObject.Find("Lines");
		doRed = true;
		doGreen = true;
		doBlue = true;
		doStand = true;
		doUp = true;
		doDown = true;
		doLeft = true;
		doRight = true;
		doTrigger = true;
		doAction = true;
		doStartingPoint = true;
		doUpV = true;
		doDownV = true;
		doLeftV = true;
		doRightV = true;
		showGrids();
		//	linesArray = new ArrayList();
//		createLineRenderer(0, 20, 0, 20);
//		createLineRenderer(20, 10, 20, 40);
	}
	
	// Update is called once per frame
	void Update () {
		handleMouseInput();

	}


	void handleMouseInput() {
		handleMouseScrollWheel();
		handleKeys();
		handleMouseClicks();
		handleMouseMovement();
		handleMouseSelect();
		handleKeyActions();
	}

	void handleMouseScrollWheel() {
		if (Input.mousePosition.x < Screen.width*(1-boxWidthPerc) - gui.checkExtraX) {
			float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
			float cameraSize = mainCamera.orthographicSize;
			float maxCameraSize = Mathf.Max(sprend.transform.localScale.x,sprend.transform.localScale.y) * cameraOriginalSize * 6.0f/5.0f;
			float minCameraSize = 1.0f * cameraOriginalSize / 5.0f;
			cameraSize = Mathf.Clamp(cameraSize - mouseWheel,minCameraSize,maxCameraSize);
			mainCamera.orthographicSize = cameraSize;
		}
	}

	void handleMouseClicks() {
		wasShiftDraggin = shiftDraggin;
		if (escapeDown) {
			shiftDraggin = false;
			shiftDragginCancelled = true;
		}
		mouseLeftDown = Input.GetMouseButton(0);
		if (Input.GetMouseButtonUp(0)) shiftDragginCancelled = false;
		if (!normalDraggin && !middleDraggin && !rightDraggin && !shiftDragginCancelled) shiftDraggin = ((shiftDraggin && mouseLeftDown) || ((shiftDown || (gui.mapMode == 1/* && gui.visibilityMode == 0*/)) && Input.GetMouseButtonDown(0)));
		if (!shiftDraggin && !middleDraggin && !rightDraggin) normalDraggin = ((normalDraggin && mouseLeftDown) || (!shiftDown && Input.GetMouseButtonDown(0)));
		mouseRightDown = Input.GetMouseButton(1);
		if (!shiftDraggin && !middleDraggin && !normalDraggin) rightDraggin = (rightDraggin && mouseRightDown) || Input.GetMouseButtonDown(1);
		mouseMiddleDown = Input.GetMouseButton(2);
		if (!shiftDraggin && !normalDraggin && !rightDraggin) middleDraggin = (middleDraggin && mouseMiddleDown) || Input.GetMouseButtonDown(2);
	}

	void handleKeys() {
		shiftDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		altDown = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
		controlDown = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
		hDown = Input.GetKey(KeyCode.H);
		iDown = Input.GetKey(KeyCode.I);
		lDown = Input.GetKey(KeyCode.L);
		sDown = Input.GetKey(KeyCode.S);
		escapeDown = Input.GetKeyDown(KeyCode.Escape);
		spaceDown = Input.GetKey(KeyCode.Space);
	}


	void handleKeyActions() {
		if (shiftDown && altDown && controlDown) {
			bool wasI = displayI;
			bool wasL = displayL;
			bool wasS = displayS;
			displayS = sDown && !displayH && !displayI && !displayL;
			displayH = hDown && !displayI && !displayL && !displayS;
			displayI = iDown && !displayH && !displayL && !displayS;
			displayL = lDown && !displayI && !displayH && !displayS;
			if (!wasI && displayI) {
				if (imageFileName != null && !imageFileName.Equals("")) {
					StartCoroutine(importGrid());
				}
			}
			if (!wasL && displayL) {
				loadNewBackgroundFile();
			}
			if (!wasS && displayS) {
				if (imageFileName != null && !imageFileName.Equals("")) {
					printGrid();
				}
			}
			if (displayH) {
				displayHTime = 20;
			}
		}
		if (!displayH && displayHTime>0) {
			displayHTime--;
		}
	}

	void handleMouseMovement() {
//		float mouseFactor = mainCamera.orthographicSize/5.0f;
		float mouseFactor = 0.3f;
		float mouseX = Input.GetAxis("Mouse X");
		float mouseY = Input.GetAxis("Mouse Y");
		mouseFactor = 18.0f;
		if (middleDraggin  && Input.mousePosition.x < Screen.width*(1-boxWidthPerc) - gui.checkExtraX) {
			Vector3 pos = mainCamera.WorldToScreenPoint(cameraTransform.position);
			pos.x -= mouseX * mouseFactor;
			pos.y -= mouseY * mouseFactor;
			cameraTransform.position = mainCamera.ScreenToWorldPoint(pos);
//				cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x - mouseX * mouseFactor,cameraTransform.localPosition.y - mouseY * mouseFactor,cameraTransform.localPosition.z);
		}
	}

	void handleMouseSelect() {
		if (shiftDraggin && (wasShiftDraggin || Input.mousePosition.x < Screen.width*(1-boxWidthPerc) - gui.checkExtraX)) {
			Vector3 v3 = Input.mousePosition;
			v3.z = 10.0f;
			v3 = mainCamera.ScreenToWorldPoint(v3);
			Vector3 posActual = new Vector3(v3.x,v3.y,v3.z);
			v3.x += gridX/2.0f;
			v3.y += gridY/2.0f;
			v3.y = gridY - v3.y;
			v3.x = Mathf.Floor(v3.x);
			v3.y = Mathf.Floor(v3.y);
			//Debug.Log(v3);
			if (!wasShiftDraggin) {
				startSquareActual = posActual;
				startSquare = new Vector2(v3.x,v3.y);
			//	startSquareActual = startSquare;
				if (gui.mapMode == 0) {
					mouseOver = (GameObject)Instantiate(gridPrefab);
					SpriteRenderer sr =  mouseOver.GetComponent<SpriteRenderer>();
					mouseOver2 = (GameObject)Instantiate(gridPrefab);
					SpriteRenderer sr2 = mouseOver2.GetComponent<SpriteRenderer>();
					sr.color = new Color(1.0f,1.0f,1.0f,0.4f);
					sr.sortingOrder = 2;
	//				sr2.color = new Color (1.0f,1.0f,1.0f,0.5f);
					sr2.color = new Color(red, green, blue, 0.4f);
					sr2.sortingOrder = 1;
				}
				else if (gui.visibilityMode == 0) {
					Debug.Log("Create Wall");
					LineRenderer lr = createLineRenderer(startSquareActual.x, startSquareActual.x, startSquareActual.y, startSquareActual.y);
					lr.renderer.sortingOrder = 10;
					Color c = new Color(red/255.0f, green/255.0f, blue/255.0f);
				//	lr.SetColors(c,c);
					Wall w = lr.gameObject.GetComponent<Wall>();
					w.gridManager = this;
					w.setStart(startSquareActual.x, startSquareActual.y);
					w.setEnd(startSquareActual.x, startSquareActual.y);
					currentWall = w;
					w.setColor(c);
					w.visibility = wallVisibility;
					w.canMelee = wallMelee;
					w.canRange = wallRange;
					w.setBothWays(wallBothWays);
					currentWall.setBlockedShown(true);
				}
				else if (gui.visibilityMode == 1) {
					RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);//, float.MaxValue, 1 << 8);

//					RaycastHit hit;
//					if (Physics.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector3.zero, out hit)) {
					if (hit) {
				/*	Vector3 mousePos2 = Input.mousePosition;
					mousePos2.z = 5.0f;
					Vector2 v = Camera.main.ScreenToWorldPoint(mousePos2);
					Collider2D[] col = Physics2D.OverlapPointAll(v);
					if (col.Length > 0) {
						foreach (Collider2D c in col) {*/
						GameObject go = hit.collider.gameObject;
						Wall w = null;
						if (go.name.Contains("Circle")) {
							selectedEdges = go.name.Contains("Start") ? 1 : 2;
							w = go.transform.parent.GetComponent<Wall>();
						}
						else {
							selectedEdges = 0;
							w = go.GetComponent<Wall>();
						}
						if (w != null) {
							if (currentWall != null && currentWall != w) {
								currentWall.setCirclesShown(false);
								currentWall.setBlockedShown(false);
								currentWall.lineRenderer.renderer.sortingOrder = 1;
							}
							currentWall = w;
							oldStartPos = currentWall.startPos;
							oldEndPos = currentWall.endPos;
							currentWall.setBlockedShown(true);
							currentWall.setCirclesShown(false);
							currentWall.renderer.sortingOrder = 10;
						}
					}
					else if (currentWall != null) {
						currentWall.setCirclesShown(false);
						currentWall.lineRenderer.renderer.sortingOrder = 1;
						currentWall.setBlockedShown(false);
						currentWall = null;
					}
					//}
				}
			}
			else if (spaceDown) {
				Vector3 v4 = startSquareActual;
				v4.x += (posActual.x - lastPos.x);
				v4.y += (posActual.y - lastPos.y);
				Vector3 posActual4 = new Vector3(v4.x,v4.y,v4.z);
				v4.x += gridX/2.0f;
				v4.y += gridY/2.0f;
				v4.y = gridY - v4.y;
				v4.x = Mathf.Floor(v4.x);
				v4.y = Mathf.Floor(v4.y);
				startSquareActual = posActual4;
				startSquare = new Vector2(v4.x,v4.y);
			}

			if (gui.mapMode==0) {
	//			posActual = v3;
				Vector2 min = new Vector2(Mathf.Min(posActual.x,startSquareActual.x),Mathf.Min(posActual.y,startSquareActual.y));
				Vector2 max = new Vector2(Mathf.Max(posActual.x,startSquareActual.x),Mathf.Max(posActual.y,startSquareActual.y));
		//		Vector2 min2 = new Vector2(Mathf.Min(Mathf.Floor (posActual.x),Mathf.Floor (startSquareActual.x)) - (((int)gridX)%2==1 ? 0.5f : 0.0f),Mathf.Min(Mathf.Floor (posActual.y),Mathf.Floor (startSquareActual.y)) - (((int)gridY)%2==1 ? 0.5f : 0.0f));
		//		Vector2 max2 = new Vector2(Mathf.Max (Mathf.Floor (posActual.x),Mathf.Floor (startSquareActual.x)) + (((int)gridX)%2==1 ? 0.5f : 1.0f) ,Mathf.Max (Mathf.Floor (posActual.y),Mathf.Floor (startSquareActual.y)) + (((int)gridY)%2==1 ? 0.5f : 1.0f));
				Vector2 min2 = new Vector2(Mathf.Min(gridX, Mathf.Max(0.0f, Mathf.Min(v3.x, startSquare.x))) - gridX/2.0f, gridY/2.0f - Mathf.Min(gridY, Mathf.Max(0.0f, Mathf.Min(v3.y, startSquare.y))));
				Vector2 max2 = new Vector2(Mathf.Max(0.0f, Mathf.Min(gridX-1.0f,Mathf.Max(v3.x, startSquare.x)) + 1.0f) - gridX/2.0f, gridY/2.0f - Mathf.Max(0.0f, 1.0f + Mathf.Min(gridY-1.0f, Mathf.Max(v3.y, startSquare.y))));
				mouseOver.transform.localScale = new Vector3(max.x - min.x,max.y - min.y, 1.0f);
				mouseOver.transform.position = new Vector3((max.x + min.x)/2.0f,(max.y + min.y)/2.0f, 2.0f);
				mouseOver2.transform.localScale = new Vector3(max2.x - min2.x, max2.y - min2.y, 1.0f);
				mouseOver2.transform.position = new Vector3((max2.x + min2.x)/2.0f, (max2.y + min2.y)/2.0f, 2.0f);
			}
			else if (currentWall != null) {
				if (gui.visibilityMode == 0) {
					currentWall.setStart(startSquareActual.x, startSquareActual.y);
					currentWall.setEnd(posActual.x, posActual.y);
				}
				else if (wasShiftDraggin) {
					Vector2 change = new Vector2(posActual.x - lastPos.x, posActual.y - lastPos.y);
					if (selectedEdges == 0 || selectedEdges == 1)
						currentWall.setStart(currentWall.startPos.x + change.x, currentWall.startPos.y + change.y);
					if (selectedEdges == 0 || selectedEdges == 2)
						currentWall.setEnd(currentWall.endPos.x + change.x, currentWall.endPos.y + change.y);
				}
//				LineRenderer lr = currentWall.lineRenderer;
//				lr.SetPosition(0, new Vector3(startSquareActual.x, startSquareActual.y, 0.0f));
//				lr.SetPosition(1, new Vector3(posActual.x, posActual.y, 0.0f));
			}
			lastPos = posActual;
		}
		else if (shiftDragginCancelled) {
			Debug.Log("Shift Cancelled");
			if (gui.mapMode==0) {
				Destroy(mouseOver);
				mouseOver = null;
				Destroy(mouseOver2);
				mouseOver2 = null;
			}
			else if (gui.visibilityMode == 0) {
				Destroy(currentWall.gameObject);
//				currentWall.lineRenderer.renderer.sortingOrder = 1;
				currentWall = null;
			}
			else {
				currentWall.setStart(oldStartPos.x, oldStartPos.y);
				currentWall.setEnd(oldEndPos.x, oldEndPos.y);
				currentWall.setCirclesShown(true);
			}
			shiftDragginCancelled = false;
		}
		else if (wasShiftDraggin) {
			Debug.Log("WAs Shift Draggin");
			Vector3 v3 = Input.mousePosition;
			v3.z = 10.0f;
			v3 = mainCamera.ScreenToWorldPoint(v3);
			v3.x += gridX/2.0f;
			v3.y += gridY/2.0f;
			v3.y = gridY - v3.y;
			v3.x = Mathf.Floor(v3.x);
			v3.y = Mathf.Floor(v3.y);
			//Debug.Log("Start: " + startSquare);
			//Debug.Log("End: " + v3);
			if (gui.mapMode == 0) {
				Vector2 min = new Vector2(Mathf.Min(v3.x,startSquare.x),Mathf.Min(v3.y,startSquare.y));
				Vector2 max = new Vector2(Mathf.Max(v3.x,startSquare.x),Mathf.Max(v3.y,startSquare.y));
				Destroy(mouseOver);
				mouseOver = null;
				Destroy(mouseOver2);
				mouseOver2 = null;
				for (int n = (int)min.x; n <= (int)max.x;n++) {
					if (n >= 0 && n < gridX) {
						for (int m = (int)min.y; m <= (int)max.y; m++) {
							if (m >= 0 && m < gridY) {
								Tile t = gridsArray[n,m];
								setProperties(t);
							}
						}
					}
				}
			}
			else if (currentWall != null) {
				if (gui.visibilityMode == 0 && gui.editWhenPlaced) gui.visibilityMode = 1;
				if (gui.visibilityMode == 1) {
					currentWall.setCirclesShown(true);
				}
				else {
					currentWall.lineRenderer.renderer.sortingOrder = 1;
					currentWall.setBlockedShown(false);
					currentWall = null;
				}
			}
		}
		else if (shiftDraggin && currentWall != null && !(Input.mousePosition.x < Screen.width*(1-boxWidthPerc) - gui.checkExtraX)) {
			selectedEdges = -1;
/*			currentWall.setCirclesShown(false);
			currentWall.lineRenderer.renderer.sortingOrder = 1;
			currentWall = null;*/
		}
		if ((normalDraggin || rightDraggin) && gui.mapMode == 0 && Input.mousePosition.x < Screen.width*(1-boxWidthPerc) - gui.checkExtraX) {
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
			if (hit) {
				GameObject go = hit.collider.gameObject;
				if (normalDraggin) {
					Tile t = go.GetComponent<TileHolder>().tile;
					setProperties(t);
		//		SpriteRenderer sR = go.GetComponent<SpriteRenderer>();
		//		sR.color = new Color(red/255.0f,green/255.0f,blue/255.0f,0.4f);
				}
				else if (rightDraggin) {
					Tile t = go.GetComponent<TileHolder>().tile;
					red = t.red;
					green = t.green;
					blue = t.blue;
					gui.colorChanged = true;
//					passable = t.passable;
					standable = t.standable;
					passableUp = t.passableUp;
					passableRight = t.passableRight;
					passableDown = t.passableDown;
					passableLeft = t.passableLeft;
					visibleUp = t.visibilityUp;
					visibleDown = t.visibilityDown;
					visibleRight = t.visibilityRight;
					visibleLeft = t.visibilityLeft;
					startingPoint = t.startingPoint;
					trigger = t.trigger;
					action = t.action;
					canTurn = t.canTurn;
				}
			}
		}
	}

	public void setProperties(Tile t) {
		t.setColor((doRed ? red : t.red), (doGreen ? green : t.green), (doBlue ? blue : t.blue), 0.4f);
		//					t.passable = passable;
		if (doStand)
			t.standable = standable;
		if (doUp)
			t.passableUp = passableUp;
		if (doRight)
			t.passableRight = passableRight;
		if (doDown)
			t.passableDown = passableDown;
		if (doLeft)
			t.passableLeft = passableLeft;
		if (doTrigger)
			t.trigger = trigger;
		if (doAction)
			t.action = action;
		if (doStartingPoint) {
			t.startingPoint = startingPoint;
			t.setStartText();
		}
		if (doUpV)
			t.visibilityUp = visibleUp;
		if (doRightV)
			t.visibilityRight = visibleRight;
		if (doDownV)
			t.visibilityDown = visibleDown;
		if (doLeftV)
			t.visibilityLeft = visibleLeft;
		if (doTurn)
			t.canTurn = canTurn;
	}

	public void printGrid() {
		string str = imageFileName + ";" + gridX + ";" + gridY;
	
		foreach (Tile t in gridsArray) {
		//	foreach (Tile t in tA) {
				str = str + ";";
				str = str + t.stringValue();
		//	}
		}
		str += ";Visibility";
		for (int n=0;n<walls.transform.childCount;n++) {
			str += ";" + walls.transform.GetChild(n).GetComponent<Wall>().stringValue();
		}
		//Debug.Log(str);
		int currAdd = 0;
		string fileDirectory = "../Files/Maps/Tile Maps";
		if (isWindows()) fileDirectory = "..\\Files\\Maps\\Tile Maps";
		string fileName = fileDirectory + "/" + imageFileName + (currAdd>0?"" +currAdd:"") + ".txt";
		while (File.Exists(fileName)) {
			currAdd++;
			fileName = fileDirectory + (isWindows()? "\\" : "/") + imageFileName + (currAdd>0?""+currAdd:"") + ".txt";
		}
//		string path = EditorUtility.OpenFolderPanel("Select Folder",fileDirectory,"pppppppppp");
		string p1 = "../Files/Maps/Tile Maps";
		if (isWindows()) p1 = "..\\Files\\Maps\\Tile Maps";
		string path = EditorUtility.SaveFilePanel("Save Tile Map",p1,imageFileName + (currAdd>0?"" +currAdd:""),"txt");
		if (path.Length>0) {

//		if (File.Exists(path + "/" + fileName))
//		{
//			Debug.Log(fileName+" already exists.");
//			return;
//		}

			StreamWriter sr = File.CreateText(path);
	//	sr.WriteLine ("This is my file.");
	//	sr.WriteLine ("I can write ints {0} or floats {1}, and so on.",
	//	              1, 4.2);
			sr.WriteLine(str);
			sr.Close();
		}
		//		string path = EditorUtility.OpenFilePanel(
//			"Overwrite with jpg",
//			"../Files/Maps/Images",
//			"jpg");

	}

	public IEnumerator importGrid() {
		string p1 = "../Files/Maps/Tile Maps";
		if (isWindows()) p1 = "..\\Files\\Maps\\Tile Maps";
		string path = EditorUtility.OpenFilePanel(
			"Overwrite with jpg",
			p1,
			"txt");
		//			Sprite spr = ((SpriteRenderer)transform.GetComponent(SpriteRenderer)).sprite;
		//			Sprite sprite = new Sprite();
		//			sprend.sprite = sprite;
		if (path.Length != 0) {
			WWW www = new WWW("file:///" + path);
			yield return www;
			string text = www.text;
			string[] tiles = text.Split(";".ToCharArray());
			clearWalls();
			if (int.Parse(tiles[1])==gridX && int.Parse(tiles[2])==gridY) {
				//Debug.Log("Works!");
				parseTiles(tiles);
			}
			else {
				//Debug.Log(Application.absoluteURL);
			
				string path1 = "../Files/Maps/Images/" + tiles[0] + ".jpg";
				if (isWindows()) {
					path1 = "..\\Files\\Maps\\Images\\" + tiles[0] + ".jpg";
				}
				string path2 = Path.GetFullPath(path1);
				//Debug.Log(path2);
				if (File.Exists(path2)) {
					//Debug.Log("file Exists!");
					loadImage(path2);
					parseTiles(tiles);
				}
				else {
					//Debug.Log ("Grid size not compatable: (" + int.Parse(tiles[0]) + "," + int.Parse(tiles[1]) + ") and (" + gridX + "," + gridY + ")");
				}
			}
//			Debug.Log(text);
		}
	}

	void parseTiles(string[] tiles) {
		bool visibility = false;
		for (int n=3;n<tiles.Length;n++) {
			if (!visibility) {
				if (tiles[n] == "Visibility") visibility = true;
				else {
					int x = Tile.xForTile(tiles[n]);
					int y = Tile.yForTile(tiles[n]);
					Tile t = gridsArray[x,y];
					t.parseTile(tiles[n]);
				}
			}
			else {
				LineRenderer lr = createLineRenderer(0, 0, 0, 0);
				lr.renderer.sortingOrder = 1;
				Wall w = lr.GetComponent<Wall>();
				w.gridManager = this;
				w.parseWall(tiles[n]);

			}
		}
	}

	void clearGrid() {
		if (gridsArray != null) {
			foreach (Tile t in gridsArray) {
		//	foreach (Tile t in tA) {
				TileHolder th = t.tileGameObject.GetComponent<TileHolder>();
				th.tile = null;
				Destroy(t.tileGameObject);
		//	}
			}
		}
		gridsArray = null;
	}

	void clearWalls() {
		if (walls != null) {
			for (int n=0;n<walls.transform.childCount;n++) {
				Destroy(walls.transform.GetChild(n).gameObject);
			}
		}
	}

	void loadGrid(float x, float y) {
		gridX = (int)x;
		gridY = (int)y;
		clearGrid();
//		foreach (GameObject g in linesArray) {
//			Destroy (g);
//		}
		gridsArray = new Tile[gridX,gridY];
		linesArray = new ArrayList();
		float minX = -x/2.0f + 0.5f;
		float minY = y/2.0f - 0.5f;
		float maxX = x/2.0f - 0.5f;
		float maxY = -y/2.0f + 0.5f;
		//Debug.Log("x: " + x + ", minX: " + minX);
		int xcur = 0;
		for (float n=minX;n<=maxX;n++) {
			int ycur = 0;
			for (float m=minY;m>=maxY;m--) {
				GameObject go = (GameObject)Instantiate(gridPrefab);
				Tile t = new Tile(go,xcur,ycur,255.0f,255.0f,255.0f,0.4f);
				TileHolder th = go.AddComponent<TileHolder>();
				th.tile = t;
				gridsArray[xcur,ycur] = t;
				go.transform.position = new Vector3(n,m,0);
				go.transform.parent = grids.transform;
		//		SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
			//	sr.sprite.border = new Vector4(1.0f,1.0f,1.0f,1.0f);
		//		sr.color = new Color(t.red,t.green,t.blue,t.alpha);
				ycur++;
			}
			xcur++;
		}
		/*
		int x1 = 1;
		for (float n=minX;n<=minX;n++) {
			GameObject go = new GameObject("LineX" + x1);
			LineRenderer lr = go.AddComponent<LineRenderer>();
			go.transform.parent = lines.transform;
			lr.SetVertexCount(2);
			lr.SetPosition(0,new Vector3(n,minY,0));
			lr.SetPosition(1,new Vector3(n,maxY,0));
			lr.material = new Material(Shader.Find("Unlit/Texture"));
			lr.SetColors(Color.black,Color.black);
			lr.SetWidth(71.0f/70.0f,71.0f/70.0f);
		}
		xx++;*/
	}

	public void loadNewBackgroundFile() {
		string path1 = "../Files/Maps/Images";
		if (isWindows()) path1 = "..\\Files\\Maps\\Images";
		string path = EditorUtility.OpenFilePanel(
			"Overwrite with jpg",
			path1,
			"jpg");
		//Debug.Log(path);
		//			Sprite spr = ((SpriteRenderer)transform.GetComponent(SpriteRenderer)).sprite;
		//			Sprite sprite = new Sprite();
		//			sprend.sprite = sprite;
		loadImage(path);
	}

	public void loadImage(string path) {
		int max = 0;
		if (path.Length != 0) {
			string[] paths = path.Split(new char[]{'/','\\'});
			string path1 = paths[paths.Length-1];
			string[] pathA = path1.Split(new char[]{'.'});
			imageFileName = pathA[0];
			float currX = sprend.transform.localScale.x / spr.texture.width;
			float currY = sprend.transform.localScale.y / spr.texture.height;
		//	Debug.Log(Application.dataPath);
			WWW www = new WWW("file://" + path);
			while (!www.isDone && max < 1000000000) {
				max++;
			}
//			yield return www;
			www.LoadImageIntoTexture(spr.texture);
			float scaleX = Mathf.Round(currX * spr.texture.width);
			float scaleY = Mathf.Round(currY * spr.texture.height);
			sprend.transform.localScale = new Vector3(scaleX, scaleY, sprend.transform.localScale.z);
			mainCamera.orthographicSize = Mathf.Max(sprend.transform.localScale.x,sprend.transform.localScale.y) * cameraOriginalSize;
			cameraTransform.localPosition = new Vector3(0,0,-10);
			clearWalls();
			loadGrid(scaleX, scaleY);
			//Debug.Log(www.texture.width + "  " + www.texture.height + "    " + sprend.transform.localScale.x + "  " + sprend.transform.localScale.y);
		}
	}


	void OnApplicationQuit() {
		
		SpriteRenderer sprend = (SpriteRenderer)transform.GetComponent("SpriteRenderer");
		Sprite spr = sprend.sprite;
//		WWW www = new WWW("file://" + "/Users/Justin/Documents/UCI/ICS 169AB/Bel Nix/Images/Maps/none.jpg");
		//Debug.Log(Application.dataPath);
		WWW www;
		if (isWindows()) {
			www = new WWW("file:\\\\" + Application.dataPath + "\\Resources\\Images\\" + tileSize + ".png");
		}
		else {
			www = new WWW("file://" + Application.dataPath + "/Resources/Images/" + tileSize + ".png");
		}
		www.LoadImageIntoTexture(spr.texture);
	}

	bool isWindows() {
		switch (Application.platform) {
		case RuntimePlatform.WindowsEditor:
		case RuntimePlatform.WindowsPlayer:
		case RuntimePlatform.WindowsWebPlayer:
			Debug.Log("Windows");
			return true;
		default:
			Debug.Log("Not Windows");
			return false;
		}
	}
	LineRenderer createLineRenderer(float xStart, float xEnd, float yStart, float yEnd) {
		GameObject lrO = GameObject.Instantiate(Resources.Load<GameObject>("Images/LineRenderer")) as GameObject;
		LineRenderer lr = lrO.GetComponent<LineRenderer>();
		lr.material = new Material(Shader.Find("Particles/Alpha Blended"));
		lr.SetPosition(0, new Vector3(xStart, yStart, 0.0f));
		lr.SetPosition(1, new Vector3(xEnd, yEnd, 0.0f));
		lrO.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
		lrO.transform.parent = walls.transform;
		return lr;
	}

}
