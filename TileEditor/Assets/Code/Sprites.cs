using UnityEngine;
using System.Collections;
using UnityEditor;

public class Sprites : MonoBehaviour {

	Transform cameraTransform;
	Camera mainCamera;
	SpriteRenderer sprend;
	Sprite spr;
	GameObject grids;
	ArrayList gridsArray = new ArrayList();
	GameObject lines;
	ArrayList linesArray;
	GameObject gridPrefab;

	float cameraOriginalSize;
	float boxWidthPerc = .2f;
	
	bool mouseLeftDown;
	bool mouseRightDown;
	bool mouseMiddleDown;

	bool shiftDown;
	Vector2 scrollPosition = new Vector2(0.0f,0.0f);

	float red = 0.0f;
	float green = 0.0f;
	float blue = 0.0f;

	
	// Use this for initialization
	void Start () {
		GameObject mainCameraObj = GameObject.Find("Main Camera");
		cameraTransform = mainCameraObj.transform;
		mainCamera = mainCameraObj.GetComponent<Camera>();
		cameraOriginalSize = mainCamera.orthographicSize;
		sprend = (SpriteRenderer)transform.GetComponent("SpriteRenderer");
		spr = sprend.sprite;
		grids = GameObject.Find("Grids");
		gridsArray = new ArrayList();
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
		if (mouseLeftDown && !shiftDown && Input.mousePosition.x < Screen.width*(1-boxWidthPerc)) {
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
			if (hit) {
				GameObject go = hit.collider.gameObject;
				SpriteRenderer sR = go.GetComponent<SpriteRenderer>();
				sR.color = new Color(red/255.0f,green/255.0f,blue/255.0f,0.4f);
			}
		}
	}

	void loadGrid(float x, float y) {
		foreach (GameObject g in gridsArray) {
			Destroy(g);
		}
//		foreach (GameObject g in linesArray) {
//			Destroy (g);
//		}
		gridsArray = new ArrayList();
		linesArray = new ArrayList();
		float minX = -x/2.0f + 0.5f;
		float minY = y/2.0f - 0.5f;
		float maxX = x/2.0f - 0.5f;
		float maxY = -y/2.0f + 0.5f;
		Debug.Log("x: " + x + ", minX: " + minX);
		for (float n=minX;n<=maxX;n++) {
			for (float m=minY;m>=maxY;m--) {
				GameObject go = (GameObject)Instantiate(gridPrefab);
				gridsArray.Add(go);
				go.transform.position = new Vector3(n,m,0);
				go.transform.parent = grids.transform;
				SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
			//	sr.sprite.border = new Vector4(1.0f,1.0f,1.0f,1.0f);
				sr.color = new Color(1.0f,1.0f,1.0f,0.2f);
			}
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


	void OnGUI() {
		GUISkin skinCopy = (GUISkin)Instantiate(GUI.skin);

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
			string path = EditorUtility.OpenFilePanel(
				"Overwrite with jpg",
				"../Images/Maps",
				"jpg");
			Debug.Log(path);

//			Sprite spr = ((SpriteRenderer)transform.GetComponent(SpriteRenderer)).sprite;
//			Sprite sprite = new Sprite();
//			sprend.sprite = sprite;
			if (path.Length != 0) {
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
		float textFieldHeight = 20.0f;
		float textLabelWidth = 15.0f;
		float textLabelX = loadButtonX;
		float textFieldWidth = loadButtonWidth - textLabelWidth;
		float textFieldX = textLabelX + textLabelWidth;
		float redY = loadButtonY + loadButtonHeight + 5.0f;
		float greenY = redY + textFieldHeight + 5.0f;
		float blueY = greenY + textFieldHeight + 5.0f;
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
		GUI.EndScrollView();

		GUI.skin = skinCopy;
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
