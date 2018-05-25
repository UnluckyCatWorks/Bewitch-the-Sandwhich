using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WetDeath : Game
{
	#region DATA
	[Header ("References")]
	public Transform isleRotator;

	[Header ("Settings")]
	public float startRotation;
	public float rotationIncrement;
	[Space]
	public int maxWeapons;
	public float spawnRate;

	public static float rotatorSpeed;
	internal List<WeaponSupply> supplies;
	private float nextSpawnTime;

	public enum Scores 
	{
		Kills,
		ThrowHits,
		DashHits
	}
	#endregion

	#region CALLBACKS
	protected override IEnumerator Logic () 
	{
		// Reset rotation
		rotatorSpeed = startRotation;

		while (true) 
		{
			if (Grabbable.globalCount < maxWeapons &&
				Time.time >= nextSpawnTime)
			{
				// Select random ingredient
				int spawn;
				do spawn = Random.Range (0, supplies.Count);
				while (supplies[spawn].ingredient);

				int ingredient = Random.Range (1, (int) IngredientID.Count);
				int type = Random.Range (0, (int)IngredientType.Count);
				if (type == (int)IngredientType.Molido) type = 0;

				// Actually spawn & update timer
				supplies[spawn].Spawn ((IngredientID)ingredient, (IngredientType)type);
				nextSpawnTime = Time.time + spawnRate;

				// With a puff
				var puff = Instantiate (supplies[spawn].ingredient.puff, supplies[spawn].transform);
				Destroy (puff, 2f);
				puff.Play (true);
			}

			// Rotate isle
			isleRotator.Rotate (Vector3.up, rotatorSpeed * Time.deltaTime);
			yield return null;
		}
	}

	public override IEnumerator ResetStage () 
	{
		HPTracker.trackers.ForEach (t=> t.StartCoroutine(t.Start ()));
		yield break;
	}

	public override void OnAwake () 
	{
		HPTracker.trackers = FindObjectsOfType<HPTracker> ().ToList ();
		supplies = isleRotator.GetComponentsInChildren<WeaponSupply> ().ToList ();
	}
	#endregion
}
