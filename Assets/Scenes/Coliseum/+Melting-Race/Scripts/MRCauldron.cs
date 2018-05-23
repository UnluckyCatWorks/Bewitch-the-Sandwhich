using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MRCauldron : Cauldron 
{
	#region DATA
	[Header("Data")]
	public float changeRate;

	[Header ("References")]
	public ParticleSystem puff;
	public Image targetImage;
	public Timer timer;
	public AudioSource correctSound;

	internal static int[] scores;
	internal IngredientID target;

	public static MRCauldron current;
	#endregion

	#region UTILS
	public static int GetWinner () 
	{
		// Stop ingredient loop
		current.StopAllCoroutines ();

		// Player 1 wins
		if (scores[0] > scores[1]) return 1;
		else
		// Player 2 wins
		if (scores[1] > scores[0]) return 2;
		// Empate
		else return 0;
	}

	public void GetStarted () 
	{
		StartCoroutine (LoopTarget ());
	}

	private IEnumerator LoopTarget () 
	{
		while (true) 
		{
			// Select randomly next target
			target = (IngredientID) Random.Range (1, (int) IngredientID.Count);
			targetImage.sprite = Resources.Load<Sprite> ("UI/" + target);
			targetImage.color = Color.white;
			puff.Play (true);

			// Switch Boat supplies
			Boat.SwitchAll (target);

			// Wait until next change
			float clock = 0f;
			while (clock <= changeRate) 
			{
				// Update timer
				timer.value = clock / changeRate;
			
				yield return null;
				clock += Time.deltaTime;
			}
		}
	}
	#endregion

	#region CALLBACKS
	protected override void OnDrop (Character owner) 
	{
		// Update score
		int id = owner.ownerID - 1;
		scores[id]++;
		(Game.manager as MeltingRace).playerScores[id].text = scores[id].ToString ("00");
		owner.Owner.ranking.scores[0]++;

		// Play sound
		correctSound.Play ();
	}

	protected override bool OptionalCheck (Character player) 
	{
		// Can only drop target ingredient on this cauldron
		return player.toy.GetComponent<Ingredient> ().id == target;
	}

	protected override void Awake () 
	{
		base.Awake ();
		current = this;
		scores = new int[2];
	}
	#endregion
}
