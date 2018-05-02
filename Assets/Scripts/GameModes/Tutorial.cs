using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
	#region DATA
	[Header ("Tuto")]
	public Material cartelMat;
	public Text cartel;
	public List<Supply> supplies;

	public static Tutorial manager;
	public static Dictionary<Phases, Check> Checks;
	public static bool onTutorial;
	#endregion

	#region CALLBACKS
	public static IEnumerator Logic () 
	{
		onTutorial = true;
		// tutorial stuff
		onTutorial = false;
		yield break;
		
		#region TAL
		/*
		// Make players visible
		//		ps.ForEach (p => p.gameObject.SetActive (true));
		// Wait a bit, & allow movement
		yield return new WaitForSecondsRealtime (3f);
		paused = false;

		#region MOVING
		/// Show movement marks
		GetTuto<Marker> ("Movement_").ForEach (m => m.On (new Color (0, 0, 0, 0)));
		SwitchCartel ("MOVE");

		/// Wait until all players are in place
		Checks.Add (TP.Moving, new TutorialCheck ());
		while (Checks[TP.Moving].AllWhoPlay) yield return null;
		Checks.Remove (TP.Moving);

		/// Turn off markers
		GetTuto<Marker> ("Movement_").ForEach (m => m.Off ());
		SwitchCartel ("");
		paused = true;
		#endregion

		#region DASHING
		// Show water pit
		yield return new WaitForSecondsRealtime (1f);
		GameObject.Find ("Plat_agua").GetComponent<Animation> ().Play ("Out");
		GameObject.Find ("Plat_agua").GetComponentInChildren<Collider> ().enabled = false;
		// Wait a bit
		yield return new WaitForSecondsRealtime (1f);
		paused = false;
		// Allow dashing
		//		ps.ForEach (p => p.RemoveCC ("Dash"));
		// Show Dash marks & icons
		GetTuto<Marker> ("Dash_").ForEach (m => m.On (new Color (0, 0, 0, 0)));
		//		icons.ForEach (i => i.Show ("Dash"));
		SwitchCartel ("DASH");

		// Wait until all players are in place
		Checks.Add (TP.Dashing, new TutorialCheck ());
		while (Checks[TP.Dashing].AllWhoPlay) yield return null;
		Checks.Remove (TP.Dashing);

		// Turn off markers
		GetTuto<Marker> ("Dash_").ForEach (m => m.Off ());
		//		icons.ForEach (i => i.Hide ("Dash"));
		SwitchCartel ("");

		// Hide water pit (play backwards)
		GameObject.Find ("Plat_agua").GetComponent<Animation> ().PlayInReverse ("Out");
		GameObject.Find ("Plat_agua").GetComponentInChildren<Collider> ().enabled = true;
		#endregion

		#region SPELL
		/// Wait a bit
		yield return new WaitForSecondsRealtime (3f);
		/// Allow spells & show icons
		//		ps.ForEach (p => p.RemoveCC ("Spells"));
		//		icons.ForEach (i => i.Show ("Spell"));
		SwitchCartel ("SPELLS");

		/// Wait until all players have landed a spell
		Checks.Add (TP.Casting_Spells, new TutorialCheck ());
		while (Checks[TP.Casting_Spells].AllWhoPlay) yield return null;
		Checks.Remove (TP.Casting_Spells);

		/// Hide icons
		//		icons.ForEach (i => i.Hide ("Spell"));
		SwitchCartel ("");
		#endregion

		#region GRABBING / THROWING
		/// Wait a bit
		yield return new WaitForSecondsRealtime (1f);
		SwitchCartel ("GRAB & HIT");
		yield return new WaitForSecondsRealtime (1f);
		/// Show icons & allow interactions
		//		ps.ForEach (p => p.RemoveCC ("Interaction"));
		//		icons.ForEach (i => i.Show ("Interaction"));
		/// Show supplies
		supplies.ForEach (s =>
		{
			s.gameObject.SetActive (true);
			/// Appear with a 'Puff'
			var puff = Instantiate (this.puff);
			puff.transform.position = s.transform.position + Vector3.up * 0.5f;
			Destroy (puff.gameObject, 2f);
			puff.Play ();
		});

		/// Wait until both players have something in hands
		//		while (ps.Any (p => p.toy == null)) yield return null;
		//		icons.ForEach (i => i.Hide ("Interaction"));
		SwitchCartel ("");
		#endregion

		yield return new WaitForSecondsRealtime (1.5f);
		GameObject.Find ("Puerta_Wrapper").GetComponent<Animation> ().Play ("DownToHell");
		yield return new WaitForSecondsRealtime (5f);
		SwitchCartel ("GO!");
		*/
		#endregion
	}

	private void Awake () 
	{
		manager = this;
		Checks = new Dictionary<Phases, Check> ();
	}
	#endregion

	#region HELPERS
	public enum Phases 
	{
		NONE,
		Moving,
		Dashing,
		Casting_Spells,
		Throwing_Stuff,
	}

	public struct Check 
	{
		List<Characters> validatedCharacters;

		// Only true if all players who
		// are currently playing are done
		public bool AllWhoPlay 
		{
			// For now, only 2 players can play at once
			get { return validatedCharacters.Count == 2; }
		}

		// Keeps track of who has validated this point
		public void Set (Characters who, bool value)
		{
			if (validatedCharacters == null)
				validatedCharacters = new List<Characters> (4);

			// Validate character
			if (value && !validatedCharacters.Contains (who))
				validatedCharacters.Add (who);
			else
			// De-validate character
			if (!value && validatedCharacters.Contains (who))
				validatedCharacters.Remove (who);
		}
	}

	public static void SetCheckFor (Characters character, Phases phase, bool value) 
	{
		// Will check only if tutorial is on given Phase
		if (!onTutorial || !Checks.ContainsKey (phase)) return;
		// Set value
		Checks[phase].Set (character, value);
	}

	private void SwitchCartel (string text) 
	{
		if (!string.IsNullOrEmpty (text))
		{
			cartel.text = text;
			cartel.CrossShowAlpha (1f, 0.2f, true);
			cartelMat.SetColor ("_EmissionColor", Color.white * 1.706f);
			cartel.material.SetFloat ("_Intensity", 14.64f);
		}
		else
		{
			cartel.text = "";
			cartel.CrossFadeAlpha (0f, 0.2f, true);
			cartelMat.SetColor ("_EmissionColor", Color.black);
			cartel.material.SetFloat ("_Intensity", 0f);
		}
	}

	private List<T> GetTuto<T> (string name) 
	{
		var list = new List<T>
		{
			GameObject.Find (name + "Alby").GetComponent<T> (),
			GameObject.Find (name + "Mary").GetComponent<T> (),
			GameObject.Find (name + "Mony").GetComponent<T> (),
			GameObject.Find (name + "Davy").GetComponent<T> ()
		};
		return list;
	}
	#endregion
}
