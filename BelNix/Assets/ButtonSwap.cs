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
        GetComponent<Animator>().SetBool("CurrentlyDefault", !GetComponent<Animator>().GetBool("CurrentlyDefault"));
    }
}
