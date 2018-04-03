using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
	public Transform respawn;
	public float stun;

	private void OnTriggerEnter (Collider other) 
	{
		if (other.tag == "Player")
		{
			other.transform.position = respawn.position;
			other.transform.rotation = respawn.rotation;

			/// Lock player until he hits floor
			var p = other.GetComponent<Character> ();
			p.AddCC ("Dead-stun", Locks.All, stun);

			/// Destroy his grabbed Object, if any
			if (p.grab)
			{
				/// Object will be destroyed when it enters the zone
				p.grab.Throw (Vector3.up, -8f, null);
				p.grab = null;
			}
		}
		else
		if (other.tag == "Grabbable")
		{	
			var g = other.GetComponentInParent<Grabbable> ();
			g.Destroy (1f);
		}
	}
}
