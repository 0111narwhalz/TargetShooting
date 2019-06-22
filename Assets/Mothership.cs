using UnityEngine;

using TargetShooting;

namespace TargetShooting
{
	//Contains all data about the ship and handles high-level physics.
	public class Mothership : MonoBehaviour
	{
		public int lastFire
		{
			get;
			private set;
		}
		
		public int framesPerShot
		{
			get;
			private set;
		}
		
		public float muzzleVelocity
		{
			get;
			private set;
		}
		
		public float hitpoints
		{
			get;
			private set;
		}
		
		public float maxHitpoints
		{
			get;
			private set;
		}
		
		Rigidbody physics;
		public Vector3 linearControlState;
		public Vector3 angularControlState;
		public bool fire;
		
		void Start()
		{
			Debug.Log(string.Format("Initializing {0} at {1}", gameObject.name, gameObject.transform.position));
			framesPerShot = 100;
			lastFire = 0;
			muzzleVelocity = 10;
			physics = gameObject.GetComponent<Rigidbody>();
			linearControlState = Vector3.zero;
			angularControlState = Vector3.zero;
			fire = false;
			maxHitpoints = 50;
			hitpoints = maxHitpoints;
		}
		
		void FixedUpdate()
		{
			//per-frame maintenance
			//Destroy object when killed
			if(hitpoints <= 0)
			{
				Explode();
				return;
			}
			//Decrement reload timer
			if(lastFire > 0)
			{
				lastFire--;
			}
			//Make sure the Rigidbody reference is still good, just in case
			if(physics == null)
			{
				physics = gameObject.GetComponent<Rigidbody>();
			}
			
			//Respond to control input
			//Weapon firing
			if(fire && lastFire <= 0)
			{
				Fire();
				lastFire = framesPerShot;
			}
			
			//Thrust
			physics.AddRelativeForce(linearControlState);
			
			//Torque
			physics.AddRelativeTorque(angularControlState);
		}
		
		void OnCollisionEnter(Collision other)
		{
			Debug.Log(string.Format("{0} hit by {1} for {2}", gameObject.name, other.collider.gameObject.name, CalculateDamage(other)));
			hitpoints -= CalculateDamage(other);
			Debug.Log(string.Format("Remaining hitpoints: {0}", hitpoints));
		}
		
		void Explode()
		{
			Debug.Log(string.Format("{0} killed", gameObject.name));
			GameObject.Destroy(gameObject);
		}
		
		float CalculateDamage(Collision coll)
		{
			return coll.relativeVelocity.magnitude;
		}
		
		void Fire()
		{
			Debug.Log(string.Format("Firing from {0} at {1}", gameObject.name, gameObject.transform.position));
			Projectile.CreateNew(gameObject, gameObject.transform.forward, muzzleVelocity);
		}
	}
}
