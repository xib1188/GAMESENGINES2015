using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GamesEngines;

public class PlayerController : MonoBehaviour {

	private GameController gm;
	private GameObject camera;

	public Ring[] ringSpace1;
	public Ring[] ringSpace2;


	private float tunnelsSideSize;
	private int tunnelsNSides;
	private int tunnelsLenght;

	private float speed;
	private int track;
	private Ring currentRing;
	private int currentTunnelNumber;
	private int currentRingNumber;
	private float turnInc;
	private bool turning;
	private int lastTrack;

	private Quaternion rotation;
	private Vector3 pointToLook;
	private Vector3 translate;

	private float distance;
	private float inc;

	private Vector3 nextPathPoint;

	private int scaleRatio = 200;


	public int CurrentTunnelNumber{
		get{ return currentTunnelNumber;}
	}

	void Awake(){
		this.enabled = false;
	}


	void Start () {
		currentTunnelNumber = 0;
		currentRingNumber = 0;
		tunnelsNSides = ringSpace1 [0].Nsides;
		track = tunnelsNSides / 2;
		turnInc = 0;
		turning = false;
		currentRing = ringSpace1 [currentRingNumber];
		nextPathPoint = currentRing.SideEndCenter(track);

		gameObject.transform.position = currentRing.SideStartCenter (track);
		gameObject.transform.rotation = currentRing.Rotation;
		gameObject.transform.localScale = new Vector3 (tunnelsSideSize*0.5f/scaleRatio,tunnelsSideSize*0.5f/scaleRatio,tunnelsSideSize*0.5f/scaleRatio);

		camera = gameObject.transform.FindChild("PlayerCam1").gameObject;
		camera.transform.localPosition = new Vector3 (0, +0.8f*(scaleRatio/tunnelsSideSize),-0.25f*(scaleRatio/tunnelsSideSize));
		camera.GetComponent<Camera> ().nearClipPlane = 0.01f;
		camera.transform.localRotation = Quaternion.AngleAxis (5, Vector3.right);
	}

	public void Inicialize(Ring[] l1, Ring[] l2, float size, float apothem, GameController gm){
		ringSpace1 = l1;	
		ringSpace2 = l2;

		//elevation = apothem / 2;
		this.distance = distance;
		this.speed = size*10f;
		this.gm = gm;
		tunnelsSideSize = size;

		this.enabled = true;
	}

	void Update () {
		if (Input.GetKey(KeyCode.A)&&!turning){
			if(track>0){
				lastTrack = track;
				track--;
				turning = true;
			}
		}
		if (Input.GetKey(KeyCode.D) && !turning){
			if(track < ringSpace1[0].SideDegrees.Length-1){
				lastTrack = track;
				track++;
				turning = true;
			}
		}
	
		Advance ();
	}

	private float Perception(){
		Vector3 dis = nextPathPoint - gameObject.transform.position;
		dis.Normalize ();
		return Vector3.Dot(gameObject.transform.forward, dis);
	}
	
	void Advance(){
		float speedDelta = speed * Time.deltaTime;
		bool alertGameManager = false;
		float dot = Perception ();

		if (dot <= 0) {


			currentRingNumber++;
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
			if(turning){
				turnInc+=0.20f;
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
			gameObject.transform.rotation = currentRing.Rotation;
			transform.Rotate(Vector3.forward,degRot*Mathf.Rad2Deg,Space.Self);

			gameObject.transform.LookAt(nextPathPoint,gameObject.transform.up);

		}


	
		transform.Translate (new Vector3 (0,0,speedDelta));

		if (alertGameManager)
			gm.playerChange = true;
	}

}
