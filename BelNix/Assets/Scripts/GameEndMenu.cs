using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameEndMenu : MonoBehaviour  {

	public GameObject winText;
	public GameObject loseText;
	public Text rewardText;
	public GameObject baseButton;
	public GameObject loadGameButton;
	public GameObject quitToMenuButton;
	public GameObject quitGameButton;

	public void setValues(int c, int exp, bool won)  {
		winText.SetActive(won);
		loseText.SetActive(!won);
		rewardText.gameObject.SetActive(won);
		baseButton.SetActive(won);
		loadGameButton.SetActive(!won);
		quitToMenuButton.SetActive(!won);
		rewardText.text = "<color=#c3c331>+" + c + UnitGUI.getSmallCapsString("c", 14) + "</color>\n+" + exp + UnitGUI.getSmallCapsString("exp", 14);

	}

	// Use this for initialization
	void Start ()  {
	
	}
	
	// Update is called once per frame
	void Update ()  {
	
	}
}
