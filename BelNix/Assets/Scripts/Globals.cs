using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CharacterInfo;

public class Globals : MonoBehaviour {
	public List<GameObject> squadMembers = new List<GameObject>();
	public List<Character> squadCharacterSheets = new List<Character>();

	// Use this for initialization
	void Start()
	{

	}
	
	// Update is called once per frame
	void Update()
	{
	
	}

	public IEnumerator loadSave(string saveName)
	{
		string path = Application.dataPath + "/Saves/" + saveName;
		WWW www = new WWW("file://" + path);
		yield return www;
		string text = www.text;
		string[] characterSheetArray = text.Split(';');
		//squadCharacterSheets.Add
	}
}
