using UnityEngine;
using System.Collections;

public class BackgroundMusic : MonoBehaviour {

	// Use this for initialization
	public AudioClip MenuMusic;
	public AudioClip ButtonMusic;
	public AudioClip CountDown;

	public AudioClip[] SoundTracks;

	private int currentTrack = 0;
	private bool inGame = false;

	private AudioSource source;

	void Awake () {
		source = GetComponent<AudioSource>();
		DontDestroyOnLoad (gameObject);
	}

	void OnLevelWasLoaded(int level){
		if (level == 0) {
			source.clip = MenuMusic;
			source.loop = true;
			source.Play();
		}
		if (level == 1) {
			Debug.Log(source.isPlaying);
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
				currentTrack = (currentTrack++)%SoundTracks.Length;
				source.clip = SoundTracks [currentTrack];
				source.Play ();
			}
		}
	
	}
}
