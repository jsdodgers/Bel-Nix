using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;

public class MapGenerator : MonoBehaviour {



	public string tileMapName;
	public int gridSize = 70;
	
	public GameGUI gui;
	public AudioBank audioBank;
	GameObject lastHit;
	GameObject map;
	Transform mapTransform;
	Transform cameraTransform;
	Camera mainCamera;
	SpriteRenderer sprend;
	Sprite spr;
	float tapTime = 0.0f;

	GameObject targetObject;

	bool isOnGUI;
	public Transform playerTransform;
	public Transform enemyTransform;
	GameObject arrowStraightPrefab;
	GameObject arrowCurvePrefab;
	GameObject arrowPointPrefab;
	GameObject playerPrefab;
	GameObject enemyPrefab;
	GameObject warningRedPrefab;
	GameObject warningYellowPrefab;
	GameObject warningBothPrefab;
//	public GameObject selectedPlayer;
	public Unit selectedUnit;
	public List<Unit> selectedUnits;
	Unit hoveredCharacter;
	ArrayList players;
	public ArrayList enemies;
	GameObject grids;
	GameObject lines;
	GameObject path;
	GameObject enemiesObj;
	GameObject[,] gridArray;
	public Tile[,] tiles;
	public ArrayList lastPlayerPath;
	GameObject mouseOver;
	Vector2 startSquare;
	Vector2 lastPosDrag;
	Vector3 startSquareActual;
	int oldTouchCount;
//	bool editingPath = false;
	Vector2 lastArrowPos = new Vector2(-1, -1);

	
	GameObject currentGrid;
	GameObject mouseDownGrid;
	SpriteRenderer currentSprite;
	Color currentSpriteColor;
	
	GameObject gridPrefab;
	
	bool mouseLeftDown;
	bool mouseRightDown;
	bool mouseMiddleDown;
	bool mouseDownGUI = false;
	bool mouseDown;
	bool mouseDownRight;
	bool mouseUp;
	
	bool shiftDown;
	bool altDown;
	bool controlDown;
	bool spaceDown;
	bool escapeDown;
	
	bool shiftDraggin = false;
	bool shiftRightDraggin = false;
	bool middleDraggin = false;
	bool normalDraggin = false;
	bool rightDraggin = false;
	bool scrolled = false;

	bool wasRightDraggin = false;
	bool rightDragginCancelled = false;
	bool wasShiftRightDraggin = false;
	bool shiftRightDragginCancelled = false;

	float lastClickTime = 0.0f;
	Tile lastClickTile = null;
	float doubleClickTime = 0.25f;
	

	Vector3 lastPos = new Vector3(0.0f, 0.0f, 0.0f);
	Vector3 cameraMoveToPos;
	public bool movingCamera;
	
	public int actualWidth = 0;
	public int actualHeight = 0;
	float timeSinceSpace = 500.0f;
	int screenWidth = 0;
	int screenHeight = 0;

	public List<Unit> priorityOrder;

	public int currentUnit;

	// Use this for initialization
	void Start () {
		GameObject mainCameraObj = GameObject.Find("Main Camera");
		cameraTransform = mainCameraObj.transform;
		mainCamera = mainCameraObj.GetComponent<Camera>();
		//	cameraOriginalSize = mainCamera.orthographicSize;
		//		sprend = (SpriteRenderer)transform.GetComponent("SpriteRenderer");
		//		spr = sprend.sprite;
		//	grids = GameObject.Find("Grids");
		//	gridsArray = new Tile[gridX,gridY];
		//	gridPrefab = (GameObject)Resources.Load("Sprite/Square_70");
		//	lines = GameObject.Find("Lines");
		//		0.02857
		
		gridPrefab = (GameObject)Resources.Load("Materials/Square_70");
		map = GameObject.Find("Map");
		mapTransform = map.transform;
		GameObject guiObj = GameObject.Find("GameGUI");
		targetObject = GameObject.Find("Target");
		audioBank = GameObject.Find("AudioBank").GetComponent<AudioBank>();
		gui = guiObj.GetComponent<GameGUI>();
		gui.mapGenerator = this;
		
		lines = mapTransform.FindChild("Lines").gameObject;
		grids = mapTransform.FindChild("Grid").gameObject;
		path = mapTransform.Find("Path").gameObject;
		enemiesObj = mapTransform.Find("Enemies").gameObject;
		playerTransform = mapTransform.Find("Players");
		enemyTransform = enemiesObj.transform;




		createGrid();
		createTiles();

		priorityOrder = new List<Unit>();
		players = new ArrayList();
		playerPrefab = (GameObject)Resources.Load("Characters/Jackie/JackieAnimPrefab");
		arrowStraightPrefab = (GameObject)Resources.Load("Materials/Arrow/ArrowStraight");
		arrowCurvePrefab = (GameObject)Resources.Load("Materials/Arrow/ArrowCurve");
		arrowPointPrefab = (GameObject)Resources.Load("Materials/Arrow/ArrowPoint");
		warningRedPrefab = (GameObject)Resources.Load("Materials/Arrow/WarningRedPrefab");
		warningYellowPrefab = (GameObject)Resources.Load("Materials/Arrow/WarningYellowPrefab");
		warningBothPrefab = (GameObject)Resources.Load("Materials/Arrow/WarningBothPrefab");
//		Vector3[] positions = new Vector3[] {new Vector3(20, -36, 0), new Vector3(10, -36, 0)};
//		Vector3[] positions = new Vector3[] {new Vector3(18, -30, 0), new Vector3(14,-30,0), new Vector3(15, -31, 0)};
//		Vector3[] positions = new Vector3[] {new Vector3(18, -30, 0)};
/*		for (int n=0;n<positions.Length;n++) {
			Vector3 pos = positions[n];
			GameObject player = GameObject.Instantiate(playerPrefab) as GameObject;
			player.transform.parent = mapTransform;
			Player p = player.GetComponent<Player>();
			p.mapGenerator = this;
			p.setPosition(pos);
			players.Add(player);
		//	player.renderer.sortingOrder = 4;
			tiles[(int)pos.x,(int)-pos.y].setCharacter(p);
			p.setPriority();
			priorityOrder.Add(p);
			p.characterName = "Player" + (n+1);
	//		p.deselect();
		}*/
		enemies = new ArrayList();
		enemyPrefab = (GameObject)Resources.Load("Characters/Jackie/JackieEnemy");
	/*	Vector3[] positions2 = new Vector3[] {new Vector3(15, -28, 0), new Vector3(17, -27, 0), new Vector3(4, -23, 0)};
		for (int n=0;n<positions2.Length;n++) {
			Vector3 pos = positions2[n];
			GameObject enemy = GameObject.Instantiate(enemyPrefab) as GameObject;
			enemy.transform.parent = enemiesObj;
			Enemy e = enemy.GetComponent<Enemy>();
			e.setPosition(pos);
			e.mapGenerator = this;
			enemies.Add(enemy);
			enemy.renderer.sortingOrder = 3;
		}*/
		int bbb = 1;
		GameObject[] mapPlayers = GameObject.FindGameObjectsWithTag("Player");
		foreach (GameObject player in mapPlayers) {
			Vector3 pos = player.transform.position;
			Player p = player.GetComponent<Player>();
			int x = (int)(pos.x - 0.5f);
			int y = (int)(pos.y + 0.5f);
			p.setPosition(new Vector3(x, y, pos.z));
			p.mapGenerator = this;
			p.gui = gui;
			players.Add(player);
			//		enemy.renderer.sortingOrder = 3;
			tiles[x,-y].setCharacter(p);
			p.loadCharacterSheet();
			p.rollInitiative();
			p.characterName = "Player" + bbb;
			priorityOrder.Add(p);
			//		e.deselect();
			bbb++;
		}
		int aaa = 1;
		GameObject[] mapEnemies = GameObject.FindGameObjectsWithTag("Enemy");
		foreach (GameObject enemy in mapEnemies) {
			Vector3 pos = enemy.transform.position;
			Enemy e = enemy.GetComponent<Enemy>();
			int x = (int)(pos.x - 0.5f);
			int y = (int)(pos.y + 0.5f);
			e.setPosition(new Vector3(x, y, pos.z));
			e.mapGenerator = this;
			e.gui = gui;
			enemies.Add(enemy);
	//		enemy.renderer.sortingOrder = 3;
			tiles[x,-y].setCharacter(e);
			e.loadCharacterSheet();
			e.rollInitiative();
			e.characterName = "Enemy" + aaa;
			priorityOrder.Add(e);
	//		e.deselect();
			aaa++;
		}
		string b4 = "";
		for (int n=0;n<priorityOrder.Count;n++) {
			if (n!=0) b4 += "\n";
			b4 += priorityOrder[n].characterName + "  " + priorityOrder[n].getInitiative();
		}
		sortPriority();

		string after = "";
		for (int n=0;n<priorityOrder.Count;n++) {
			if (n!=0) after += "\n";
			after += priorityOrder[n].characterName + "  " + priorityOrder[n].getInitiative();
		}
		importGrid();
		createSelectionArea();
		createSelectionUnits();
//		StartCoroutine(importGrid());
		Debug.Log(b4 + "\n\n" + after);
//		Debug.Log(after);
//		priorityOrder = priorityOrder.
	}

	public void enterPriority() {
		if (isInCharacterPlacement()) removeCharacterPlacementObjects();
		foreach (Unit u in priorityOrder) {
			u.rollInitiative();
		}
		sortPriority();
		nextPlayer();
	}

