using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Game : MonoBehaviour
{
	#region DATA
	// 0 (draw), 1 or 2
	private int roundWinner;

	public static Ingredient[] ingredients;

	public static int rounds;
	public static Modes mode;

	public static Game manager;
	public static bool stopped;
	#endregion

	#region UTILS
	public static void DeclareWinner (int winner) 
	{
		if (winner != 0) 
		{
			manager.roundWinner = winner;
			Player.all[winner - 1].ranking.roundsWon++;
		}
		// Empate (don't count points for anyone!)
		else manager.roundWinner = 0;
	}

	public abstract IEnumerator ResetStage ();
	#endregion

	#region CALLBACKS
	protected IEnumerator Start () 
	{
		// If not coming from Lobby
		if (mode == Modes.UNESPECIFIED) 
		{
			// Play game music
			UIMaster.PlayMusic (UIMaster.Musics.Game);

			// Set correct mode, so testing works
			string modeName = manager.GetType ().Name;
			mode = (Modes) Enum.Parse (typeof (Modes), modeName);
		}
		// Reset game stats for both players
		Player.all.ForEach (p => p.ranking = new GameStats (3));

		// Start rounds
		for (int round=1; round<=rounds; round++) 
		{
			stopped = true;
			// Display round info
			yield return RoundDisplay.Show (round, roundWinner);
			roundWinner = -1;
			stopped = false;

			// Start actual game mode logic
			var logic = StartCoroutine (Logic ());

			// Wait until winner is proclaimed
			while (roundWinner == -1) yield return null;
			StopCoroutine (logic);
		}

		// At the end of the game
		stopped = true;
		UIMaster.PlayEffect (UIMaster.SFx.Whistle);
		yield return new WaitForSeconds (1f);
		StartCoroutine (ResetStage ());
		UIMaster.ShowRanking ();
	}

	protected abstract IEnumerator Logic ();

	private void Awake () 
	{
		// Load all ingredients
		if (ingredients == null)
			ingredients = Resources.LoadAll<Ingredient> ("Prefabs/Ingredients");

		// If not coming from the Lobby
		if (mode == Modes.UNESPECIFIED) 
		{
			Character.SpawnPack ();
			enabled = true;
			rounds = 2;
			OnAwake ();
		}
		manager = this;
	}
	public virtual void OnAwake () { }
	#endregion

	#region HELPERS
	public enum Modes 
	{
		UNESPECIFIED,
		Tutorial,

		// Game Modes
		WetDeath,
		MeltingRace,
		WizardWeather,
		CauldronCapture,

		// Specials
		Count = CauldronCapture,
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
