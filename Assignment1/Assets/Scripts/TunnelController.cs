using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace GamesEngines{
	public class TunnelController : MonoBehaviour {

		private Tunnel tunnel;
		
		public int tunnelLength = 50;
		public int nTunnelSides = 6;
		public float tunnelSideSize = 1.5f;
		public Vector3 initialCenter;

		public Vector2 initialRotation;
		public Vector2 finalRotation;
		public float[] initialForm;
		public float[] finalForm;

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

		public void Inicialize(int tunnelLength, int nTunnelSides, float tunnelSideSize, Vector3 initialCenter,
		                       Vector2 initialRotation, Vector2 finalRotation, float[] initialForm, float[] finalForm){
			this.tunnelLength = tunnelLength;
			this.nTunnelSides = nTunnelSides;
			this.tunnelSideSize = tunnelSideSize;
			this.initialCenter = initialCenter;
			this.initialRotation = initialRotation;
			this.finalRotation = finalRotation;
			this.initialForm = initialForm;
			this.finalForm = finalForm;

			tunnel = new Tunnel(tunnelLength,nTunnelSides,tunnelSideSize);
			tunnel.GenerateTunnel(initialCenter,initialRotation,finalRotation,initialForm,finalForm);

			this.enabled = true;
		}

		List<BackgroundWorker> workers = new List<BackgroundWorker>();

		public void Refresh(Vector3 initialCenter, Vector2 initialRotation, Vector2 finalRotation,
		                    float[] initialForm, float[] finalForm){
			working = true;
			this.initialCenter = initialCenter;
			this.initialRotation = initialRotation;
			this.finalRotation = finalRotation;
			this.initialForm = initialForm;
			this.finalForm = finalForm;

			BackgroundWorker backgroundWorker = new BackgroundWorker();

			backgroundWorker.DoWork += (o, a) =>
			{
				GenerateNewTunnel();
			};
			backgroundWorker.RunWorkerCompleted += (o, a) =>
			{
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
			mesh.Clear();
			mesh.vertices = tunnel.GenerateMeshVertices ().ToArray ();
			mesh.triangles = tunnel.GenerateMeshTriangles ().ToArray ();
			mesh.uv = tunnel.GenerateMeshUvs().ToArray();
			mesh.RecalculateNormals ();

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