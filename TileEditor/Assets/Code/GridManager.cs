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

	int gridX = 0;
	int gridY = 0;

	public bool displayH;
	public bool displayI;
	public int displayHTime = 0;

	public float red = 0.0f;
	public float green = 0.0f;
	public float blue = 0.0f;

	public bool passable = true;
	public bool standable = true;

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
		handleMouseClicks();
		handleKeys();
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
		mouseLeftDown = Input.GetMouseButton(0);
		mouseRightDown = Input.GetMouseButton(1);
		mouseMiddleDown = Input.GetMouseButton(2);
	}

	void handleKeys() {
		shiftDown = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		altDown = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
		controlDown = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
		hDown = Input.GetKey(KeyCode.H);
		iDown = Input.GetKey(KeyCode.I);
	}

	void handleKeyActions() {
		if (shiftDown && altDown && controlDown) {
			bool wasI = displayI;
			displayH = hDown && !displayI;
			displayI = iDown && !displayH;
			if (!wasI && displayI) {
				loadNewBackgroundFile();
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
		if ((mouseRightDown || mouseMiddleDown)  && Input.mousePosition.x < Screen.width*(1-boxWidthPerc)) {
			Vector3 pos = mainCamera.WorldToScreenPoint(cameraTransform.position);
			pos.x -= mouseX * mouseFactor;
			pos.y -= mouseY * mouseFactor;
			cameraTransform.position = mainCamera.ScreenToWorldPoint(pos);
//				cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x - mouseX * mouseFactor,cameraTransform.localPosition.y - mouseY * mouseFactor,cameraTransform.localPosition.z);
		}
	}

	void handleMouseSelect() {
		if (mouseLeftDown && Input.mousePosition.x < Screen.width*(1-boxWidthPerc)) {
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
			if (hit) {
				GameObject go = hit.collider.gameObject;
				if (!shiftDown) {
					Tile t = go.GetComponent<TileHolder>().tile;
					t.setColor(red, green, blue, 0.4f);
					t.passable = passable;
					t.standable = standable;
		//		SpriteRenderer sR = go.GetComponent<SpriteRenderer>();
		//		sR.color = new Color(red/255.0f,green/255.0f,blue/255.0f,0.4f);
				}
				else {
					Tile t = go.GetComponent<TileHolder>().tile;
					red = t.red;
					green = t.green;
					blue = t.blue;
					passable = t.passable;
					standable = t.standable;
				}
			}
		}
	}

	public void printGrid() {
		string str = gridX + ";" + gridY;
	
		foreach (Tile t in gridsArray) {
		//	foreach (Tile t in tA) {
				str = str + ";";
				str = str + t.stringValue();
		//	}
		}
		Debug.Log(str);
		string path = EditorUtility.OpenFolderPanel("Select Folder","../Files/Maps/Tile Maps","");
		int currAdd = 0;
		string fileName = path + "/" + imageFileName + (currAdd>0?"" +currAdd:"") + ".txt";
		while (File.Exists(fileName)) {
			currAdd++;
			fileName = path + "/" + imageFileName + (currAdd>0?""+currAdd:"") + ".txt";
		}
//		if (File.Exists(path + "/" + fileName))
//		{
//			Debug.Log(fileName+" already exists.");
//			return;
//		}

		StreamWriter sr = File.CreateText(fileName);
	//	sr.WriteLine ("This is my file.");
	//	sr.WriteLine ("I can write ints {0} or floats {1}, and so on.",
	//	              1, 4.2);
		sr.WriteLine(str);
		sr.Close();
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
			if (int.Parse(tiles[0])==gridX && int.Parse(tiles[1])==gridY) {
				Debug.Log("Works!");
				for (int n=2;n<tiles.Length;n++) {
					int x = Tile.xForTile(tiles[n]);
					int y = Tile.yForTile(tiles[n]);
					Tile t = gridsArray[x,y];
					t.parseTile(tiles[n]);
				}
			}
			else {
				Debug.Log ("Grid size not compatable: (" + int.Parse(tiles[0]) + "," + int.Parse(tiles[1]) + ") and (" + gridX + "," + gridY + ")");
			}
//			Debug.Log(text);
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
		if (path.Length != 0) {
			string[] paths = path.Split(new char[]{'/'});
			string path1 = paths[paths.Length-1];
			string[] pathA = path1.Split(new char[]{'.'});
			imageFileName = pathA[0];
			float currX = sprend.transform.localScale.x / spr.texture.width;
			float currY = sprend.transform.localScale.y / spr.texture.height;
			WWW www = new WWW("file:///" + path);
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
