using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class TutorialGame : Game 
{
	[Header ("Intro")]
	public GameObject presentador;
	public ParticleSystem puff;

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
		/// Players reference
		var ps = FindObjectsOfType<Character> ().ToList ();
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
		paused = false;
		#endregion
	}

	private void Awake () 
	{
        /// Set up game
        RenderSettings.ambientIntensity = 2.9f;
		paused = true;
	}
}
