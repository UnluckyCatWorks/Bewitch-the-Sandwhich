using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Supply : BInteractable
{
	public GameObject prefab;
	public override void Action (BCharacter player)
	{
		// Instantiate prefab
		var go = Instantiate(prefab, transform.position, Quaternion.identity);
		player.gobj = go.GetComponent<BGrabbableObject> ();
		player.gobj.body.isKinematic = true;
	}

	public override PlayerIsAbleTo CheckInteraction (BCharacter player) 
	{
		// Can only grab things if nothing on hand already
		if (player.gobj != null) return PlayerIsAbleTo.None;

		// If everything's fine
		return PlayerIsAbleTo.Action;
	}
}
