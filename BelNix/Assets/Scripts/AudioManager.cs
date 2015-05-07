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
		if (phazingMusic != null)  {
			music = phazingMusic.GetComponent<AudioSource>();
			anim = phazingMusic.GetComponent<Animator>();
			cml = phazingMusic.GetComponent<CleanMusicLoop>();
		}
		if (constantMusic != null)  {
			cMusic = constantMusic.GetComponent<AudioSource>();
		}
        primaryAudioManager = this;
        audioBank = MapGenerator.mg.audioBank;
	}

    public void playAudioClip(SFXClip clipName, float volume = 1.0f)
    {
        audioBank.playAudioClip(clipName, volume);
    }

    public void playAudioClip(SFXClip clipName, Vector3 position, float volume = 1.0f)
    {
        audioBank.playClipAtPoint(clipName, position, volume);
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