	public void sortPriority() {
		List<Unit> po1 = new List<Unit>();
		foreach (Unit cs in priorityOrder) {
			po1.Add(cs);
		}
		priorityOrder.Sort((first, second) => (first.getInitiative() > second.getInitiative() ? -1 : (first.getInitiative() == second.getInitiative() && po1.IndexOf(first) < po1.IndexOf(second) ? -1 : 1)));
	}

	public bool isInCharacterPlacement() {
		return currentUnit == -1;
	}

	public bool isInPriority() {
		return currentUnit != -1;
	}

	public int selectionWidth = 100;
	public float spriteSize = 64.0f;
	public float spriteSeparator = 30.0f;
	float scaleFactor = 100.0f/64.0f;

	public void removeCharacterPlacementObjects() {
		GameObject gold = GameObject.Find("Selection");
		if (gold != null)
			Destroy(gold);
		foreach (Unit u in selectionUnits) {
			players.Remove(u);
			priorityOrder.Remove(u);
			Destroy(u.gameObject);
		}
	}

	public void createSelectionArea() {
		if (!isInCharacterPlacement()) return;
		selectionUnitsX = Screen.width/2.0f - (selectionWidth)/2.0f;
		if (Screen.height < selectionUnits.Count * (spriteSize + spriteSeparator) + spriteSeparator)
			selectionUnitsX -= (selectionWidth - spriteSize)/4.0f;
		repositionSelectionUnits();
		if (Screen.width == screenWidth && Screen.height == screenHeight) return;

//		selectionUnitsX *= scaleFactor;
		screenHeight = Screen.height;
		screenWidth = Screen.width;
		GameObject gold = GameObject.Find("Selection");
		if (gold != null)
			Destroy(gold);
		float texWidth = selectionWidth * scaleFactor;
		float texHeight = Screen.height * scaleFactor;
		float texX = ((Screen.width) * scaleFactor/2.0f - texWidth)/100.0f;
		float texY = -texHeight/2.0f/100.0f;
		Texture2D tex = makeTexBorder((int)texWidth, (int)texHeight, new Color(30.0f/255.0f, 40.0f/255.0f, 210.0f/255.0f));
		GameObject go = new GameObject();
		go.name = "Selection";
		SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
		sr.sortingOrder = 900;
//		sr.sprite.texture = tex;
		//		Sprite spr = Sprite.Create(tex, new Rect(Screen.width - 100, 0.0f, 100.0f, Screen.height), new Vector2(Screen.width - 100, 0.0f));
		Sprite spr = Sprite.Create(tex, new Rect(0.0f, 0.0f, texWidth, texHeight), new Vector2(0.0f, 0.0f));
	//	tex.pix
		sr.sprite = spr;
		sr.transform.parent = Camera.main.transform;
		go.transform.localPosition = new Vector3(texX, texY, 1.0f);
//		sr.transform.localPosition = new Vec
//		go.transform.parent = 
	}

	public List<Unit> selectionUnits;
	float selectionUnitsX;
	public void createSelectionUnits() {
		selectionUnits = new List<Unit>();
		TextAsset t = Resources.Load<TextAsset>("Saves/Characters");
		string[] chars = t.text.Split(new char[]{';'});
		for (int n=0;n<chars.Length-1;n++) {
			GameObject p = (GameObject)GameObject.Instantiate(playerPrefab);
			SpriteRenderer sr = p.GetComponent<SpriteRenderer>();
			sr.sortingOrder = 90000;
			p.transform.parent = Camera.main.transform;
			Unit pl = p.GetComponent<Unit>();
			pl.mapGenerator = this;
			pl.gui = gui;
			players.Add(pl);
			priorityOrder.Add(pl);
			pl.loadCharacterSheetFromTextFile(chars[n]);
			pl.rollInitiative();
			selectionUnits.Add(pl);
			Debug.Log(pl.characterSheet.personalInfo.getCharacterName().fullName());
		}
		sortPriority();
		repositionSelectionUnits();
	}

	public void repositionSelectionUnits() {
		if (selectionUnits == null) return;
		float y = Screen.height / 2.0f - spriteSeparator - spriteSize/2.0f + gui.selectionUnitScrollPosition.y;
		float initialY = y;
		int n=0;
		foreach (Unit p in selectionUnits) {
			if (n==selectionCurrentIndex) {
				y -= spriteSeparator + spriteSize;
			}
			if (p.gameObject != selectedSelectionObject) {
				p.transform.localPosition = new Vector3(selectionUnitsX/spriteSize, y/spriteSize, 1.0f);
			}
			else {
		//		Debug.Log("Selected: " + p.characterSheet.personalInfo.getCharacterName().fullName());
			}
			y -= spriteSeparator + spriteSize;
			n++;
		}
		if (selectedSelectionObject!=null) {
			if (Input.mousePosition.x >= Screen.width - selectionWidth) {
				float y2 = spriteSeparator + spriteSize/2.0f - gui.selectionUnitScrollPosition.y;
				int mouseY = (int)(Screen.height - Input.mousePosition.y);
				int p = (int)((mouseY - y2) / (spriteSeparator + spriteSize) + 1) - 1;
				if (p<selectionCurrentIndex || selectionCurrentIndex == -1) p++;
				selectionCurrentIndex = p;
			//	Debug.Log(Input.mousePosition.y + "   " +  p);
				int pos = (int)((initialY - selectedSelectionObject.transform.localPosition.y) / (spriteSeparator + spriteSize));
			//	selectionCurrentIndex = pos;
	//			Debug.Log(pos);
			}
			else {
				selectionCurrentIndex = -1;
			}
		}
	}

	Texture2D makeTexBorder(int width, int height, Color col )
	{
		Color[] pix = new Color[width * height];
		for( int i = 0; i < pix.Length; ++i )
		{
			//	Debug.Log("it is: " + (i/width));
			if (i/width == 0 || i/width == height-1) pix[i] = Color.black;
			else if (i%width == 0 || i % width == width-1) pix[i] = Color.black;
			else pix[ i ] = col;
		}
		Texture2D result = new Texture2D( width, height );
		result.SetPixels( pix );
		result.Apply();
		return result;
	}
	

	

	public Unit nextPlayer() {
		if (currentUnit >=0 && currentUnit < priorityOrder.Count)
			getCurrentUnit().removeCurrent();
		currentUnit++;
		currentUnit%=priorityOrder.Count;
		resetPlayerPath();
		if (selectedUnit) {
			selectedUnit.resetVars();
			if (selectedUnit.attackEnemy) {
				selectedUnit.attackEnemy.deselect();
			}
		}
		/*
		if (selectedUnit) {
			resetAroundCharacter(selectedUnit);
			lastPlayerPath = new ArrayList();
			selectedUnit.resetPath();
			selectedUnit.attackEnemy = null;
			string color = "Materials/SelectionCircleGreen";
			if (selectedUnit.team != 0) color = "Materials/SelectionCircleRed";
		//	selectedUnit.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(color);
	//		selectedUnit.transform.FindChild("Circle").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(color);
			selectedUnit.resetVars();
			selectedUnit.deselect();
		}
		Unit u;
		while (selectedUnits.Count != 0) {
			u = selectedUnits[0];
			resetAroundCharacter(u);
			u.deselect();
			selectedUnits.RemoveAt(0);
		}*/
		deselectAllUnits();
//		if (hoveredCharacter) {
//			resetAroundCharacter(hoveredCharacter);
//		}
		selectedUnit = getCurrentUnit();
	//	selectedUnit.transform.FindChild("Circle").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Materials/SelectionCircleWhite");
		if (selectedUnit) {
			selectedUnit.resetVars();
//			setAroundCharacter(selectedUnit);
			addCharacterRange(selectedUnit);
			selectedUnit.setSelected();
			selectedUnit.setCurrent();
			moveCameraToSelected();
			lastPlayerPath = selectedUnit.currentPath;
	//		editingPath = false;
		}
//		setTargetObjectPosition();
		return getCurrentUnit();
	}

	public Unit getCurrentUnit() {
		if (currentUnit == -1) return null;
		return priorityOrder[currentUnit];
	}

	public void importGrid() {
		string text = Resources.Load<TextAsset>("Maps/Tile Maps/" + tileMapName).text;// + ((tileMapName.Length >= 4 && tileMapName.EndsWith(".txt")) ? "" : ".txt")).text;
		Debug.Log(text);
		string[] tiles = text.Split(new char[]{';'});
		if (int.Parse(tiles[1])==actualWidth && int.Parse(tiles[2])==actualHeight) {
			////Debug.Log("Works!");
			parseTiles(tiles);
		}
		currentUnit = -1;
//		nextPlayer();
	//	Debug.Log (getCurrentUnit().characterName);
//		moveCameraToSelected(true);
		/*
		string pathName = Application.dataPath + "/Resources/Maps/Tile Maps/" + tileMapName;
		if (!(tileMapName.Length >= 4 && tileMapName.EndsWith(".txt"))) {
			pathName += ".txt";
		}
		Debug.Log(pathName);
		if (File.Exists(pathName)) {
			Debug.Log("File Exists");
			//Debug.Log("Exists!");
			WWW www = new WWW("file:///" + pathName);
			yield return www;
			string text = www.text;
			string[] tiles = text.Split(new char[]{';'});
			if (int.Parse(tiles[1])==actualWidth && int.Parse(tiles[2])==actualHeight) {
				////Debug.Log("Works!");
				parseTiles(tiles);
			}
			currentUnit = -1;
			nextPlayer();
			Debug.Log (getCurrentUnit().characterName);
			moveCameraToSelected(true);
		}*/
	}

