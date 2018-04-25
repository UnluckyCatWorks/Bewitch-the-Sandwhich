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
			/// Lock player until he hits floor
			var p = other.GetComponent<Character> ();
			p.AddCC ("Dead-stun", Locks.All, stun);

			p.transform.position = p.valid.position + (Vector3.up * 0.8f);
			var puff = Instantiate (fx, p.transform.position, Quaternion.identity);
			Destroy (puff.gameObject, 2f);
			puff.Play ();

			/// Destroy his grabbed Object, if any
			if (p.toy)
			{
				/// Object will be destroyed when it enters the zone
				p.toy.Throw (Vector3.up, -8f, null);
				p.toy = null;
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
