using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
	#region INTERACTION
	public abstract PlayerIsAbleTo CheckInteraction (Character player);

	public abstract void Action (Character player);                     // Pressing Action key
	public virtual void Special (Character player) { /* optional */ }   // Pressing Special key 

	// Helper
	public static PlayerIsAbleTo Result (bool action, bool special) 
	{
		if (action && special) return PlayerIsAbleTo.Both;

		else if (action) return PlayerIsAbleTo.Action;
		else if (special) return PlayerIsAbleTo.Special;

		else return PlayerIsAbleTo.None;
	}
	#endregion

	#region CALLBACKS
	[NonSerialized]
	public Marker marker;
	protected virtual void Awake () 
	{
		marker = GetComponentInChildren<Marker> ();
	}
	#endregion
}
