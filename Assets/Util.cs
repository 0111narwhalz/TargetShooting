using System;
using UnityEngine;

namespace TargetShooting
{
	static class Util
	{
		public static System.Random rng = new System.Random();
		
		public static Vector3[] PosVelCartToSphr(Vector3 pos, Vector3 vel)
			=> new Vector3[2]{PosCartToSphr(pos), VelCartToSphr(pos, vel)};
		
		public static Vector3 PosCartToSphr(Vector3 pos)
		{
			Vector3 output = Vector3.zero;
			//TODO: account for arctangent symmetry
			output.x = (float)Math.Atan(pos.y / pos.x); //theta
			output.y = (float)Math.Asin(pos.z / pos.magnitude); //phi
			output.z = (float)pos.magnitude; //rho
			return output;
		}
		
		public static Vector3 VelCartToSphr(Vector3 pos, Vector3 vel)
		{
			Vector3 output = Vector3.zero;
			//TODO: ???
			return output;
		}
	}
}
