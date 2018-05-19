using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GrabHelper : Interactable
{
	#region DATA
	internal Grabbable parent;
	#endregion

	#region INTERACTION
	public override void Action (Character player) 
	{
		parent.GrabFor (player);
	}

	public override bool CheckInteraction (Character player) 
	{
		// Can only grab object if nothing on hand already
		// and this helper itself is active
		return (player.toy == null && enabled);
	}
	#endregion

	#region CALLBACKS
	private void Update () 
	{
		// Keep helper always at ~= parent's lowest point
		transform.position = parent.body.worldCenterOfMass - (Vector3.up * 0.5f);
		// Keep always unrotated
		transform.rotation = Quaternion.identity;
	}
	#endregion
}
