using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cauldron : BInteractable
{
	[Header ("Cauldron Settings")]
	public int playerOwner;
	public float cooktime;
	public float burntime;
	public List<IngredientInfo> currentMix;
	// Blockout
	public SpriteRenderer[] icons;
	public Renderer mesh;
	public AnimationCurve colorCurve;

	#region INTERACTION
	public override void Action (BCharacter player)
	{
		// If player is dropping
		if (player.gobj != null)
		{
			var ingredient = player.gobj as Ingredient;
			StartCoroutine ( Add (ingredient) );
			player.gobj = null;
			timer = 0;
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

	public override void Special (BCharacter player)
	{
		// Cool down the cauldron
		timer = 0;
	}

	public override PlayerIsAbleTo CheckInteraction (BCharacter player) 
	{
		// If not valid Player
		if (player.id != playerOwner)
			return PlayerIsAbleTo.None;

		var action = true;
		var special = true;

		// If player is dropping
		if (player.gobj != null)
		{
			// Check if it's an ingredient
			if ((player.gobj as Ingredient) == null)
				action = false;

			// Can't add more than 4 ingredients
			if (currentMix.Count == 4)
				action = false;
		}

		else
		{
			// Check if actual ingredients in
			// and if time is good for picking
			if (currentMix.Count == 0 || timer < cooktime)
				action = false;
		}

		// Can't cooldown if nothing in
		if (currentMix.Count == 0)
			special = false;

		return Result (action, special);
	}
	#endregion

	public Potion FindPotion () 
	{
		var ps = Game.manager.potions;
		// Check against all craftable potions (0 is reserverd)
		for (var i=1; i!=ps.Length; i++)
		{
			// Make a copy of current mix
			var mix = new List<IngredientInfo> (currentMix);
			for (var m=0; m!=ps[i].receipt.Length; m++)
			{
				// Check against every ingredient of the potion receipt
				var ingredient = ps[i].receipt[m];
				var match = mix.IndexOf(ingredient);

				// If found a match remove it from copy
				if (match != -1) mix.RemoveAt(match);
				// If haven't found a match break loop
				else break;
			}
			// If original count matches, and all elements
			// have been checked, means this potion is the good one
			if (mix.Count == 0 && currentMix.Count == ps[i].receipt.Length)
				return ps[i];
		}

		// If loops end, no potion is valid (return Potion-0)
		return ps[0];
	}

	IEnumerator Add (Ingredient ig) 
	{
		// Drop inside cauldron
		var factor = 0f;
		while (factor <= 1f)
		{
			var newPos = Vector3.Lerp(ig.transform.position, transform.position, factor);
			ig.transform.position = newPos;
			factor += Time.deltaTime * 5f;
			yield return null;
		}
		// Update UI
		var newSprite = ig.icons[(int)ig.type];
		icons[currentMix.Count].sprite = newSprite;
		// Add to mix
		currentMix.Add(ig.info);
		Destroy(ig.gameObject);
	}

	private void Clear () 
	{
		currentMix.Clear ();
		foreach (var s in icons) s.sprite = null;
		mesh.material.color = Color.white;
		timer = 0;
	}

	#region CALLBACKS
	private float timer;
	private void Update ()
	{
		if (currentMix.Count == 0) return;

		Color color;
		if (timer < cooktime && timer >= cooktime * 0.9f)
		{
			var cookFactor = timer / (cooktime * 0.1f);
			color = Color.Lerp(Color.yellow, Color.green, cookFactor * 3f);
		}
		else if (timer > cooktime)
		{
			var burnFactor = colorCurve.Evaluate((timer - cooktime) / burntime);
			color = Color.Lerp(Color.green, Color.red, burnFactor);
		}
		else
		{
			var cookFactor = timer / (cooktime * 0.9f);
			color = Color.Lerp(Color.white, Color.yellow, cookFactor);
		}
		mesh.material.color = color;

		if (timer > cooktime + burntime)
		{
			Debug.Log("Caldero dead");
			Clear();
		}
		else timer += Time.deltaTime;
	}
}

#endregion}
