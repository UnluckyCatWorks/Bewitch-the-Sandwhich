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

	protected override IEnumerator Logic () 
	{
		#region INTRO CUTSCENE
		var pAnim = presentador.GetComponent<Animator> ();
		var menu = GameObject.Find ("UI_Menu").GetComponent<Animator> ();
		var focos = GameObject.Find ("Focos").GetComponent<Animator> ();
		var rig = GameObject.Find ("Camera_Rig").GetComponent<Animator> ();

		menu.SetTrigger ("Play");
		yield return new WaitForSecondsRealtime (.2f);
		rig.SetTrigger ("Play");
		yield return new WaitForSecondsRealtime (2.9f);
		focos.SetTrigger ("Play");
		focos.SetTrigger ("ToPresentador");
		yield return new WaitForSecondsRealtime (2f);

		/// Presentador
		puff.Play (true);
		presentador.SetActive (true);
		#endregion



		/// Players reference
		var ps = FindObjectsOfType<Character> ().ToList ();
		/// Supress spells & dash for now
		ps.ForEach (p=> p.AddCC ("Dash", Locks.Dash));
		ps.ForEach (p=> p.AddCC ("Spells", Locks.Spells));
		ps.ForEach (p=> p.AddCC ("Interaction", Locks.Interaction));

		// TODO 
		yield return null;
	}

	private void Awake () 
	{
		DontDestroyOnLoad (gameObject);
		paused = true;
	}
}
