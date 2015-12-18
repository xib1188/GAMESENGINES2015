using UnityEngine;
using System.Collections;

/*
 * controls the collisions with the objects
 */ 
public class CollisionController : MonoBehaviour {

	public int MAX_LIVES = 5;
	float MAX_SPELL_TIME = 10f;

	public int lives; //lives the user has
	string activeSpell ; //name of the spell if there is one active ("Shield" or "Bullet")
	float spellStartTime; //time when a spell is activated
	float explosionStartTime; //used to delay the explosion
	float EXPLOSION_DELAY = .1f;

	string lastCollisionName; //used to avoid multiple collisions with the same object
	GameObject mainCamera;
	GameObject playerCamera;
	GameObject collisionObj;

	//UI items
	public GameObject[] UIlives;
	public GameObject UIbullet;
	public GameObject UIshield;

	void Start () {
		lives = 2; //inital lives
		activeSpell = "";
		spellStartTime = -1;
		explosionStartTime = -1;
		lastCollisionName = "";
		mainCamera = GameObject.Find ("Main Camera");
		playerCamera = gameObject.transform.FindChild("PlayerCam1").gameObject;
	}

	void Update () {
		//check first for explosion
		if (explosionStartTime >= 0) { //if explosion has been activated check delay time
			if(Time.time - explosionStartTime >= EXPLOSION_DELAY){
				//obtain explosion component
				GameObject expl = GameObject.Find("Components").transform.FindChild("Explosion").gameObject;

				//put the component on the spaceship position
				expl.transform.position = gameObject.transform.position;

				//destroy objects and start explosion
				if(collisionObj != null) Destroy(collisionObj);
				GameObject.Destroy(gameObject);
				expl.GetComponent<ExplosionSoundController>().Detonate();
			}
		}
		//check for spell
		if (activeSpell != "") {
			//if there is a spell and it arrives to its timeout
			if (Time.time - spellStartTime >= MAX_SPELL_TIME) {
				//turn off UI components
				if(activeSpell == "Shield"){
					gameObject.transform.FindChild ("SphereShield").gameObject.SetActive(false);
					UIshield.SetActive(false);
				}
				//turn off UI components
				else{
					gameObject.transform.FindChild ("SphereBullet").gameObject.SetActive(false);
					//notice to Player controller script to avoid extra speed
					gameObject.GetComponent<PlayerController>().activeBullet = false;
					UIbullet.SetActive(false);
				}
				//reset values
				spellStartTime = -1;
				activeSpell = "";

			}
		}
	}

	//collisions callback
	void OnCollisionEnter(Collision col){
		//if the collision object has not been collisioned yet
		if (col.gameObject.name != lastCollisionName) {
			lastCollisionName = col.gameObject.name;
			//if heart add lives 
			if (col.gameObject.tag == "Heart") {
				lives = Mathf.Min (lives + 1, MAX_LIVES);
				UIlives[lives-1].SetActive(true);

			}//if shield active it
			else if (col.gameObject.tag == "Shield") {
				//player has not any spell
				if (activeSpell == "") {
					//set control values
					activeSpell = "Shield";
					spellStartTime = Time.time;
					//set UI for the spell
					GameObject sphere = gameObject.transform.FindChild ("SphereShield").gameObject;
					sphere.SetActive (true);
					UIshield.SetActive(true);
				}
				//player has "Bullet" spell active
				else if(activeSpell == "Bullet"){
					//set control values
					activeSpell = "Shield";
					spellStartTime = Time.time;
					//set UI for the spell and remove UI for the current spell
					gameObject.transform.FindChild ("SphereBullet").gameObject.SetActive(false);
					gameObject.transform.FindChild ("SphereShield").gameObject.SetActive(true);
					gameObject.GetComponent<PlayerController>().activeBullet = false;
					UIbullet.SetActive(false);
					UIshield.SetActive(true);
				} 	
				//player has the same spell
				else
					//update time
					spellStartTime = Time.time;

			}
			//same as shield
			else if(col.gameObject.tag == "Bullet"){
				if (activeSpell == "") {
					activeSpell = "Bullet";
					spellStartTime = Time.time;
					GameObject sphere = gameObject.transform.FindChild ("SphereBullet").gameObject;
					sphere.SetActive (true);
					UIbullet.SetActive(true);
					gameObject.GetComponent<PlayerController>().activeBullet = true;
				}else if(activeSpell == "Shield"){
					activeSpell = "Bullet";
					spellStartTime = Time.time;
					gameObject.transform.FindChild ("SphereShield").gameObject.SetActive(false);
					gameObject.transform.FindChild ("SphereBullet").gameObject.SetActive(true);
					UIshield.SetActive(false);
					UIbullet.SetActive(true);
					gameObject.GetComponent<PlayerController>().activeBullet = true;
				} 	
				else
					spellStartTime = Time.time;
			}
			//if collision with TNT box
			else if (col.gameObject.tag == "TNT") {
				//check for active spells
				if (activeSpell == "Shield") {
					activeSpell = "";
					spellStartTime = -1;
					GameObject currentSphere = gameObject.transform.FindChild ("SphereShield").gameObject;
					currentSphere.SetActive (false);
					UIshield.SetActive(false);
				}
				else if(activeSpell == ""){
					lives--;
					if (lives == 0)
						SetExplosion ();
					UIlives[lives].SetActive(false);
				}

			} 
			//if collision with Nuclear drum
			else {
				//check for spells and update lives
				if (activeSpell == "Shield") {
					lives--;
					activeSpell = "";
					spellStartTime = -1;
					GameObject currentSphere = gameObject.transform.FindChild ("SphereShield").gameObject;
					currentSphere.SetActive (false);
					UIshield.SetActive(false);
					UIlives[lives].SetActive(false);

				}else if(activeSpell == ""){
					lives -= 2;
					UIlives[lives+1].SetActive(false);
					if (lives >= 0)UIlives[lives].SetActive(false);
				}
				if (lives <= 0){
					SetExplosion ();
				}

			}
			//save the collision object
			collisionObj = col.gameObject;
			//destroy the object
			col.gameObject.GetComponent<ComponentsController>().toDestroy = true;

		}
	}

	//explosion behaviour
	void SetExplosion(){
		//set delay time
		explosionStartTime = Time.time;

		//positioning the main camera and turn it on to display the effect
		PlayerController pc = gameObject.GetComponent<PlayerController> ();
		Vector3 point = pc.GetPathPoint (5);
		//disable the player controller to stop the game movement
		pc.enabled = false;
		//turn the player camera off
		playerCamera.GetComponent<Camera> ().enabled = false;

		mainCamera.transform.position = point;
		mainCamera.transform.LookAt (transform.position);
		mainCamera.GetComponent<Camera> ().enabled = true;

	}


}
