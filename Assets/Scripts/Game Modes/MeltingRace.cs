using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Span = System.TimeSpan;

public class MeltingRace : Game 
{
	#region DATA
	[Header ("Game settings")]
	public float maxTime;
	public Transform isle;
	public Text timer;

	[Header ("Rain Settings")]
	public float maxRadius;
	public float rainRate;
	public float objectsDuration;

	internal Span time;
	internal List<Ingredient> spawnedIngredients;
	#endregion

	#region CALLBACKS
	protected override IEnumerator Logic ()
	{
		// Start cauldron
		MRCauldron.GetStarted ();

		float nextRainTime = 0f;
		time = Span.FromSeconds (maxTime);
		while (time.TotalSeconds > 0f)
		{
			// Hacer la islita pequeña
			float value = (float)time.TotalSeconds / maxTime;
			value = Mathf.Lerp (0.43f, 1f, value);
			isle.localScale = new Vector3 (value, 1f, value);

			#region RAIN LOOP
			// Make it rain baby
			if (Time.time > nextRainTime)
			{
				// Raining inside isle only 20% of the time
				bool spawnOptimal = Random.Range (0f, 100f) <= 20f;
				var spawnPos = spawnOptimal ?
					Random.insideUnitSphere * maxRadius * (value)
					:
					Random.insideUnitSphere * maxRadius;
				// Correct it
				spawnPos.z += isle.position.z;
				spawnPos.y = 35;

				// 50% additional change to get the correct ingredient if spawned on the isle
				int ingredientToSpawn = spawnOptimal &&
					Random.Range (0f, 100f) <= 50f ?
					(int) MRCauldron.target - 1
					:
					Random.Range (0, ingredients.Length);

				// Actually rain
				var ig = Instantiate (ingredients[ingredientToSpawn]);
				ig.transform.rotation = Random.rotation;
				ig.transform.position = spawnPos;
				ig.body.isKinematic = false;
				// Destroy after delay to avoid accumlation
				ig.Destroy (objectsDuration);

				// Get next rain time
				NextRainTime (out nextRainTime);
			}
			#endregion

			yield return null;
			time -= Span.FromSeconds (Time.deltaTime);
			timer.text = string.Format ("{0:00}:{1:00}", time.Minutes, time.Seconds);
		}

		// Declare winner
		var winner = MRCauldron.GetWinner ();
		DeclareWinner (winner);
	}

	public override IEnumerator ResetStage () 
	{
		MRCauldron.scores = new int[2];
		spawnedIngredients.ForEach(i=> i.Destroy ());

		// Turn isle to it's original size
		float factor = 0f;
		while (factor <= 1.1f) 
		{

		}

		value = Mathf.Lerp (0.43f, 1f, value);
		isle.localScale = new Vector3 (value, 1f, value);
	}
	#endregion

	void NextRainTime (out float rainTime) 
	{
		float delay = Random.Range (rainRate * 0.2f, rainRate * 1.8f);
		rainTime = Time.time + delay;
	}
}
