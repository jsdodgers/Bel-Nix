using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ClipName  {
	CrushingSwing,
	CrushingHit
}

public enum SFXClip { 
    UISpark, 
    TurretShoot, 
    Footstep1, Footstep2, Footstep3, Footstep4, 
    BloodSplash, 
    BluntImpact, CrushingSwing, CrushingHit,
    ComposureDamage, ComposureBreak, 
    ButtonUp, ButtonDown }

public class AudioBank : MonoBehaviour  {

    Dictionary<SFXClip, AudioClip> clipList;
    Queue<GameObject> SFXPlayers;
    GameObject SFXContainerTemplate;
    private const int MAX_SFXPLAYER_COUNT = 20;

	// Use this for initialization
	void Start ()  {
        clipList = new Dictionary<SFXClip, AudioClip>();
        SFXPlayers = new Queue<GameObject>();
        SFXContainerTemplate = new GameObject("SFX Player", typeof(AudioSource));
        SFXContainerTemplate.transform.SetParent(transform);
        SFXContainerTemplate.GetComponent<AudioSource>().playOnAwake = false;
        loadSFX();
	}

    private void loadSFX()
    {
        importAudioClip(SFXClip.Footstep1,      "footstep1");
        importAudioClip(SFXClip.Footstep2,      "footstep2");
        importAudioClip(SFXClip.Footstep3,      "footstep3");
        importAudioClip(SFXClip.Footstep4,      "footstep4");
        importAudioClip(SFXClip.TurretShoot,    "turret-shoot");
        importAudioClip(SFXClip.UISpark,        "zapv1");
        importAudioClip(SFXClip.BloodSplash,    "blood-splash");
        importAudioClip(SFXClip.BluntImpact,    "Combat-CrushHit");
        importAudioClip(SFXClip.CrushingSwing,  "Combat-CrushSwing");
        importAudioClip(SFXClip.CrushingHit,    "Combat-CrushHit");
        importAudioClip(SFXClip.ComposureDamage,"composure-hit");
        importAudioClip(SFXClip.ComposureBreak, "composure-break");
        importAudioClip(SFXClip.ButtonDown,     "Button_DOWN");
        importAudioClip(SFXClip.ButtonUp,       "Button_UP");
    }

    public void importAudioClip(SFXClip key, string filename)
    {
        clipList.Add(key, Resources.Load<AudioClip>("Audio/SFX/" + filename));
    }
	
	// Update is called once per frame
	void Update ()  {
	
	}

	public void playClipAtPoint(SFXClip clipName, Vector3 position, float volume = 1.0f)  {
		AudioClip clip = null;
        clipList.TryGetValue(clipName, out clip);
        if (clip == null)
            return;
		AudioSource.PlayClipAtPoint(clip, position, volume);
	}

    public void playAudioClip(SFXClip clipName, float volume = 1.0f)
    {
        playClipAtPoint(clipName, Camera.main.transform.position, volume);
    }
}
