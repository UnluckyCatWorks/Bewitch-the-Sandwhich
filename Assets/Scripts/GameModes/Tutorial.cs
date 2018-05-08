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
	public void StartTutorial () 
	{
		StartCoroutine (Logic ());
	}

	private IEnumerator Logic () 
	{
		// Start tutorial
		onTutorial = true;
		Game.paused = false;

		#region PREPARATION
		// Get some references
		var menu = GameObject.Find ("UI_MENU").GetComponent<Animator> ();
		var focos = GameObject.Find ("Focos").GetComponent<Animator> ();
		var rig = GameObject.Find ("Camera_Rig").GetComponent<Animator> ();
		var modeMenu = GameObject.Find ("UI_MODE_SELECTION");

		// Create Players' Characters
		var ps = Character.SpawnPack ();
		// Position them
		var positions = Lobby.Get<Transform> ("Start_", false);
		ps[0].transform.position = positions[0].position;
		ps[1].transform.position = positions[1].position;
		// Restrict their capabilities
		ps.ForEach (p => p.AddCC ("Movement", Locks.Movement));
		ps.ForEach (p => p.AddCC ("Dash", Locks.Dash));
		ps.ForEach (p => p.AddCC ("Spell", Locks.Spells));
		ps.ForEach (p => p.AddCC ("Interactions", Locks.Interaction));

		// Spawn the Tuto_Icons on them
		var iconsPrefab = Resources.Load<TutoIcons> ("Prefabs/Tuto_Icons");
		var icons = new List<TutoIcons>
		{
			Instantiate (iconsPrefab, ps[0].transform),
			Instantiate (iconsPrefab, ps[1].transform)
		};
		icons[0].InitializeAs (Player.all[0].scheme.type);
		icons[1].InitializeAs (Player.all[1].scheme.type);
		#endregion

		// Go to the scene
		rig.SetTrigger ("ToScene");
		focos.SetTrigger ("Light_Scene");
		StartCoroutine (Extensions.FadeAmbient (1.9f, 3f, 0.5f));
		// Wait until mode menu is out of camera
		yield return new WaitForSeconds (1.1f);
		modeMenu.SetActive (false);

		// Wait a bit
		yield return new WaitForSeconds (3f);

		#region MOVING
		SwitchCartel ("MOVE");

		// Show movement marks
		var markers = Lobby.Get<TutoPoint> ("Movement_", false);
		// Show correct icon (depends on input scheme)
		for (int i=0; i!=2; i++) 
		{
			int id = (int) Player.all[i].scheme.type;
			var child = markers[i].transform.GetChild (id+1);

			markers[i].marker.sign = child.GetComponent<SpriteRenderer> ();
			child.gameObject.SetActive (true);
		}
		// Assign observed characters
		markers[0].observedCharacter = ps[0].ID;
		markers[1].observedCharacter = ps[1].ID;
		// Turn them on
		markers[0].marker.On (ps[0].focusColor);
		markers[1].marker.On (ps[1].focusColor);

		// Alow movement
		ps.ForEach (p=> p.RemoveCC ("Movement"));

		// Wait until all players are in place
		Checks.Add (Phases.Moving, new Check ());
		while (!Checks[Phases.Moving].Ready) yield return null;
		Checks.Remove (Phases.Moving);

		// Turn off markers
		markers[0].marker.Off (TutoPoint.validColor);
		markers[0].marker.Off (ps[0].focusColor);
		Destroy (markers[0]);
		markers[1].marker.Off (TutoPoint.validColor);
		markers[1].marker.Off (ps[1].focusColor);
		Destroy (markers[1]);

		SwitchCartel ("");
		#endregion

		#region DASHING
		// Show water pit
		Game.paused = true;
		yield return new WaitForSecondsRealtime (1f);
		GameObject.Find ("Plat_agua").GetComponent<Animation> ().Play ("Out");
		GameObject.Find ("Plat_agua").GetComponentInChildren<Collider> ().enabled = false;

		// Wait a bit
		yield return new WaitForSecondsRealtime (1f);

		// Allow dashing
		ps.ForEach (p=> p.RemoveCC ("Dash"));
		Game.paused = false;

		// Show Dash marks
		markers = Lobby.Get<TutoPoint> ("Dash_", false);
		// Assign observed characters
		markers[0].observedCharacter = ps[0].ID;
		markers[1].observedCharacter = ps[1].ID;
		// Turn them on
		markers[0].marker.On (ps[0].focusColor);
		markers[1].marker.On (ps[1].focusColor);

		// Show icons
		icons.ForEach (i=> i.Show ("Dash"));
		SwitchCartel ("DASH");

		// Wait until all players are in place
		Checks.Add (Phases.Dashing, new Check ());
		while (!Checks[Phases.Dashing].Ready) yield return null;
		Checks.Remove (Phases.Dashing);

		// Turn off markers
		markers[0].marker.Off (TutoPoint.validColor);
		markers[0].marker.Off (ps[0].focusColor);
		Destroy (markers[0]);
		markers[1].marker.Off (TutoPoint.validColor);
		markers[1].marker.Off (ps[1].focusColor);
		Destroy (markers[1]);

		// Hide icons
		icons.ForEach (i=> i.Hide ("Dash"));
		SwitchCartel ("");

		// Hide water pit (play backwards)
		GameObject.Find ("Plat_agua").GetComponent<Animation> ().PlayInReverse ("Out");
		GameObject.Find ("Plat_agua").GetComponentInChildren<Collider> ().enabled = true;
		#endregion

		#region SPELL
		// Wait a bit
		yield return new WaitForSecondsRealtime (3f);
		SwitchCartel ("SPELLS");

		// Allow spells & show icons
		ps.ForEach (p=> p.RemoveCC ("Spells"));
		icons.ForEach (i=> i.Show ("Spell"));

		// Wait until all players have landed a spell
		Checks.Add (Phases.Casting_Spells, new Check ());
		while (!Checks[Phases.Casting_Spells].Ready) yield return null;
		Checks.Remove (Phases.Casting_Spells);

		// Hide icons
		icons.ForEach (i=> i.Hide ("Spell"));
		SwitchCartel ("");
		#endregion

		// End tutorial
		onTutorial = false;
		#region TAL
		/*
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
		// Reset
		Checks = new Dictionary<Phases, Check> ();
		onTutorial = false;
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

	public class Check 
	{
		List<Characters> validatedCharacters = new List<Characters> (2);

		// Only true if all players who
		// are currently playing are done
		public bool Ready 
		{
			get
			{
				// For now, only 2 players can play at once
				return (validatedCharacters.Count == 2);
			}
		}

		// Keeps track of who has validated this point
		public void Set (Characters who, bool value)
		{
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
	#endregion
}
