using UnityEngine;
using System.Collections;

/*
 * script used to pass through the score from game scene to Game over scene
 */
public class ScoreScript : MonoBehaviour {
	public int score;
	void Awake(){
		//object created on game scene and needed on gameOver scene
		DontDestroyOnLoad (gameObject);
	}
	void OnLevelWasLoaded(int level){
		if (level == 3) {
			//if game over scene is loaded show the score
			GUIText gui = GameObject.Find ("ScoreText").GetComponent<GUIText> ();
			gui.text = "SCORE: " + score;
		} else if (level == 0)
			//if menu screen is loaded destroy this object
			GameObject.Destroy (gameObject);
	}
}