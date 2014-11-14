using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.IO;

public class MapGenerator : MonoBehaviour {


	public string tileMapName;
	int gridSize = 70;
	
	GameGUI gui;
	public AudioBank audioBank;
	GameObject lastHit;
	GameObject map;
	Transform mapTransform;
	Transform cameraTransform;
	Camera mainCamera;
	SpriteRenderer sprend;
	Sprite spr;

	GameObject targetObject;

	bool isOnGUI;
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
	
	Vector3 lastPos = new Vector3(0.0f, 0.0f, 0.0f);
	
	int actualWidth = 0;
	int actualHeight = 0;

	public List<Unit> priorityOrder;

	int currentUnit;

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
		Vector3[] positions = new Vector3[] {new Vector3(18, -30, 0), new Vector3(17,-30,0), new Vector3(15, -31, 0)};
//		Vector3[] positions = new Vector3[] {new Vector3(18, -30, 0)};
		for (int n=0;n<positions.Length;n++) {
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
		}
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
		int aaa = 1;
		GameObject[] mapEnemies = GameObject.FindGameObjectsWithTag("Enemy");
		foreach (GameObject enemy in mapEnemies) {
			Vector3 pos = enemy.transform.position;
			Enemy e = enemy.GetComponent<Enemy>();
			int x = (int)(pos.x - 0.5f);
			int y = (int)(pos.y + 0.5f);
			e.setPosition(new Vector3(x, y, pos.z));
			e.mapGenerator = this;
			enemies.Add(enemy);
	//		enemy.renderer.sortingOrder = 3;
			tiles[x,-y].setCharacter(e);
			e.setPriority();
			e.characterName = "Enemy" + aaa;
			priorityOrder.Add(e);
	//		e.deselect();
			aaa++;
		}
		string b4 = "";
		for (int n=0;n<priorityOrder.Count;n++) {
			if (n!=0) b4 += "\n";
			b4 += priorityOrder[n].characterName + "  " + priorityOrder[n].getPriority();
		}
		List<Unit> po1 = new List<Unit>();
		foreach (Unit cs in priorityOrder) {
			po1.Add(cs);
		}
		priorityOrder.Sort((first, second) => (first.getPriority() > second.getPriority() ? -1 : (first.getPriority() == second.getPriority() && po1.IndexOf(first) < po1.IndexOf(second) ? -1 : 1)));
		
		string after = "";
		for (int n=0;n<priorityOrder.Count;n++) {
			if (n!=0) after += "\n";
			after += priorityOrder[n].characterName + "  " + priorityOrder[n].getPriority();
		}
		StartCoroutine(importGrid());
		Debug.Log(b4 + "\n\n" + after);
//		Debug.Log(after);
//		priorityOrder = priorityOrder.
	}



	public Unit nextPlayer() {
		if (currentUnit >=0 && currentUnit < priorityOrder.Count)
			getCurrentUnit().removeCurrent();
		currentUnit++;
		currentUnit%=priorityOrder.Count;
		resetPlayerPath();
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
		}
		if (hoveredCharacter) {
			resetAroundCharacter(hoveredCharacter);
		}
		selectedUnit = getCurrentUnit();
	//	selectedUnit.transform.FindChild("Circle").GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Materials/SelectionCircleWhite");
		if (selectedUnit) {
			setAroundCharacter(selectedUnit);
			selectedUnit.setSelected();
			selectedUnit.setCurrent();
	//		editingPath = false;
		}
