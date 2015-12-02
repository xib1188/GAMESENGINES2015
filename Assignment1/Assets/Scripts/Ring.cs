using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace GamesEngines{
	public class RingException : UnityException{
		public RingException(string msg){

		}
	}

	/*
	 * Class Ring represents a 3D figure.
	 * This figure is a 2D extruded figure centered in "startCenter", 
	 * with "sideDegreesPercent.Lenght" sides of "sideSize" each one.
	 * This figure is oriented in space via "alphaBeta" degrees,
	 * and its form is represented via "sideDegreesPercent".
	 * "vertices1" and "vertices2" represents de vertices of the figure in space.
	 */

	public class Ring{

		private Vector3 startCenter;
		private float sideSize;
		private Vector2 alphaBeta;
		private float[] sideDegreesPercent;

		private Vector3[] vertices1;
		private Vector3[] vertices2;

		/*
		 * constructor
		 */ 
		public Ring(){}

		/*
		 * consultants
		 */ 
		public List<Vector3> MeshVertices{
			//return a lis of vertices that can be used to represent the figure with a mesh.
			get{ return CollapseVertices (vertices1, vertices2);}
		}
		public Vector3[] FrontVertices{
			//return first set of vertices of the figure
			get{ return vertices1;}
		}
		public Vector3[] BackVertices{
			//return second set of vertices of the figure
			get{ return vertices2;}
		}
		public float SideSize{
			//return the side size of the figure
			get{ return sideSize;}
		}
		public Vector3 Direction {
			//return a normalized director vector of the figure 
			get{ return AlphaBetaToDirection(alphaBeta);}
		}
		public Vector2 AlphaBeta{
			//return the orientation angles alpha & beta 
			get{ return alphaBeta;}
		}
		public float[] SideDegreesPercent {
			//return the percents of degrees of the figure
			get{ return sideDegreesPercent;}
		}
		public float[] SideDegrees{
			//return the side degrees of the figures (radians)
			get{ return PercentsToDegrees(sideDegreesPercent);}
		}
		public Vector3 StartCenter{
			//return de center of the first set of vertices of the figure
			get{ return startCenter;}
		}
		public Vector3 EndCenter{
			//return de center of the second set of vertices of the figure
			get{ return (AlphaBetaToDirection(alphaBeta) * sideSize) + startCenter;}
		}
		public Quaternion Rotation{
			//return the ring rotation from forward direction
			get{
				return Quaternion.FromToRotation (Vector3.forward, AlphaBetaToDirection(alphaBeta));}
		}

		public int Nsides{
			get{return sideDegreesPercent.Length;}
		}

		public Vector3 SideStartCenter(int side){
			return (vertices1 [side] + vertices1 [side + 1]) / 2;
		}
		public Vector3 SideEndCenter(int side){
			return (vertices2 [side] + vertices2 [side + 1]) / 2;
		}

		public Ring Clone(){
			//return a clone of the object
			Ring r = new Ring ();
			r.startCenter = startCenter;
			r.sideSize = sideSize;
			r.alphaBeta = alphaBeta;
			r.sideDegreesPercent = (float[])sideDegreesPercent.Clone ();
			r.vertices1 = (Vector3[]) vertices1.Clone ();
			r.vertices2 = (Vector3[]) vertices2.Clone ();
			return r;
		}

		public void GenerateRing(Vector3 center, Vector2 alphaBeta, float[] sideDegreesPercent, float sideSize){
			/* post: the implicit parameter has been set according to the parameters.
			 * 		Its vertices has been calculated from this parameters.
			 */
			this.startCenter = center;
			this.alphaBeta = alphaBeta;
			this.sideDegreesPercent = sideDegreesPercent;
			this.sideSize = sideSize;

			this.vertices1 = CalculateVertices (center, alphaBeta,
				                        sideDegreesPercent, sideSize);

			Vector3 newCenter = (AlphaBetaToDirection(alphaBeta) * sideSize) + center;
			this.vertices2 = CalculateVertices (newCenter, alphaBeta,
				                        sideDegreesPercent, sideSize);
		}


		public void LinkRing(Ring r, Vector2 alphaBeta, float[] sideDegreesPercent){
			/* pre: "sideDegreesPercent.length" must be equal to "r.sideDegreesPercent.length"
			 * 
			 * post: the implicit parameter has been set according to the parameters.
			 * 		 Its vertices has been calculated in order to follow Ring r in space.
			 */ 
			if (sideDegreesPercent.Length != r.SideDegreesPercent.Length)
				throw new RingException ("sideDegreesPercent.length must be equal to Ring.sideDegreesPercent.length.");

			Vector3 linkStartCenter = (r.Direction * r.SideSize) + r.startCenter;

			this.alphaBeta = alphaBeta;
			this.startCenter = linkStartCenter;
			this.sideSize = r.SideSize;
			this.sideDegreesPercent = sideDegreesPercent;

			this.vertices1 = r.vertices2;
			Vector3 newCenter = (AlphaBetaToDirection(alphaBeta) * r.SideSize) + linkStartCenter;
			this.vertices2 = CalculateVertices (newCenter, alphaBeta,
			                                    sideDegreesPercent, r.SideSize);
		}


		private static Vector3[] CalculateVertices(Vector3 center, Vector2 alphaBeta,
		                                           float[] sideDegreesPercent, float sideSize){
			/*post: returns an array of size(sideDegreesPercent.Length + 1) that defines
			*		the main vertices of Ring's part
			*/		
			int nSides = sideDegreesPercent.Length;
			int vCount = nSides + 1;
			Vector3[] v = new Vector3[vCount];
			int indexSide = sideDegreesPercent.Length / 2;
			int indexVertex = indexSide;
			float portionDeg = (360.0f / nSides);
			float portionRad = portionDeg * Mathf.Deg2Rad;
			float apothem = sideSize / (Mathf.Tan (portionRad / 2));

			//calculus of the 2 vertices of the centered side (centered to (0,0,0))
			v[indexVertex++] = new Vector3 (-sideSize / 2,- apothem/2, 0);
			v[indexVertex++] = new Vector3 (sideSize / 2,- apothem/2, 0);

			//calculus of the vertices of the right sides (centered to (0,0,0))
			float rads = 0;
			for (int i = indexSide+1; i < nSides; i++) {
				rads+= portionRad*sideDegreesPercent[i];
				float x = v[indexVertex-1].x + sideSize * Mathf.Cos(rads);
				float y = v[indexVertex-1].y + sideSize * Mathf.Sin(rads);
				
				v [indexVertex++] = new Vector3 (x, y, 0);
			}
			//calculus of the vertices of the left sides (centered to (0,0,0))
			indexVertex = indexSide-1;
			rads = 0;
			for (int i = indexSide - 1; i >= 0; i--) {
				rads-= portionRad*sideDegreesPercent[i];
				float x = v[indexVertex+1].x - sideSize * Mathf.Cos(rads);
				float y = v[indexVertex+1].y - sideSize * Mathf.Sin(rads);
				
				v [indexVertex--] = new Vector3 (x, y, 0);
			}
			//calculus of the orientation
			Quaternion q = Quaternion.FromToRotation (Vector3.forward, AlphaBetaToDirection(alphaBeta));
			for (int i = 0; i < vCount; i++) {
				v[i] = (q * v[i]) + center;
			}
			return v;
		}


		private static List<Vector3> CollapseVertices(Vector3[] v1, Vector3[] v2){
			/* pre: v1 contains front vertices, v2 cantains back vertices, and
			 * 		v1.Length = v2.Length
			 * post: returns a list of vertices that define the whole ring.
			 *		That list can be used directly to make a mesh.
			 */
			int nSides = v1.Length - 1;
			int vertexPerCell = 6;
			Vector3[] v = new Vector3[vertexPerCell * nSides];
			int inc = 0;
			for(int i = 0; i < nSides; i++){
				v[inc] = v1[i];
				v[inc+1] = v2[i];
				v[inc+2] = v2[i+1];
				v[inc+3] = v1[i];
				v[inc+4] = v2[i+1];
				v[inc+5] = v1[i+1];
				inc += vertexPerCell; 
			}
			List<Vector3> l = new List<Vector3> ();
			l.AddRange (v);
			return l;
		}


		private static Vector3 AlphaBetaToDirection(Vector2 alphaBeta){
			/* post: return a direction vector defined by rotations alpha and beta
			 */ 
			Vector3 direction = new Vector3 ();
			direction.x = Mathf.Sin(alphaBeta.x)*Mathf.Cos(alphaBeta.y);
			direction.y = Mathf.Sin(alphaBeta.x)*Mathf.Sin(alphaBeta.y);
			direction.z = Mathf.Cos (alphaBeta.x);
			return direction;
		}

		private static float[] PercentsToDegrees(float[] percents){
			/* post: calculate the angles based on its percents */
			float[] degrees = new float[percents.Length];
			float portionDeg = (360.0f / percents.Length);
			float portionRad = portionDeg * Mathf.Deg2Rad;
			int index = percents.Length / 2;
			degrees [index] = 0;
			float rad = 0;
			for (int i = index+1; i < percents.Length; i++) {
				rad += portionRad*percents[i];
				degrees[i] = rad;
			}
			rad = 0;
			for (int i = index-1; i >=0; i--) {
				rad -= portionRad*percents[i];
				degrees[i] = rad;
			}
			return degrees;
		}
	}
}