using UnityEngine;
using System.Collections;
using UnityEditor;

public class Sprites : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
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
			SpriteRenderer sprend = (SpriteRenderer)transform.GetComponent("SpriteRenderer");
//			Sprite spr = ((SpriteRenderer)transform.GetComponent(SpriteRenderer)).sprite;
			Sprite spr = sprend.sprite;
//			Sprite sprite = new Sprite();
//			sprend.sprite = sprite;
			if (path.Length != 0) {
				Texture2D texture = new Texture2D(512,512);
				WWW www = new WWW("file:///" + path);
				www.LoadImageIntoTexture(texture);
//				sprite.texture = texture;
		//		Debug.Log(sprite);
				spr.texture.LoadImage(texture.EncodeToPNG());
				int width = 512;
				int height = 512;
			//	GetImageSize(texture,&width,&height);
		//		spr.texture.Resize(width,height);
			}
		}
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
