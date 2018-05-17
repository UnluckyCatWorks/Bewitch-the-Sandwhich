using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MRCauldron : Cauldron
{
	#region DATA
	public static MRCauldron instance;

	[Header("Data")]
	public float changeRate;

	[Header ("References")]
	public ParticleSystem puff;
	public IngredientID target;
	public Image targetImage;
	public Timer timer;

	public static int[] scores;
	private static Coroutine targetLoop;
	#endregion

	#region UTILS
	public static Characters DeclareWinner () 
	{
		// Stop ingredient loop
		instance.StopCoroutine (targetLoop);

		// Player 1 wins
		if (scores[0] > scores[1]) return Player.all[0].playingAs;
		else
		// Player 2 wins
		if (scores[1] > scores[0]) return Player.all[1].playingAs;
		// EMPATE!!!
		 #warning LOCO EL EMPATE QUE?
		else return Characters.NONE;
	}

	public static void GetStarted () 
	{
		targetLoop = instance.StartCoroutine (instance.LoopTarget ());
	}

	private IEnumerator LoopTarget () 
	{
		while (true)
		{
			// Switch target with a puff
			puff.Play (true);

			// Select randomly next target
			target = (IngredientID)Random.Range (1, (int) IngredientID.Count);
			targetImage.sprite = Resources.Load<Sprite> ("UI/" + target);
			targetImage.color = Color.white;

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
		scores[owner.ownerID - 1]++;
	}

	protected override bool OptionalCheck (Character player) 
	{
		// Can only drop target ingredient on this cauldron
		return player.toy.GetComponent<Ingredient> ().id == target;
	}

	protected override void Awake () 
	{
		base.Awake ();
		instance = this;
		scores = new int[2];
	}
	#endregion
}
