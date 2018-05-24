using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
	#region DATA
	[Header ("Tuto")]
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
		Game.stopped = false;
		Cursor.visible = false;

		#region PREPARATION
		// Get some references
		var focos = GameObject.Find ("Focos").GetComponent<Animator> ();
		var rig = GameObject.Find ("Camera_Rig").GetComponent<Animator> ();

		// Create Players' Characters
		var ps = Character.SpawnPack ();
		// Restrict their capabilities
		ps.ForEach (p => p.AddCC ("Movement", Locks.Movement));
		ps.ForEach (p => p.AddCC ("Dash", Locks.Dash));
		ps.ForEach (p => p.AddCC ("Spells", Locks.Spells));
		ps.ForEach (p => p.AddCC ("Interaction", Locks.Interaction));

		// Spawn the Tuto_Icons on them
		var iconsPrefab = Resources.Load<TutoIcons> ("Prefabs/UI/Tuto_Icons");
		var icons = new List<TutoIcons>
		{
			Instantiate (iconsPrefab, ps[0].transform),
			Instantiate (iconsPrefab, ps[1].transform)
		};
		icons[0].InitializeAs (Player.Get (1).scheme.type);
		icons[1].InitializeAs (Player.Get (2).scheme.type);

		// Disable showcase characters
		var showcase = Game.Get<Transform> ("Showcase_", true);
		showcase.ForEach (t=> t.gameObject.SetActive (false));
		#endregion

		// Go to the scene
		rig.SetTrigger ("ToScene");
		focos.SetTrigger ("Light_Scene");
		StartCoroutine (Extensions.FadeAmbient (1.6f, 3f, 0.5f));
		ModeCarrousel.Switch (state: false);

		// Wait a bit
		yield return new WaitForSeconds (1f);

		#region MOVING
		Title.Show ("MOVE", 2f);

		// Show movement marks
		var markers = Game.Get<TutoPoint> ("Movement_", false);
		// Show correct icon (depends on input scheme)
		for (int i=1; i!=3; i++) 
		{
			int id = (int) Player.Get (i).scheme.type;
			var child = markers[i-1].transform.GetChild (id+1);

			markers[i-1].marker.sign = child.GetComponent<SpriteRenderer> ();
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
		markers[1].marker.Off (TutoPoint.validColor);
		markers[1].marker.Off (ps[1].focusColor);
		#endregion

		#region DASHING
		// Show water pit
		Game.stopped = true;
		yield return new WaitForSeconds (1f);
		GameObject.Find ("Plat_agua").GetComponent<Animation> ().PlayRewind ("Out");
		GameObject.Find ("Plat_agua").GetComponentInChildren<Collider> ().enabled = false;

		// Wait a bit
		yield return new WaitForSeconds (1f);

		// Allow dashing
		ps.ForEach (p=> p.RemoveCC ("Dash"));
		Game.stopped = false;

		// Show Dash marks
		markers = Game.Get<TutoPoint> ("Dash_", false);
		// Assign observed characters
		markers[0].observedCharacter = ps[0].ID;
		markers[1].observedCharacter = ps[1].ID;
		// Turn them on
		markers[0].marker.On (ps[0].focusColor);
		markers[1].marker.On (ps[1].focusColor);

		// Show icons
		icons.ForEach (i=> i.Show ("Dash"));
		Title.Show ("DASH", 2f);

		// Wait until all players are in place
		Checks.Add (Phases.Dashing, new Check ());
		while (!Checks[Phases.Dashing].Ready) yield return null;
		Checks.Remove (Phases.Dashing);

		// Turn off markers
		markers[0].marker.Off (TutoPoint.validColor);
		markers[0].marker.Off (ps[0].focusColor);
		markers[1].marker.Off (TutoPoint.validColor);
		markers[1].marker.Off (ps[1].focusColor);
		// Hide icons
		icons.ForEach (i=> i.Hide ("Dash"));

		// Hide water pit (play backwards)
		GameObject.Find ("Plat_agua").GetComponent<Animation> ().PlayInReverse ("Out");
		GameObject.Find ("Plat_agua").GetComponentInChildren<Collider> ().enabled = true;
		#endregion

		#region SPELL
		// Wait a bit
		yield return new WaitForSeconds (3f);
		Title.Show ("SPELLS", 2f);

		// Allow spells & show icons
		ps.ForEach (p=> p.RemoveCC ("Spells"));
		icons.ForEach (i=> i.Show ("Spell"));

		// Wait until all players have landed a spell
		Checks.Add (Phases.Casting_Spells, new Check ());
		while (!Checks[Phases.Casting_Spells].Ready) yield return null;
		Checks.Remove (Phases.Casting_Spells);

		// Hide icons
		yield return new WaitForSeconds (1f);
		icons.ForEach (i=> i.Hide ("Spell"));
		#endregion

		#region GRABBING / THROWING
		// Wait a bit
		yield return new WaitForSeconds (1f);
		Title.Show ("GRAB'N'HIT", 2f);
		yield return new WaitForSeconds (1f);

		// Show icons & allow interactions
		ps.ForEach (p=> p.RemoveCC ("Interaction"));
		icons.ForEach (i=> i.Show ("Interaction"));

		// Show supplies
		supplies.ForEach (s => 
		{
			// Appear with a 'Puff'
			s.gameObject.SetActive (true);
			var puff = Instantiate (Lobby.manager.puff);
			puff.transform.position = s.transform.position + Vector3.up * 0.5f;
			Destroy (puff.gameObject, 2f);
			puff.Play ();
		});

		Checks.Add (Phases.Throwing_Stuff, new Check ());
		while (!Checks[Phases.Throwing_Stuff].Ready) yield return null;
		Checks.Remove (Phases.Throwing_Stuff);

		yield return new WaitForSeconds (2f);
		#endregion

		Title.Show ("CONGRATULATIONS!", 2.5f);
		Lobby.manager.confetti.Play ();
		yield return new WaitForSeconds (3.2f);
		Title.Show ("YOU'RE READY FOR THE ARENA", 2f);
		yield return new WaitForSeconds (3f);

		#region RETURN
		// Return to Mode selection
		focos.SetTrigger ("Go_Off");
		rig.SetTrigger ("ToModeSelect");

		// Make supplies dissappear
		supplies.ForEach (s =>
		{
			// Dissappear with a 'Puff'
			s.gameObject.SetActive (false);
			var puff = Instantiate (Lobby.manager.puff);
			puff.transform.position = s.transform.position + Vector3.up * 0.5f;
			Destroy (puff.gameObject, 2f);
			puff.Play ();
		});
		// Make all grabbables dissapear
		FindObjectsOfType<Grabbable> ().ToList ().ForEach (g=> g.Destroy ());

		yield return new WaitForSeconds (1f);
		// Make players dissapear
		ps.ForEach (p =>
		{
			// Destroy with a 'Puff'
			Destroy (p.gameObject);

			var puff = Instantiate (Lobby.manager.puff);
			puff.transform.position = p.transform.position + Vector3.up * 0.5f;
			Destroy (puff.gameObject, 2f);
			puff.Play ();
		});

		// Make mode-selection appear
		yield return new WaitForSeconds (0.5f);
		ModeCarrousel.Switch (state: true);
		// Re-enable showcase character
		yield return new WaitForSeconds (0.5f);
		showcase.ForEach (s=> s.gameObject.SetActive(true));
		#endregion

		// End tutorial
		Cursor.visible = true;
		onTutorial = false;
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

	public static bool SetCheckFor (Characters character, Phases phase, bool value) 
	{
		// Will check only if tutorial is on given Phase
		if (!onTutorial || !Checks.ContainsKey (phase)) return false;
		// Set value
		Checks[phase].Set (character, value);
		return true;
	}
	#endregion
}
