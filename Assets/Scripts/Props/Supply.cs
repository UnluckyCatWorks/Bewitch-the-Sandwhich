using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Supply : Interactable
{
	public IngredientID ingredient;

	public override void Action (Character player) 
	{
		// Instantiate prefab
		var prefab = Resources.Load<Grabbable> ("Prefabs/Ingredients/" + ingredient.ToString ());
		var go = Instantiate(prefab, transform.position, Quaternion.identity);
		player.toy = go.GetComponent<Grabbable> ();
		player.toy.body.isKinematic = true;

		// Register spawned ingredient
		if (Game.manager is MeltingRace)
			MeltingRace.spawnedIngredients.Add (prefab);
	}

	public override bool CheckInteraction (Character player) 
	{
		// Can only grab things if nothing on hand already
		if (player.toy != null) return false;
		// Can't grab infinite amount of stuff
		if (Grabbable.globalCount >= Grabbable.globalLimit) return false;

		// If everything's fine
		return true;
	}
}
