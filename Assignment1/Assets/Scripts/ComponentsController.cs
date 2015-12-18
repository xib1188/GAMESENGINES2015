using UnityEngine;
using System.Collections;

/*
 * 
 * script added to each object in the game and controll its behaviour
 */ 
public class ComponentsController : MonoBehaviour {

	private AudioSource source;
	[HideInInspector]
	public bool toDestroy = false;

	//control the number of collision (to avoid multiple collisions)
	private bool first;

	void Start () {
		source = GetComponent<AudioSource> ();
		first = true;
	}
	

	void Update () {
		if (toDestroy && !source.isPlaying) {
			Destroy (gameObject);
		}
	}

	//collision callback
	void OnCollisionEnter(Collision col){
		if (first) {
			//if not a heart object
			if (gameObject.tag != "Heart") {
				//if TNT or nuclear activate the explosion efect
				if (gameObject.tag == "TNT" || gameObject.tag == "Nuclear") {
					transform.FindChild ("Explosion(Clone)").gameObject.SetActive (true);
				}
				source.Play ();
			} 
			else {
				//if heart updates lives if required
				CollisionController player = GameObject.FindGameObjectWithTag ("Player").GetComponent<CollisionController> ();
				int lives = player.lives;
				if (lives < player.MAX_LIVES) {
					source.Play ();
				}
			}
			first = false;
		}

	}

}
