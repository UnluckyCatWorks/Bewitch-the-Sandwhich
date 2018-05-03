using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine;
using UnityEngine.UI;

public class Lobby : Game 
{
	#region DATA
	[Header ("Intro")]
	public bool bypassHostIntro;
	public GameObject presentador;
	public ParticleSystem puff;
	public ParticleSystem confetti;

	// Modes
	public static bool firstTime;
	private static bool passedMainMenu;
	#endregion

	#region UI UTILS
	public void Play () 
	{
		Cursor.visible = false;
		passedMainMenu = true;
	}

	public void Exit () 
	{
		#if UNITY_EDITOR
		/// Atajito
		UIMaster.LoadScene ("Coliseum");
		#endif
		Application.Quit ();
	}
	#endregion

	#region CALLBACKS
	protected override IEnumerator Logic ()
	{
		#region PREPARATION
		// Determinate if this is the first time playing the Game
		firstTime = PlayerPrefs.HasKey ("Already_Played");

		// Get some references
		var menu = GameObject.Find ("UI_MENU").GetComponent<Animator> ();
		var focos = GameObject.Find ("Focos").GetComponent<Animator> ();
		var rig = GameObject.Find ("Camera_Rig").GetComponent<Animator> ();
		#endregion

		#region MENU APPEARS
		// Turn off image-effects
		var postfx = FindObjectOfType<PostProcessVolume> ();
		postfx.weight = 0f;

		// Wait until logos are shown
		yield return new WaitForSeconds (2.5f);
		menu.SetTrigger ("Show_Menu");

		// Fade-in image effects
		float factor = 0f;
		while (factor <= 1.1f)
		{
			float value = Mathf.Pow (factor, 2);
			postfx.weight = value;

			yield return null;
			factor += Time.deltaTime / /*duration*/ 1f;
		}
		Cursor.visible = true;
		#endregion

		// Wait until player plays "Play" button
		yield return new WaitUntil (() => passedMainMenu == true);
		// Hide Menu
		menu.SetTrigger ("Hide_Menu");

		// If first time playing BtW,
		// the Host will appear & talk some shit
		if (!firstTime && !bypassHostIntro)
		{
			#region HOST INTRODUCION
			focos.SetTrigger ("ToHost");
			rig.SetTrigger ("ToHost");
			yield return new WaitForSeconds (3f);

			// Host appears
			puff.Play (true);
			presentador.SetActive (true);
			var pAnim = presentador.GetComponent<Animator> ();

			// Intro dialog
			yield return new WaitForSeconds (3f);
			yield return Dialog.StartNew ("Lobby/Intro", pAnim);
			pAnim.SetBool ("In", false);

			// When dialog is over,
			// Host dissapears
			puff.Play (true);
			yield return new WaitForSeconds (0.1f);
			presentador.SetActive (false);
			yield return new WaitForSeconds (0.2f);
			focos.SetTrigger ("GoOff");
			#endregion
		}

		// Otherwise, just go directly
		// to the character selection part
		rig.SetTrigger ("ToCharSelect");
		StartCoroutine (Extensions.FadeAmbient (0.9f, 2f, 0.8f));
		// Enable Selectors
		FindObjectsOfType<Selector> ().ToList ().ForEach (s=> s.SwitchState (state: true));
	}

	protected override void Awake () 
	{
		base.Awake ();
		Cursor.visible = false;
	} 
	#endregion
}
