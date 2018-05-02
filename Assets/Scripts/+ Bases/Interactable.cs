using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public abstract class Interactable : MonoBehaviour
{
	#region DATA
	// Marker is always a separated object
	internal Marker marker;
	#endregion

	#region INTERACTION
	public abstract void Action (Character player);
	public abstract bool CheckInteraction (Character player);
	#endregion

	#region CALLBACKS
	protected virtual void Awake () 
	{
		// Find marker
		marker = GetComponentInChildren<Marker> (true);
		// If can't find it, proabbly it's inside a wrapper
		if (!marker)
		{
			var parent = transform.parent;
			marker = parent.GetComponentInChildren<Marker> ();
		}
	}
	#endregion
}
