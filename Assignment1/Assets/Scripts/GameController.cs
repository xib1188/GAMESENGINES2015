using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using GamesEngines;

/*
 * Game behaviour and scripts comunication
 */
public class GameController : MonoBehaviour {

	//list of tunnels' controllers
	public List<TunnelController> tControllers;

	//player object and controller
	public GameObject player;
	private PlayerController playerController;

	//number of sides of the tunnels and its size
	private int nSides;
	private float sideSize;

	//initial values for the next tunnel that will be created
	private Vector3 initialCenter;
	private Vector2 initialRotation;
	private float[] initialForm;

	//tunnels' lenght
	private int tunnelDepth;

	//tunnel update control
	private int pCurrent = 0;
	public bool waiting = false; //control for the background works
	
	public GameObject UIscore;
	[HideInInspector]
	public bool playerChange;//activated by player controller to create new tunnel
	[HideInInspector]
	public int score;

	//count for every spell object (to name it properly, used by tunnelController)
	private long[] objectCounters;

	public long[] Counters {
		get{ return objectCounters;}
	}

	//set up
	void Awake () {
		tControllers = new List<TunnelController> ();
		objectCounters = new long[3];
		objectCounters [0] = objectCounters [1] = objectCounters [2] = 0;
		score = 0;
		tunnelDepth = 100;
		nSides = 6;//Random.Range (6, 10);
		sideSize = 1f;
		initialCenter = new Vector3 (0, 0, 0);
		initialRotation = new Vector2 (0, 0);
		Vector2 finalRotation = RandomRotation ();
		initialForm = new float[]{1,1,1,1,1,1};//RandomForm();
		float[] finalForm = new float[]{1,1,1,1,1,1};//RandomForm();
		//first 2 tunnels creation
		for (int i = 0; i < 2; i++) {
			GameObject g = new GameObject();
			g.name = "Tunnel" + i;
			TunnelController tc = g.AddComponent<TunnelController>();
			tc.Inicialize(tunnelDepth,nSides,sideSize,score,initialCenter,
			              initialRotation,finalRotation,initialForm,finalForm, (i==0),this);
			tControllers.Add(tc);
			//update info for the next tunnel creation and concatenate it with this one
			initialCenter = tc.EndCenter;
			initialRotation = finalRotation;
			finalRotation = RandomRotation();
			initialForm = finalForm;
			finalForm = RandomForm();
		}

		//inicialize player controller
		playerController = player.AddComponent<PlayerController> ();
		playerController.Inicialize(tControllers[0].TunnelRingsList.ToArray(),
		                            tControllers[1].TunnelRingsList.ToArray(), sideSize,
		                            tControllers[0].Apothem,this);
		//stop running 'til new advice
		playerChange = false;
		this.enabled = false;
	}

	//returns a random rotation
	public Vector2 RandomRotation(){
		return new Vector2(Random.Range(0f,180f)*Mathf.Deg2Rad,Random.Range(0f,359f)*Mathf.Deg2Rad);
	}

	//returns a random form 
	public float[] RandomForm(){
		float[] forms = new float[nSides];
		for (int i = 0; i < nSides; i++) {
			forms [i] = Random.Range(-1f,1f);
		}
		return forms;
	}


	//background works set up and structures
	class Arg{
		public int depth;
		public int nSides;
		public float sideSize;
		public Vector3 center;
		public Vector2 iRot;
		public Vector2 fRot;
		public float[] iForm;
		public float[] fForm;
		public int pCurrent;
	}

	List<BackgroundWorker> workers = new List<BackgroundWorker>();

	//when new tunnel is needed
	private void RefreshTcontroller(){
		Vector2 rotation = RandomRotation ();
		float[] form = RandomForm();
		pCurrent = (playerController.CurrentTunnelNumber+1)%2;
		tControllers[pCurrent].Refresh(score,initialCenter,initialRotation,rotation,initialForm,form);
		initialRotation = rotation;
		initialForm = form;
		//set waiting true to avoid tasks overlaping
		waiting = true;
	}

	//update player controller data
	void UpdateData(){
		initialCenter = tControllers[pCurrent].EndCenter;
		if (pCurrent == 0) playerController.ringSpace1 = tControllers [0].TunnelRingsList.ToArray ();
		else playerController.ringSpace2 = tControllers [1].TunnelRingsList.ToArray ();
	}
	
	// Update is called once per frame
	void Update () {
		//enable player controller if need
		if(playerController != null && !playerController.enabled) playerController.enabled = true;

		//update data if needed
		if (waiting) {
			if (!(waiting = tControllers [pCurrent].working)) {
				UpdateData ();
			}
		}
		else{
			//detect if the player needs new tunnel
			if (playerChange) {
				RefreshTcontroller ();
				playerChange = false;
			}
		}
		//update the score on UI
		UIscore.GetComponent<GUIText> ().text = score.ToString ("D9");

	}

	public PlayerController PlayerController{
		get{return playerController;}
	}

	public Vector3 InitialCenter{
		get{ return initialCenter;}
		set{ initialCenter = value;}
	}
	public Vector2 InitialRotation{
		get{ return initialRotation;}
		set{ initialRotation = value;}
	}
	public float[] InitialForm{
		get{ return initialForm;}
		set{ initialForm = value;}
	}


}
