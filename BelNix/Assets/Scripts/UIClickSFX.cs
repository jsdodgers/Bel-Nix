using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class UIClickSFX : MonoBehaviour, IPointerDownHandler  {

	// Use this for initialization
	void Start () {
//        audioManager = AudioManager.getAudioManager();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnPointerDown(PointerEventData e)
    {
        AudioManager.playSFXClip(SFXClip.UIClick);
    }
}
