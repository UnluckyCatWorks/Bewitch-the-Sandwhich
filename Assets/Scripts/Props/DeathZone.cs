using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathZone : MonoBehaviour
{
	public ParticleSystem fx;
	private const float Stun = 0.25f;

	private void OnTriggerEnter (Collider other) 
	{
		if (other.tag == "Player")
		{
			// Lock player until he hits floor
			var p = other.GetComponent<Character> ();
			p.AddCC ("Dead-stun", Locks.All, Locks.All, Stun);
			p.Respawn ();

			var puff = Instantiate (fx, p.transform.position, Quaternion.identity);
			Destroy (puff.gameObject, 2f);
			puff.Play ();

			// Destroy his grabbed Object, if any
			if (p.toy) 
			{
				// Object will be destroyed when it enters the zone
				p.toy.Throw (Vector3.up * -8f, null);
				p.toy = null;
			}
		}
		else
		// If a grabbable object enters death-zone
		if (other.tag == "Grabbable")
		{	
			var g = other.GetComponentInParent<Grabbable> ();
			g.Destroy (1f);
		}
	}
}
