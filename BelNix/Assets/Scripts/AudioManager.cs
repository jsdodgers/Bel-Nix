using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SFXClip {UISpark, TurretShoot, Footstep1, Footstep2, Footstep3, Footstep4, BloodSplash, BluntImpact, ComposureDamage, ComposureBreak, ButtonUp, ButtonDown}

public class AudioManager : MonoBehaviour  {
	public GameObject phazingMusic;
	public GameObject constantMusic;
	AudioSource music;
	AudioSource cMusic;
	Animator anim;
	CleanMusicLoop cml;
    Dictionary<SFXClip, AudioClip> clipList;
    Queue<GameObject> SFXPlayers;
    GameObject SFXContainerTemplate;
    private const int MAX_SFXPLAYER_COUNT = 20;
    private static AudioManager primaryAudioManager;
    
	[SerializeField] private float transitionTime;
	// Use this for initialization
	void Start ()  {
        primaryAudioManager = this;
		if (phazingMusic != null)  {
			music = phazingMusic.GetComponent<AudioSource>();
			anim = phazingMusic.GetComponent<Animator>();
			cml = phazingMusic.GetComponent<CleanMusicLoop>();
		}
		if (constantMusic != null)  {
			cMusic = constantMusic.GetComponent<AudioSource>();
		}
        clipList = new Dictionary<SFXClip, AudioClip>();
        SFXPlayers = new Queue<GameObject>();
        SFXContainerTemplate = new GameObject("SFX Player", typeof(AudioSource));
        SFXContainerTemplate.transform.SetParent(transform);
        SFXContainerTemplate.GetComponent<AudioSource>().playOnAwake = false;
        loadSFX();
	}

    private void loadSFX()
    {
        importAudioClip(SFXClip.Footstep1, "footstep1");
        importAudioClip(SFXClip.Footstep2, "footstep2");
        importAudioClip(SFXClip.Footstep3, "footstep3");
        importAudioClip(SFXClip.Footstep4, "footstep4");
        importAudioClip(SFXClip.TurretShoot, "turret-shoot");
        importAudioClip(SFXClip.UISpark, "zapv1");
        importAudioClip(SFXClip.BloodSplash, "blood-splash");
        importAudioClip(SFXClip.BluntImpact, "Combat-CrushHit");
        importAudioClip(SFXClip.ComposureDamage, "composure-hit");
        importAudioClip(SFXClip.ComposureBreak, "composure-break");
        importAudioClip(SFXClip.ButtonDown, "Button_DOWN");
        importAudioClip(SFXClip.ButtonUp, "Button_UP");
    }
    public void importAudioClip(SFXClip key, string filename)
    {
        clipList.Add(key, Resources.Load<AudioClip>("Audio/SFX/" + filename));
    }

    public void playAudioClip(SFXClip clipName, float volume)
    {
        AudioClip clip;
        clipList.TryGetValue(clipName, out clip);
        if (clip == null)
            return;

        GameObject newSFXPlayer = (GameObject) Instantiate(SFXContainerTemplate);
        newSFXPlayer.transform.SetParent(transform, false);

        if (SFXPlayers.Count >= MAX_SFXPLAYER_COUNT)
            Destroy(SFXPlayers.Dequeue());
        SFXPlayers.Enqueue(newSFXPlayer);

        AudioSource SFXPlayer = newSFXPlayer.GetComponent<AudioSource>();
        SFXPlayer.clip = clip;
        SFXPlayer.volume = volume;
        SFXPlayer.Play();
    }

    public static AudioManager getAudioManager()
    {
        return primaryAudioManager;
    }

	public void invokeFadeInMusic()  {
		if(phazingMusic != null)  {
			if(music.time >= cml.loopEnd - transitionTime)
			 {
				Invoke("fadeInMusic", transitionTime + 0.25f);
			}
			else
			 {
				fadeInMusic();
			}
		}
	}

	public void invokeFadeOutMusic()  {
		if(phazingMusic != null)  {
			if(music.time >= cml.loopEnd - transitionTime)
			 {
				Invoke("fadeOutMusic", transitionTime + 0.25f);
			}
			else
			 {
				fadeOutMusic();
			}
		}
	}

	void fadeOutMusic()  {
		anim.SetBool("isPlaying", false);
	}
	
	void fadeInMusic()  {
		anim.SetBool("isPlaying", true);
	}
}
