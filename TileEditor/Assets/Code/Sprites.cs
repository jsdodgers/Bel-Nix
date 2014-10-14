using UnityEngine;
using System.Collections;
using UnityEditor;

public class Sprites : MonoBehaviour {

	Transform cameraTransform;
	Camera mainCamera;
	SpriteRenderer sprend;
	Sprite spr;
	GameObject grids;
	ArrayList gridsArray;
	GameObject gridPrefab;

	float cameraOriginalSize;

	bool mouseLeftDown;
	bool mouseRightDown;
	bool mouseMiddleDown;

	float xx = 0.0f;

	
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
	}
	
	// Update is called once per frame
	void Update () {
		handleMouseInput();

	}


	void handleMouseInput() {
		handleMouseScrollWheel();
		handleMouseClicks();
		handleMouseMovement();
	}

	void handleMouseScrollWheel() {
		float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
		float cameraSize = mainCamera.orthographicSize;
		float maxCameraSize = Mathf.Max(sprend.transform.localScale.x,sprend.transform.localScale.y) * cameraOriginalSize * 6.0f/5.0f;
		float minCameraSize = 1.0f * cameraOriginalSize / 5.0f;
		cameraSize = Mathf.Clamp(cameraSize - mouseWheel,minCameraSize,maxCameraSize);
		mainCamera.orthographicSize = cameraSize;
	}

	void handleMouseClicks() {
		mouseLeftDown = Input.GetMouseButton(0);
		mouseRightDown = Input.GetMouseButton(1);
		mouseMiddleDown = Input.GetMouseButton(2);
	}

	void handleMouseMovement() {
//		float mouseFactor = mainCamera.orthographicSize/5.0f;
		float mouseFactor = 0.3f;
		float mouseX = Input.GetAxis("Mouse X");
		float mouseY = Input.GetAxis("Mouse Y");
		mouseFactor = 18.0f;
		if (mouseRightDown || mouseMiddleDown) {
			Vector3 pos = mainCamera.WorldToScreenPoint(cameraTransform.position);
			pos.x -= mouseX * mouseFactor;
			pos.y -= mouseY * mouseFactor;
			cameraTransform.position = mainCamera.ScreenToWorldPoint(pos);
//				cameraTransform.localPosition = new Vector3(cameraTransform.localPosition.x - mouseX * mouseFactor,cameraTransform.localPosition.y - mouseY * mouseFactor,cameraTransform.localPosition.z);
		}
	}

	void loadGrid(float x, float y) {
		foreach (GameObject g in gridsArray) {
			Destroy(g);
		}
		gridsArray = new ArrayList();
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
		xx++;
	}


	void OnGUI() {
		if (GUI.Button(new Rect(10,10,100,50),"Button Test")) {
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
		//		Texture2D texture = new Texture2D(512,512);
		//		Sprite sp2 = Resources.Load<Sprite>("Images/none");
		//		sp2 = (Sprite)Instantiate(sp2);
				float currX = sprend.transform.localScale.x / spr.texture.width;
				float currY = sprend.transform.localScale.y / spr.texture.height;
	//			float currX = 1.0f / sp2.texture.width;
	//			float currY = 1.0f / sp2.texture.height;
		//		Debug.Log(sp2.texture.width + "   " + sp2.texture.height);
				WWW www = new WWW("file:///" + path);
//				Sprite sp = Resources.Load<Sprite>("file:///" + path);
		//		sprend.sprite = sp;
		///		spr.texture.Resize(www.texture.width,www.texture.height);
			//	spr.rect = new Rect(0,0,www.texture.width,www.texture.height);
				www.LoadImageIntoTexture(spr.texture);
//				sprite.texture = texture;
		//		Debug.Log(sprite);
//				spr.texture.LoadImage(texture.EncodeToPNG());
//				int width = 512;
//				int height = 512;
//				sprend.sprite = sp2;
				float scaleX = currX * spr.texture.width;
				float scaleY = currY * spr.texture.height;
				sprend.transform.localScale = new Vector3(scaleX, scaleY, sprend.transform.localScale.z);
				mainCamera.orthographicSize = Mathf.Max(sprend.transform.localScale.x,sprend.transform.localScale.y) * cameraOriginalSize;
				cameraTransform.localPosition = new Vector3(0,0,-10);
				loadGrid(scaleX, scaleY);
//				sprend.transform.localScale.x = currX * spr.texture.width;
//				sprend.transform.localScale.y = currY * spr.texture.height;
				Debug.Log(www.texture.width + "  " + www.texture.height + "    " + sprend.transform.localScale.x + "  " + sprend.transform.localScale.y);
			//	GetImageSize(texture,&width,&height);
		//		spr.texture.Resize(width,height);
			}
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
