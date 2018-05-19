using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Span = System.TimeSpan;

public class EnchantedWeather : Game 
{
	#region DATA
	[Header ("Game settings")]
	public float roundDuration;
	public EWCauldron cauldronPrefab;
	public Text timer;

	[Header ("Isle settings")]
	public Transform isleRotator;
	public Vector2 rotatorSpeed;
	public Vector2 isleSpeed;

	[Header ("Rain Settings")]
	public Vector4 rainRates;
	public float objectsDuration;

	internal Span time;
	internal bool playersHaveCauldron;
	private static List<Ingredient> spawnedStuff = new List<Ingredient> ();
	#endregion

	#region CALLBACKS
	protected override IEnumerator Logic () 
	{
		float nextRainTime = 0f;
		time = Span.FromSeconds (roundDuration);

		if (!playersHaveCauldron)
		{
			// Spawn Cauldrons for the players
			var ps = FindObjectsOfType<Character> ();
			foreach (var p in ps)
			{
				// Avoid any player interaction
				p.AddCC ("EW", Locks.Interaction);
				p.simulateCarrying = true;

				// Add them their cauldron
				var cauldron = Instantiate (cauldronPrefab);
				cauldron.SetUpFor (p);
			}
			playersHaveCauldron = true;
		}

		while (time.TotalSeconds > 0f)
		{
			float value = 1f - (float) time.TotalSeconds / roundDuration;
			value = Mathf.Pow (value, 2.5f);

			// Make isle rotate
			float rotator = Mathf.Lerp (rotatorSpeed.x, rotatorSpeed.y, value);
			float isle = Mathf.Lerp (isleSpeed.x, isleSpeed.y, value);

			isleRotator.Rotate (Vector3.up, rotator * Time.deltaTime);
			isleRotator.GetChild (0).Rotate (Vector3.up, -isle * Time.deltaTime);

			#region RAIN LOOP
			// Make it rain baby
			if (Time.time > nextRainTime)
			{
				// Select rain point
				var spawnPos = Random.insideUnitSphere * /*coliseum radius*/ 13f;
				spawnPos.y = 25f;

				// Select ingredient and state
				int ingredientToSpawn = Random.Range (0, ingredients.Length);
				IngredientType ingredientState;
				do
				{
					int state = Random.Range (0, (int) IngredientType.Count);
					ingredientState = (IngredientType) state;
				}
				// Skip "molido" porque no existe
				while (ingredientState == IngredientType.Molido);

				// Spawn ingredient
				var ig = Instantiate (ingredients[ingredientToSpawn]);
				ig.Process (ingredientState);

				// Randomize transform
				ig.transform.rotation = Random.rotation;
				ig.transform.position = spawnPos;
				ig.body.isKinematic = false;

				// Destroy after delay to avoid accumlation
				ig.Destroy (objectsDuration);

				// Register for final clean-up
				spawnedStuff.Add (ig);

				// Get next rain time
				NextRainTime (out nextRainTime);
			}
			#endregion

			yield return null;
			time -= Span.FromSeconds (Time.deltaTime);
			timer.text = string.Format ("{0:0}:{1:00}", time.Minutes, time.Seconds);
		}
	}

	public override IEnumerator ResetStage () 
	{
		EWCauldron.scores = new int[2];

		throw new System.Exception ();
	}
	#endregion

	void NextRainTime (out float rainTime) 
	{
		float delay = rainRates[Random.Range (0, 4)];
		rainTime = Time.time + delay;
	}
}
