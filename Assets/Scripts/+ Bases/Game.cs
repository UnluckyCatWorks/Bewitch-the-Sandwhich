using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Game : MonoBehaviour
{
	#region DATA
	public bool justTesting;
	public Characters roundWinner;

	public static int rounds;
	public static Modes mode;

	public static Game manager;
	public static bool stopped;
	#endregion

	#region UTILS
	protected virtual void ResetStage () { /*Reset the scene for the next round*/ }
	#endregion

	#region CALLBACKS
	protected IEnumerator Start () 
	{
		if (this is Lobby) 
		{
			StartCoroutine (Logic ());
			yield break;
		}

		// Reset game stats for both players
		Player.all.ForEach (p=> p.currentStats = new GameStats ());

		// Start rounds
		for (int round=1; round<=rounds; round++) 
		{
			// Display round info
			yield return RoundDisplay.Show (round, roundWinner, ResetStage);
			roundWinner = Characters.NONE;
			stopped = false;

			// Start actual game mode logic
			StartCoroutine (Logic ());

			// Wait until winner is proclaimed
			while (roundWinner == Characters.NONE)
				yield return null;
		}
	}
	protected abstract IEnumerator Logic ();


	private void Awake () 
	{
		manager = this;

		if (!(this is Lobby) && !justTesting)
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
