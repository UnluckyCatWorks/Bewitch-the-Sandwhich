using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabHelper : Interactable
{
	[NonSerialized]
	public Grabbable parent;

	#region INTERACTION
	public override void Action (Character player) 
	{
		/// Make player grab this object
		parent.body.isKinematic = true;
		player.grab = parent;
		/// Disable to improve performance
		enabled = false;
	}

	public override bool CheckInteraction (Character player) 
	{
		/// Can only grab object if nothing on hand already
		return (player.grab == null && !parent.body.isKinematic && enabled);
	}
	#endregion

	private void Update ()
	{
		/// Keep helper always with parent & un-rotated
		transform.position = parent.body.worldCenterOfMass;
		transform.rotation = Quaternion.identity;
	}
}
