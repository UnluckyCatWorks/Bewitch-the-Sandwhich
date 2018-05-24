using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSupply : Interactable 
{
	internal Ingredient ingredient;

	#region INTERACTABLE
	public override void Action (Character player) 
	{
		// Transfer ownership to player
		ingredient.transform.SetParent (null, true);
		ingredient.GrabFor (player);
		ingredient = null;
	}

	public override bool CheckInteraction (Character player) 
	{
		// Can only grab things if nothing on hand already
		if (player.toy != null) return false;

		// If everything's fine
		return true;
	}
	#endregion

	#region CALLBACKS
	private void Spawn (IngredientID ingredient, IngredientType type) 
	{
		// Instantiate prefab
		var prefab = Resources.Load<Ingredient> ("Prefabs/Ingredients/" + ingredient.ToString ());
		this.ingredient = Instantiate (prefab, transform);
		this.ingredient.Process (type);
	}
	#endregion
}
