using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Game : MonoBehaviour
{
	#region DATA
	public static Modes mode;
	public static int rounds;

	public static Game manager;
	public static bool stopped;
	#endregion

	#region UTILS
	private IEnumerator StartRound (int round) 
	{
		var prefab = Resources.Load<Animation> ("Prefabs/UI/RoundCounter");
		var indicator = Instantiate (prefab, UIMaster.manager.transform);

		// For the first round, don't show backgroun
		if (round == 1) indicator.transform.GetChild (0).gameObject.SetActive (false);
		// Change round number
		indicator.GetComponentInChildren<Text> ().text = "Round " + round;

		// Wait until fade-in is done
		yield return new WaitForSeconds (indicator["In"].clip.averageDuration + 0.2f);
		//=> Do stuff to restart the level...?

		// Fade-out
		indicator.Play ("Out");
		yield return new WaitForSeconds (indicator["Out"].clip.averageDuration);
	}
	#endregion

	#region CALLBACKS
	protected IEnumerator Start () 
	{
		// In the lobby just do the logic,
		// it's not an actual game mode
		if (this is Lobby) 
		{
			StartCoroutine (Logic ());
			yield break;
		}

		for (int round=1; round<=rounds; round++) 
		{
			stopped = true;
			yield return StartRound (round);
			stopped = false;

			// Start actual game mode logic
			yield return Logic ();
		}
	}
	protected abstract IEnumerator Logic ();

	public void FinalizeGame () 
	{

	}

	protected virtual void Awake () 
	{
		// Self reference
		manager = this;
	}
	#endregion

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
		CountNoTutorial = Count-1
	}
}
