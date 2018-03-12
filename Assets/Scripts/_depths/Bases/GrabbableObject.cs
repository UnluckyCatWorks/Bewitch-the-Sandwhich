using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Grabbable : MonoBehaviour
{
	[HideInInspector]
	public Rigidbody body;

	[HideInInspector]
	public bool beingThrown;
	public void Throw (Vector3 direction, float force)
	{
		body.isKinematic = false;
		body.velocity = Vector3.zero;
		body.AddForce(force * direction, ForceMode.Impulse);
		beingThrown = true;
	}

	protected virtual void OnCollisionEnter (Collision col)
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
				var force = player.movingSpeed * 0.4f;
				body.AddForce(force, ForceMode.Impulse);
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
