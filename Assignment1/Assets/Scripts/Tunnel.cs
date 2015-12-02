using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GamesEngines{
	public class TunnelException : UnityException{
		public TunnelException(string msg){
			Debug.Log (msg);
		}
	}

	/* Class tunnel represents a list of concatenated rings */

	public class Tunnel{

		private List<Ring> rings;
		private float sideSize;
		private int nSides;
		private int depth;

		/*
		 * constructors
		 */ 
		public Tunnel(){
			depth = 20;
			nSides = 8;
			sideSize = 1.0f;
		}

		public Tunnel(int depth, int nSides, float sideSize){
			this.depth = depth;
			this.nSides = nSides;
			this.sideSize = sideSize;
		}

		/*
		 * consultants
		 */ 
		public List<Ring> Rings{
			//returns a list containing the rings that form the tunnel
			get{ 
				List<Ring> list = new List<Ring>();
				for(int i = 0; i < this.rings.Count; i++){
					list.Add(this.rings[i].Clone());
				}
				return list;}
		}

		public int NSides{
			//returns the number of sides the tunnel has
			get{ return this.nSides;}
		}

		public int Depth{
			//returns the tunnel lenght (number of rings)
			get{ return this.depth;}
		}

		public float Apothem{
			//returns the aphotem of the tunnel
			get{ return (sideSize / (Mathf.Tan (Mathf.PI / nSides)));}
		}

		public Vector3 EndCenter{
			//returns the end center of the tunnel
			get{ return rings[depth-1].EndCenter;}
		}
		 
		public Vector3 GetRingStartCenter(int ring){
			//returns the start center of the Ring in the tunnel indexed by ring
			return rings [ring].StartCenter;
		}

		public Vector3 GetRingEndCenter(int ring){
			//returns the end center of the Ring in the tunnel indexed by ring
			return rings [ring].EndCenter;
		}

		public List<Vector3> GenerateVertices(){
			List<Vector3> vertices = new List<Vector3> ();
			for (int i = 0; i < depth; i++) {
				vertices.AddRange(rings[i].FrontVertices);
				vertices.AddRange(rings[i].BackVertices);
			}
			return vertices;
		}
		
		public List<Vector3> GenerateMeshVertices(){
			List<Vector3> vertices1 = new List<Vector3> ();
			List<Vector3> vertices2 = new List<Vector3> ();
			for (int i = 0; i < depth; i++) {
				vertices1.AddRange(rings[i].MeshVertices);
				vertices2.AddRange(rings[i].MeshVertices);
			}
			List<Vector3> vert = new List<Vector3> ();
			vert.AddRange (vertices1);
			vert.AddRange (vertices2);
			return vert;
		}
		
		public List<int> GenerateMeshTriangles(){
			List<int> triangles1 = new List<int> ();
			List<int> triangles2 = new List<int> ();
			int total = depth * nSides * 6;
			for (int i = 0; i < total; i+=3) {
				triangles1.Add(i);
				triangles1.Add(i+1);
				triangles1.Add(i+2);
				triangles2.Add(i+total);
				triangles2.Add(i+2+total);
				triangles2.Add(i+1+total);
			}
			List<int> tri = new List<int> ();
			tri.AddRange (triangles1);
			tri.AddRange (triangles2);
			return tri;
		}

		public List<Vector2> GenerateMeshUvs(){
			List<Vector2> uvs1 = new List<Vector2> ();
			List<Vector2> uvs2 = new List<Vector2> ();
			int total = depth * nSides * 6;
			int range = Random.Range (0, 3);
			float x,y;
			x = 0f;
			y = 0.5f;
			Vector2[] fl = new Vector2[6];
			fl [0] = new Vector2 (0+x,0+y);
			fl [1] = new Vector2 (0+x,0.5f+y);
			fl [4] = new Vector2 (0.5f+x,0.5f+y);
			fl [3] = new Vector2 (0f+x,0f+y);
			fl [5] = new Vector2 (0.5f+x,0.5f+y);
			fl [2] = new Vector2 (0.5f+x,0f+y);

			for (int i = 0; i < total; i++) {
				uvs1.Add(fl[i%6]);
				uvs2.Add(fl[i%6]);
			}
			List<Vector2> uvs = new List<Vector2> ();
			uvs.AddRange (uvs1);
			uvs.AddRange (uvs2);
			return uvs;
		}


		


		public void GenerateTunnel(Vector3 initialCenter, Vector2 initialAlphaBeta, Vector2 finalAlphaBeta,
		                           float[] initialForm, float[] finalForm){
			if (initialForm.Length != nSides || finalForm.Length != nSides)
				throw new TunnelException ("GenerateTunnel: tunnel forms length must be equal to tunnel nSides");

			rings = new List<Ring>();

			Vector2[] angles = GenerateAlphaBetaSequence (depth, initialAlphaBeta, finalAlphaBeta);

			List<float[]> forms = GenerateFormsSequence (depth, initialForm,finalForm);

			Ring r = new Ring ();
			r.GenerateRing (initialCenter, angles[0], forms[0], this.sideSize);
			this.rings.Add (r);


			for (int i = 1; i < this.depth; i++) {
				Ring rc = new Ring();
				rc.LinkRing(this.rings[i-1],angles[i],forms[i]);
				this.rings.Add(rc);
			}

		}



		/*private and static methods*/

		private static Vector2[] GenerateAlphaBetaSequence(int depth, Vector2 initialAlphaBeta, Vector2 finalAlphaBeta){

			Vector2 diff = (finalAlphaBeta - initialAlphaBeta)/(depth-1);
			Vector2[] sequence = new Vector2[depth];
			Vector2 alphaBeta = initialAlphaBeta;
			for (int i = 0; i < depth; i++) {
				sequence[i] = alphaBeta;
				alphaBeta+=diff;
			}
			return sequence;
		}

		private static List<float[]> GenerateFormsSequence(int n, float[] initialForm, float[] finalForm){
			int formSize;
			if((formSize=initialForm.Length)!= finalForm.Length)
				throw new TunnelException ("GenerateFormSequence: tunnel forms length must be equals");

			List<float[]> forms = new List<float[]> ();

			float[] partialForm = (float[])initialForm.Clone ();


			float[] inc = new float[formSize];
			for(int i = 0; i < formSize; i++) 
				inc[i] = (finalForm[i] - initialForm[i])/(n-1);

			for (int i = 0; i < n; i++) {
				forms.Add((float[])partialForm.Clone());
				for(int j = 0; j < formSize; j++){
					partialForm[j] += inc[j];
				}
			}
			return forms;
		}





	}
}