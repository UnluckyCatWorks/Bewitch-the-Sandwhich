using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class TutorialGame : Game 
{
	#region UI UTILS
	public void Play () 
	{
		paused = false;
		Cursor.visible = false;
		DontDestroyOnLoad (GameObject.Find ("UI"));
		SceneManager.LoadScene ("Waterloo");
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
		DontDestroyOnLoad (GameObject.Find ("UI"));

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
