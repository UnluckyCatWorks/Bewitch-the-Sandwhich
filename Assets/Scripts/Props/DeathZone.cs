using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
	public ParticleSystem fx;
	private const float stun = 1f;

	private void OnTriggerEnter (Collider other) 
	{
		if (other.tag == "Player")
		{
			other.transform.position = other.GetComponent<Character> ().lastAlivePos + (Vector3.up * 0.8f);
			var puff = Instantiate (fx, other.transform.position, Quaternion.identity);
			puff.Play ();

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
