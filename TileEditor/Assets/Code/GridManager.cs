using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;

public class GridManager : MonoBehaviour {

	Transform cameraTransform;
	Camera mainCamera;
	SpriteRenderer sprend;
	Sprite spr;
	GameObject grids;
//	ArrayList gridsArray = new ArrayList();
	Tile[,] gridsArray;
	GameObject lines;
	ArrayList linesArray;
	GameObject gridPrefab;
	GameObject mouseOver;
	GameObject mouseOver2;

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

	bool shiftDraggin = false;
	bool middleDraggin = false;
	bool normalDraggin = false;
	bool rightDraggin = false;
	bool wasShiftDraggin = false;
	bool shiftDragginCancelled = false;
	Vector2 startSquare;
	Vector2 lastPos;
	Vector3 startSquareActual;

	int gridX = 0;
	int gridY = 0;

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
	
	public int trigger;
	public int action; 


	public string imageFileName = "";
	
	// Use this for initialization
	void Start () {
		GameObject mainCameraObj = GameObject.Find("Main Camera");
		cameraTransform = mainCameraObj.transform;
		mainCamera = mainCameraObj.GetComponent<Camera>();
		cameraOriginalSize = mainCamera.orthographicSize;
		sprend = (SpriteRenderer)transform.GetComponent("SpriteRenderer");
		spr = sprend.sprite;
		grids = GameObject.Find("Grids");
		gridsArray = new Tile[gridX,gridY];
		gridPrefab = (GameObject)Resources.Load("Sprite/Square_70");
		lines = GameObject.Find("Lines");
	//	linesArray = new ArrayList();
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
		if (Input.mousePosition.x < Screen.width*(1-boxWidthPerc)) {
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
		if (!normalDraggin && !middleDraggin && !rightDraggin && !shiftDragginCancelled) shiftDraggin = ((shiftDraggin && mouseLeftDown) || (shiftDown && Input.GetMouseButtonDown(0)));
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
		escapeDown = Input.GetKey(KeyCode.Escape);
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
		if (middleDraggin  && Input.mousePosition.x < Screen.width*(1-boxWidthPerc)) {
			Vector3 pos = mainCamera.WorldToScreenPoint(cameraTransform.position);
			pos.x -= mouseX * mouseFactor;
			pos.y -= mouseY * mouseFactor;
			cameraTransform.position = mainCamera.ScreenToWorldPoint(pos);
//				cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x - mouseX * mouseFactor,cameraTransform.localPosition.y - mouseY * mouseFactor,cameraTransform.localPosition.z);
		}
	}

	void handleMouseSelect() {
		if (shiftDraggin && (wasShiftDraggin || Input.mousePosition.x < Screen.width*(1-boxWidthPerc))) {
			Vector3 v3 = Input.mousePosition;
			v3.z = 10.0f;
			v3 = mainCamera.ScreenToWorldPoint(v3);
			Vector3 posActual = new Vector3(v3.x,v3.y,v3.z);
			v3.x += gridX/2.0f;
			v3.y += gridY/2.0f;
			v3.y = gridY - v3.y;
			v3.x = Mathf.Floor(v3.x);
			v3.y = Mathf.Floor(v3.y);
			Debug.Log(v3);
			if (!wasShiftDraggin) {
				startSquareActual = posActual;
				startSquare = new Vector2(v3.x,v3.y);
			//	startSquareActual = startSquare;
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


			lastPos = posActual;
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
		else if (shiftDragginCancelled) {
			Destroy(mouseOver);
			mouseOver = null;
			Destroy(mouseOver2);
			mouseOver2 = null;
		}
		else if (wasShiftDraggin) {
			Vector3 v3 = Input.mousePosition;
			v3.z = 10.0f;
			v3 = mainCamera.ScreenToWorldPoint(v3);
			v3.x += gridX/2.0f;
			v3.y += gridY/2.0f;
			v3.y = gridY - v3.y;
			v3.x = Mathf.Floor(v3.x);
			v3.y = Mathf.Floor(v3.y);
			Debug.Log("Start: " + startSquare);
			Debug.Log("End: " + v3);
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
		if ((normalDraggin || rightDraggin) && Input.mousePosition.x < Screen.width*(1-boxWidthPerc)) {
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
//					passable = t.passable;
					standable = t.standable;
					passableUp = t.passableUp;
					passableRight = t.passableRight;
					passableDown = t.passableDown;
					passableLeft = t.passableLeft;
					trigger = t.trigger;
					action = t.action;
				}
			}
		}
	}

	public void setProperties(Tile t) {
		t.setColor(red, green, blue, 0.4f);
		//					t.passable = passable;
		t.standable = standable;
		t.passableUp = passableUp;
		t.passableRight = passableRight;
		t.passableDown = passableDown;
		t.passableLeft = passableLeft;
		t.trigger = trigger;
		t.action = action;
	}

	public void printGrid() {
		string str = imageFileName + ";" + gridX + ";" + gridY;
	
		foreach (Tile t in gridsArray) {
		//	foreach (Tile t in tA) {
				str = str + ";";
				str = str + t.stringValue();
		//	}
		}
		Debug.Log(str);
		int currAdd = 0;
		string fileDirectory = "../Files/Maps/Tile Maps";
		string fileName = fileDirectory + "/" + imageFileName + (currAdd>0?"" +currAdd:"") + ".txt";
		while (File.Exists(fileName)) {
			currAdd++;
			fileName = fileDirectory + "/" + imageFileName + (currAdd>0?""+currAdd:"") + ".txt";
		}
//		string path = EditorUtility.OpenFolderPanel("Select Folder",fileDirectory,"pppppppppp");
		string path = EditorUtility.SaveFilePanel("Save Tile Map","../Files/Maps/Tile Maps",imageFileName + (currAdd>0?"" +currAdd:""),"txt");
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
		string path = EditorUtility.OpenFilePanel(
			"Overwrite with jpg",
			"../Files/Maps/Tile Maps",
			"txt");
		//			Sprite spr = ((SpriteRenderer)transform.GetComponent(SpriteRenderer)).sprite;
		//			Sprite sprite = new Sprite();
		//			sprend.sprite = sprite;
		if (path.Length != 0) {
			WWW www = new WWW("file:///" + path);
			yield return www;
			string text = www.text;
			string[] tiles = text.Split(";".ToCharArray());
			if (int.Parse(tiles[1])==gridX && int.Parse(tiles[2])==gridY) {
				Debug.Log("Works!");
				parseTiles(tiles);
			}
			else {
				Debug.Log(Application.absoluteURL);
			
				string path1 = "../Files/Maps/Images/" + tiles[0] + ".jpg";
				string path2 = Path.GetFullPath(path1);
				Debug.Log(path2);
				if (File.Exists(path2)) {
					Debug.Log("file Exists!");
					loadImage(path2);
					parseTiles(tiles);
				}
				else {
					Debug.Log ("Grid size not compatable: (" + int.Parse(tiles[0]) + "," + int.Parse(tiles[1]) + ") and (" + gridX + "," + gridY + ")");
				}
			}
//			Debug.Log(text);
		}
	}

	void parseTiles(string[] tiles) {
		for (int n=3;n<tiles.Length;n++) {
			int x = Tile.xForTile(tiles[n]);
			int y = Tile.yForTile(tiles[n]);
			Tile t = gridsArray[x,y];
			t.parseTile(tiles[n]);
		}
	}

	void clearGrid() {
		foreach (Tile t in gridsArray) {
		//	foreach (Tile t in tA) {
				TileHolder th = t.tileGameObject.GetComponent<TileHolder>();
				th.tile = null;
				Destroy(t.tileGameObject);
		//	}
		}
		gridsArray = null;
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
		Debug.Log("x: " + x + ", minX: " + minX);
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
		string path = EditorUtility.OpenFilePanel(
			"Overwrite with jpg",
			"../Files/Maps/Images",
			"jpg");
		Debug.Log(path);
		//			Sprite spr = ((SpriteRenderer)transform.GetComponent(SpriteRenderer)).sprite;
		//			Sprite sprite = new Sprite();
		//			sprend.sprite = sprite;
		loadImage(path);
	}

	public void loadImage(string path) {
		int max = 0;
		if (path.Length != 0) {
			string[] paths = path.Split(new char[]{'/'});
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
			loadGrid(scaleX, scaleY);
			Debug.Log(www.texture.width + "  " + www.texture.height + "    " + sprend.transform.localScale.x + "  " + sprend.transform.localScale.y);
		}
	}


	void OnApplicationQuit() {
		
		SpriteRenderer sprend = (SpriteRenderer)transform.GetComponent("SpriteRenderer");
		Sprite spr = sprend.sprite;
//		WWW www = new WWW("file://" + "/Users/Justin/Documents/UCI/ICS 169AB/Bel Nix/Images/Maps/none.jpg");
		Debug.Log(Application.dataPath);
		WWW www = new WWW("file://" + Application.dataPath + "/Resources/Images/70.png");
		www.LoadImageIntoTexture(spr.texture);
	}

	/*
	public static bool GetImageSize(Texture2D asset, out int width, out int height) {
		if (asset != null) {
			string assetPath = AssetDatabase.GetAssetPath(asset);
			TextureImporter importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
			
			if (importer != null) {
				object[] args = new object[2] { 0, 0 };
				MethodInfo mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
				mi.Invoke(importer, args);
				
				width = (int)args[0];
				height = (int)args[1];
				
				return true;
			}
		}
		
		height = width = 0;
		return false;
	}*/
	
}
