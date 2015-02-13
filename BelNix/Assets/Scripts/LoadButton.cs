using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class LoadButton : MonoBehaviour {


	public void loadGame(GameObject textObject)
	{
		GameGUI.escapeMenuOpen = false;
		Text text = textObject.GetComponent<Text>();
		Saves.loadSave(text.text);
		Application.LoadLevel(2);
	}

}
