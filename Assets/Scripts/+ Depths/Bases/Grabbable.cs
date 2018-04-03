using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
	[NonSerialized] public Rigidbody body;
	[NonSerialized] public GrabHelper helper;

	#region UTILS
	[NonSerialized] public bool beingThrown;
	[NonSerialized] public Character throwerPlayer;

	public void Throw (Vector3 direction, float force, Character owner)
	{
		body.isKinematic = false;
		body.velocity = Vector3.zero;
		body.AddForce (force * direction - Vector3.up, ForceMode.Impulse);
		body.AddTorque (force * Vector3.up, ForceMode.Impulse);
		/// It's only throwed if someone intended to
		beingThrown = (owner != null);
		/// Keep record of who throw it
		throwerPlayer = owner;
		/// Enable helper
		helper.enabled = true;
	}

	public void Destroy (float delay = 0f) 
	{
		if (delay != 0) Destroy (gameObject, delay);
		else Destroy (gameObject);
		Destroy (helper.gameObject);
	}
	#endregion

	#region CALLBACKS
	protected virtual void OnCollisionEnter (Collision col) 
	{
		if (!beingThrown) return;
		/// If hit a player
		var p = col.gameObject.GetComponent<Character> ();
		/// Be sure it's not the same player that threw it!
		if (!p) return;
		if (p.id == throwerPlayer.id) return;

		/// Knock player & stop knocing mode
		p.Knock (body.velocity.normalized);
		beingThrown = false;
	}

	protected virtual void Awake () 
	{
		body = GetComponent<Rigidbody> ();
		/// Instantiate Grab Helper
		var helper = Resources.Load<GrabHelper> ("Prefabs/Grab_Helper");
		helper = Instantiate (helper);
		/// Find Marker
		helper.marker = helper.GetComponentInChildren<Marker> ();
		/// Set up references
		helper.parent = this;
		this.helper = helper;
	} 
	#endregion
}
