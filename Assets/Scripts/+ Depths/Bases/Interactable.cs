using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public abstract class Interactable : MonoBehaviour
{
	#region DATA
	// Marker is always a separated object
	public Marker marker;
	#endregion

	#region INTERACTION
	public abstract void Action (Character player);
	public abstract bool CheckInteraction (Character player);
	#endregion

	#region CALLBACKS
	protected virtual void Awake () 
	{
		/// Find wrapper
		Transform t;
		if (insideWrapper) t = transform.parent;
		else t = transform;
		marker = t.GetComponentInChildren<Marker> (true);
	}
	#endregion
}
