using UnityEngine;
using System.Collections;

public class ComponentsController : MonoBehaviour {

	private AudioSource source;
	[HideInInspector]
	public bool toDestroy = false;


	void Start () {
		source = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		if (toDestroy && !source.isPlaying) {
			Destroy (gameObject);
		}
	}

	void OnCollisionEnter(Collision col){
		source.Play ();
	}

}
