using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Supply : Interactable
{
	public IngredientID ingredient;

	public override void Action (Character player)
	{
		// Instantiate prefab
		var prefab = Resources.Load<Grabbable> ("Ingredients/" + ingredient.ToString ());
		var go = Instantiate(prefab, transform.position, Quaternion.identity);
		player.grab = go.GetComponent<Grabbable> ();
		player.grab.body.isKinematic = true;
	}

	public override PlayerIsAbleTo CheckInteraction (Character player) 
	{
		// Can only grab things if nothing on hand already
		if (player.grab != null) return PlayerIsAbleTo.None;

		// If everything's fine
		return PlayerIsAbleTo.Action;
	}
}
