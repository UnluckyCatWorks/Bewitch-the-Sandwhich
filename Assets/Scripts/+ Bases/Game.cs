using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Game : MonoBehaviour
{
	#region DATA
	private Characters roundWinner;

	public static Ingredient[] ingredients;

	public static int rounds;
	public static Modes mode;

	public static Game manager;
	public static bool stopped;
	#endregion

	#region UTILS
	public static void DeclareWinner (Characters winner) 
	{
		manager.roundWinner = winner;
		Character.Get (winner).Owner.currentStats.roundScore++;
	}

	public abstract IEnumerator ResetStage ();
	#endregion

	#region CALLBACKS
	protected IEnumerator Start () 
	{
		// Reset game stats for both players
		Player.all.ForEach (p=> p.currentStats = new GameStats ());

		// Start rounds
		for (int round=1; round<=rounds; round++) 
		{
			// Display round info
			yield return RoundDisplay.Show (round, roundWinner);
			roundWinner = Characters.NONE;
			stopped = false;

			// Start actual game mode logic
			var logic = StartCoroutine (Logic ());

			// Wait until winner is proclaimed
			while (roundWinner == Characters.NONE) yield return null;
			StopCoroutine (logic);
		}

		// On finalizing every round
		stopped = true;
		yield return RoundDisplay.Show (rounds, roundWinner);
		#warning show rankings instead!!!
		UIMaster.LoadScene (Modes.Tutorial);
	}

	protected abstract IEnumerator Logic ();

	private void Awake () 
	{
		manager = this;

		// Load all ingredients
		if (ingredients == null)
			ingredients = Resources.LoadAll<Ingredient> ("Prefabs/Ingredients");

		// If not coming from the Lobby
		if (mode == Modes.UNESPECIFIED)
		{
			rounds = 3;
			enabled = true;
		}
		Character.SpawnPack ();
		OnAwake ();
	}
	protected virtual void OnAwake () { }
	#endregion

	#region HELPERS
	public enum Modes 
	{
		UNESPECIFIED,
		Tutorial,

		// Game Modes
		MeltingRace,
		CauldronCapture,
		EnchantedWeather,

		// Specials
		Count = EnchantedWeather,
		CountNoTutorial = Count - 1
	}

	public static List<T> Get<T> (string name, bool forEachChar, Action<T> a = null) 
	{
		List<T> list;
		if (forEachChar)
		{
			list = new List<T>
			{
				GameObject.Find (name + Characters.Bobby).GetComponent<T> (),
				GameObject.Find (name + Characters.Lilith).GetComponent<T> (),
				GameObject.Find (name + Characters.Amy).GetComponent<T> (),
				GameObject.Find (name + Characters.Milton).GetComponent<T> ()
			};
		}
		else
		{
			list = new List<T>
			{
				GameObject.Find (name + "1").GetComponent<T> (),
				GameObject.Find (name + "2").GetComponent<T> ()
			};
		}

		if (a != null) list.ForEach (a);
		return list;
	}
	#endregion
}
