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

	}

	protected virtual void Awake () 
	{
		body = GetComponent<Rigidbody> ();
	}
}
