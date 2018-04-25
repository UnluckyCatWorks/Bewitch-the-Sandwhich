using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cauldron : Interactable
{
	/// For alpha
	public static bool over;

	[Header ("Cauldron Settings")]
	public int playerOwner;
	public float cookTime;
	public float safeTime;
	public float burnTime;

	private List<IngredientInfo> currentMix;

	#region INTERACTION
	public override void Action (Character player)
	{
		/// If player is dropping
		if (player.toy != null)
		{
			/// Get ingredient from player
			var ingredient = player.toy as Ingredient;
			player.toy = null;

			/// Add ingredient & restart time
			StartCoroutine ( Add (ingredient) );
		}
	}

	public override bool CheckInteraction (Character player) 
	{
		/// Is player valid?
		if (player.id != playerOwner) return false;

		/// Is player dropping an ingredient?
		if ((player.toy as Ingredient) == null) return false;

		// For alpha => Only processed ingredients
		if ((player.toy as Ingredient).type == IngredientType.TALCUAL) return false;

		/// If everything's ok
		return true;
	}
	#endregion

	#region UTILITIES
	public int FindPotion () 
	{
		Recipe[] rs = null; // OrderMaster.recipes;

		// Check against all craftable potions
		for (var i = 1; i != rs.Length; i++)
		{
			// Make a copy of current mix
			var mix = new List<IngredientInfo> (currentMix);
			for (var m = 0; m != rs[i].info.Length; m++)
			{
				// Check against every ingredient of the potion receipt
				var ingredient = rs[i].info[m];
				var match = mix.IndexOf (ingredient);

				// If found a match remove it from copy
				if (match != -1) mix.RemoveAt (match);
				// If haven't found a match break loop
				else break;
			}
			// If original count matches, and all elements
			// have been checked, means this potion is the good one
			if (mix.Count == 0 && currentMix.Count == rs[i].info.Length)
				return i;
		}

		// If loops end, no potion is valid
		return -1;
	}

	private Grabbable InstantiatePotion ( int recipe ) 
	{
		// TODO
		//throw new NotImplementedException ();
		return null;
	}
	#endregion

	#region COOKING
	IEnumerator Add (Ingredient ig)
	{
		/// Drop inside cauldron
		var factor = 0f;
		while (factor <= 1f)
		{
			var newPos = Vector3.Lerp (ig.transform.position, transform.position, factor);
			ig.transform.position = newPos;

			factor += Time.deltaTime * 5f;
			yield return null;
		}

		/// Update UI
		// - ???

		/// Add to mix
		currentMix.Add (ig.info);
		ig.Destroy ();

		// For alpha => PROCLAIM WINNER
		if (!over)
		{
			GameObject.Find (playerOwner.ToString ()).GetComponent<UnityEngine.UI.Text> ().enabled = true;
			over = true;
		}
	}
	#endregion

	protected override void Awake () 
	{
		base.Awake ();
		currentMix = new List<IngredientInfo> (4);
	}
}
