using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
/*
 * it controls the creation of tunnel and objects inside it
 * 
 */
namespace GamesEngines{
	public class TunnelController : MonoBehaviour {
		private bool firstRun;
		private Tunnel tunnel;
		
		public int tunnelLength = 50;
		public int nTunnelSides = 6;
		public float tunnelSideSize = 1.5f;
		public Vector3 initialCenter;

		private GameController gc;

		public Vector2 initialRotation;
		public Vector2 finalRotation;
		public float[] initialForm;
		public float[] finalForm;

		private int score;

		Mesh mesh;
		MeshRenderer meshRenderer;
		Texture2D texture;
		public TextAsset textureAsset;

		public bool working = false;

		private long[] objectCounters;

		void Awake(){
			this.enabled = false;
		}

		void Start(){
			Random.seed = (int)Random.Range (1, 10000);
			//mesh init
			mesh = gameObject.AddComponent<MeshFilter>().mesh;
			meshRenderer = gameObject.AddComponent<MeshRenderer>();

			mesh.Clear();
			gameObject.AddComponent<BoxCollider2D> ();
			texture = new Texture2D(200	,  200);
			if (File.Exists ("E:/xavi/DIT/GAME_ENGINES/assignment/Assignment1/Assets/Resources/texturepng.png")) {
				byte[] fileData = File.ReadAllBytes("E:/xavi/DIT/GAME_ENGINES/assignment/Assignment1/Assets/Resources/texturepng.png");
				texture.LoadImage(fileData);
			}

			texture.filterMode = FilterMode.Point;

			GenerateMesh();

			meshRenderer.material.SetTexture(0, texture);


		}
		//set up
		public void Inicialize(int tunnelLength, int nTunnelSides, float tunnelSideSize,int score, Vector3 initialCenter,
		                       Vector2 initialRotation, Vector2 finalRotation, float[] initialForm, float[] finalForm, bool first, GameController gc){
			this.tunnelLength = tunnelLength;
			this.nTunnelSides = nTunnelSides;
			this.tunnelSideSize = tunnelSideSize;
			this.initialCenter = initialCenter;
			this.initialRotation = initialRotation;
			this.finalRotation = finalRotation;
			this.initialForm = initialForm;
			this.finalForm = finalForm;
			this.score = score;
			this.firstRun = first;
			this.gc = gc;
			objectCounters = gc.Counters;
			tunnel = new Tunnel(tunnelLength,nTunnelSides,tunnelSideSize);
			tunnel.GenerateTunnel(initialCenter,initialRotation,finalRotation,initialForm,finalForm);
			CreateObjects ();
			//enabled by player controller
			this.enabled = true;
		}

		List<BackgroundWorker> workers = new List<BackgroundWorker>();
		//create a new tunnel using background workers
		public void Refresh(int score, Vector3 initialCenter, Vector2 initialRotation, Vector2 finalRotation,
		                    float[] initialForm, float[] finalForm){
			working = true;
			this.initialCenter = initialCenter;
			this.initialRotation = initialRotation;
			this.finalRotation = finalRotation;
			this.initialForm = initialForm;
			this.finalForm = finalForm;
			this.score = score;
			objectCounters = gc.Counters;
			BackgroundWorker backgroundWorker = new BackgroundWorker();

			backgroundWorker.DoWork += (o, a) =>
			{
				GenerateNewTunnel();

			};
			backgroundWorker.RunWorkerCompleted += (o, a) =>
			{
				CreateObjects ();
				GenerateMesh();	

				meshRenderer.material.SetTexture(0, texture);
				working = false;
			};
			workers.Add(backgroundWorker);
			
			backgroundWorker.RunWorkerAsync(null);
		}

		public void GenerateNewTunnel (){
			tunnel = new Tunnel(tunnelLength,nTunnelSides,tunnelSideSize);
			tunnel.GenerateTunnel(initialCenter,initialRotation,finalRotation,initialForm,finalForm);
		}



		void GenerateMesh(){
			int uvsState = Random.Range(1, tunnel.Depth);
			mesh.Clear();
			mesh.vertices = tunnel.GenerateMeshVertices ().ToArray ();
			mesh.triangles = tunnel.GenerateMeshTriangles ().ToArray ();
			mesh.uv = tunnel.GenerateMeshUvs(uvsState).ToArray();
			mesh.RecalculateNormals ();
		}
		private List<GameObject> cubes;