	void parseTiles(string[] tilesArr) {
		List<Vector2> positions = new List<Vector2>();
		for (int n=3;n<tilesArr.Length;n++) {
			int x = Tile.xForTile(tilesArr[n]);
			int y = Tile.yForTile(tilesArr[n]);
			Tile t = tiles[x,y];
			t.parseTile(tilesArr[n]);
			if (t.startingPoint) {
				GameObject go = gridArray[x,y];
				SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
				Color c = Color.green;
				c.a = .4f;
				sr.color = c;
				positions.Add(new Vector2(go.transform.position.x, go.transform.position.y));
			}
		}
		if (positions.Count > 0) {
			Vector2 avg = getAverage(positions);
			moveCameraToPosition(new Vector3(avg.x+.5f, avg.y-.5f, 0.0f), true);
		}
	}

	Vector2 getAverage(List<Vector2> positions) {
		Vector2 avg = new Vector2(0.0f, 0.0f);
		foreach (Vector2 pos in positions) {
			avg += pos;
		}
		avg /= positions.Count;
		return avg;
	}

	void createGrid() {
		
		
		sprend = map.GetComponent<SpriteRenderer>();
		spr = sprend.sprite;
		//Debug.Log("Start()");
		int width = spr.texture.width;
		int height = spr.texture.height;
//		if (spr.texture.width % 64 == 0) gridSize = 64;
		actualWidth = width / gridSize;
		actualHeight = height / gridSize;
		Camera.main.orthographicSize = Mathf.Min(Mathf.Max(actualWidth/3, actualHeight/2) + 2, 10.0f);
		Vector3 newPos = Camera.main.transform.position;
		newPos.x = ((float)actualWidth) / 2.0f;
		newPos.y = -((float)actualHeight)/ 2.0f;
		Camera.main.transform.position = newPos;
		//Debug.Log("width: " + width + ", height: " + height);
		//Debug.Log("actualWidth: " + actualWidth + ", actualHeight: " + actualHeight);
		//Debug.Log("newPos: " + newPos);
		//Debug.Log("End");
		selectedUnit = null;
		lastPlayerPath = new ArrayList();
		selectedUnits = new List<Unit>();
		for (int n=0;n<=actualHeight;n++) {
			createLineRenderer(-0.025f, actualWidth + 0.025f, -n, -n);
		}
		for (int n=0;n<=actualWidth;n++) {
			createLineRenderer(n, n, 0.025f, -actualHeight - 0.025f);
		}
		loadGrid(actualWidth, actualHeight);
		//		setAroundPlayer(currentMoveDist, viewDist);
	}

	void createTiles() {
		tiles = new Tile[actualWidth,actualHeight];
		for (int n=0;n<actualWidth;n++) {
			for (int m=0;m<actualHeight;m++) {
				Tile t1 = new Tile();
				tiles[n,m] = t1;
				if (n>0) {
					Tile t2 = tiles[n-1,m];
					t2.rightTile = t1;
					t1.leftTile = t2;
				}
				if (m>0) {
					Tile t2 = tiles[n,m-1];
					t2.downTile = t1;
					t1.upTile = t2;
				}
			}
		}
	}
	
	void createLineRenderer(float xStart, float xEnd, float yStart, float yEnd) {
		GameObject lrO = GameObject.Instantiate(Resources.Load("Materials/LineRenderer",typeof(GameObject)) as GameObject) as GameObject;
		LineRenderer lr = lrO.GetComponent<LineRenderer>();
		lr.SetPosition(0, new Vector3(xStart, yStart, 0.0f));
		lr.SetPosition(1, new Vector3(xEnd, yEnd, 0.0f));
		lrO.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
		lrO.transform.parent = lines.transform;
	}

	public void removeCharacter(Unit cs) {
		int index = priorityOrder.IndexOf(cs);
		priorityOrder.Remove(cs);
		if (enemies.Contains(cs.gameObject)) enemies.Remove(cs.gameObject);
		if (players.Contains(cs.gameObject)) players.Remove(cs.gameObject);
		if (index < currentUnit) currentUnit--;
		else if (index == currentUnit) {
			currentUnit--;
			gui.selectedMovement = false;
			nextPlayer();
		}
	}

	public bool hasEnemy(int x, int y, Unit cs) {
		return tiles[x,y].hasEnemy(cs);
	}

	public bool hasAlly(int x, int y, Unit cs) {
		return tiles[x,y].hasAlly(cs);
	}

	public bool canPass(Direction dir, int x, int y, Unit cs, Direction dirFrom) {
		return tiles[x,y].canPass(dir, cs, dirFrom);
	}

	public bool canAttack(Direction dir, int x, int y, Unit cs) {
		int pass = tiles[x,y].passabilityInDirection(dir);
		return pass >0 && pass <10;
	}

	// Update is called once per frame
	void Update () {
		handleInput();
		moveCamera();
		if (isInCharacterPlacement()) {
			createSelectionArea();
		}
	//	setTargetObjectScale();
	}

	float cameraSpeed = 32.0f;
	void moveCamera() {
		if (!movingCamera) return;
	//	float speed = 32.0f;
		float dist = cameraSpeed * Time.deltaTime;
//		float distLeft = Mathf.
		Vector3 pos = Camera.main.transform.position;
		Vector3 left = new Vector3(cameraMoveToPos.x - pos.x, cameraMoveToPos.y - pos.y, cameraMoveToPos.z - pos.z);
		float distLeft = Mathf.Sqrt(Mathf.Pow(left.x,2) + Mathf.Pow(left.y,2) + Mathf.Pow(left.z,2));
		if (distLeft <= dist) {
			Camera.main.transform.position = cameraMoveToPos;
			movingCamera = false;
		}
		else {
			Vector3 move = new Vector3(left.x, left.y, left.z);
			move.x /= distLeft;
			move.y /= distLeft;
			move.z /= distLeft;
			move.x *= dist;
			move.y *= dist;
			move.z *= dist;
			pos += move;
			Camera.main.transform.position = pos;
		}
	}

	public void moveCameraToSelected(bool instantly = false, float speed = 32.0f) {
		if (selectedUnit == null) return;
		Vector3 sel = selectedUnit.transform.position;
		moveCameraToPosition(sel, instantly, speed);
	}

	public void moveCameraToPosition(Vector3 position, bool instantly = false, float speed = 32.0f) {
		cameraSpeed = speed;
		position.z = Camera.main.transform.position.z;
		if (instantly) {
			Camera.main.transform.position = position;
		}
		else {
			movingCamera = true;
			cameraMoveToPos = position;
		}
	}

	public void resetPlayerPath() {
		if (lastPlayerPath!=null) {
			for (int n=path.transform.childCount-1; n >= 0;n--) {
				Transform t = path.transform.GetChild(n);
				GameObject go = t.gameObject;
				t.parent = null;
				Destroy(go);
			
			}
		}
	}

	public void setPlayerPath(ArrayList path1) {
		for (int n=1;n<path1.Count;n++) {
			Vector2 v = (Vector2)path1[n];
			Vector2 v0 = (Vector2)path1[n-1];


			GameObject go;
			if (n == path1.Count-1) {
				go = GameObject.Instantiate(arrowPointPrefab) as GameObject;
				int xDif = (int)(v.x - v0.x);
				int yDif = (int)(v.y - v0.y);
				go.transform.eulerAngles = new Vector3(0.0f, 0.0f, (xDif==-1 ?90.0f : (xDif==1 ? 270.0f : (yDif == -1 ? 0.0f : 180.0f))));
			}
			else {
				Vector2 v2 = (Vector2)path1[n+1];
				if (v2.x == v0.x || v2.y == v0.y) {
					go = GameObject.Instantiate(arrowStraightPrefab) as GameObject;
					if (v2.y == v0.y) {
						go.transform.eulerAngles = new Vector3(0.0f, 0.0f, 90.0f);
					}
				}
				else {
					int xDif1 = (int)(v2.x - v.x);
					int yDif1 = (int)(v2.y - v.y);
					int xDif2 = (int)(v.x - v0.x);
					int yDif2 = (int)(v.y - v0.y);
			//		//Debug.Log("xDif1: " + xDif1 + " yDif1: " + yDif1 + " xDif2: " + xDif2 + " yDif2: " + yDif2);
					go = GameObject.Instantiate(arrowCurvePrefab) as GameObject;
					float rot = 0.0f;
					if (xDif1 == -1 && yDif2 == -1) rot = 270.0f;
					else if (xDif2 == 1 && yDif1 == -1) rot = 180.0f;
					else if (xDif2 == 1 && yDif1 == 1) rot = 270.0f;
					else if (xDif1 == 1 && yDif2 == 1) rot = 90.0f;
					else if (xDif1 == -1 && yDif2 == 1) rot = 180.0f;
					else if (xDif2 == -1 && yDif1 == -1) rot = 90.0f;
				//	if (xDif == -1 && yDif == -1) rot = 90.0f;
				//	if (xDif == 1 && yDif == 1) rot = 270.0f;
				//	if (xDif == -1 && yDif == 1) rot = 180.0f;
					go.transform.eulerAngles = new Vector3(0.0f, 0.0f, rot);
				}
			}
		//	go.renderer.sortingOrder = 2;
		//	= GameObject.Instantiate(arrowStraightPrefab) as GameObject;
			go.transform.parent = path.transform;
			go.transform.localPosition = new Vector3(v.x + 0.5f - 1/64.0f, -v.y - 0.5f - 1/64.0f, 0.0f);

			Tile t = tiles[(int)v0.x,(int)v0.y];
			Direction direction = Direction.Left;
			if (v0.x < v.x) direction = Direction.Right;
			if (v0.y < v.y) direction = Direction.Down;
			if (v0.y > v.y) direction = Direction.Up;
			bool isDifficult = t.isDifficultTerrain(direction);
			bool provokes = (gui.selectedMovementType == MovementType.Move && t.provokesOpportunity(direction, selectedUnit));
			if (isDifficult || provokes) {
				GameObject warning;
				if (isDifficult && provokes) warning = GameObject.Instantiate(warningBothPrefab) as GameObject;
				else if (isDifficult) warning = GameObject.Instantiate(warningYellowPrefab) as GameObject;
				else warning = GameObject.Instantiate(warningRedPrefab) as GameObject;
				warning.transform.parent = path.transform;
				warning.transform.localPosition = new Vector3(v0.x + (direction==Direction.Right ? 1.0f : (direction==Direction.Left ? 0.0f : 0.5f)), -v0.y - (direction==Direction.Down ? 1.0f : (direction==Direction.Up ? 0.0f : 0.5f)), 0.0f);
			}

		}
	}

