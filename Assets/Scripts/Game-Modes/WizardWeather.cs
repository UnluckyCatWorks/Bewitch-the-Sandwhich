using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Span = System.TimeSpan;

public class WizardWeather : Game 
{
	#region DATA
	[Header ("Game settings")]
	public float roundDuration;
	public WWCauldron cauldronPrefab;
	public List<Text> playerScores;
	public Text timer;

	[Header ("Isle settings")]
	public Transform isleRotator;
	public Vector2 rotatorSpeed;
	public Vector2 isleSpeed;

	[Header ("Rain Settings")]
	public Vector4 rainRates;

	internal Span time;
	private static List<Ingredient> spawnedStuff;
	#endregion

	#region CALLBACKS
	protected override IEnumerator Logic () 
	{
		float nextRainTime = 0f;
		time = Span.FromSeconds (roundDuration);

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
			if (Time.time > nextRainTime && 
				Grabbable.globalCount < Grabbable.globalLimit) 
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
				ig.gameObject.AddComponent<IngredientScaler> ();
				ig.Process (ingredientState);
				ig.Throw (-Vector3.up, null, true);

				// Randomize transform
				ig.transform.rotation = Random.rotation;
				ig.transform.position = spawnPos;

				// Set physics up
				ig.body.drag = 1f;
				ig.body.isKinematic = false;
				ig.body.interpolation = RigidbodyInterpolation.None;

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

		var winner = WWCauldron.GetWinner ();
		DeclareWinner (winner);
	}

	public override IEnumerator ResetStage () 
	{
		WWCauldron.scores = new int[2];
		playerScores.ForEach (s => s.text = "00");

		foreach (var i in spawnedStuff) 
		{
			if (i && i.isActiveAndEnabled)
				i.Destroy ();
		}
		spawnedStuff.Clear ();
		yield return new WaitForSeconds (1f);
	}

	public override void OnAwake () 
	{
		spawnedStuff = new List<Ingredient> ();

		// Spawn Cauldrons for the players
		var ps = FindObjectsOfType<Character> ();
		foreach (var p in ps)
		{
			// Avoid any player interaction
			p.AddCC ("Enchanted-Cauldron", Locks.Interaction);
			p.simulateCarrying = true;

			// Add them their cauldron
			var cauldron = Instantiate (cauldronPrefab);
			cauldron.SetUpFor (p);
		}
		playerScores[0].color = Player.all[0].character.focusColor;
		playerScores[1].color = Player.all[1].character.focusColor;
		playerScores.ForEach (s => s.text = "00");
	}
	#endregion

	void NextRainTime (out float rainTime) 
	{
		float delay = rainRates[Random.Range (0, 4)];
		rainTime = Time.time + delay;
	}
}
