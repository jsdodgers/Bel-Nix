using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour  {
	public GameObject phazingMusic;
	public GameObject constantMusic;
	AudioSource music;
	AudioSource cMusic;
	Animator anim;
	CleanMusicLoop cml;

    AudioBank audioBank;
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
        if (MapGenerator.mg != null)
            audioBank = MapGenerator.mg.audioBank;
        else
        {
            gameObject.AddComponent("AudioBank");
            audioBank = GetComponent<AudioBank>();
        }
	}

    public void playAudioClip(SFXClip clipName, float volume = 1.0f)
    {
        if (audioBank == null)
        {
            gameObject.AddComponent("AudioBank");
            audioBank = GetComponent<AudioBank>();
        }
        audioBank.playAudioClip(clipName, volume);
    }

    public void playAudioClip(SFXClip clipName, Vector3 position, float volume = 1.0f)
    {
        if (audioBank == null)
        {
            gameObject.AddComponent("AudioBank");
            audioBank = GetComponent<AudioBank>();
        }
        audioBank.playClipAtPoint(clipName, position, volume);
    }

    public static AudioManager getAudioManager()
    {
        if (primaryAudioManager == null)
        {
            return ((GameObject)Instantiate(new GameObject("AudioManager", typeof(AudioManager)))).GetComponent<AudioManager>();
        }
        else
            return primaryAudioManager;
    }
    public static void playSFXClip(SFXClip clipName, float volume = 1.0f)
    {
        primaryAudioManager.playAudioClip(clipName, volume);
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
