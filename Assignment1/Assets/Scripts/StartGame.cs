using UnityEngine;
using System.Collections;

public class StartGame : MonoBehaviour {

	public GameObject LoadingImage;

	public GameObject MusicObject;


	void Awake(){

	}


	public void LoadScene(int level){
		MusicObject.GetComponent<BackgroundMusic> ().ClickedButton ();
		LoadingImage.SetActive (true);
		Application.LoadLevel (level);
	}
}
