﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MapGenerator : MonoBehaviour {
	
	int gridSize = 70;
	
	
	GameObject lastHit;
	GameObject map;
	Transform mapTransform;
	Transform cameraTransform;
	Camera mainCamera;
	SpriteRenderer sprend;
	Sprite spr;

	GameObject arrowStraightPrefab;
	GameObject arrowCurvePrefab;
	GameObject arrowPointPrefab;
	GameObject playerPrefab;
	GameObject selectedPlayer;
	GameObject hoveredPlayer;
	ArrayList players;
	GameObject grids;
	GameObject lines;
	GameObject path;
	GameObject[,] gridArray;
	ArrayList lastPlayerPath;

	Vector2 lastArrowPos = new Vector2(-1, -1);

	
	GameObject currentGrid;
	GameObject mouseDownGrid;
	SpriteRenderer currentSprite;
	Color currentSpriteColor;
	
	GameObject gridPrefab;
	
	bool mouseLeftDown;
	bool mouseRightDown;
	bool mouseMiddleDown;
	
	bool shiftDown;
	bool altDown;
	bool controlDown;
	bool spaceDown;
	bool escapeDown;
	
	bool shiftDraggin = false;
	bool middleDraggin = false;
	bool normalDraggin = false;
	bool rightDraggin = false;
	bool scrolled = false;
	
	Vector3 lastPos = new Vector3(0.0f, 0.0f, 0.0f);
	
	int actualWidth = 0;
	int actualHeight = 0;
	
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
		
		players = new ArrayList();
		playerPrefab = (GameObject)Resources.Load("Characters/Jackie/JackiePrefab");
		arrowStraightPrefab = (GameObject)Resources.Load("Materials/Arrow/ArrowStraight");
		arrowCurvePrefab = (GameObject)Resources.Load("Materials/Arrow/ArrowCurve");
		arrowPointPrefab = (GameObject)Resources.Load("Materials/Arrow/ArrowPoint");
		Vector3[] positions = new Vector3[] {new Vector3(20, -36, 0), new Vector3(10, -36, 0)};
		for (int n=0;n<positions.Length;n++) {
			Vector3 pos = positions[n];
			GameObject player = GameObject.Instantiate(playerPrefab) as GameObject;
			player.transform.parent = mapTransform;
			Player p = player.GetComponent<Player>();
			p.mapGenerator = this;
			p.setPosition(pos);
			players.Add(player);
		}
		lines = mapTransform.FindChild("Lines").gameObject;
		grids = mapTransform.FindChild("Grid").gameObject;
		path = mapTransform.Find("Path").gameObject;
		sprend = map.GetComponent<SpriteRenderer>();
		spr = sprend.sprite;
		Debug.Log("Start()");
		int width = spr.texture.width;
		int height = spr.texture.height;
		actualWidth = width / gridSize;
		actualHeight = height / gridSize;
		Camera.main.orthographicSize = Mathf.Min(Mathf.Max(actualWidth/3, actualHeight/2) + 2, 10.0f);
		Vector3 newPos = Camera.main.transform.position;
		newPos.x = ((float)actualWidth) / 2.0f;
		newPos.y = -((float)actualHeight)/ 2.0f;
		Camera.main.transform.position = newPos;
		Debug.Log("width: " + width + ", height: " + height);
		Debug.Log("actualWidth: " + actualWidth + ", actualHeight: " + actualHeight);
		Debug.Log("newPos: " + newPos);
		Debug.Log("End");
		//		GameObject lrO = new GameObject();
		
		
		//		LineRenderer lr = lrO.AddComponent<LineRenderer>();
		//		lr.SetColors(Color.black,Color.black);
		//		lr.SetPosition(0, new Vector3(0.0f, -actualHeight/2, 0.0f));
		//		lr.SetPosition(1, new Vector3(actualWidth + 10, -actualHeight/2, 0.0f));
		//		lr.material = Resources.Load("Materials/LineMaterial",typeof(Material)) as Material;
		
		for (int n=0;n<=actualHeight;n++) {
			createLineRenderer(-0.025f, actualWidth + 0.025f, -n, -n);
		}
		for (int n=0;n<=actualWidth;n++) {
			createLineRenderer(n, n, 0.025f, -actualHeight - 0.025f);
		}
		loadGrid(actualWidth, actualHeight);
		//		setAroundPlayer(currentMoveDist, viewDist);
	}
	
	void createLineRenderer(float xStart, float xEnd, float yStart, float yEnd) {
		GameObject lrO = GameObject.Instantiate(Resources.Load("Materials/LineRenderer",typeof(GameObject)) as GameObject) as GameObject;
		LineRenderer lr = lrO.GetComponent<LineRenderer>();
		lr.SetPosition(0, new Vector3(xStart, yStart, 0.0f));
		lr.SetPosition(1, new Vector3(xEnd, yEnd, 0.0f));
		lrO.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
		lrO.transform.parent = lines.transform;
	}
	
	// Update is called once per frame
	void Update () {
		handleInput();
	}

	void resetPlayerPath() {
		if (lastPlayerPath!=null) {
//			foreach (Vector2 v in lastPlayerPath) {
//			foreach (GameObject p in path.transform.) {
			for (int n=path.transform.childCount-1; n >= 0;n--) {
				Transform t = path.transform.GetChild(n);
				GameObject go = t.gameObject;
				t.parent = null;
				Destroy(go);
			/*	GameObject g = gridArray[(int)v.x, (int)v.y];
				SpriteRenderer sr = g.GetComponent<SpriteRenderer>();
				Color c = new Color(0.0f, 0.0f, 1.0f, 0.4f);
				if (sr!=currentSprite) {
					sr.color = c;
				}
				else {
					currentSpriteColor = c;
				}*/
			}
		}
	}

	void setPlayerPath(ArrayList path1) {
//		Debug.Log("Set Player Path");
//		bool first = true;
		for (int n=1;n<path1.Count;n++) {
			Vector2 v = (Vector2)path1[n];
			Vector2 v0 = (Vector2)path1[n-1];
//		foreach (Vector2 v in path1) {
//			if (first) {
//				first = false;
//				continue;
//			}


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
					Debug.Log("xDif1: " + xDif1 + " yDif1: " + yDif1 + " xDif2: " + xDif2 + " yDif2: " + yDif2);
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
			go.renderer.sortingOrder = 2;
		//	= GameObject.Instantiate(arrowStraightPrefab) as GameObject;
			go.transform.parent = path.transform;
			go.transform.localPosition = new Vector3(v.x + 0.5f, -v.y - 0.5f, 0.0f);

		//	Debug.Log(v);
			/*
			GameObject g = gridArray[(int)v.x, (int)v.y];
			SpriteRenderer sr = g.GetComponent<SpriteRenderer>();
			Color c = new Color(0.0f, 1.0f, 1.0f, 0.4f);
			if (sr!=currentSprite) {
				sr.color = c;
			}
			else {
				currentSpriteColor = c;
				sr.color = c;
			}*/
		}
	}
	
	void resetAroundPlayer(Player player1, int view) {
		for (int x = (int)Mathf.Max(player1.position.x - view,0); x < (int)Mathf.Min(player1.position.x + 1 + view, actualWidth); x++) {
			for (int y = (int)Mathf.Max(-player1.position.y - view,0); y < (int)Mathf.Min(-player1.position.y + 1.0f + view, actualHeight); y ++) {
				GameObject go = gridArray[x,y];
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
	}
	
	void setAroundPlayer(Player player1, int radius, int view, int attackRange) {
		int type = 4;
		for (int x = (int)Mathf.Max(player1.position.x - view,0); x < (int)Mathf.Min(player1.position.x + 1 + view, actualWidth); x++) {
			for (int y = (int)Mathf.Max(-player1.transform.localPosition.y - view,0); y < (int)Mathf.Min(-player1.position.y + 1.0f + view, actualHeight); y++) {
				GameObject go = gridArray[x,y];
				SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
				if (isInPlayerRadius(player1,radius,x,y)) {
					Color c = new Color(0.0f, 0.0f, 1.0f, 0.4f);
					if (sr!=currentSprite) {
						sr.color = c;
					}
					else {
						currentSpriteColor = c;
					}
				}
				else if (isInPlayerRadius(player1,radius + attackRange,x,y)) {
					Color c = new Color(1.0f,0.0f,0.0f, 0.4f);
					if (sr!=currentSprite) {
						sr.color = c;
					}
					else {
						currentSpriteColor = c;
					}
				}
				else if (type == 0 || (type==1 && isInPlayerRadius(player1,view,x,y)) || (type==2 &&  isInCircularRadius(player1,view,x,y)) || (type==3 &&  isInCircularRadius2(player1,view,x,y)) || (type==4 &&  isInCircularRadius3(player1,view,x,y))) {
					Color c = Color.clear;
					if (sr!=currentSprite) {
						sr.color = c;
					}
					else {
						currentSpriteColor = c;
					}
				}
			}
		}
	}
	
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
	}
	
	int totalMoveDist(Player player1, int x, int y) {
		return (int)Mathf.Abs(player1.position.x - x) + (int)Mathf.Abs(-player1.position.y  - y);
	}
	void loadGrid(int x, int y) {
		//	clearGrid();
		//		foreach (GameObject g in linesArray) {
		//			Destroy (g);
		//		}
		gridArray = new GameObject[x,y];
		//	gridsArray = new Tile[gridX,gridY];
		//	linesArray = new ArrayList();
		//	float minX = -x/2.0f + 0.5f;
		//	float minY = y/2.0f - 0.5f;
		//	float maxX = x/2.0f - 0.5f;
		//	float maxY = -y/2.0f + 0.5f;
		//	Debug.Log("x: " + x + ", minX: " + minX);
		int xcur = 0;
		for (float n=0;n<x;n++) {
			int ycur = 0;
			for (float m=0;m<y;m++) {
				GameObject go = (GameObject)Instantiate(gridPrefab);
				gridArray[(int)n,(int)m] = go;
				//	Tile t = new Tile(go,xcur,ycur,255.0f,255.0f,255.0f,0.4f);
				//	TileHolder th = go.AddComponent<TileHolder>();
				//	th.tile = t;
				//	gridsArray[xcur,ycur] = t;
				go.transform.position = new Vector3(n,m*-1,0);
				go.transform.parent = grids.transform;
				SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
				//	sr.sprite.border = new Vector4(1.0f,1.0f,1.0f,1.0f);
				//		float red = ((float)n)/((float)x);
				//		float green = ((float)m)/((float)y);
				//		float blue = 1.0f - Mathf.Max(red,green);
				//		blue = 2.0f - red - green;
				//		sr.color = new Color(red,green,blue,0.4f);
				sr.color = Color.clear;
				//				sr.color = Color.black;
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
			lr.SetVertexCount(2);ect
			lr.SetPosition(0,new Vor3(n,minY,0));
			lr.SetPosition(1,new Vector3(n,maxY,0));
			lr.material = new Material(Shader.Find("Unlit/Texture"));
			lr.SetColors(Color.black,Color.black);
			lr.SetWidth(71.0f/70.0f,71.0f/70.0f);
		}
		xx++;*/
	}
	
	
	void handleInput() {
		handleMouseScrollWheel();
		handleKeys();
		handleMouseClicks();
		handleMouseMovement();
		handleMouseSelect();
		//	handleKeyActions();
	}
	
	
	
	
	void handleMouseScrollWheel() {
		//	if (Input.mousePosition.x < Screen.width*(1-boxWidthPerc)) {
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
	}
	
	bool lastPlayerPathContains(Vector2 v) {
		if (lastPlayerPath==null) return false;
		foreach (Vector2 v2 in lastPlayerPath) {
			if (v2.x == v.x && v2.y == v.y) return true;
		}
		return false;
	}

	void handleMouseClicks() {
		mouseLeftDown = Input.GetMouseButton(0);
		if (!normalDraggin && !middleDraggin && !rightDraggin) shiftDraggin = ((shiftDraggin && mouseLeftDown) || (shiftDown && Input.GetMouseButtonDown(0)));
		if (!shiftDraggin && !middleDraggin && !rightDraggin) normalDraggin = ((normalDraggin && mouseLeftDown) || (!shiftDown && Input.GetMouseButtonDown(0)));
		mouseRightDown = Input.GetMouseButton(1);
		if (!shiftDraggin && !middleDraggin && !normalDraggin) rightDraggin = (rightDraggin && mouseRightDown) || Input.GetMouseButtonDown(1);
		mouseMiddleDown = Input.GetMouseButton(2);
		if (!shiftDraggin && !normalDraggin && !rightDraggin) middleDraggin = (middleDraggin && mouseMiddleDown) || Input.GetMouseButtonDown(2);
		bool mouseDown = Input.GetMouseButtonDown(0);
		bool mouseUp = Input.GetMouseButtonUp(0);
		if (mouseUp) {
			if (!selectedPlayer) {
				selectedPlayer = hoveredPlayer;
			}
			lastArrowPos = new Vector2(-1000, -1000);
		}
		if (normalDraggin) {
			/*
			int x = (int)currentGrid.transform.localPosition.x;
			int y = (int)currentGrid.transform.localPosition.y;
			if (isInPlayerRadius(currentMoveDist,x,-y)) {
				if (player) {
					resetAroundPlayer(viewDist);
					currentMoveDist -= totalMoveDist(x,-y);
					if (currentMoveDist == 0) currentMoveDist = 5;
					Vector3 pos = player.transform.localPosition;
					pos.x = x + .5f;
					pos.y = y - .5f;
					player.transform.localPosition = pos;
					setAroundPlayer(currentMoveDist, viewDist);
				}
			}
			*/
			int x = -1;
			int y = 1;
			if (currentGrid!=null) {
				x = (int)currentGrid.transform.localPosition.x;
				y = (int)currentGrid.transform.localPosition.y;
			}
			Vector2 v = new Vector2(x, -y);


			if (selectedPlayer && !Player.vectorsEqual(v, lastArrowPos) && x>=0 && -y>=0) {
				Player p = selectedPlayer.GetComponent<Player>();

				resetPlayerPath();
				if (!lastPlayerPathContains(v)) {
					lastPlayerPath = p.addPathTo(v);
				}
				else {
					lastPlayerPath = p.removeFromPathTo(v);
				}
				string s = "Path: ";
				foreach (Vector2 v1 in lastPlayerPath) {
					s += v1.ToString() + ", ";
				}
				Debug.Log(s);
				setPlayerPath(lastPlayerPath);
				lastArrowPos = v;

				/*
				if (isInPlayerRadius(p,p.currentMoveDist,x,-y)) {
					resetAroundPlayer(p,p.viewDist);
//					p.currentMoveDist -= totalMoveDist(p,x,-y);
					Vector3 pos = p.position;
					pos.x = x;
					pos.y = y;
					Debug.Log(pos);
					p.setMoveDist(p.currentMoveDist - totalMoveDist(p,x,-y));
					p.setPosition(pos);
					if (p.currentMoveDist == 0) {
						hoveredPlayer = selectedPlayer;
						selectedPlayer = null;
						resetMoveDistances();
						setAroundPlayer(p, p.currentMoveDist, p.viewDist, p.attackRange);
						//						resetAroundPlayer(p, p.viewDist);
					}
					else {
						setAroundPlayer(p, p.currentMoveDist, p.viewDist, p.attackRange);
					}
					//		if (p.currentMoveDist == 0) {
					//		}
				}
				else if (isInPlayerRadius(p, p.currentMoveDist + p.attackRange, x, -y)) {
					if (hoveredPlayer) {
						resetAroundPlayer(p,p.viewDist);
						selectedPlayer = hoveredPlayer;
						Player p2 = selectedPlayer.GetComponent<Player>();
						setAroundPlayer(p2, p2.currentMoveDist, p2.viewDist, p2.attackRange);
					}
				}
				else {
					selectedPlayer = hoveredPlayer;
					resetAroundPlayer(p,p.viewDist);
					Player p2 = selectedPlayer.GetComponent<Player>();
					setAroundPlayer(p2, p2.currentMoveDist, p2.viewDist, p2.attackRange);
				}
			*/
			}
		}
	}
	
	
	void resetMoveDistances() {
		bool reset = true;
		foreach (GameObject player in players) {
			Player p = player.GetComponent<Player>();
			if (p.currentMoveDist!=0) reset = false;
		}
		foreach (GameObject player in players) {
			Player p = player.GetComponent<Player>();
//			if (reset) p.currentMoveDist = p.maxMoveDist;
			if (reset) p.setMoveDist(p.maxMoveDist);
		}
		
	}
	
	void handleMouseMovement() {
		
		
		var mPos = Input.mousePosition;
		mPos.z = 10.0f;
		Vector3 pos1 = mainCamera.ScreenToWorldPoint(mPos);
		if (middleDraggin || scrolled) {//  && Input.mousePosition.x < Screen.width*(1-boxWidthPerc)) {
			//= mainCamera.WorldToScreenPoint(cameraTransform.position);
			if (!Input.GetMouseButtonDown(0) || scrolled) {
				float xDiff = pos1.x - lastPos.x;
				float yDiff = pos1.y - lastPos.y;
				Vector3 pos = mapTransform.position;
				pos.x += xDiff;
				pos.y += yDiff;
				mapTransform.position = pos;
			}
		}
		lastPos = pos1;
		if (shiftDown && selectedPlayer) {
			float midSlope = (pos1.y - selectedPlayer.transform.position.y)/(pos1.x - selectedPlayer.transform.position.x);
			float rotation = Mathf.Atan(midSlope) + Mathf.PI/2.0f;
			Vector3 rot1 = selectedPlayer.transform.eulerAngles;
			if (pos1.x > selectedPlayer.transform.position.x) {
				rotation += Mathf.PI;
			}
			rot1.z = rotation * 180 / Mathf.PI;
			selectedPlayer.transform.eulerAngles = rot1;
		}
	}
	
	
	void handleMouseSelect() {
		
		
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 100.0f, 1<<9);
		//		Physics2D.Ray
		if (hit) {
			GameObject go = hit.collider.gameObject;
			if (go != lastHit) {
				lastHit = go;
			//	Debug.Log("Not Equal!");
				if (!selectedPlayer) {
					if (hoveredPlayer) {
						Player p = hoveredPlayer.GetComponent<Player>();
						resetAroundPlayer(p, p.viewDist);
					}
				}
				hoveredPlayer = null;
				foreach (GameObject pGo in players) {
					Player p = pGo.GetComponent<Player>();
					if (Mathf.Floor(p.position.x) == Mathf.Floor(go.transform.localPosition.x) && Mathf.Floor(p.position.y) == Mathf.Floor(go.transform.localPosition.y)) {
					//	Debug.Log ("Is a Player!");
						hoveredPlayer = pGo;
						if (!selectedPlayer) {
							setAroundPlayer(p, p.currentMoveDist, p.viewDist, p.attackRange);
						}
					}
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
						bool did = false;
						if (selectedPlayer) {
							Player p = selectedPlayer.GetComponent<Player>();
						//	Debug.Log(" c: " + lastPlayerPath[0] + " d: " + new Vector2((int)currentGrid.transform.localPosition.x, (int)-currentGrid.transform.localPosition.y));
					//		if (Player.exists(lastPlayerPath, new Vector2((int)currentGrid.transform.localPosition.x, (int)-currentGrid.transform.localPosition.y))) {
						//		Debug.Log("Yup!");
					//			spr.color = new Color(0.0f, 1.0f, 1.0f, 0.4f);
					//			did = true;
					//		}
							if (isInPlayerRadius(p, p.currentMoveDist,(int)currentGrid.transform.localPosition.x,(int)-currentGrid.transform.localPosition.y)) {
								spr.color = new Color(0.0f, 0.0f, 0.50f, 0.4f);
								did = true;
							}
							else if (isInPlayerRadius(p, p.currentMoveDist + p.attackRange,(int)currentGrid.transform.localPosition.x,(int)-currentGrid.transform.localPosition.y)) {
								did = true;
								spr.color = new Color(0.50f, 0.0f, 0.0f, 0.4f);
							}
						}
						if (!did) {
						//	Debug.Log("Nope...");
							spr.color = new Color(0.40f, 0.40f, 0.40f, 0.4f);
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
		
	}
	
}