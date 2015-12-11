using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using GamesEngines;

public class GameController : MonoBehaviour {


	public List<TunnelController> tControllers;

	public GameObject player;
	private PlayerController playerController;

	//private BackgroundWork bWork;

	private int nSides;
	private float sideSize;

	private Vector3 initialCenter;
	private Vector2 initialRotation;
	private float[] initialForm;

	private int tunnelDepth;

	/*tunnel update control*/
	private int pCurrent = 0;
	private bool waiting = false;

	[HideInInspector]
	public bool playerChange;
	[HideInInspector]
	public int score;

	private long[] objectCounters;



	public long[] Counters {
		//return a normalized director vector of the figure 
		get{ return objectCounters;}
	}

	void Awake () {
		tControllers = new List<TunnelController> ();
		objectCounters = new long[3];
		objectCounters [0] = objectCounters [1] = objectCounters [2] = 0;
		//bWork = new BackgroundWork ();
		//bWork.gC = this;
		score = 0;
		tunnelDepth = 100;
		nSides = 6;//Random.Range (6, 10);
		sideSize = 1f;
		initialCenter = new Vector3 (0, 0, 0);
		initialRotation = new Vector2 (0, 0);
		Vector2 finalRotation = RandomRotation ();
		initialForm = new float[]{1,1,1,1,1,1};//RandomForm();
		float[] finalForm = new float[]{1,1,1,1,1,1};//RandomForm();
		for (int i = 0; i < 2; i++) {
			GameObject g = new GameObject();
			g.name = "Tunnel" + i;
			TunnelController tc = g.AddComponent<TunnelController>();
			tc.Inicialize(tunnelDepth,nSides,sideSize,score,initialCenter,
			              initialRotation,finalRotation,initialForm,finalForm, (i==0),this);
			tControllers.Add(tc);
		
			initialCenter = tc.EndCenter;
			initialRotation = finalRotation;
			finalRotation = RandomRotation();
			initialForm = finalForm;
			finalForm = RandomForm();
		}

		//player = GameObject.Find ("Player");
		//player.transform.localScale = new Vector3 (0.001f, 0.001f, 0.001f);
		playerController = player.AddComponent<PlayerController> ();

		playerController.Inicialize(tControllers[0].TunnelRingsList.ToArray(),
		                            tControllers[1].TunnelRingsList.ToArray(), sideSize,
		                            tControllers[0].Apothem,this);

		playerChange = false;

		this.enabled = false;
	}

	public Vector2 RandomRotation(){
		return new Vector2(Random.Range(0f,180f)*Mathf.Deg2Rad,Random.Range(0f,359f)*Mathf.Deg2Rad);
	}

	public float[] RandomForm(){
		float[] forms = new float[nSides];
		for (int i = 0; i < nSides; i++) {
			forms [i] = Random.Range(-1f,1f);
		}
		return forms;
	}



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

	private void RefreshTcontroller(){
		Vector2 rotation = RandomRotation ();
		float[] form = RandomForm();
		pCurrent = (playerController.CurrentTunnelNumber+1)%2;
		tControllers[pCurrent].Refresh(score,initialCenter,initialRotation,rotation,initialForm,form);
		initialRotation = rotation;
		initialForm = form;
		waiting = true;
	}

	void UpdateData(){
		initialCenter = tControllers[pCurrent].EndCenter;
		if (pCurrent == 0) playerController.ringSpace1 = tControllers [0].TunnelRingsList.ToArray ();
		else playerController.ringSpace2 = tControllers [1].TunnelRingsList.ToArray ();

	}
	
	// Update is called once per frame
	void Update () {
		if(playerController != null && !playerController.enabled) playerController.enabled = true;
		if (waiting) {
			if (!(waiting = tControllers [pCurrent].working)) {
				UpdateData ();
			}
		}
		else{
			if (playerChange) {
				RefreshTcontroller ();
				playerChange = false;
			}//RefreshTcontroller();
		}

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
