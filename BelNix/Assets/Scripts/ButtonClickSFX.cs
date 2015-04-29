using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonClickSFX : MonoBehaviour, IPointerDownHandler, IPointerUpHandler  {

    [SerializeField, Range(0, 1)]
    private float volume;

    private AudioManager audioManager;
    private bool currentlyClicking = false;


	// Use this for initialization
	void Start () {
        audioManager = AudioManager.getAudioManager();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnPointerDown(PointerEventData e)
    {
        //buttonDown();
        audioManager.playAudioClip(SFXClip.ButtonDown, volume);
    }
    public void OnPointerUp(PointerEventData e)
    {
        //buttonUp();
        audioManager.playAudioClip(SFXClip.ButtonUp, volume);
    }


    public void buttonDown()
    {
        if(!currentlyClicking)
        {
            audioManager.playAudioClip(SFXClip.ButtonDown, 1.0f);
            currentlyClicking = true;
            Debug.Log("Button Down!");
        }
    }

    public void buttonUp()
    {
        audioManager.playAudioClip(SFXClip.ButtonUp, 1.0f);
        currentlyClicking = false;
        Debug.Log("Button Up!");
    }
}