	public void resetCharacterRange() {
		Unit p = null;
		if (this.selectedUnit) p = this.selectedUnit;
		else if (this.hoveredCharacter) p = this.hoveredCharacter;
		if (p != null) {
			resetAroundCharacter(p);
		}
		foreach (Unit u in selectedUnits) {
			resetAroundCharacter(u);
		}
		if (p != null) {
			setAroundCharacter(p);
		}
		foreach (Unit u in selectedUnits) {
			setAroundCharacter(u);
		}
	}

	public void resetAroundCharacter(Unit cs) {
		resetAroundCharacter(cs, cs.viewDist);
	}

	public void resetAroundCharacter(Unit cs, int view) {
		for (int x = (int)Mathf.Max(cs.position.x - view,0); x < (int)Mathf.Min(cs.position.x + 1 + view, actualWidth); x++) {
			for (int y = (int)Mathf.Max(-cs.position.y - view,0); y < (int)Mathf.Min(-cs.position.y + 1.0f + view, actualHeight); y ++) {
				GameObject go = gridArray[x,y];
				tiles[x,y].resetStandability();
				SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
				Color c = Color.clear;
				if (sr!=currentSprite) {
					sr.color = c;
				}
				else {
					currentSpriteColor = c;
				}
			}
		}
		setCurrentSpriteColor();
	}

	public void setSpriteColor(Tile t, SpriteRenderer sr) {
		Color c = Color.clear;
		if (t.canStandCurr) c = new Color(0.0f, 0.0f, 1.0f, 0.4f);
		else if (t.canAttackCurr) c = new Color(1.0f, 0.0f, 0.0f, 0.4f);
		else if (isInCharacterPlacement() && t.startingPoint) c = new Color(0.0f, 1.0f, 0.0f, 0.4f);
		if (sr != currentSprite) {
			sr.color = c;
		}
		else {
			currentSpriteColor = c;
		}
	}

	public void setAroundCharacter(Unit cs) {
		setAroundCharacter(cs, ((cs == getCurrentUnit() && selectedUnits.Count==0) ? cs.currentMoveDist : cs.moveDistLeft), cs.viewDist, cs.attackRange);
	}

	public void setAroundCharacter(Unit cs, int radius, int view, int attackRange) {
		setCharacterCanStand((int)cs.position.x, (int)-cs.position.y, radius, 0, attackRange, cs);
		int type = 4;
		/*
		for (int x = (int)Mathf.Max(cs.position.x - view,0); x < (int)Mathf.Min(cs.position.x + 1 + view, actualWidth); x++) {
			for (int y = (int)Mathf.Max(-cs.transform.localPosition.y - view,0); y < (int)Mathf.Min(-cs.position.y + 1.0f + view, actualHeight); y++) {
				GameObject go = gridArray[x,y];
				SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
				Tile t = tiles[x,y];
				setSpriteColor(t, sr);
			}
		}*/
	}

	public void setCharacterCanStand(int x, int y, int radiusLeft, int currRadius, int attackRange, Unit cs, Direction dirFrom = Direction.None) {
		if (currRadius == 0) //Debug.Log(attackRange);
		if (x < 0 || y < 0 || x >= actualWidth || y >= actualHeight) return;
		Tile t = tiles[x,y];
		if (t.canStandCurr && t.minDistCurr <= currRadius) return;
		t.canStandCurr = true;
		t.minDistCurr = currRadius;
		if ((selectedUnits.Count != 0 || selectedUnit != getCurrentUnit()) && gui.showAttack)
			setCharacterCanAttack(x, y, attackRange, 0, cs);
		if (radiusLeft == 0) return;
		if (canPass(Direction.Left, x, y, cs, dirFrom))
			setCharacterCanStand(x-1, y, radiusLeft-1, currRadius+1, attackRange, cs, Direction.Left);
		if (canPass(Direction.Right, x, y, cs, dirFrom))
			setCharacterCanStand(x+1, y, radiusLeft-1, currRadius+1, attackRange, cs, Direction.Right);
		if (canPass(Direction.Up, x, y, cs, dirFrom))
			setCharacterCanStand(x, y-1, radiusLeft-1, currRadius+1, attackRange, cs, Direction.Up);
		if (canPass(Direction.Down, x, y, cs, dirFrom))
			setCharacterCanStand(x, y+1, radiusLeft-1, currRadius+1, attackRange, cs, Direction.Down);
	}

	public void setCharacterCanAttack(int x, int y, int radiusLeft, int currRadius, Unit cs) {
	if (x < 0 || y < 0 || x >= actualWidth || y >= actualHeight) return;
		Tile t = tiles[x,y];
		if (t.canStandCurr && currRadius != 0) return;
		if (t.canAttackCurr && t.minAttackCurr <= currRadius) return;
		if (t.standable) {
			t.canAttackCurr = true;
			t.minAttackCurr = currRadius;
		}
	//	Debug.Log("can attack: " + x + ", " + y);
		if (radiusLeft == 0) return;
		if (canAttack(Direction.Left, x, y, cs))
			setCharacterCanAttack(x-1,y,radiusLeft-1,currRadius+1, cs);
		if (canAttack(Direction.Right, x, y, cs))
			setCharacterCanAttack(x+1,y,radiusLeft-1,currRadius+1, cs);
		if (canAttack(Direction.Up, x, y, cs))
			setCharacterCanAttack(x,y-1,radiusLeft-1,currRadius+1, cs);
		if (canAttack(Direction.Down, x, y, cs))
			setCharacterCanAttack(x,y+1,radiusLeft-1,currRadius+1, cs);
	}
/*
	bool isInPlayerRadius(Player player1, int radius, int x, int y) {
		return Mathf.Abs(player1.position.x - x) + Mathf.Abs(-player1.position.y - y) <= radius;
	}
	
	bool isInCircularRadius(Player player1, int radius, int x, int y) {
		return Mathf.Pow(player1.position.x - x,2) + Mathf.Pow(-player1.position.y - y,2) < Mathf.Pow(radius,2);
	}
	
	bool isInCircularRadius2(Player player1, int radius, int x, int y) {
		return Mathf.Pow(player1.position.x - x,2) + Mathf.Pow(-player1.position.y - y,2) <= Mathf.Pow(radius,2);
	}
	
	bool isInCircularRadius3(Player player1, int radius, int x, int y) {
		return Mathf.Pow(player1.position.x - x,2) + Mathf.Pow(-player1.position.y - y,2) - 2 <= Mathf.Pow(radius,2);
	}*/
	
	int totalMoveDist(Unit cs, int x, int y) {
		return (int)Mathf.Abs(cs.position.x - x) + (int)Mathf.Abs(-cs.position.y  - y);
	}
	void loadGrid(int x, int y) {
		gridArray = new GameObject[x,y];
		int xcur = 0;
		for (float n=0;n<x;n++) {
			int ycur = 0;
			for (float m=0;m<y;m++) {
				GameObject go = (GameObject)Instantiate(gridPrefab);
				gridArray[(int)n,(int)m] = go;
				go.transform.position = new Vector3(n,m*-1,0);
				go.transform.parent = grids.transform;
				SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
				sr.color = Color.clear;
				ycur++;
			}
			xcur++;
		}
	}
	
	
	void handleInput() {
		handleGUIPos();
		handleMouseScrollWheel();
		handleKeys();
		handleMouseButtons();
		handleKeyPan();
		handleMouseClicks();
		handleMouseSelect();
		handleMouseMovement();
	}
	

	void handleGUIPos() {
	//	isOnGUI = gui.moveButtonRect().Contains(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
		isOnGUI = gui.mouseIsOnGUI();
	}
	
	
	void handleMouseScrollWheel() {
		//	if (Input.mousePosition.x < Screen.width*(1-boxWidthPerc)) {
		Camera.main.orthographicSize = Screen.height / 128.0f;
		if (gui.mouseIsOnScrollView()) return;
		return;
		Vector3 v3 = Input.mousePosition;
		if (v3.x < 0 || v3.y < 0 || v3.x > Screen.width || v3.y > Screen.height) return;
		float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
		float cameraSize = mainCamera.orthographicSize;
		float originalCameraSize = cameraSize;
		float maxCameraSize = Mathf.Max(actualWidth/3,actualHeight/2) * 1.5f;
		maxCameraSize = Mathf.Min(maxCameraSize,10.0f);
		float minCameraSize = 1.0f;
		cameraSize = Mathf.Clamp(cameraSize - mouseWheel,minCameraSize,maxCameraSize);
		mainCamera.orthographicSize = cameraSize;
		scrolled = originalCameraSize != cameraSize;
		//	}
	}
	
	
	
