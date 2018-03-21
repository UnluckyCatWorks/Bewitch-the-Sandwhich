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
			var p = other.GetComponent<Character> ();
			p.AddCC ("Dead-stun", Locks.All, stun);
		}
		else
		if (other.tag == "Grabbable") Destroy (other.gameObject);
	}
}
