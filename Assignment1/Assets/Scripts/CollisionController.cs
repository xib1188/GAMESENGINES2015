using UnityEngine;
using System.Collections;

public class CollisionController : MonoBehaviour {
	int MAX_LIVES = 4;
	int MAX_BULLETS = 3;
	float MAX_SPELL_TIME = 10f;
	int bullets;
	int lives;
	string activeSpell;
	float spellStartTime;
	float explosionStartTime;
	float EXPLOSION_DELAY = .1f;

	string lastCollisionName;
	GameObject mainCamera;
	GameObject playerCamera;
	GameObject collisionObj;

	void Start () {
		lives = 2;
		activeSpell = "";
		bullets = 0;
		spellStartTime = -1;
		explosionStartTime = -1;
		lastCollisionName = "";
		mainCamera = GameObject.Find ("Main Camera");
		playerCamera = gameObject.transform.FindChild("PlayerCam1").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		if (explosionStartTime >= 0) {
			//GameObject.Destroy(collisionObj);
			if(Time.time - explosionStartTime >= EXPLOSION_DELAY){
				GameObject expl = GameObject.Find("Components").transform.FindChild("Explosion").gameObject;
				expl.transform.position = gameObject.transform.position;
				GameObject.Destroy(gameObject);
				expl.GetComponent<ExplosionSoundController>().Detonate();
				//expl.SetActive (true);
			}
		}
		if (activeSpell == "Shield") {
			if (Time.time - spellStartTime >= MAX_SPELL_TIME) {
				gameObject.transform.FindChild ("SphereShield").gameObject.SetActive(false);
				spellStartTime = -1;
				activeSpell = "";
			}
		}
		else if (activeSpell == "Bullet") {
			//
		}
	}

	void OnCollisionEnter(Collision col){
		if (col.gameObject.name != lastCollisionName) {
			lastCollisionName = col.gameObject.name;
			if (col.gameObject.tag == "Heart") {
				lives = Mathf.Min (lives + 1, MAX_LIVES);
				col.gameObject.GetComponent<ComponentsController>().toDestroy = true;
			}else if (col.gameObject.tag == "Shield") {
				if (activeSpell == "") {
					activeSpell = "Shield";
					spellStartTime = Time.time;
					GameObject sphere = gameObject.transform.FindChild ("SphereShield").gameObject;
					sphere.SetActive (true);
				} else if (activeSpell == "Shield")
					spellStartTime = Time.time;
				else {
					activeSpell = "Shield";
					bullets = 0;
					GameObject currentSphere = gameObject.transform.FindChild ("SphereBullet").gameObject;
					currentSphere.SetActive (false);
					spellStartTime = Time.time;
					GameObject sphere = gameObject.transform.FindChild ("SphereShield").gameObject;
					sphere.SetActive (true);
				}
				col.gameObject.GetComponent<ComponentsController>().toDestroy = true;
			} else if(col.gameObject.tag == "Bullet"){
				if (activeSpell == "") {
					activeSpell = "Bullet";
					bullets = MAX_BULLETS;
					GameObject sphere = gameObject.transform.FindChild ("SphereBullet").gameObject;
					sphere.SetActive (true);
				} else if (activeSpell == "Bullet")
					bullets = MAX_BULLETS;
				else {
					activeSpell = "Bullet";
					bullets = MAX_BULLETS;
					spellStartTime = -1;
					GameObject currentSphere = gameObject.transform.FindChild ("SphereShield").gameObject;
					currentSphere.SetActive (false);
					GameObject sphere = gameObject.transform.FindChild ("SphereBullet").gameObject;
					sphere.SetActive (true);
				}
				col.gameObject.GetComponent<ComponentsController>().toDestroy = true;
			} else if (col.gameObject.tag == "TNT") {
				if (activeSpell == "Shield") {
					activeSpell = "";
					spellStartTime = -1;
					GameObject currentSphere = gameObject.transform.FindChild ("SphereShield").gameObject;
					currentSphere.SetActive (false);
				} else {
					lives--;
					if (lives == 0)
						SetExplosion ();
				}
				Destroy(col.gameObject);
			} else {
				if (activeSpell == "Shield") {
					lives--;
					activeSpell = "";
					spellStartTime = -1;
					GameObject currentSphere = gameObject.transform.FindChild ("SphereShield").gameObject;
					currentSphere.SetActive (false);
				} else
					lives -= 2;

				if (lives <= 0)
					SetExplosion ();
				Destroy(col.gameObject);
			}
		}
	}
	
	void SetExplosion(){
		explosionStartTime = Time.time;

		PlayerController pc = gameObject.GetComponent<PlayerController> ();
		Vector3 point = pc.GetPathPoint (5);
		pc.enabled = false;

		playerCamera.GetComponent<Camera> ().enabled = false;

		mainCamera.transform.position = point;
		mainCamera.transform.LookAt (transform.position);

		mainCamera.GetComponent<Camera> ().enabled = true;

	}
		
		
	
}