	void handleKeys() {
		shiftDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		altDown = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
		controlDown = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
		escapeDown = Input.GetKey(KeyCode.Escape);
		spaceDown = Input.GetKey(KeyCode.Space);
		if (Input.GetKeyDown(KeyCode.M)) {
			gui.openTab = (gui.openTab==Tab.M ? Tab.None : Tab.M);
		}
		if (Input.GetKeyDown(KeyCode.C)) {
			Debug.Log("KeyCode.C");
			gui.openTab = (gui.openTab==Tab.C ? Tab.None : Tab.C);
		}
		if (Input.GetKeyDown(KeyCode.K)) {
			gui.openTab = (gui.openTab==Tab.K ? Tab.None : Tab.K);
		}
		if (Input.GetKeyDown(KeyCode.I)) {
			gui.openTab = (gui.openTab==Tab.I ? Tab.None : Tab.I);
		}
		if (Input.GetKeyDown(KeyCode.T)) {
			gui.openTab = (gui.openTab==Tab.T ? Tab.None : Tab.T);
		}
		if (Input.GetKeyDown(KeyCode.Escape) && !normalDraggin && !shiftDraggin) {
			gui.openTab = Tab.None;
		}
		handleArrows();
		handleSpace();
	}
	float maxTimeSpace = .25f;
	void handleSpace() {
		timeSinceSpace += Time.deltaTime * Time.timeScale;
		if (Input.GetKeyDown(KeyCode.Space)) {
			if (timeSinceSpace <= maxTimeSpace && isInPriority()) {
				deselectAllUnits();
				selectUnit(getCurrentUnit(),false);
				lastPlayerPath = selectedUnit.currentPath;
			}
			moveCameraToSelected();
			timeSinceSpace = 0.0f;
		}
	}

