using UnityEngine;
using System;

using TargetShooting;

namespace TargetShooting
{
	//Exposes sensor data
	public class Sensors : MonoBehaviour
	{
		bool dirty = true;
		GameObject target;
		Transform ownTransform;
		Transform tgtTransform;
		
		Vector3 rpos; //rpos and rvel are in spherical coordinates, with the pole forward.
		Vector3 rvel; //Vectors are <theta, phi, rho>
		float enemyHP;
		
		Vector3 angVel;
		float ownHP;
		float muzzleVel;
		float reloadFraction;
		bool fireReady;
		
		void Start()
		{
			ownTransform = gameObject.GetComponent<Transform>();
			LockTarget();
			UpdateSensors();
		}
		
		void FixedUpdate()
		{
			dirty = true;
		}
		
		void UpdateSensors()
		{
			if(target == null)
			{
				Debug.Log("{0} Target lost", gameObject);
				if(!LockTarget())
				{
					Debug.Log("Target not found");
					return;
				}
			}
			//Obtain reference to own Mothership for repeated use
			Mothership ship = gameObject.GetComponent<Mothership>();
			
			//Relative position to enemy agent (Cartesian)
			Vector3 rposCart = ownTransform.InverseTransformPoint(tgtTransform.position);
			
			//Relative velocity to enemy agent (Cartesian)
			Vector3 rvelCart = ownTransform.InverseTransformVector(target.GetComponent<Rigidbody>().velocity - gameObject.GetComponent<Rigidbody>().velocity);
			
			//Convert rpos and rvel to spherical
			Vector3[] rel = Util.PosVelCartToSphr(rposCart, rvelCart);
			rpos = rel[0];
			rvel = rel[1];
			
			//Hitpoints of enemy agent as fraction of total
			enemyHP = target.GetComponent<Mothership>().hitpoints / target.GetComponent<Mothership>().maxHitpoints;
			
			//Own angular velocity
			angVel = gameObject.GetComponent<Rigidbody>().angularVelocity;
			
			//Own hitpoints as fraction of total
			ownHP = ship.hitpoints / ship.maxHitpoints;
			
			//Weapon muzzle velocity
			muzzleVel = ship.muzzleVelocity;
			
			//Weapon time until next shot as fraction of total reload time
			reloadFraction = (float)ship.lastFire / ship.framesPerShot;
			
			//Weapon ready state
			fireReady = ship.lastFire == 0;
			
			dirty = false;
		}
		
		bool LockTarget()
		{
			//TODO: acquire target
			Mothership[] potentialTargets = UnityEngine.Object.FindObjectsOfType<Mothership>();
			if(potentialTargets.Length == 0 || (potentialTargets.Length == 1 && potentialTargets[0].gameObject == gameObject))
			{
				return false;
			}
			target = potentialTargets[0].gameObject == gameObject ? potentialTargets[1].gameObject : potentialTargets[0].gameObject;
			tgtTransform = target.GetComponent<Transform>();
			Debug.Log(string.Format("{0} Target Locked: {1}", gameObject.name, target.name));
			return true;
		}
	}
}
