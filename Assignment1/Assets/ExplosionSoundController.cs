using UnityEngine;
using System.Collections;

public class ExplosionSoundController : MonoBehaviour {

	public void Detonate(){
		GameObject mainMusic = GameObject.FindGameObjectWithTag ("Music");
		mainMusic.GetComponent<AudioSource>().enabled=false;
		gameObject.SetActive (true);
		AudioSource source = GetComponent<AudioSource>();
		source.Play ();
	}

}
