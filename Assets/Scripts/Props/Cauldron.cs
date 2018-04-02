using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cauldron : Interactable
{
	[Header ("Cauldron Settings")]
	public int playerOwner;
	public float cookTime;
	public float safeTime;
	public float burnTime;

	private Timer timer;
	private List<IngredientInfo> currentMix;

	#region INTERACTION
	public override void Action (Character player)
	{
		/// If player is dropping
		if (player.grab != null)
		{
			/// Get ingredient from player
			var ingredient = player.grab as Ingredient;
			player.grab = null;

			/// Add ingredient & restart time
			StartCoroutine ( Add (ingredient) );
			clock = 0f;
		}
	}

	public override bool CheckInteraction (Character player) 
	{
		/// If not valid Player
		if (player.id != playerOwner) return false;

		/// If player is dropping
		if (player.grab)
		{
			/// Check if it's an ingredient
			if ((player.grab as Ingredient) == null) return false;
			/// Can't add more than 4 ingredients
			// ???
			if (currentMix.Count == 4) return false;
		}
		else return false;

		/// If everything's ok
		return true;
	}
	#endregion

	#region UTILITIES
	public int FindPotion () 
	{
		var rs = OrderMaster.recipes;

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
	private enum Theme 
	{
		NotCooked,
		Cooked,
		Overcooked
	}

	private void Clear ()
	{
		timer.ChangeTo (Theme.NotCooked);
		currentMix.Clear ();
		StopCoroutine (process);
		process = null;
		clock = 0;
	}

	IEnumerator Add (Ingredient ig)
	{
		/// Restart cooking process
		if (process != null)
		{
			StopCoroutine (process);
			timer.ChangeTo (Theme.NotCooked);
		}
		process = StartCoroutine (Cooking ());

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
		Destroy (ig.gameObject);
	}
	#endregion

	[NonSerialized]
	public float clock;
	private Coroutine process;
	private IEnumerator Cooking () 
	{
		clock = 0f;
		// Wait until is cooked
		while (clock <= cookTime)
		{
			if (!Game.paused)
			{
				clock += Time.deltaTime;
				timer.SetSlider (clock / cookTime);
			}
			yield return null;
		}

		// Change phase && wait a bit
		timer.ChangeTo (Theme.Cooked);
		yield return new WaitForSeconds (safeTime);

		// Wait until it's burned
		while (clock <= cookTime + burnTime) 
		{
			if (!Game.paused) 
			{
				clock += Time.deltaTime;
				timer.SetSlider ((clock - cookTime) / burnTime);
			}
			yield return null;
		}
		// If it burns
		timer.ChangeTo (Theme.Overcooked);
		Clear ();
	}

	protected override void Awake () 
	{
		base.Awake ();
		currentMix = new List<IngredientInfo> (4);
		timer = transform.parent.GetComponentInChildren<Timer> (true);
	}
}
