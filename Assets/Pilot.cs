using UnityEngine;

using TargetShooting;

namespace TargetShooting
{
	//Placeholder controller; will be used as template for later controllers.
	public class Pilot : MonoBehaviour
	{
		Mothership ship;
		
		void Start()
		{
			//actual important start stuff
			ship = gameObject.GetComponent<Mothership>();
			
			//placeholder control stuff
			Debug.Log("Initializing pilot");
		}
		
		void FixedUpdate()
		{
			//actual important per-frame stuff
			
			//placeholder control stuff
			ship.fire = true;
		}
	}
}