	void handleArrows() {
		if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow)) {
			if (selectedUnit) {

				if (selectedUnits.Count == 0) {
					int curr = priorityOrder.IndexOf(selectedUnit);
					curr++;
					curr %= priorityOrder.Count;

					/*
					resetAroundCharacter(selectedUnit);
					resetPlayerPath();
					lastPlayerPath = new ArrayList();
					selectedUnit.resetPath();
					selectedUnit.attackEnemy = null;
					selectedUnit.deselect();*/
					deselectAllUnits();
					selectUnit(priorityOrder[curr], false);
					lastPlayerPath = selectedUnit.currentPath;
				}
			}
			else {
				selectUnit(getCurrentUnit(), false);
			}
		}
		if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow)) {

			if (selectedUnit) {
				if (selectedUnits.Count == 0) {
					int curr = priorityOrder.IndexOf(selectedUnit);
					curr--;
					while (curr < 0) curr += priorityOrder.Count;
					curr %= priorityOrder.Count;
					/*
					resetAroundCharacter(selectedUnit);
					resetPlayerPath();
					lastPlayerPath = new ArrayList();
					selectedUnit.resetPath();
					selectedUnit.attackEnemy = null;
					selectedUnit.deselect();
					selectedUnit = null;*/
					deselectAllUnits();
					selectUnit(priorityOrder[curr], false);
					lastPlayerPath = selectedUnit.currentPath;
				}
			}
			else {
				selectUnit(getCurrentUnit(), false);
			}
		}
	}
	
	bool lastPlayerPathContains(Vector2 v) {
		if (lastPlayerPath==null) return false;
		foreach (Vector2 v2 in lastPlayerPath) {
			if (v2.x == v.x && v2.y == v.y) return true;
		}
		return false;
	}

	bool leftClickIsMakingSelection() {
		return selectedUnit == getCurrentUnit() && selectedUnits.Count == 0 && guiSelectionType();
	}

	bool guiSelectionType() {
		return (gui.selectedStandard && (gui.selectedStandardType == StandardType.Attack || gui.selectedStandardType == StandardType.Throw || gui.selectedStandardType == StandardType.Intimidate)) ||
			(gui.selectedMovement && (gui.selectedMovementType == MovementType.Move || gui.selectedMovementType == MovementType.BackStep));
	}


	void handleMouseButtons() {
		wasShiftRightDraggin = shiftDraggin;
		wasRightDraggin = normalDraggin;
		if (escapeDown && !leftClickIsMakingSelection()) {
			if (normalDraggin) rightDragginCancelled = true;
			normalDraggin = false;
			if (shiftDraggin) shiftRightDragginCancelled = true;
			shiftDraggin = false;
		}
		

		mouseLeftDown = Input.GetMouseButton(0);
		mouseMiddleDown = Input.GetMouseButton(2);
		if (Input.touchCount == 2) Debug.Log("Time elapsed: " + (Time.time - tapTime));
		if (!shiftDraggin && (!normalDraggin || Time.time - tapTime <= 0.25f) && !rightDraggin && !shiftRightDraggin) {
		//	Debug.Log("Middle Set: " +   (middleDraggin && mouseMiddleDown) + "   "  + (!isOnGUI && Input.GetMouseButtonDown(2)) 
			middleDraggin = (middleDraggin && (mouseMiddleDown || (mouseLeftDown && Input.touchCount == 2))) || ((!isOnGUI || isOnGUI) && Input.GetMouseButtonDown(2)) || ((!isOnGUI || isOnGUI) && (Input.GetMouseButtonDown(0) || (Input.GetMouseButton(0) && normalDraggin)) && (Input.touchCount >= 2 && (oldTouchCount == 2 || true)));
			normalDraggin = normalDraggin && !middleDraggin;
		}
		if (!normalDraggin && !middleDraggin && !rightDraggin && !shiftRightDraggin) shiftDraggin = ((shiftDraggin && mouseLeftDown) || (!isOnGUI && shiftDown && Input.GetMouseButtonDown(0)));
		if (!shiftDraggin && !middleDraggin && !rightDraggin && !shiftRightDraggin) {
			if (normalDraggin && mouseLeftDown) {
				normalDraggin = true;
			}
			else if (!isOnGUI && !shiftDown && Input.GetMouseButtonDown(0)) {
				normalDraggin = true;
				tapTime = Time.time;
			}
			else normalDraggin = false;
		}
		mouseRightDown = Input.GetMouseButton(1);
		if (!normalDraggin && !middleDraggin && !rightDraggin && !shiftDraggin) shiftRightDraggin = ((shiftRightDraggin && mouseRightDown) || (!isOnGUI && shiftDown && Input.GetMouseButtonDown(1)));
		if (!shiftDraggin && !middleDraggin && !normalDraggin && !shiftRightDraggin) rightDraggin = (rightDraggin && mouseRightDown) || (!shiftDown && Input.GetMouseButtonDown(1));
		mouseDown = Input.GetMouseButtonDown(0) && Input.touchCount != 2;
		mouseUp = Input.GetMouseButtonUp(0) && Input.touchCount != 2;
		mouseDownRight = Input.GetMouseButtonDown(1) && Input.touchCount != 2;
	//	if (mouseDown || mouseUp)
	//		Debug.Log("MouseDown: " + mouseDown + " MouseUp: " + mouseUp);
		if (mouseDown) mouseDownGUI = isOnGUI;
//		oldTouchCount = Input.touchCount;
	}

	public bool performingAction() {
		if (getCurrentUnit()==null) return false;
		return getCurrentUnit().isPerformingAnAction();
	}

	void handleMouseClicks() {
		if (performingAction()) return;
		handleMouseDown();
		handleMouseUp();
	}

	public GameObject selectedSelectionObject = null;
	Vector2 selectedSelectionDiff = new Vector2(0,0);
	void handleMouseDown() {
		if ((mouseDown && !leftClickIsMakingSelection()) && !isOnGUI && !rightDraggin) {
			if (!shiftDown) {
				deselectAllUnits();
				selectedUnit = hoveredCharacter;
				if (selectedUnit) {
//					setAroundCharacter(selectedUnit);
					addCharacterRange(selectedUnit);
					selectedUnit.setSelected();
					lastPlayerPath = selectedUnit.currentPath;
				}
//			setTargetObjectPosition();
			}
			else {
				Unit u = hoveredCharacter;
				selectUnit(u, true);
			}
		}
		if ((isInCharacterPlacement() && (mouseDown && !leftClickIsMakingSelection()) && !rightDraggin && !middleDraggin)) {
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 100.0f, 1<<10);
			if (hit) {
				GameObject go = hit.collider.gameObject;
				selectedSelectionObject = go;
//				selectedUnit = go.GetComponent<Unit>();
//				selectedUnit.setSelected();
				deselectAllUnits();
				selectUnit(go.GetComponent<Unit>(),false);
				go.transform.parent = playerTransform;
				go.GetComponent<SpriteRenderer>().sortingOrder = 90001;
				Vector3 pos = Input.mousePosition;
				pos.z = 10.0f;
				pos = Camera.main.ScreenToWorldPoint(pos);
				selectedSelectionDiff = new Vector2(pos.x - go.transform.localPosition.x, pos.y - go.transform.localPosition.y);
				if (lastHit) {
					int posX = (int)lastHit.transform.localPosition.x;
					int posY = -(int)lastHit.transform.localPosition.y;
					Tile t = tiles[posX,posY];
					if (t.getCharacter()==go.GetComponent<Unit>()) {
						selectionStartingTile = t;
					}
					else selectionStartingTile = null;
				}
				selectionCurrentIndex = selectionUnits.IndexOf(go.GetComponent<Unit>());
				if (selectionStartingTile==null) {
					selectionStartingIndex = selectionCurrentIndex;
				}
				selectionUnits.Remove(go.GetComponent<Unit>());
				selectionStartingPos = go.transform.position;
			}
		}
	
		
		if (mouseDown && !shiftDown && !isOnGUI && !rightDraggin && leftClickIsMakingSelection()) {
			if (gui.selectedStandard && (gui.selectedStandardType == StandardType.Attack || gui.selectedStandardType == StandardType.Throw || gui.selectedStandardType == StandardType.Intimidate)) {
				if (lastHit) {
					int posX = (int)lastHit.transform.localPosition.x;
					int posY = -(int)lastHit.transform.localPosition.y;
					
					if (selectedUnit.attackEnemy) {
						selectedUnit.attackEnemy.deselect();
						selectedUnit.attackEnemy = null;
					}
					if (tiles[posX,posY].canAttackCurr) {
						if (gui.selectedStandardType == StandardType.Attack || gui.selectedStandardType == StandardType.Intimidate)
							selectedUnit.attackEnemy = tiles[posX,posY].getEnemy(selectedUnit);
						else if (gui.selectedStandardType == StandardType.Throw)
							selectedUnit.attackEnemy = tiles[posX,posY].getCharacter();
						selectedUnit.setRotationToAttackEnemy();
					}
					if (selectedUnit.attackEnemy)
						selectedUnit.attackEnemy.setTarget();
				}
			}
		}
		if ((normalDraggin && leftClickIsMakingSelection()) && !mouseDownGUI) {		
			
			int x = -1;
			int y = 1;
			if (currentGrid!=null) {
				x = (int)currentGrid.transform.localPosition.x;
				y = (int)currentGrid.transform.localPosition.y;
			}
			Vector2 v = new Vector2(x, -y);
			
			
			if (selectedUnit && !Unit.vectorsEqual(v, lastArrowPos) && x>=0 && -y>=0) {
				//	Player p = selectedPlayer.GetComponent<Player>();
				//Debug.Log(p.currentMoveDist + "     aaa!!");
				if (gui.selectedMovement) {
					resetPlayerPath();
					if (!lastPlayerPathContains(v)) {
						lastPlayerPath = selectedUnit.addPathTo(v);
					}
					else {
						lastPlayerPath = selectedUnit.removeFromPathTo(v);
					}
					if (lastPlayerPath.Count > 1)
						setPlayerPath(lastPlayerPath);
					lastArrowPos = v;
				}
				
			}
		}
	}

	Tile selectionStartingTile = null;
	int selectionStartingIndex = -1;
	public int selectionCurrentIndex = -1;
	Vector3 selectionStartingPos;
	void handleMouseUp() {
		if (mouseUp && isInCharacterPlacement() && !rightDraggin && !middleDraggin) {
			if (selectedSelectionObject) {
				rightDragginCancelled = true;
				int posX = (int)lastHit.transform.localPosition.x;
				int posY = -(int)lastHit.transform.localPosition.y;
				Vector3 mousePos = Input.mousePosition;
				bool overThing = mousePos.x >= Screen.width - selectionWidth;
				Debug.Log(mousePos.x + " " + Screen.width + "    " + overThing);
				Tile t = tiles[posX, posY];
				Unit u2 = selectedSelectionObject.GetComponent<Unit>();
				if (t.startingPoint) {
					if (t.hasCharacter()) {
						Unit u = t.getCharacter();
						if (selectionStartingTile!=null) {
							selectionStartingTile.setCharacter(u);
							u.setPosition(new Vector3(selectionStartingPos.x - 0.5f, selectionStartingPos.y + 0.5f, 1.0f));
						}
						else {
							u.transform.parent = cameraTransform;
							u.GetComponent<SpriteRenderer>().sortingOrder = 90000;
							t.removeCharacter();
//							selectionUnits.Add(u);
							selectionUnits.Insert(selectionUnits.IndexOf(selectedSelectionObject.GetComponent<Unit>()),u);
						}
					}
					u2.setPosition(new Vector3(posX, -posY, 1.0f));
					t.setCharacter(u2);
					selectionUnits.Remove(u2);
					u2.GetComponent<SpriteRenderer>().sortingOrder = 10;
				}
				else {
					if (selectionStartingTile!=null && !overThing) {
						u2.GetComponent<SpriteRenderer>().sortingOrder = 10;
					}
					else {
						u2.GetComponent<SpriteRenderer>().sortingOrder = 90000;
						if (selectionStartingTile != null) {
							selectionStartingTile.removeCharacter();
						}
						u2.transform.parent = cameraTransform;
						if (!selectionUnits.Contains(u2)) {
							if (selectionStartingIndex<0 && selectionCurrentIndex<0) {
								selectionUnits.Add(u2);
							}
							else if (selectionCurrentIndex>=0) {
								selectionUnits.Insert(selectionCurrentIndex,u2);
							}
							else {
								selectionUnits.Insert(selectionStartingIndex,u2);
							}
						}
					}
					u2.transform.position = selectionStartingPos;
				}
				selectedSelectionObject = null;
				selectionStartingTile = null;
				selectionStartingIndex = -1;
				selectionCurrentIndex = -1;
			}
		}
		if (mouseUp && !shiftDown && !mouseDownGUI && !rightDraggin && leftClickIsMakingSelection()) {// && getCurrentUnit()==selectedUnit && selectedUnits.Count == 0) {
			if (lastHit) {
//				selectedUnit.attackEnemy = null;

				int posX = (int)lastHit.transform.localPosition.x;
				int posY = -(int)lastHit.transform.localPosition.y;
				if ((gui.selectedMovement && (gui.selectedMovementType == MovementType.BackStep || gui.selectedMovementType == MovementType.Move)) || (gui.selectedStandard && (gui.selectedStandardType == StandardType.Throw || gui.selectedStandardType==StandardType.Attack || gui.selectedStandardType == StandardType.Intimidate))) {
					if (Time.time - lastClickTime <= doubleClickTime && tiles[posX, posY] == lastClickTile) {
						if (gui.selectedMovement) {
							if (tiles[posX, posY].getCharacter() != selectedUnit) {
								selectedUnit.startMoving(gui.selectedMovementType==MovementType.BackStep);
							}
						}
						else if (gui.selectedStandardType == StandardType.Attack) {
							if (selectedUnit.attackEnemy) {
								selectedUnit.startAttacking();
							}
						}
						else if (gui.selectedStandardType == StandardType.Throw) {
							if (selectedUnit.attackEnemy) {
								selectedUnit.startThrowing();
							}
						}
						else if (gui.selectedStandardType == StandardType.Intimidate) {
							if (selectedUnit.attackEnemy) {
								selectedUnit.startIntimidating();
							}
						}
					}
				}
				if (gui.selectedMovement) {
					bool changed = false;
					for (int n=selectedUnit.currentPath.Count-1;n>=1;n--) {
						Vector2 v = (Vector2)selectedUnit.currentPath[n];
						if (!tiles[(int)v.x,(int)v.y].canStand()) {
							changed = true;
							selectedUnit.currentPath.RemoveAt(n);
							selectedUnit.setPathCount();
						}
						else {
							break;
						}
					}
					if (changed) {
						resetPlayerPath();
						lastPlayerPath = selectedUnit.currentPath;
						setPlayerPath(lastPlayerPath);
					}
					Vector2 v2 = (Vector2)lastPlayerPath[lastPlayerPath.Count-1];
					lastClickTile = tiles[(int)v2.x,(int)v2.y];
					lastClickTime = Time.time;
				}
				else if (gui.selectedStandard && (gui.selectedStandardType == StandardType.Throw || gui.selectedStandardType == StandardType.Attack || gui.selectedStandardType == StandardType.Intimidate)) {
					lastClickTile = tiles[posX, posY];
					lastClickTime = Time.time;
				}

			}
		//	editingPath = false;
			lastArrowPos = new Vector2(-1000, -1000);
		}
	}

	public void removePlayerPath() {
		resetPlayerPath();
//		lastPlayerPath = new ArrayList();
		selectedUnit.resetPath();
		lastPlayerPath = selectedUnit.currentPath;
	}
	
	public void selectUnit(Unit u, bool remove) {
		if (u) {
			if (!selectedUnit) {
				selectedUnit = u;
//				setAroundCharacter(u);
				addCharacterRange(u);
				u.setSelected();
			}
			else {
				if (selectedUnits.Count == 0) {
					removePlayerPath();
					selectedUnit.attackEnemy = null;
					lastPlayerPath = selectedUnit.currentPath;
				}
				if (selectedUnit == u) {
					if (remove) {
						selectedUnit.deselect();
						selectedUnit = null;
						if (selectedUnits.Count != 0) {
							selectedUnit = selectedUnits[0];
							selectedUnits.RemoveAt(0);
							lastPlayerPath = selectedUnit.currentPath;
						}
//						resetCharacterRange();
						resetRanges();
					}
				}
				else if (selectedUnits.Contains(u)) {
					if (remove) {
						u.deselect();
						selectedUnits.Remove(u);
					//	resetCharacterRange();
						resetRanges();
					}
				}
				else {
					if (u != getCurrentUnit())
						selectedUnits.Add(u);
					else {
						selectedUnits.Add(selectedUnit);
						selectedUnit = u;
						lastPlayerPath = selectedUnit.currentPath;
					}
//					setAroundCharacter(u);
					addCharacterRange(u);
					u.setSelected();
				}
			}
		}
	}

	public void deselectAllUnits() {
		if (selectedUnit) {
		//	resetAroundCharacter(selectedUnit);
			resetPlayerPath();
			lastPlayerPath = new ArrayList();
			selectedUnit.resetPath();
			selectedUnit.attackEnemy = null;
			selectedUnit.deselect();
			selectedUnit = null;
		}
		Unit u;
		while (selectedUnits.Count != 0) {
			u = selectedUnits[0];
		//	resetAroundCharacter(u);
			u.deselect();
			selectedUnits.RemoveAt(0);
		}
		removeAllRanges();
	}

	public void drawAllRanges() {
		drawRanges(0, 0, actualWidth, actualHeight);
	}

	public void drawRanges(int minX, int minY, int maxX, int maxY) {
		setCurrentSpriteColor();
		for (int x = minX; x < maxX; x++) {
			for (int y=minY; y < maxY; y++) {
				Tile t = tiles[x,y];
				SpriteRenderer sr = gridArray[x, y].GetComponent<SpriteRenderer>();
				setSpriteColor(t, sr);
			}
		}
	}

	public void resetAttack() {
		resetAttack(getCurrentUnit());
	}

	public void resetAttack(Unit u) {
		if (u.attackEnemy && u.attackEnemy.isTarget) {
			u.attackEnemy.deselect();
		}
		u.attackEnemy = null;
	}

	public void resetRanges() {
		removeAllRanges(false);
		if (selectedUnit) {
			addCharacterRange(selectedUnit, false);
			foreach (Unit u in selectedUnits) {
				addCharacterRange(u, false);
			}
		}
		drawAllRanges();
	//	}
	}

	public void removeAllRanges() {
		removeAllRanges(true);
	}

	public void removeAllRanges(bool draw) {
		foreach (Tile t in tiles) {
			t.resetStandability();
		}
		if (draw) drawAllRanges();
	}
	public void addCharacterRange(Unit u) {
		addCharacterRange(u, true);
	}

	public void addCharacterRange(Unit u, bool draw) {
		bool isOther = selectedUnit != getCurrentUnit() || selectedUnits.Count > 0;
		if ((gui.showMovement && isOther) || ((gui.selectedMovement && (gui.selectedMovementType == MovementType.Move || gui.selectedMovementType == MovementType.BackStep)) && !isOther))
			setAroundCharacter(u);
		else if ((gui.showAttack && isOther) || (gui.selectedStandard && gui.selectedStandardType == StandardType.Attack && !isOther))
			setCharacterCanAttack((int)u.position.x, (int)-u.position.y, u.attackRange,0, u);
		else if ((gui.selectedStandard && (gui.selectedStandardType == StandardType.Throw || gui.selectedStandardType == StandardType.Intimidate) && !isOther))
			setCharacterCanAttack((int)u.position.x, (int)-u.position.y, 1, 0, u);
		if (draw) drawAllRanges();
	}

	public void resetMoveDistances() {
	//	bool reset = true;
	//	foreach (GameObject player in players) {
	//		Player p = player.GetComponent<Player>();
	//		if (p.currentMoveDist!=0) reset = false;
	//	}
//		foreach (Unit character in priorityOrder) {
		///	Player p = player.GetComponent<Player>();
//			if (reset) p.currentMoveDist = p.maxMoveDist;
		//	character.setMoveDist(character.maxMoveDist);
//		}
		
	}


	void handleCharacterPlacementMovement() {
		Vector2 mouseMovement = new Vector2(Input.GetAxis("Mouse X"),Input.GetAxis("Mouse Y"));
		if (selectedSelectionObject) {
		//	Vector3 pos = Camera.main.WorldToScreenPoint(selectedSelectionObject.transform.position);
		//	pos.x += mouseMovement.x;
		//	pos.y += mouseMovement.y;
		//	pos.z = 10.0f;
		//	pos = Camera.main.ScreenToWorldPoint(pos);
			Vector3 pos = Input.mousePosition;
			pos.z = 10.0f;
			pos = Camera.main.ScreenToWorldPoint(pos);
			pos.x -= selectedSelectionDiff.x;
			pos.y -= selectedSelectionDiff.y;
			selectedSelectionObject.transform.localPosition = pos;
		}
	}
	
	void handleMouseMovement() {
		if (isInCharacterPlacement()) handleCharacterPlacementMovement();
		
		var mPos = Input.mousePosition;
		mPos.z = 10.0f;
		Vector3 pos1 = mainCamera.ScreenToWorldPoint(mPos);
		if (((middleDraggin && Input.touchCount == oldTouchCount) || scrolled || rightDraggin) && !movingCamera) {//  && Input.mousePosition.x < Screen.width*(1-boxWidthPerc)) {
			//= mainCamera.WorldToScreenPoint(cameraTransform.position);
			if (!Input.GetMouseButtonDown(0) || scrolled) {
				float xDiff = pos1.x - lastPos.x;
				float yDiff = pos1.y - lastPos.y;
			//	Vector3 pos = mapTransform.position;
			//	pos.x += xDiff;
			//	pos.y += yDiff;
			//	mapTransform.position = pos;
				Vector3 pos = mainCamera.transform.position;
				pos.x -= xDiff;
				pos.y -= yDiff;
				mainCamera.transform.position = pos;
			}
		}
		lastPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
		oldTouchCount = Input.touchCount;
	}

	void handleKeyPan() {
		if (movingCamera) return;
		float xDiff = 0;
		float yDiff = 0;
		float eachFrame = 4.0f;
		if (shiftDown) eachFrame = 1.5f;
		eachFrame *= Time.deltaTime;
		if (Input.GetKey(KeyCode.W)) yDiff -= eachFrame;
		if (Input.GetKey(KeyCode.S)) yDiff += eachFrame;
		if (Input.GetKey(KeyCode.A)) xDiff += eachFrame;
		if (Input.GetKey(KeyCode.D)) xDiff -= eachFrame;
		if (xDiff==0 && yDiff==0) return;
	//	Vector3 pos = mapTransform.position;
		Vector3 pos = mainCamera.transform.position;
		pos.x -= xDiff;
		pos.y -= yDiff;
		mainCamera.transform.position = pos;
	//	mapTransform.position = pos;
		lastPos.x -= xDiff;
		lastPos.y -= yDiff;
	}

	void rotatePlayerTowardsMouse() {
		var mPos = Input.mousePosition;
		mPos.z = 10.0f;
		Vector3 pos1 = mainCamera.ScreenToWorldPoint(mPos);
		if (shiftDown && selectedUnit) {
			float midSlope = (pos1.y - selectedUnit.transform.position.y)/(pos1.x - selectedUnit.transform.position.x);
			float rotation = Mathf.Atan(midSlope) + Mathf.PI/2.0f;
			Vector3 rot1 = selectedUnit.transform.eulerAngles;
			if (pos1.x > selectedUnit.transform.position.x) {
				rotation += Mathf.PI;
			}
			rot1.z = rotation * 180 / Mathf.PI;
			selectedUnit.transform.eulerAngles = rot1;
		}
	}

	int getDistance(Unit u, int x, int y) {
		return Mathf.Abs((int)u.position.x - x) + Mathf.Abs((int)u.position.y + y);
	}

	public Tile getMostInterestingTile(Unit u) {
		Tile interesting = null;
		int interestingNess = 0;
		int tileDist = 0;
		for (int x = Mathf.Max(0, (int)u.position.x - u.maxMoveDist); x < Mathf.Min(actualWidth, (int)u.position.x + u.maxMoveDist + 1); x++) {
			for (int y = Mathf.Max(0, (int)-u.position.y - u.maxMoveDist); y < Mathf.Min(actualHeight, (int)-u.position.y + u.maxMoveDist + 1); y++) {
				Tile t = tiles[x,y];
				int d = getDistance(u, x, y);
				int i = t.getInterestingNess(u, d);
				if (i > interestingNess || (i == interestingNess && d < tileDist)) {
					interestingNess = i;
					tileDist = d;
					interesting = t;
				}
			}
		}
		return interesting;
	}
	
	
	void handleMouseSelect() {
	//	if ((shiftRightDraggin || rightDraggin) && ((wasShiftRightDraggin || wasRightDraggin) || !isOnGUI)) {
		if ((!isInCharacterPlacement() || selectedSelectionObject==null) && !leftClickIsMakingSelection() && (shiftDraggin || normalDraggin) && ((wasRightDraggin || wasShiftRightDraggin) || !isOnGUI)) {
			Vector3 v3 = Input.mousePosition;
			v3.z = 10.0f;
			v3 = mainCamera.ScreenToWorldPoint(v3);
			Vector3 posActual = new Vector3(v3.x,v3.y,v3.z);
		//	v3.x += gridX/2.0f;
		//	v3.y += gridY/2.0f;
		//	v3.y = gridY - v3.y;
			v3 -= mapTransform.position;
			v3.x = Mathf.Floor(v3.x);
			v3.y = Mathf.Floor(-v3.y);
			//Debug.Log(v3);
			if (!(wasRightDraggin || wasShiftRightDraggin) || mouseOver == null) {
				startSquareActual = posActual;
				startSquare = new Vector2(v3.x,v3.y);
				//	startSquareActual = startSquare;
				mouseOver = (GameObject)Instantiate(gridPrefab);
				mouseOver.layer = 0;
				mouseOver.transform.parent = mapTransform;
				SpriteRenderer sr =  mouseOver.GetComponent<SpriteRenderer>();
				sr.color = new Color(1.0f,1.0f,1.0f,0.4f);
				sr.sortingOrder = 200;
			}
			else if (spaceDown) {
				Vector3 v4 = startSquareActual;
				v4.x += (posActual.x - lastPosDrag.x);
				v4.y += (posActual.y - lastPosDrag.y);
				Vector3 posActual4 = new Vector3(v4.x,v4.y,v4.z);
			//	v4.x += gridX/2.0f;
			//	v4.y += gridY/2.0f;
			//	v4.y = gridY - v4.y;
			//	v4.x = Mathf.Floor(v4.x);
			//	v4.y = Mathf.Floor(v4.y);
				startSquareActual = posActual4;
				startSquare = new Vector2(v4.x,-v4.y);
			}
			
			
			lastPosDrag = posActual;
			//			posActual = v3;
			Vector2 min = new Vector2(Mathf.Min(posActual.x,startSquareActual.x),Mathf.Min(posActual.y,startSquareActual.y));
			Vector2 max = new Vector2(Mathf.Max(posActual.x,startSquareActual.x),Mathf.Max(posActual.y,startSquareActual.y));
			//		Vector2 min2 = new Vector2(Mathf.Min(Mathf.Floor (posActual.x),Mathf.Floor (startSquareActual.x)) - (((int)gridX)%2==1 ? 0.5f : 0.0f),Mathf.Min(Mathf.Floor (posActual.y),Mathf.Floor (startSquareActual.y)) - (((int)gridY)%2==1 ? 0.5f : 0.0f));
			//		Vector2 max2 = new Vector2(Mathf.Max (Mathf.Floor (posActual.x),Mathf.Floor (startSquareActual.x)) + (((int)gridX)%2==1 ? 0.5f : 1.0f) ,Mathf.Max (Mathf.Floor (posActual.y),Mathf.Floor (startSquareActual.y)) + (((int)gridY)%2==1 ? 0.5f : 1.0f));
	//		Vector2 min2 = new Vector2(Mathf.Min(gridX, Mathf.Max(0.0f, Mathf.Min(v3.x, startSquare.x))) - gridX/2.0f, gridY/2.0f - Mathf.Min(gridY, Mathf.Max(0.0f, Mathf.Min(v3.y, startSquare.y))));
	//		Vector2 max2 = new Vector2(Mathf.Max(0.0f, Mathf.Min(gridX-1.0f,Mathf.Max(v3.x, startSquare.x)) + 1.0f) - gridX/2.0f, gridY/2.0f - Mathf.Max(0.0f, 1.0f + Mathf.Min(gridY-1.0f, Mathf.Max(v3.y, startSquare.y))));
			mouseOver.transform.localScale = new Vector3(max.x - min.x,max.y - min.y, 1.0f);
		//	mouseOver.transform.position = new Vector3((max.x + min.x)/2.0f,(max.y + min.y)/2.0f, 2.0f);
			mouseOver.transform.position = new Vector3(min.x,max.y, 2.0f);
		//	mouseOver2.transform.localScale = new Vector3(max2.x - min2.x, max2.y - min2.y, 1.0f);
		//	mouseOver2.transform.position = new Vector3((max2.x + min2.x)/2.0f, (max2.y + min2.y)/2.0f, 2.0f);
		}
		else if ((!isInCharacterPlacement() || selectedSelectionObject==null) && !leftClickIsMakingSelection() && (rightDragginCancelled || shiftRightDragginCancelled)) {
			if (mouseOver) {
				Destroy(mouseOver);
				mouseOver = null;
				if (!selectedUnit && hoveredCharacter && !rightDraggin && !shiftRightDraggin) {
				//	setAroundCharacter(hoveredCharacter);
				}
				setCurrentSpriteColor();
			}
			rightDragginCancelled = false;
			shiftRightDragginCancelled = false;
	//		Destroy(mouseOver2);
	//		mouseOver2 = null;
		}
		else if ((!isInCharacterPlacement() || selectedSelectionObject==null) && !leftClickIsMakingSelection() && (wasRightDraggin || wasShiftRightDraggin)) {
			Vector3 v3 = Input.mousePosition;
			v3.z = 10.0f;
			v3 = mainCamera.ScreenToWorldPoint(v3);
		//	v3.x += gridX/2.0f;
		//	v3.y += gridY/2.0f;
		//	v3.y = gridY - v3.y;
			v3 = v3 - mapTransform.position;
			v3.x = Mathf.Floor(v3.x);
			v3.y = Mathf.Floor(-v3.y);
		//	startSquare.x -= mapTransform.position.x;
		//	startSquare.y += mapTransform.position.y;
		
			//Debug.Log("Start: " + startSquare);
			//Debug.Log("End: " + v3);
			Vector2 min = new Vector2(Mathf.Min(v3.x,startSquare.x),Mathf.Min(v3.y,startSquare.y));
			Vector2 max = new Vector2(Mathf.Max(v3.x,startSquare.x),Mathf.Max(v3.y,startSquare.y));
			Destroy(mouseOver);
			mouseOver = null;
	//		Destroy(mouseOver2);
	//		mouseOver2 = null;
	//		Debug.Log(min + "   " + max);
			if (Mathf.Abs (v3.x - startSquare.x) >= 0.0001f || Mathf.Abs(v3.y - startSquare.y) >= 0.0001f) {
				for (int n = (int)min.x; n <= (int)max.x;n++) {
					if (n >= 0 && n < actualWidth) {
						for (int m = (int)min.y; m <= (int)max.y; m++) {
							if (m >= 0 && m < actualHeight) {
								Tile t = tiles[n,m];
								if (t.hasCharacter()) {
//								t.getCharacter().setSelected();
									selectUnit(t.getCharacter(), false);
								}
							}
//							setProperties(t);
						}
					}
				}
			}
		}


		
	//	//Debug.Log("Start Mouse Select");
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 100.0f, 1<<9);
		//		Physics2D.Ray
		GameObject go = null;
		if (hit.collider != null) go = hit.collider.gameObject;
		if (go != null && !isOnGUI) {
		//	Debug.Log(go + "  " + go.transform.position);
			if (go != lastHit) {
				lastHit = go;
				if (!selectedUnit) {
					if (hoveredCharacter) {
					//	resetAroundCharacter(hoveredCharacter, hoveredCharacter.viewDist);
					}
				}
				Tile t = tiles[(int)go.transform.localPosition.x,(int)-go.transform.localPosition.y];
				hoveredCharacter = t.getCharacter();
			}
			if (middleDraggin) {
				//	Tile t = go.GetComponent<TileHolder>().tile;
				//	setProperties(t);
				//		SpriteRenderer sR = go.GetComponent<SpriteRenderer>();
				//		sR.color = new Color(red/255.0f,green/255.0f,blue/255.0f,0.4f);
			}
			else {
				SpriteRenderer spr = go.GetComponent<SpriteRenderer>();
				if (spr) {
					if (spr != currentSprite) {
						currentGrid = go;
						if (currentSprite != null) {
							currentSprite.color = currentSpriteColor;
						}
						currentSprite = spr;
						currentSpriteColor = spr.color;
					//	if (!(rightDraggin || shiftRightDraggin)) {
						if (!((normalDraggin || shiftDraggin) && !leftClickIsMakingSelection() && !(isInCharacterPlacement() && selectedSelectionObject!=null))) {
							setCurrentSpriteColor();
						}
					}
				}
			}
		}
		else if (currentSprite!=null) {
			currentSprite.color = currentSpriteColor;
			currentGrid = null;
			currentSprite = null;
		}
	//	//Debug.Log("End Mouse Select");
	}

	public void setCurrentSpriteColor() {
		if (currentSprite == null) return;
		GameObject go = currentSprite.gameObject;
		Transform transform = go.transform;
		bool did = false;
	//	if (selectedPlayer) {
	//		Player p = selectedPlayer.GetComponent<Player>();
			//	//Debug.Log(" c: " + lastPlayerPath[0] + " d: " + new Vector2((int)currentGrid.transform.localPosition.x, (int)-currentGrid.transform.localPosition.y));
			//		if (Player.exists(lastPlayerPath, new Vector2((int)currentGrid.transform.localPosition.x, (int)-currentGrid.transform.localPosition.y))) {
			//		//Debug.Log("Yup!");
			//			spr.color = new Color(0.0f, 1.0f, 1.0f, 0.4f);
			//			did = true;
			//		}
			//	if (isInPlayerRadius(p, p.currentMoveDist,(int)currentGrid.transform.localPosition.x,(int)-currentGrid.transform.localPosition.y)) {
			Tile t = tiles[(int)transform.localPosition.x,(int)-transform.localPosition.y];
			if (t.canStandCurr) {
				currentSprite.color = new Color(0.0f, 0.0f, 0.50f, 0.4f);
				did = true;
			}
			//							else if (isInPlayerRadius(p, p.currentMoveDist + p.attackRange,(int)currentGrid.transform.localPosition.x,(int)-currentGrid.transform.localPosition.y)) {
			else if (t.canAttackCurr) {
				did = true;
				currentSprite.color = new Color(0.50f, 0.0f, 0.0f, 0.4f);
			}
			else if (t.startingPoint && isInCharacterPlacement()) {
				did = true;
				currentSprite.color = new Color(0.0f, 0.5f, 0.0f, 0.4f);
			}
	//	}
		if (!did) {
			//	//Debug.Log("Nope...");
			currentSprite.color = new Color(0.40f, 0.40f, 0.40f, 0.4f);
		}
	}
	
}
