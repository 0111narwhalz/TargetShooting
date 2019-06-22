using UnityEngine;

using TargetShooting;

namespace TargetShooting
{
	//Manages the projectile itself. Primary purpose appears to be deleting itself.
	public class Projectile : MonoBehaviour
	{
		int lifespan;
		
		void Start()
		{
			lifespan = 1000;
		}
		
		void FixedUpdate()
		{
			if(lifespan-- <= 0)
			{
				GameObject.Destroy(gameObject);
			}
		}
		
		void OnCollisionEnter()
		{
			GameObject.Destroy(gameObject);
		}
		
		public static void CreateNew(Vector3 worldPosition, Vector3 worldVelocity)
		{
			GameObject projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			projectile.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
			projectile.transform.position = worldPosition;
			projectile.AddComponent<Projectile>();
			projectile.AddComponent<Rigidbody>();
			Rigidbody projRBody = projectile.GetComponent<Rigidbody>();
			projRBody.useGravity = false;
			projRBody.velocity = worldVelocity;
		}
		
		public static void CreateNew(GameObject emitter, Vector3 worldDirection, float speed)
		{
			Vector3 offset = worldDirection;
			Rigidbody rbody = emitter.GetComponent<Rigidbody>();
			Vector3 worldVelocity = worldDirection * speed;
			worldVelocity += rbody.velocity;
			CreateNew(emitter.transform.position + offset, worldVelocity);
		}
	}
}
