using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine;
using UnityEngine.UI;
using TP = TutorialGame.TutorialPhases;

public class TutorialGame : Game 
{
	#region DATA
	[Header ("Intro")]
	public GameObject presentador;
	public ParticleSystem puff;
	public ParticleSystem confetti;

	[Header("Tuto")]
	public Material cartelMat;
	public Text cartel;
	public List<Supply> supplies;

	// Validation
	public static Dictionary<TP, TutorialCheck> Checks;
	#endregion

	#region UI UTILS
	public void Play () 
	{
		throw new System.NotImplementedException ();
	}

	public void PlayTuto () 
	{
		Cursor.visible = false;
		enabled = true;
	}

	public void Exit () 
	{
		#if UNITY_EDITOR
		/// Atajito
		Cortinilla.LoadScene ("Coliseum");
		#endif
		Application.Quit ();
	}
	#endregion

	#region TUTORIAL PHASES
	// Starts when clicked "Play"
	protected override IEnumerator Logic () 
	{
		// Turn off image-effects
		var postfx = FindObjectOfType<PostProcessVolume> ();
		postfx.weight = 0f;

		// Wait until logos are shown
		yield return new WaitForSeconds (2.5f);
		GameObject.Find ("UI_Menu").GetComponent<Animator> ().SetTrigger ("Show_Menu");

		// Fade-in image effects
		float factor = 0f;
		while (factor <= 1.1f)
		{
			float value = Mathf.Pow (factor, 2);
			postfx.weight = value;

			yield return null;
			factor += Time.deltaTime / /*duration*/ 1f;
		}
		yield break;

		// Players references
		var ps = FindObjectsOfType<Character> ().ToList ();
		#warning esto lo quiero hacer con un prfab y tal, mas comodo. ademas que tendre que poder cambiar los controles PFFF
		var icons = ps.Select (p => p.GetComponentInChildren<HUDIcons> ()).ToList ();

		// Limit players interaction
		ps.ForEach (p => p.AddCC ("Dash", Locks.Dash));
		ps.ForEach (p => p.AddCC ("Spells", Locks.Spells));
		ps.ForEach (p => p.AddCC ("Interaction", Locks.Interaction));

		// Make players invisible
		ps.ForEach (p => p.gameObject.SetActive (false));

		#region INTRO CUTSCENE
		// Get some scene references
		var menu = GameObject.Find ("UI_Menu").GetComponent<Animator> ();
		var focos = GameObject.Find ("Focos").GetComponent<Animator> ();
		var rig = GameObject.Find ("Camera_Rig").GetComponent<Animator> ();

		// Make everything black
		StartCoroutine (Extensions.FadeAmbient (0f, 2.5f, 0.8f));
		// Menu goes out
		menu.SetTrigger ("Play");
		yield return new WaitForSecondsRealtime (.2f);
		// Camera goes to Host
		rig.SetTrigger ("Play");
		yield return new WaitForSecondsRealtime (2.9f);
		// Lights focus on Host
		focos.SetTrigger ("Play");
		focos.SetTrigger ("ToPresentador");
		yield return new WaitForSecondsRealtime (2f);

		#region HOST
		/// Host appears
		puff.Play (true);
		presentador.SetActive (true);
		var pAnim = presentador.GetComponent<Animator> ();

		/// Intro dialog
		yield return new WaitForSecondsRealtime (3f);
		yield return Dialog.StartNew ("Tutorial/Text", pAnim);
		pAnim.SetBool ("In", false);

		/// When dialog is over,
		/// Host dissapears
		puff.Play (true);
		yield return new WaitForSecondsRealtime (0.1f);
		presentador.SetActive (false);
		#endregion

		// Camera goes to scene
		rig.SetTrigger ("ToScene");
		// Light up the scene
		focos.SetTrigger ("ToScene");
		StartCoroutine (Extensions.FadeAmbient (2.9f, 2.5f, 1.4f));
		#endregion

		#region TUTORIAL
		// Make players visible
		ps.ForEach (p => p.gameObject.SetActive (true));
		// Wait a bit, & allow movement
		yield return new WaitForSecondsRealtime (3f);
		paused = false;

		#region MOVING
		/// Show movement marks
		GetTuto<Marker> ("Movement_").ForEach (m => m.On (new Color(0,0,0,0)));
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
		ps.ForEach (p => p.RemoveCC ("Dash"));
		// Show Dash marks & icons
		GetTuto<Marker> ("Dash_").ForEach (m => m.On (new Color (0, 0, 0, 0)));
		icons.ForEach (i => i.Show ("Dash"));
		SwitchCartel ("DASH");

		// Wait until all players are in place
		Checks.Add (TP.Dashing, new TutorialCheck ());
		while (Checks[TP.Dashing].AllWhoPlay) yield return null;
		Checks.Remove (TP.Dashing);

		// Turn off markers
		GetTuto<Marker> ("Dash_").ForEach (m => m.Off ());
		icons.ForEach (i => i.Hide ("Dash"));
		SwitchCartel ("");

		// Hide water pit (play backwards)
		GameObject.Find ("Plat_agua").GetComponent<Animation> ().PlayInReverse ("Out");
		GameObject.Find ("Plat_agua").GetComponentInChildren<Collider> ().enabled = true;
		#endregion

		#region SPELL
		/// Wait a bit
		yield return new WaitForSecondsRealtime (3f);
		/// Allow spells & show icons
		ps.ForEach (p => p.RemoveCC ("Spells"));
		icons.ForEach (i => i.Show ("Spell"));
		SwitchCartel ("SPELLS");

		/// Wait until all players have landed a spell
		Checks.Add (TP.Casting_Spells, new TutorialCheck ());
		while (Checks[TP.Casting_Spells].AllWhoPlay) yield return null;
		Checks.Remove (TP.Casting_Spells);

		/// Hide icons
		icons.ForEach (i => i.Hide ("Spell"));
		SwitchCartel ("");
		#endregion

		#region GRABBING / THROWING
		/// Wait a bit
		yield return new WaitForSecondsRealtime (1f);
		SwitchCartel ("GRAB & HIT");
		yield return new WaitForSecondsRealtime (1f);
		/// Show icons & allow interactions
		ps.ForEach (p => p.RemoveCC ("Interaction"));
		icons.ForEach (i => i.Show ("Interaction"));
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
		while (ps.Any (p => p.toy == null)) yield return null;
		icons.ForEach (i => i.Hide ("Interaction"));
		SwitchCartel ("");
		#endregion

		yield return new WaitForSecondsRealtime (1.5f);
		GameObject.Find ("Puerta_Wrapper").GetComponent<Animation> ().Play ("DownToHell");
		yield return new WaitForSecondsRealtime (5f);
		SwitchCartel ("GO!");
		#endregion

		// Cortinilla y al coliseo
		Cortinilla.LoadScene ("Coliseum");
	} 
	#endregion

	protected override void Awake () 
	{
		base.Awake ();
		Checks = new Dictionary<TP, TutorialCheck> ();
	}

	#region HERLPERS
	public enum TutorialPhases 
	{
		Moving,
		Dashing,
		Casting_Spells,
		Throwing_Stuff,
	}

	public struct TutorialCheck 
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

	public static bool IsChecking (TP phase) 
	{
		if (manager is TutorialGame &&
			Checks.ContainsKey (phase))
			return true;
		else
			return false;
	}

	private void SwitchCartel (string text) 
	{
		if (!string.IsNullOrEmpty(text))
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
