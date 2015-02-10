using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ButtonSwap : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //GetComponent<Button>().image.sprite = defaultSprite;
        //currentSprite = defaultSprite;
	}

    public void toggleSprite()
    {
		setSprite(!GetComponent<Animator>().GetBool("CurrentlyDefault"));
    }
	public void setSprite(bool currentlyDefault) {
		Debug.Log(currentlyDefault);
		GetComponent<Animator>().SetBool("CurrentlyDefault", currentlyDefault);

	}
}
