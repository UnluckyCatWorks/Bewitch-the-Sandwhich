using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Supply : Interactable
{
	public IngredientID ingredient;

	public override void Action (Character player) 
	{
		/// Instantiate prefab
		var prefab = Resources.Load<Grabbable> ("Prefabs/Ingredients/" + ingredient.ToString ());
		var go = Instantiate(prefab, transform.position, Quaternion.identity);
		player.toy = go.GetComponent<Grabbable> ();
		player.toy.body.isKinematic = true;
	}

	public override bool CheckInteraction (Character player) 
	{
		/// Can only grab things if nothing on hand already
		if (player.toy != null) return false;

		/// If everything's fine
		return true;
	}
}
