using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
	#region DATA
	public const uint globalLimit = 10;
	public static uint globalCount;

	internal Rigidbody body;
	internal GrabHelper helper;
	internal List<Collider> colliders;

	private Character throwerPlayer;
	private bool beingThrown;
	private Vector3 throwDir;

	private ParticleSystem puff;
	#endregion

	#region UTILS
	public void GrabFor (Character grabber) 
	{
		grabber.toy = this;
		body.isKinematic = true;
		body.interpolation = RigidbodyInterpolation.Interpolate;

		throwerPlayer = null;
		beingThrown = false;

		// Disable colliders so the player doesn't start fucking floating
		colliders.ForEach (c=> c.enabled = false);
		helper.enabled = false;
	}

	public void Throw (Vector3 force, Character owner, bool forceThrow = false) 
	{
		body.isKinematic = false;
		body.interpolation = RigidbodyInterpolation.None;

		// Apply forces
		body.velocity = Vector3.zero;
		body.AddForce (force - (Vector3.up * 0.3f), ForceMode.Impulse);
		body.AddTorque (force.magnitude * Vector3.up, ForceMode.Impulse);

		// It's only throwed if someone intended to
		if (owner || forceThrow) 
		{
			if (owner) owner.toy = null;
			throwDir = force.normalized;
			throwerPlayer = owner;
			beingThrown = true;
		}
		else beingThrown = false;

		colliders.ForEach (c=> c.enabled = true);
		helper.enabled = true;
	}
	#endregion

	#region DEATH ITSELF
	public void DestroySilenty () 
	{
		HolyShitJustDestroyThis ();
	}

	public void Destroy (float delay = 0f) 
	{
		StartCoroutine (DestroyAfter (delay));
	}

	IEnumerator DestroyAfter (float delay)
	{
		if (delay != 0f) yield return new WaitForSeconds (delay);
		// Unparent puff effect so it won't be destroyed
		Destroy (puff.gameObject, 1.5f);
		puff.transform.SetParent (null, true);
		puff.Play ();

		// -.-
		HolyShitJustDestroyThis ();
	}

	private void HolyShitJustDestroyThis () 
	{
		StopAllCoroutines ();
		Destroy (helper.gameObject);
		Destroy (gameObject);
	} 
	#endregion

	#region CALLBACKS
	protected virtual void OnCollisionEnter (Collision col) 
	{
		// If it's not being thrown,
		// just collide & chill
		if (!beingThrown) return;

		// If hit a player
		var victim = col.gameObject.GetComponent<Character> ();
		if (!victim) return;
		// Be sure it's not the same player who threw it!
		if (throwerPlayer && victim.ID == throwerPlayer.ID) return;

		// Knock player & stop being a flying weapon
		victim.Knock (throwDir * 1.2f, 1f);
		beingThrown = false;

		// Mark tutorial check
		if (throwerPlayer) Tutorial.SetCheckFor (throwerPlayer.ID, Tutorial.Phases.Throwing_Stuff, true);
	}

	protected virtual void Awake () 
	{
		globalCount++;
		body = GetComponent<Rigidbody> ();
		puff = GetComponentInChildren<ParticleSystem> ();
		colliders = GetComponentsInChildren<Collider> (true).ToList ();

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
