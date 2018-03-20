using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
	[NonSerialized]
	public Rigidbody body;

	[NonSerialized]
	public bool beingThrown;
	public void Throw (Vector3 direction, float force)
	{
		body.isKinematic = false;
		body.velocity = Vector3.zero;
		body.AddForce(force * direction, ForceMode.Impulse);
		beingThrown = true;
	}

	protected virtual void OnCollisionStay (Collision col) 
	{
		// When hitting a player
		if (col.gameObject.tag == "Player")
		{
			var player = col.gameObject.GetComponent<Character>();
			// If thrown to, kncok them back
			if (beingThrown)
			{
				// Calculate knock direction
				var knock = player.transform.position - col.contacts[0].point;
				knock.y = 0;
				knock.Normalize();

				// Knock player
				player.StartCoroutine(player.Knock(1f, knock * 20f));
				beingThrown = false;
			}

			// If not, pull object back
			else
			{
				var force = player.movingSpeed;
				body.AddForce(force, ForceMode.Force);
			}
		}
		// Otherwise, just collide 
		else beingThrown = false;
	}

	protected virtual void Awake () 
	{
		body = GetComponent<Rigidbody> ();
	}
}
