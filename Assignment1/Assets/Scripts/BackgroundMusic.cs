using UnityEngine;
using System.Collections;

/*
 * controls the music behaviour
 */ 
public class BackgroundMusic : MonoBehaviour {
	
	public AudioClip MenuMusic;
	public AudioClip ButtonMusic;
	public AudioClip CountDown;

	public AudioClip[] SoundTracks;

	private int currentTrack;
	private bool inGame = false;

	private AudioSource source;

	void Awake () {
		source = GetComponent<AudioSource>();
		DontDestroyOnLoad (gameObject);
		currentTrack = Random.Range (0, 4);
	}

	void OnLevelWasLoaded(int level){
		if (level == 0) {
			source.clip = MenuMusic;
			source.loop = true;
			source.Play();
		}
		if (level == 1) {
			source.clip = CountDown;
			source.loop = false;
			source.Play();
		}
	}

	public void ClickedButton(){
		source.clip = ButtonMusic;
		source.loop = false;
		source.Play ();
	}

	public void GameStart(){
		inGame = true;
		source.clip = SoundTracks [currentTrack];
		source.loop = false;
		source.Play ();

	}

	void Update(){
		if (inGame) {
			if(!source.isPlaying){
				currentTrack++;
				if(currentTrack == SoundTracks.Length) currentTrack =0;
				source.clip = SoundTracks [currentTrack];
				source.Play ();
			}
		}
	
	}
}
