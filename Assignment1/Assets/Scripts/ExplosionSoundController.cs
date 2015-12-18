using UnityEngine;
using System.Collections;
/*
 * scrips added to the explosion component to control its behaviour
 */ 
public class ExplosionSoundController : MonoBehaviour {

	private bool detonated = false;
	private float MAX_TIME = 3;
	private float time = -1;
	public int score;

	public void Detonate(){
		//stop game music
		GameObject mainMusic = GameObject.FindGameObjectWithTag ("Music");	
		mainMusic.GetComponent<AudioSource>().enabled=false;
		//activate the explosion visual and sound effect
		gameObject.SetActive (true);
		AudioSource source = GetComponent<AudioSource>();
		source.Play ();
		//set delay to game over scene
		time = Time.time;
		GameObject game = GameObject.Find ("Game");
		score = game.GetComponent<GameController>().score;
		detonated = true;
	}

	void Update(){
		if (detonated && (Time.time - time >= MAX_TIME)) {
			//create an object to pass the score throught the scenes
			GameObject scoreObject = new GameObject();
			ScoreScript ss = scoreObject.AddComponent<ScoreScript>();
			ss.score = score;
			//load game over scene
			Application.LoadLevel (3);
		}
	}

}
