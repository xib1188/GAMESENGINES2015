using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GamesEngines;

/*
 * 
 * control the movement and behaviour of the spaceship
 */
public class PlayerController : MonoBehaviour {

	private GameController gm;
	private GameObject camera;

	/*
	 * two ring spaces (tunnels), when the player is inside one, the game controller
	 * may make changes in the other
	 */
	public Ring[] ringSpace1;
	public Ring[] ringSpace2;

	/*tunnels characteristics*/
	private float tunnelsSideSize;
	private int tunnelsNSides;
	private int tunnelsLenght;

	[HideInInspector]
	public bool activeBullet;

	private float speedInc;
	//player speed
	private float speed;
	//player current track inside the tunnel
	private int track;
	//player current ring inside the tunnel
	private Ring currentRing;
	//current used ring space
	private int currentTunnelNumber;
	//index of the current ring
	private int currentRingNumber;
	//turn slerp control var
	private float turnInc;
	//indicates if the player is changing betwen tracks
	private bool turning;
	//track index the player comes from
	private int lastTrack;

	//transform variables
	private Quaternion rotation;
	private Vector3 pointToLook;
	private Vector3 translate;

	//path control
	private float distance;
	private float inc;

	private Vector3 nextPathPoint;

	//don't touch!!! scale model
	private int scaleRatio = 200;


	public Vector3 GetPathPoint(int n){
		/*return the point position on the path of the (current_point + n) point*/
		/*used by collision controller to place the camara on explosion*/
		int diff;
		int tn;
		if(currentRingNumber+n >= ringSpace1.Length){
			diff = (currentRingNumber+n-ringSpace1.Length);
			tn =  (currentTunnelNumber + 1) % 2;
		}
		else{
			diff = currentRingNumber+n;
			tn = currentTunnelNumber;
		}
		if (tn == 0) return ringSpace1 [diff].StartCenter;
		return ringSpace2 [diff].StartCenter;
	}


	public int CurrentTunnelNumber{
		get{ return currentTunnelNumber;}
	}

	void Awake(){
		//stop running 'til advice (game controller)
		this.enabled = false;
	}

	//set up values
	public void Inicialize(Ring[] l1, Ring[] l2, float size, float apothem, GameController gm){
		/*function called by tha game controller to init and start the game*/
		ringSpace1 = l1;	
		ringSpace2 = l2;

		this.distance = distance;
		this.speed = size;
		this.gm = gm;
		tunnelsSideSize = size;

		currentTunnelNumber = 0;
		currentRingNumber = 0;
		tunnelsNSides = ringSpace1 [0].Nsides;
		track = tunnelsNSides / 2;
		turnInc = 0;
		turning = false;
		activeBullet = false;
		currentRing = ringSpace1 [currentRingNumber];
		nextPathPoint = currentRing.SideEndCenter(track);
		//player (ship) init
		gameObject.transform.position = currentRing.SideStartCenter (track);
		gameObject.transform.rotation = currentRing.Rotation;
		gameObject.transform.localScale = new Vector3 (tunnelsSideSize*0.5f/scaleRatio,tunnelsSideSize*0.5f/scaleRatio,tunnelsSideSize*0.5f/scaleRatio);
		//camera init
		camera = gameObject.transform.FindChild("PlayerCam1").gameObject;
		camera.transform.localPosition = new Vector3 (0, +0.8f*(scaleRatio/tunnelsSideSize),-0.25f*(scaleRatio/tunnelsSideSize));
		camera.GetComponent<Camera> ().nearClipPlane = 0.01f;
		camera.transform.localRotation = Quaternion.AngleAxis (5, Vector3.right);
	}

	void Update () {

			/*key control to change current track*/
		if (Input.GetKey (KeyCode.A) && !turning) {
			if (track > 0) {
				lastTrack = track;
				track--;
				turning = true;
			}
		}
		if (Input.GetKey (KeyCode.D) && !turning) {
			if (track < ringSpace1 [0].SideDegrees.Length - 1) {
				lastTrack = track;
				track++;
				turning = true;
			}
		}
		//key control to extra speed if "bullet" spell is active
		if(Input.GetKey (KeyCode.W)){
			
			if(activeBullet){
				speedInc = 2;
			}
			else speedInc = 1;
		}
		else speedInc = 1;
		
		Advance (); //advance method

		//speed on start goes from 0 to tunnelsSideSize*10 "soft start"
		if(speed < tunnelsSideSize*10){
			int aux = (int)(speed/tunnelsSideSize)+1;
			speed = tunnelsSideSize*aux;
		}

	}


	private float Perception(){
		/*this function returns <=0 if the next target point is behind the player
		 *(used to change the next point to path follower)	
		 */ 
		Vector3 dis = nextPathPoint - gameObject.transform.position;
		dis.Normalize ();
		return Vector3.Dot(gameObject.transform.forward, dis);
	}
	
	void Advance(){
		float speedDelta = speed * Time.deltaTime * speedInc;

		/*
		 * when a new ringspace is reached, the game manager is alerted.
		 * Then, the gameManager is free to make changes in the non-used ringspace
		 */ 
		bool alertGameManager = false;
		float dot = Perception ();

		if (dot <= 0) {

			//if current point is reached, next point is the new target
			currentRingNumber++;
			if(currentRingNumber%5 == 0)gm.score++;

			//if the next point is greater than the ringspace length, change ringspace is needed
			if (currentRingNumber == ringSpace1.Length) {
				currentRingNumber = 0;
				currentTunnelNumber = (currentTunnelNumber + 1) % 2;
				alertGameManager = true;
			}
			if (currentTunnelNumber == 0) {
				currentRing = ringSpace1 [currentRingNumber];
			} else {
				currentRing = ringSpace2 [currentRingNumber];
			}

			float degRot;
			//if the player is changing the track, rotation is required
			if(turning){
				turnInc+=0.25f;
				nextPathPoint = Vector3.Slerp(currentRing.SideEndCenter(lastTrack),currentRing.SideEndCenter(track),turnInc);
				degRot = (currentRing.SideDegrees[track]-currentRing.SideDegrees[lastTrack])*turnInc + currentRing.SideDegrees[lastTrack];
				
				if(turnInc >= 1){
					turnInc = 0;
					turning = false;
				}
			}
			else{
				nextPathPoint = currentRing.SideEndCenter(track);
				degRot = currentRing.SideDegrees[track];
			}

			//update spaceship info
			gameObject.transform.rotation = currentRing.Rotation;
			transform.Rotate(Vector3.forward,degRot*Mathf.Rad2Deg,Space.Self);
			gameObject.transform.LookAt(nextPathPoint,gameObject.transform.up);
		}
		transform.Translate (new Vector3 (0,0,speedDelta));

		//if new tunnel needed alert game controller to create a new one
		if (alertGameManager)
			gm.playerChange = true;
	}


}