//		setTargetObjectPosition();
		return getCurrentUnit();
	}

	public Unit getCurrentUnit() {
		return priorityOrder[currentUnit];
	}

	public IEnumerator importGrid() {
		string pathName = Application.dataPath + "/Resources/Maps/Tile Maps/" + tileMapName;
		if (!(tileMapName.Length >= 4 && tileMapName.EndsWith(".txt"))) {
			pathName += ".txt";
		}
		if (File.Exists(pathName)) {
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
		}
	}
	
	void parseTiles(string[] tilesArr) {
		for (int n=3;n<tilesArr.Length;n++) {
			int x = Tile.xForTile(tilesArr[n]);
			int y = Tile.yForTile(tilesArr[n]);
			Tile t = tiles[x,y];
			t.parseTile(tilesArr[n]);
		}
	}

	void createGrid() {
		
		
		sprend = map.GetComponent<SpriteRenderer>();
		spr = sprend.sprite;
		//Debug.Log("Start()");
		int width = spr.texture.width;
		int height = spr.texture.height;
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
		if (index <= currentUnit) currentUnit--;
		priorityOrder.Remove(cs);
		if (enemies.Contains(cs.gameObject)) enemies.Remove(cs.gameObject);
		if (players.Contains(cs.gameObject)) players.Remove(cs.gameObject);
	}

	public bool hasEnemy(int x, int y, Unit cs) {
		return tiles[x,y].hasEnemy(cs);
	}

	public bool hasAlly(int x, int y, Unit cs) {
		return tiles[x,y].hasAlly(cs);
	}

	public bool canPass(Direction dir, int x, int y, Unit cs) {
		return tiles[x,y].canPass(dir, cs);
	}

	public bool canAttack(Direction dir, int x, int y, Unit cs) {
		int pass = tiles[x,y].passabilityInDirection(dir);
		return pass >0 && pass <10;
	}

	// Update is called once per frame
	void Update () {
		handleInput();
	//	setTargetObjectScale();
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
			go.transform.localPosition = new Vector3(v.x + 0.5f, -v.y - 0.5f, 0.0f);

			Tile t = tiles[(int)v0.x,(int)v0.y];
			Direction direction = Direction.Left;
			if (v0.x < v.x) direction = Direction.Right;
			if (v0.y < v.y) direction = Direction.Down;
			if (v0.y > v.y) direction = Direction.Up;
			bool isDifficult = t.isDifficultTerrain(direction);
			bool provokes = t.provokesOpportunity(direction, selectedUnit);
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

	public void setAroundCharacter(Unit cs) {
		setAroundCharacter(cs, cs.currentMoveDist, cs.viewDist, cs.attackRange);
	}

	public void setAroundCharacter(Unit cs, int radius, int view, int attackRange) {
		setCharacterCanStand((int)cs.position.x, (int)-cs.position.y, radius, 0, attackRange, cs);
		setCurrentSpriteColor();
		int type = 4;
		for (int x = (int)Mathf.Max(cs.position.x - view,0); x < (int)Mathf.Min(cs.position.x + 1 + view, actualWidth); x++) {
			for (int y = (int)Mathf.Max(-cs.transform.localPosition.y - view,0); y < (int)Mathf.Min(-cs.position.y + 1.0f + view, actualHeight); y++) {
				GameObject go = gridArray[x,y];
				SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
				Tile t = tiles[x,y];
				Color c = Color.clear;
				if (t.canStandCurr) c = new Color(0.0f, 0.0f, 1.0f, 0.4f);
				else if (t.canAttackCurr) c = new Color(1.0f, 0.0f, 0.0f, 0.4f);
				if (sr != currentSprite) {
					sr.color = c;
				}
				else {
					currentSpriteColor = c;
				}
			}
		}
	}

	public void setCharacterCanStand(int x, int y, int radiusLeft, int currRadius, int attackRange, Unit cs) {
		if (currRadius == 0) //Debug.Log(attackRange);
		if (x < 0 || y < 0 || x >= actualWidth || y >= actualHeight) return;
		Tile t = tiles[x,y];
		if (t.canStandCurr && t.minDistCurr <= currRadius) return;
		t.canStandCurr = true;
		t.minDistCurr = currRadius;
	//	setCharacterCanAttack(x, y, attackRange, 0, cs);
		if (radiusLeft == 0) return;
		if (canPass(Direction.Left, x, y, cs))
			setCharacterCanStand(x-1, y, radiusLeft-1, currRadius+1, attackRange, cs);
		if (canPass(Direction.Right, x, y, cs))
			setCharacterCanStand(x+1, y, radiusLeft-1, currRadius+1, attackRange, cs);
		if (canPass(Direction.Up, x, y, cs))
			setCharacterCanStand(x, y-1, radiusLeft-1, currRadius+1, attackRange, cs);
		if (canPass(Direction.Down, x, y, cs))
			setCharacterCanStand(x, y+1, radiusLeft-1, currRadius+1, attackRange, cs);
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
		handleKeyPan();
		handleMouseClicks();
		handleMouseMovement();
		handleMouseSelect();
	}
	

	void handleGUIPos() {
	//	isOnGUI = gui.moveButtonRect().Contains(new Vector2(Input.mousePosition.x, Input.mousePosition.y));
		isOnGUI = gui.mouseIsOnGUI();
	}
	
	
	void handleMouseScrollWheel() {
		//	if (Input.mousePosition.x < Screen.width*(1-boxWidthPerc)) {
		if (gui.mouseIsOnScrollView()) return;
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
		handleArrows();
	}

	void handleArrows() {
		if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.RightArrow)) {
			if (selectedUnit) {

				if (selectedUnits.Count == 0) {
					int curr = priorityOrder.IndexOf(selectedUnit);
					curr++;
					curr %= priorityOrder.Count;
					resetAroundCharacter(selectedUnit);
					resetPlayerPath();
					lastPlayerPath = new ArrayList();
					selectedUnit.resetPath();
					selectedUnit.attackEnemy = null;
					selectedUnit.deselect();
					selectedUnit = null;
					selectUnit(priorityOrder[curr], false);
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
					resetAroundCharacter(selectedUnit);
					resetPlayerPath();
					lastPlayerPath = new ArrayList();
					selectedUnit.resetPath();
					selectedUnit.attackEnemy = null;
					selectedUnit.deselect();
					selectedUnit = null;
					selectUnit(priorityOrder[curr], false);
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

	void handleMouseClicks() {
		wasShiftRightDraggin = shiftRightDraggin;
		wasRightDraggin = rightDraggin;
		if (escapeDown) {
			if (rightDraggin) rightDragginCancelled = true;
			rightDraggin = false;
			if (shiftRightDraggin) shiftRightDragginCancelled = true;
			shiftRightDraggin = false;
		}

		mouseLeftDown = Input.GetMouseButton(0);
		if (!normalDraggin && !middleDraggin && !rightDraggin && !shiftRightDraggin) shiftDraggin = ((shiftDraggin && mouseLeftDown) || (!isOnGUI && shiftDown && Input.GetMouseButtonDown(0)));
		if (!shiftDraggin && !middleDraggin && !rightDraggin && !shiftRightDraggin) normalDraggin = ((normalDraggin && mouseLeftDown) || (!isOnGUI && !shiftDown && Input.GetMouseButtonDown(0)));
		mouseRightDown = Input.GetMouseButton(1);
		if (!normalDraggin && !middleDraggin && !rightDraggin && !shiftDraggin) shiftRightDraggin = ((shiftRightDraggin && mouseRightDown) || (!isOnGUI && shiftDown && Input.GetMouseButtonDown(1)));
		if (!shiftDraggin && !middleDraggin && !normalDraggin && !shiftRightDraggin) rightDraggin = (rightDraggin && mouseRightDown) || (!isOnGUI && !shiftDown && Input.GetMouseButtonDown(1));
		mouseMiddleDown = Input.GetMouseButton(2);
		if (!shiftDraggin && !normalDraggin && !rightDraggin && !shiftRightDraggin) middleDraggin = (middleDraggin && mouseMiddleDown) || (!isOnGUI && Input.GetMouseButtonDown(2));
		bool mouseDown = Input.GetMouseButtonDown(0);
		bool mouseUp = Input.GetMouseButtonUp(0);
		bool mouseDownRight = Input.GetMouseButtonDown(1);
		if (mouseDown) mouseDownGUI = isOnGUI;
	/*	if (mouseDown && !shiftDown && !isOnGUI && !rightDraggin) {
			//	if (!selectedUnit || (!selectedUnit.moving && !selectedUnit.attacking)) {
			if (selectedUnit && (!selectedUnit.moving && !selectedUnit.attacking)) {
				if (selectedUnit!=null && currentGrid!=null) {
					int x = (int)currentGrid.transform.localPosition.x;
					int y = (int)currentGrid.transform.localPosition.y;
				//	Player p = selectedPlayer.GetComponent<Player>();
				
			//		editingPath = true; // hoveredCharacter==null && (tiles[x,-y].canStandCurr || tiles[x,-y].canAttackCurr);//isInPlayerRadius(p, p.currentMoveDist + p.attackRange, x, -y);
				}
			}
		}*/
	//	Debug.Log("MouseDownRight: " + mouseDownRight + "  " + isOnGUI + "  " + normalDraggin);
		if (mouseDownRight && !isOnGUI && !normalDraggin) {
			if (!shiftDown) {
				if (selectedUnit) {
					resetAroundCharacter(selectedUnit);
					resetPlayerPath();
					lastPlayerPath = new ArrayList();
					selectedUnit.resetPath();
					selectedUnit.attackEnemy = null;
					selectedUnit.deselect();
				}
				Unit u;
				while (selectedUnits.Count != 0) {
					u = selectedUnits[0];
					resetAroundCharacter(u);
					u.deselect();
					selectedUnits.RemoveAt(0);
				}
				selectedUnit = hoveredCharacter;
				if (selectedUnit) {
					setAroundCharacter(selectedUnit);
					selectedUnit.setSelected();
				}
//			setTargetObjectPosition();
			}
			else {
				Unit u = hoveredCharacter;
				selectUnit(u, true);
			}
		}
		if (mouseUp && !shiftDown && !mouseDownGUI && !rightDraggin && getCurrentUnit()==selectedUnit) {
			if (selectedUnit && lastHit) {
				selectedUnit.attackEnemy = null;
				int posX = (int)lastHit.transform.localPosition.x;
				int posY = -(int)lastHit.transform.localPosition.y;
		
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
				if (lastPlayerPath.Count > 0) {
					Vector2 last = (Vector2)lastPlayerPath[lastPlayerPath.Count-1];
					if (Mathf.Abs((int)last.x - posX) + Mathf.Abs((int)last.y - posY) <= selectedUnit.attackRange) {
						selectedUnit.attackEnemy = tiles[posX,posY].getEnemy(selectedUnit);
					}
				}
			}
		//	editingPath = false;
			lastArrowPos = new Vector2(-1000, -1000);
		}
		if (normalDraggin && !mouseDownGUI && getCurrentUnit()==selectedUnit && selectedUnits.Count == 0) {		
		
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

	public void selectUnit(Unit u, bool remove) {
		if (u) {
			if (!selectedUnit) {
				selectedUnit = u;
				setAroundCharacter(u);
				u.setSelected();
			}
			else {
				if (selectedUnits.Count == 0) {
					resetPlayerPath();
					lastPlayerPath = new ArrayList();
					selectedUnit.resetPath();
					selectedUnit.attackEnemy = null;
				}
				if (selectedUnit == u) {
					if (remove) {
						selectedUnit.deselect();
						selectedUnit = null;
						if (selectedUnits.Count != 0) {
							selectedUnit = selectedUnits[0];
							selectedUnits.RemoveAt(0);
						}
						resetCharacterRange();
					}
				}
				else if (selectedUnits.Contains(u)) {
					if (remove) {
						u.deselect();
						selectedUnits.Remove(u);
						resetCharacterRange();
					}
				}
				else {
					if (u != getCurrentUnit())
						selectedUnits.Add(u);
					else {
						selectedUnits.Add(selectedUnit);
						selectedUnit = u;
					}
					setAroundCharacter(u);
					u.setSelected();
				}
			}
		}
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
	
	void handleMouseMovement() {
		
		
		var mPos = Input.mousePosition;
		mPos.z = 10.0f;
		Vector3 pos1 = mainCamera.ScreenToWorldPoint(mPos);
		if (middleDraggin || scrolled || shiftDraggin) {//  && Input.mousePosition.x < Screen.width*(1-boxWidthPerc)) {
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
	}

	void handleKeyPan() {
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
	
	
	void handleMouseSelect() {
		if ((shiftRightDraggin || rightDraggin) && ((wasShiftRightDraggin || wasRightDraggin) || !isOnGUI)) {
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
			if (!(wasRightDraggin || wasShiftRightDraggin)) {
				startSquareActual = posActual;
				startSquare = new Vector2(v3.x,v3.y);
				//	startSquareActual = startSquare;
				mouseOver = (GameObject)Instantiate(gridPrefab);
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
		else if (rightDragginCancelled || shiftRightDragginCancelled) {
			Destroy(mouseOver);
			mouseOver = null;
			if (!selectedUnit && hoveredCharacter && !rightDraggin && !shiftRightDraggin) {
				setAroundCharacter(hoveredCharacter);
			}
			setCurrentSpriteColor();
	//		Destroy(mouseOver2);
	//		mouseOver2 = null;
		}
		else if (wasRightDraggin || wasShiftRightDraggin) {
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
			Debug.Log(min + "   " + max);
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
		if (hit && !isOnGUI) {
			GameObject go = hit.collider.gameObject;
			if (go != lastHit) {
				lastHit = go;
				if (!selectedUnit) {
					if (hoveredCharacter) {
						resetAroundCharacter(hoveredCharacter, hoveredCharacter.viewDist);
					}
				}
		/*		hoveredCharacter = null;
				foreach (GameObject pGo in players) {
					Player p = pGo.GetComponent<Player>();
					if (Mathf.Floor(p.position.x) == Mathf.Floor(go.transform.localPosition.x) && Mathf.Floor(p.position.y) == Mathf.Floor(go.transform.localPosition.y)) {
					//	//Debug.Log ("Is a Player!");
						hoveredPlayer = pGo;
						if (!selectedPlayer) {
							setAroundPlayer(p, p.currentMoveDist, p.viewDist, p.attackRange);
						}
					}
				}*/
				Tile t = tiles[(int)go.transform.localPosition.x,(int)-go.transform.localPosition.y];
				hoveredCharacter = t.getCharacter();
				if (!selectedUnit && hoveredCharacter && !rightDraggin && !shiftRightDraggin) {
					setAroundCharacter(hoveredCharacter);
				}

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
						if (!(rightDraggin || shiftRightDraggin)) {
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
	//	}
		if (!did) {
			//	//Debug.Log("Nope...");
			currentSprite.color = new Color(0.40f, 0.40f, 0.40f, 0.4f);
		}
	}
	
}