		//creates randomly objects to put in the tunnel
		void CreateObjects(){
			//inicialize cube array
			if (cubes != null) {
				for (int i = 0; i < cubes.Count; i++) {
					Destroy (cubes [i]);
				}
			}
			//find objects to clone them
			GameObject[] spells = new GameObject[3];
			spells[0] = GameObject.Find("Components").transform.FindChild("Shield").gameObject;
			spells [1] = GameObject.Find ("Components").transform.FindChild ("Bullet").gameObject;
			spells [2] = GameObject.Find ("Components").transform.FindChild ("Heart").gameObject;

			
			cubes = new List<GameObject> ();
			//number of cubes based on the score (+score === +cubes === +difficulty)
			int nCubes = Mathf.Min ((score / 300) + 1, 10) * 10;
			int nSides = Mathf.Min ((score / 3000) + 1, (tunnel.NSides / 2) - 1);	

			//ratios of danger object (r/10, r+=1 every 5000 score)
			float dangerRatio = nSides / 10f;

			List<Vector3> lvert = tunnel.GenerateVertices ();
			float scaleSize = tunnelSideSize / 1.75f;

			int iinc = tunnel.Depth / nCubes;
			int starti = 0;
			if (firstRun)
				starti = 10;

			//every iinc a new object is created
			for (int i = starti; i < tunnelLength; i += iinc) {

				bool[] s = new bool[nTunnelSides];
				for (int j = 0; j < nSides; j++) {
					int side = Random.Range (0, nTunnelSides);
					while (s[side])
						side = Random.Range (0, nTunnelSides);

					//rate for spell
					bool isSpell = (Random.Range (0,tunnelLength/(3+score/300)) == 0);
					//rate for nuclear 
					bool isDangerous = false;
					if(!isSpell) isDangerous = (Random.Range(0,10-nSides) == 0);

					GameObject obj;
					Material mat;
					if(isSpell){
						//random creates a spell
						int r = Random.Range(0,3);
						obj = Instantiate(spells[r])as GameObject;
						obj.SetActive(true);
						obj.name = "SpellClone"+objectCounters[0];
						objectCounters[0]++;
						obj.AddComponent<ComponentsController>();
					}
					else if(isDangerous){
						//create a nuclear and inicialize its components
						obj = GameObject.CreatePrimitive (PrimitiveType.Cylinder);
						obj.transform.localScale = new Vector3 (scaleSize, scaleSize/1.5f, scaleSize);
						mat = Resources.Load("danger",typeof(Material)) as Material;
						obj.GetComponent<Renderer>().material = mat;
						obj.tag = "Nuclear";
						obj.name = "Nuclear"+objectCounters[1];
						AudioSource audio = obj.AddComponent<AudioSource>();
						audio.clip = Resources.Load("Sounds/Explosion",typeof(AudioClip)) as AudioClip;
						audio.playOnAwake = false;
						audio.enabled = true;
						obj.AddComponent<ComponentsController>();
						GameObject components = GameObject.Find("Components");
						GameObject expl = components.transform.FindChild("Explosion").gameObject;
						GameObject explClone = Instantiate(expl) as GameObject;
						explClone.transform.parent = obj.transform;
						objectCounters[1]++;
					}
					else{
						//create a tnt and inicialize its components
						obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
						obj.transform.localScale = new Vector3 (scaleSize, scaleSize, scaleSize);
						mat = Resources.Load("TNT",typeof(Material)) as Material;
						obj.GetComponent<Renderer>().material = mat;
						obj.name = "TNT"+objectCounters[2];
						AudioSource audio = obj.AddComponent<AudioSource>();
						audio.clip = Resources.Load("Sounds/Explosion",typeof(AudioClip)) as AudioClip;
						audio.playOnAwake = false;
						audio.enabled = true;
						obj.AddComponent<ComponentsController>();
						GameObject components = GameObject.Find("Components");
						GameObject expl = components.transform.FindChild("Explosion").gameObject;
						GameObject explClone = Instantiate(expl) as GameObject;
						explClone.transform.parent = obj.transform;
						objectCounters[2]++;
						obj.tag = "TNT";

					}
					//get the point to place the object
					Vector3 center1 = (lvert [i * (nTunnelSides + 1) * 2 + side] + lvert [i * (nTunnelSides + 1)*2 + side + 1]) / 2;
					Vector3 center2 = (lvert [(i + 1) * (nTunnelSides + 1)*2  + side] + lvert [(i + 1) * (nTunnelSides + 1)*2 + side + 1]) / 2;
					Vector3 center = (center1 + center2) / 2f;
					//place the object
					obj.transform.position = center;
					obj.transform.rotation = tunnel.Rings [i].Rotation;
					float degRot = tunnel.Rings[i].SideDegrees[side];
					obj.transform.Rotate(Vector3.forward,degRot*Mathf.Rad2Deg);
					//scale and rotate the object 
					if(isDangerous)obj.transform.Translate(0, scaleSize/2.25f,0);
					else{
						obj.transform.Translate(0, scaleSize/2f,0);
						if(!isSpell)obj.transform.Rotate(Vector3.up,90);
						else{
							if(obj.tag == "Heart")obj.transform.Rotate(Vector3.up,-90);
							else if(obj.tag == "Shield")obj.transform.Rotate(Vector3.right,90);
							else obj.transform.Rotate(Vector3.up,180);
						}
					}

					obj.AddComponent<Rigidbody>().useGravity = false;
					 

					cubes.Add (obj);
				}
			}

		}



		public Vector3 EndCenter{
			get{return tunnel.EndCenter;}
		}
		public float Apothem{
			get{ return tunnel.Apothem;}
		}

		public List<Ring> TunnelRingsList{
			get{ return tunnel.Rings;}
		}

		//control about background workers
		void Update(){
			for(int i = workers.Count - 1 ; i >=  0 ; i --)
			{
				if (workers[i].IsBusy) 
				{
					workers[i].Update();
				}
				else
				{
					workers.Remove(workers[i]);
				}
			}
		}


	}


}

/*
 * Rotations alpha: A, beta B 
 * 
 * left (-1,0,0)	-> A(90) B(180) / A(270) B(0)
 * right (1,0,0)	-> A(90) B(0) / A(270) B(180)
 * forward (0,0,1)	-> A(0)
 * back (0,0,-1)	-> A(180)
 * up (0,1,0)		-> A(90) B(90) / A(270) B(270)
 * down (0,-1,0)	-> A(90) B(270) / A(270) B(90)
 * 
 * 
 */