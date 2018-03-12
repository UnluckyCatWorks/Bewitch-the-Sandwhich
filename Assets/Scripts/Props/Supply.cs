using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Supply : Interactable
{
	public GameObject prefab;
	public override void Action (Character player)
	{
		// Instantiate prefab
		var go = Instantiate(prefab, transform.position, Quaternion.identity);
		player.gobj = go.GetComponent<Grabbable> ();
		player.gobj.body.isKinematic = true;
	}

	public override PlayerIsAbleTo CheckInteraction (Character player) 
	{
		// Can only grab things if nothing on hand already
		if (player.gobj != null) return PlayerIsAbleTo.None;

		// If everything's fine
		return PlayerIsAbleTo.Action;
	}
}
