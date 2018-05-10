using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
	#region DATA
	public const uint globalLimit = 10;
	public static uint globalCount;

	internal Rigidbody body;
	internal GrabHelper helper;         // Helps keeping the marker correct

	internal Character throwerPlayer;   // The character that threw it
	internal bool beingThrown;          // Otherwise just falling, bruh 
	#endregion

	#region UTILS
	// Should be used to make a
	// character grab an object, ALWAYS
	public void GrabFor (Character grabber) 
	{
		grabber.toy = this;
		// Disable helper as it can't be grabbed
		helper.enabled = false;
		// Make kinematic as it should have physics
		body.isKinematic = true;

		// Reset values just-in-case
		beingThrown = false;
		throwerPlayer = null;
	}

	// Should be used to throw the object, ALWAYS
	public void Throw (Vector3 force, Character owner) 
	{
		body.isKinematic = false;
		// Apply forces
		body.velocity = Vector3.zero;
		body.AddForce (force - Vector3.up, ForceMode.Impulse);
		body.AddTorque (force.magnitude * Vector3.up, ForceMode.Impulse);

		// It's only throwed if someone intended to
		if (owner)
		{
			owner.toy = null;
			beingThrown = true;
			// Keep record of who threw it
			throwerPlayer = owner;
		}
		else beingThrown = false;

		/// Enable helper
		helper.enabled = true;
	}

	// Destroy both object & its helper
	public void Destroy (float delay = 0f) 
	{
		if (delay == 0) Destroy (gameObject);
		else			Destroy (gameObject, delay);

		Destroy (helper.gameObject);
	}
	#endregion

	#region CALLBACKS
	protected virtual void OnCollisionEnter (Collision col) 
	{
		// If it's not being thrown,
		// just collide & chill
		if (!beingThrown) return;

		// If hit a player
		var victim = col.gameObject.GetComponent<Character> (); if (!victim) return;
		// Be sure it's not the same player who threw it!
		if (victim.ID == throwerPlayer.ID) return;

		// Knock player & stop being a flying weapon
		victim.Knock (body.velocity.normalized, 0.20f);
		beingThrown = false;

		// Mark tutorial check
		Tutorial.SetCheckFor (throwerPlayer.ID, Tutorial.Phases.Throwing_Stuff, true);
	}

	protected virtual void Awake () 
	{
		globalCount++;
		body = GetComponent<Rigidbody> ();

		// Instantiate Grab Helper & set it up
		var helper = Resources.Load<GrabHelper> ("Prefabs/Grab_Helper");
		helper = Instantiate (helper);
		helper.marker = helper.GetComponentInChildren<Marker> ();

		// Set up references
		helper.parent = this;
		this.helper = helper;
	}

	private void OnDestroy () 
	{
		globalCount--;
	}
	#endregion
}
