using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cauldron : Interactable
{
	[Header ("Cauldron Settings")]
	public Timer timer;
	public int playerOwner;
	public float cookTime;
	public float safeTime;
	public float burnTime;

	private List<IngredientInfo> currentMix;

	#region INTERACTION
	public override void Action (Character player)
	{
		// If player is dropping
		if (player.gobj != null)
		{
			// Get ingredient from player
			var ingredient = player.gobj as Ingredient;
			player.gobj = null;

			// Add ingredient and restart time
			StartCoroutine ( Add (ingredient) );
			time = 0f;
		}
		else
		{
			// Grab potion
			var potion = FindPotion();
			player.gobj = Instantiate(potion, transform.position, Quaternion.identity);
			// Clear cauldron
			Clear();
		}
	}

	public override void Special (Character player) 
	{
		// Stop / Resume caludron
		stopped = !stopped;
	}

	public override PlayerIsAbleTo CheckInteraction (Character player) 
	{
		// If not valid Player
		if (player.id != playerOwner) return PlayerIsAbleTo.None;

		var action = true;
		var special = true;
		// If player is dropping
		if (player.gobj != null)
		{
			// Check if it's an ingredient
			if ((player.gobj as Ingredient) == null) action = false;
			// Can't add more than 4 ingredients
			if (currentMix.Count == 4) action = false;
		}

		// Check if actual ingredients in,
		// and if time is good for picking
		else if (currentMix.Count == 0 || time < cookTime) action = false;

		// Can't stop fire if nothing in,
		if (currentMix.Count == 0) special = false;

		return Result (action, special);
	}
	#endregion

	#region UTILITIES
	private enum Theme 
	{
		NotCooked,
		Cooked,
		Overcooked
	}

	public Potion FindPotion () 
	{
		var ps = Game.manager.potions;

		// Check against all craftable potions (0 is reserverd)
		for (var i = 1; i != ps.Length; i++)
		{
			// Make a copy of current mix
			var mix = new List<IngredientInfo> (currentMix);
			for (var m = 0; m != ps[i].receipt.Length; m++)
			{
				// Check against every ingredient of the potion receipt
				var ingredient = ps[i].receipt[m];
				var match = mix.IndexOf (ingredient);

				// If found a match remove it from copy
				if (match != -1) mix.RemoveAt (match);
				// If haven't found a match break loop
				else break;
			}
			// If original count matches, and all elements
			// have been checked, means this potion is the good one
			if (mix.Count == 0
				&& currentMix.Count == ps[i].receipt.Length)
				return ps[i];
		}

		// If loops end, no potion is valid (return Potion-0)
		return ps[0];
	}

	private void Clear () 
	{
		timer.ChangeTo (Theme.NotCooked);
		currentMix.Clear ();
		StopCoroutine (process);
		process = null;
		time = 0;
	}

	IEnumerator Add (Ingredient ig) 
	{
		// Restart cooking process
		if (process != null)
		{
			timer.ChangeTo (Theme.NotCooked);
			StopCoroutine (process);
		}
		process = StartCoroutine (Cooking ());

		// Drop inside cauldron
		var factor = 0f;
		while (factor <= 1f)
		{
			var newPos = Vector3.Lerp (ig.transform.position, transform.position, factor);
			ig.transform.position = newPos;
			factor += Time.deltaTime * 5f;
			yield return null;
		}

		// Update UI
		// - ???

		// Add to mix
		currentMix.Add (ig.info);
		Destroy (ig.gameObject);
	}
	#endregion

	private float time;
	private bool stopped;
	private Coroutine process;
	private IEnumerator Cooking () 
	{
		// Wait until is cooked
		while (time <= cookTime)
		{
			if (!stopped)
			{
				time += Time.deltaTime;
				timer.SetSlider (time / cookTime);
			}
			yield return null;
		}

		// Change phase && wait a bit
		timer.ChangeTo (Theme.Cooked);
		yield return new WaitForSeconds (safeTime);

		// Wait until it's burned
		while (time <= cookTime + burnTime)
		{
			if (!stopped)
			{
				time += Time.deltaTime;
				timer.SetSlider ((time - cookTime) / burnTime);
			}
			yield return null;
		}
		// If it burns
		timer.ChangeTo (Theme.Overcooked);
		yield return new WaitForSeconds (1f);
		Clear ();
	}

	protected override void Awake ()
	{
		base.Awake ();
		currentMix = new List<IngredientInfo> (4);
	}
}
