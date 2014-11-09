using UnityEngine;
using System.Collections;

public enum ClipName {
	CrushingSwing,
	CrushingHit
}

public class AudioBank : MonoBehaviour {

	
	AudioClip crushingSwing;
	AudioClip crushingHit;

	// Use this for initialization
	void Start () {
		crushingSwing = Resources.Load<AudioClip>("Audio/SFX/Combat-CrushSwing");
		crushingHit = Resources.Load<AudioClip>("Audio/SFX/Combat-CrushHit");
	//	crushingHit = Resources.Load<AudioClip>("Audio/SFX/Scratchy Record");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void playClipAtPoint(ClipName clipName, Vector3 position) {
		AudioClip clip = null;
		float volume = 1.0f;
		switch (clipName) {
		case ClipName.CrushingSwing:
			clip = crushingSwing;
			break;
		case ClipName.CrushingHit:
			clip = crushingHit;
			volume = 1.0f;
			break;
		default:
			break;
		}
		if (clip != null)
			AudioSource.PlayClipAtPoint(clip, position, volume);
	}
}
