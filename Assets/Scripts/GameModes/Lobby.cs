using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Lobby : Game 
{
	#region DATA
	[Header ("Intro")]
	public bool bypassHostIntro;
	public GameObject presentador;
	public ParticleSystem puff;
	public ParticleSystem confetti;

	// Control
	public static bool firstTime;
	private static bool passedMainMenu;
	public static bool charactersSelected;
	#endregion

	#region UI UTILS
	public void Play () 
	{
		Cursor.visible = false;
		passedMainMenu = true;
	}

	public void Exit () 
	{
		PlayerPrefs.Save ();
		Application.Quit ();
	}
	#endregion

	#region CALLBACKS
	protected override IEnumerator Logic () 
	{
		#region PREPARATION
		// Get some references
		var menu = GameObject.Find ("UI_MENU").GetComponent<Animator> ();
		var focos = GameObject.Find ("Focos").GetComponent<Animator> ();
		var rig = GameObject.Find ("Camera_Rig").GetComponent<Animator> ();
		ModeCarrousel.Switch (state: false);
		#endregion

		#region MENU APPEARS
		// Turn off image-effects
		var postfx = FindObjectOfType<PostProcessVolume> ();
		postfx.weight = 0f;

		// Wait until logos are shown
		yield return new WaitForSeconds (3.5f);
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

		// Determinate if this is the first time playing the Game
		firstTime = !PlayerPrefs.HasKey ("The_Sandwich_Has_Been_Bewitched");
		if (firstTime && !bypassHostIntro)
		{
			// If first time playing BtW,
			// the Host will appear & talk some shit
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
			focos.SetTrigger ("Go_Off");
			#endregion
		}

		// Otherwise, just go directly to
		#region CHARACTER SELECTION
		Cursor.visible = true;
		rig.SetTrigger ("ToCharSelect");
		StartCoroutine (Extensions.FadeAmbient (0.7f, 2f, 0.8f));
		// Enable Selectors
		FindObjectsOfType<Selector> ().ToList ().ForEach (s => s.SwitchState (state: true));

		// Wait until both players are ready
		yield return new WaitUntil (() => charactersSelected);
		#endregion

		if (firstTime && !bypassHostIntro) 
		{
			// maybe show host again and talk
			// some other shit
		}

		// Wait a bit
		yield return new WaitForSeconds (0.5f);

		// Go to mode selection
		rig.SetTrigger ("ToModeSelect");
		ModeCarrousel.Switch (state: true);
	}

	protected override void Awake () 
	{
		base.Awake ();
		Cursor.visible = false;
		RenderSettings.ambientIntensity = 0f;
		// Reset static vars
		passedMainMenu = false;
		charactersSelected = false;
	}
	#endregion

	#region HELPERS
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

		if (a!=null) list.ForEach (a);
		return list;
	}
	#endregion
}
