using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour  {
	public GameObject phazingMusic;
	public GameObject constantMusic;
	AudioSource music;
	AudioSource cMusic;
	Animator anim;
	CleanMusicLoop cml;
    Dictionary<string, AudioClip> clipList;
    Queue<GameObject> SFXPlayers;
    GameObject SFXContainerTemplate;
    private const int MAX_SFXPLAYER_COUNT = 20;
    
	[SerializeField] private float transitionTime;
	// Use this for initialization
	void Start ()  {
		if (phazingMusic != null)  {
			music = phazingMusic.GetComponent<AudioSource>();
			anim = phazingMusic.GetComponent<Animator>();
			cml = phazingMusic.GetComponent<CleanMusicLoop>();
		}
		if (constantMusic != null)  {
			cMusic = constantMusic.GetComponent<AudioSource>();
		}
        clipList = new Dictionary<string, AudioClip>();
        SFXPlayers = new Queue<GameObject>();
        SFXContainerTemplate = new GameObject("SFX Player", typeof(AudioSource));
        SFXContainerTemplate.transform.SetParent(transform);
        SFXContainerTemplate.GetComponent<AudioSource>().playOnAwake = false;
        loadSFX();
	}

    private void loadSFX()
    {
        for (int i = 0; i < 4; i++)
            importAudioClip("footstep" + i, "footstep" + i);
        importAudioClip("turret-shoot", "turret-shoot");
        importAudioClip("zap", "zapv1");
        importAudioClip("blood-splash", "blood-splash");
        importAudioClip("blunt-impact", "Combat-CrushHit");
    }
    public void importAudioClip(string key, string filename)
    {
        clipList.Add(key, Resources.Load<AudioClip>("Audio/SFX/" + filename));
    }

    public void playAudioClip(string clipName, float volume)
    {
        AudioClip clip;
        clipList.TryGetValue(clipName, out clip);
        if (clip == null)
            return;

        GameObject newSFXPlayer = (GameObject) Instantiate(SFXContainerTemplate);
        newSFXPlayer.transform.SetParent(transform, false);
        AudioSource SFXPlayer = newSFXPlayer.GetComponent<AudioSource>();
        if (SFXPlayers.Count >= MAX_SFXPLAYER_COUNT)
            Destroy(SFXPlayers.Dequeue());
        SFXPlayers.Enqueue(newSFXPlayer);


        SFXPlayer.clip = clip;
        SFXPlayer.volume = volume;
        SFXPlayer.Play();
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
