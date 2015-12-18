using UnityEngine;
using System.Collections;
/*
 * buttons behaviour script
 * used to load scenes
 */
public class StartGame : MonoBehaviour {

	public GameObject loadingImage;


	public AudioClip onPressClip;
	public AudioClip onMouseEnterClip;

	void Awake(){

	}


	public void LoadScene(int level){
		GameObject musicObject = GameObject.FindGameObjectWithTag ("Music");
		if (!musicObject.GetComponent<AudioSource> ().enabled)
			musicObject.GetComponent<AudioSource> ().enabled = true;
		GameObject sec = musicObject.transform.FindChild("SecondaryMusic").transform.gameObject;
		sec.GetComponent<AudioSource> ().clip = onPressClip;
		sec.GetComponent<AudioSource> ().Play ();
		if (level == 1) {
			//if start game, loading screen active
			loadingImage.SetActive (true);
			Application.LoadLevel (level);
		} else if (level == 0) {
			//if menu is loaded need to destroy music object to avoid duplicates
			Destroy (musicObject);
			Application.LoadLevel (level);
		}else if (level == 4) Application.Quit();
		else Application.LoadLevel (level);;
	}

	public void OnPointerEnter(){
		GameObject musicObject = GameObject.FindGameObjectWithTag ("Music");
		GameObject sec = musicObject.transform.FindChild("SecondaryMusic").transform.gameObject;
		sec.GetComponent<AudioSource> ().clip = onMouseEnterClip;
		sec.GetComponent<AudioSource> ().Play ();

	}
}
