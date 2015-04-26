using UnityEngine;
using System.Collections;

public class ButtonClick : MonoBehaviour {
    AudioManager audioManager;
    private bool currentlyClicking = false;

	// Use this for initialization
	void Start () {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void buttonDown()
    {
        if(!currentlyClicking)
        {
            audioManager.playAudioClip(SFXClip.ButtonDown, 0.8f);
            currentlyClicking = true;
            Debug.Log("Button Down!");
        }
    }

    public void buttonUp()
    {
        audioManager.playAudioClip(SFXClip.ButtonUp, 0.8f);
        currentlyClicking = false;
        Debug.Log("Button Up!");
    }
}
