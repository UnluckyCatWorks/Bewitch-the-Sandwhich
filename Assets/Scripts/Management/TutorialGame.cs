using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class TutorialGame : Game 
{
	#region MyRegion
	[Header ("Intro")]
	public GameObject presentador;
	public ParticleSystem puff;
	public ParticleSystem confetti;

	[Header("Tuto")]
	public Material cartelMat;
	public Text cartel;

	// Validation
	public static Dictionary<string, int> Checks;
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
		Application.Quit ();
	}
	#endregion

	/// Starts when clicked "Play"
	protected override IEnumerator Logic () 
	{
		/// Players references
		var ps = FindObjectsOfType<Character> ().ToList ();
		var icons = ps.Select (p => p.GetComponentInChildren<HUDIcons> ()).ToList ();
		/// Limit players interaction
		ps.ForEach (p => p.AddCC ("Dash", Locks.Dash));
		ps.ForEach (p => p.AddCC ("Spells", Locks.Spells));
		ps.ForEach (p => p.AddCC ("Interaction", Locks.Interaction));
		/// Make players invisible
		ps.ForEach (p => p.gameObject.SetActive (false));

		#region INTRO CUTSCENE
		var menu = GameObject.Find ("UI_Menu").GetComponent<Animator> ();
		var focos = GameObject.Find ("Focos").GetComponent<Animator> ();
		var rig = GameObject.Find ("Camera_Rig").GetComponent<Animator> ();

		/// Make everything black
		StartCoroutine (Extensions.FadeAmbient (0f, 2.5f, 0.8f));
		/// Menu goes out
		menu.SetTrigger ("Play");
		yield return new WaitForSecondsRealtime (.2f);
		/// Camera goes to Host
		rig.SetTrigger ("Play");
		yield return new WaitForSecondsRealtime (2.9f);
		/// Lights focus on Host
		focos.SetTrigger ("Play");
		focos.SetTrigger ("ToPresentador");
		yield return new WaitForSecondsRealtime (2f);

        #region HOST
        /// Host appears
        puff.Play (true);
        presentador.SetActive (true);
        presentador.GetComponentInChildren <Renderer> ().sharedMaterial.SetColor ("_EmissionColor", Color.white * 0.288f);
        var pAnim = presentador.GetComponent<Animator> ();

        /// Intro dialog
        yield return new WaitForSecondsRealtime (2f);
        yield return Dialog.StartNew ("Tutorial/Text", pAnim);
        pAnim.SetBool ("In", false);

        /// When dialog is over,
        /// Host dissapears
        puff.Play (true);
        yield return new WaitForSecondsRealtime (0.1f);
        presentador.SetActive (false); 
        #endregion

        /// Camera goes to scene
        rig.SetTrigger ("ToScene");
		/// Light up the scene
		focos.SetTrigger ("ToScene");
        StartCoroutine (Extensions.FadeAmbient (2.9f, 2.5f, 1.4f));
		#endregion

		#region TUTORIAL
		/// Make players visible
		ps.ForEach (p => p.gameObject.SetActive (true));
		/// Wait a bit, & allow movement
		yield return new WaitForSecondsRealtime (3f);
		paused = false;

		/// Show movement marks
		GetTuto<Marker> ("Movement_").ForEach ( m=> m.On( m.name.Contains("Alby")? 1 : 2));
		SwitchCartel ("MOVE");

		/// Wait until all players are in place
		Checks.Add ("Movement", 0);
		while (Checks["Movement"] != 2) yield return null;
		Checks.Remove ("Movement");

		/// Turn off markers
		GetTuto<Marker> ("Movement_").ForEach (m => m.Off(0, bypass: true));
		SwitchCartel ("");
		paused = true;

		/// Show water pit
		yield return new WaitForSecondsRealtime (1f);
		GameObject.Find ("Plat_agua").GetComponent<Animation> ().Play ("Out");
		GameObject.Find ("Plat_agua").GetComponentInChildren<Collider> ().enabled = false;
		/// Wait a bit
		yield return new WaitForSecondsRealtime (1f);
		paused = false;
		/// Allow dashing
		ps.ForEach (p => p.RemoveCC ("Dash"));
		/// Show Dash marks & icons
		GetTuto<Marker> ("Dash_").ForEach (m => m.On (m.name.Contains ("Alby") ? 1 : 2));
		icons.ForEach (i=> i.Show ("Dash"));
		SwitchCartel ("DASH");

		/// Wait until all players are in place
		Checks.Add ("Dash", 0);
		while (Checks["Dash"] != 2) yield return null;
		Checks.Remove ("Dash");

		/// Turn off markers
		GetTuto<Marker> ("Dash_").ForEach (m => m.Off (0, bypass: true));
		icons.ForEach (i => i.Hide ("Dash"));
		SwitchCartel ("");

		/// Hide water pit (play backwards)
		GameObject.Find ("Plat_agua").GetComponent<Animation> ()["Out"].speed = -1;
		GameObject.Find ("Plat_agua").GetComponent<Animation> ()["Out"].normalizedTime = 1;
		GameObject.Find ("Plat_agua").GetComponent<Animation> ().Play ("Out");
		GameObject.Find ("Plat_agua").GetComponentInChildren<Collider> ().enabled = true;
		#endregion
	}

	private void Awake () 
	{
		/// Set up game
		Checks = new Dictionary<string, int> ();
        RenderSettings.ambientIntensity = 2.9f;
		paused = true;
	}

	#region HERLPERS
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
			GameObject.Find (name + "Mary").GetComponent<T> ()
		};
		return list;
	}
	#endregion
}
