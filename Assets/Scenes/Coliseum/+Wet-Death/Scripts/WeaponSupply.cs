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
		// Can't grab air
		if (ingredient == null) return false;
		// Can only grab things if nothing on hand already
		if (player.toy != null) return false;

		// If everything's fine
		return true;
	}
	#endregion

	#region CALLBACKS
	private void LateUpdate () 
	{
		// Keep position & rotation
		if (ingredient) ingredient.helper.LateUpdate ();
	}

	public void Spawn (IngredientID ingredient, IngredientType type) 
	{
		// Instantiate prefab
		var prefab = Resources.Load<Ingredient> ("Prefabs/Ingredients/" + ingredient.ToString ());
		this.ingredient = Instantiate (prefab, transform);
		this.ingredient.Process (type);

		// Correct behaviour
		this.ingredient.helper.enabled = false;
		marker = this.ingredient.helper.marker;
		this.ingredient.body.interpolation = RigidbodyInterpolation.None;
		this.ingredient.colliders.ForEach (c=> c.enabled = false);
		this.ingredient.helper.colliders.ForEach (c => c.enabled = false);

		// Relocate
		this.ingredient.transform.localPosition = (Vector3.up * 0.3f);
		this.ingredient.transform.localScale *= 1.75f;
	}
	#endregion
}
