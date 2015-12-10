using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace GamesEngines{
	public class TunnelController : MonoBehaviour {
		private bool firstRun;
		private Tunnel tunnel;
		
		public int tunnelLength = 50;
		public int nTunnelSides = 6;
		public float tunnelSideSize = 1.5f;
		public Vector3 initialCenter;

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
			//GenerateTexture();
			GenerateMesh();

			meshRenderer.material.SetTexture(0, texture);
		}

		public void Inicialize(int tunnelLength, int nTunnelSides, float tunnelSideSize,int score, Vector3 initialCenter,
		                       Vector2 initialRotation, Vector2 finalRotation, float[] initialForm, float[] finalForm, bool first){
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
			tunnel = new Tunnel(tunnelLength,nTunnelSides,tunnelSideSize);
			tunnel.GenerateTunnel(initialCenter,initialRotation,finalRotation,initialForm,finalForm);
			CreateObjects ();
			this.enabled = true;
		}

		List<BackgroundWorker> workers = new List<BackgroundWorker>();

		public void Refresh(int score, Vector3 initialCenter, Vector2 initialRotation, Vector2 finalRotation,
		                    float[] initialForm, float[] finalForm){
			working = true;
			this.initialCenter = initialCenter;
			this.initialRotation = initialRotation;
			this.finalRotation = finalRotation;
			this.initialForm = initialForm;
			this.finalForm = finalForm;
			this.score = score;

			BackgroundWorker backgroundWorker = new BackgroundWorker();

			backgroundWorker.DoWork += (o, a) =>
			{
				GenerateNewTunnel();

			};
			backgroundWorker.RunWorkerCompleted += (o, a) =>
			{
				CreateObjects ();
				GenerateMesh();	
				//GenerateTexture();
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

		void GenerateTexture(){
			int r = Random.Range (1, 4);
			for (int x = 0; x < tunnel.NSides; x++){
				Color c = new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f));
				for (int y = 0; y < tunnel.Depth; y++){
					texture.SetPixel(x, y, c);                     
				}
			}
			texture.Apply();
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

		void CreateObjects(){
			if (cubes != null) {
				for (int i = 0; i < cubes.Count; i++) {
					Destroy (cubes [i]);
				}
			}
			cubes = new List<GameObject> ();
			//number of cubes based on the score (+score === +cubes === +difficulty)
			int nCubes = Mathf.Min ((score / 500) + 1, 10) * 10;
			int nSides = Mathf.Min ((score / 5000) + 1, (tunnel.NSides / 2) - 1);	

			//ratios of danger object (r/10, r+=1 every 5000 score)
			float dangerRatio = nSides / 10f;

			List<Vector3> lvert = tunnel.GenerateVertices ();
			float scaleSize = tunnelSideSize / 1.75f;

			int iinc = tunnel.Depth / nCubes;
			int starti = 0;
			if (firstRun)
				starti = 10;


			for (int i = starti; i < tunnelLength; i += iinc) {
				bool[] s = new bool[nTunnelSides];
				for (int j = 0; j < nSides; j++) {
					int side = Random.Range (0, nTunnelSides-1);
					while (s[side])
						side = Random.Range (0, nTunnelSides-1);

					bool isDangerous = (Random.Range(0,10-nSides) == 0);

					GameObject obj;
					Material mat;
					if(isDangerous){
						obj = GameObject.CreatePrimitive (PrimitiveType.Cylinder);
						obj.transform.localScale = new Vector3 (scaleSize, scaleSize/1.5f, scaleSize);
						mat = Resources.Load("danger",typeof(Material)) as Material;
					}
					else{
						obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
						obj.transform.localScale = new Vector3 (scaleSize, scaleSize, scaleSize);
						mat = Resources.Load("TNT",typeof(Material)) as Material;
					}

					Vector3 center1 = (lvert [i * (nTunnelSides + 1) * 2 + side] + lvert [i * (nTunnelSides + 1)*2 + side + 1]) / 2;
					Vector3 center2 = (lvert [(i + 1) * (nTunnelSides + 1)*2  + side] + lvert [(i + 1) * (nTunnelSides + 1)*2 + side + 1]) / 2;
					Vector3 center = (center1 + center2) / 2f;

					obj.transform.position = center;
					obj.transform.rotation = tunnel.Rings [i].Rotation;
					float degRot = tunnel.Rings[i].SideDegrees[side];
					obj.transform.Rotate(Vector3.forward,degRot*Mathf.Rad2Deg);

					if(isDangerous)obj.transform.Translate(0, scaleSize/2.25f,0);
					else{
						obj.transform.Translate(0, scaleSize/2f,0);
						obj.transform.Rotate(Vector3.up,90);
					}

					obj.AddComponent<Rigidbody>().useGravity = false;
					 
					obj.GetComponent<Renderer>().material = mat;
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