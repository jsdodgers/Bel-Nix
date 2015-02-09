using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {
	public GameObject phazingMusic;
	public GameObject constantMusic;
	AudioSource music;
	AudioSource cMusic;
	Animator anim;
	CleanMusicLoop cml;

	[SerializeField] private float transitionTime;
	// Use this for initialization
	void Start () {
		music = phazingMusic.GetComponent<AudioSource>();
		anim = phazingMusic.GetComponent<Animator>();
		cml = phazingMusic.GetComponent<CleanMusicLoop>();
		cMusic = constantMusic.GetComponent<AudioSource>();
	}

	public void invokeFadeInMusic()
	{
		if(music.time >= cml.loopEnd - transitionTime)
		{
			Invoke("fadeInMusic", transitionTime + 0.25f);
		}
		else
		{
			fadeInMusic();
		}
	}

	public void invokeFadeOutMusic()
	{
		if(music.time >= cml.loopEnd - transitionTime)
		{
			Invoke("fadeOutMusic", transitionTime + 0.25f);
		}
		else
		{
			fadeOutMusic();
		}
	}

	void fadeOutMusic()
	{
		anim.SetBool("isPlaying", false);
	}
	
	void fadeInMusic()
	{
		anim.SetBool("isPlaying", true);
	}
}
