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

		yield return new WaitForSeconds (1f);
		yield return DialogMaster.StartNew ("Tutorial/1");
		paused = false;

		/// Wait until both players meet
		float dist = 100f;
		while (dist >= 2.5f)
		{
			var p1 = ps[0].transform.position;
			var p2 = ps[1].transform.position;
			dist = Vector3.Distance (p1, p2);
			yield return null;
		}
		yield return new WaitForSeconds (1f);
		yield return DialogMaster.StartNew ("Tutorial/2");
		ps.ForEach (p => p.RemoveCC ("Dash"));
		/// Wait a bit
		yield return new WaitForSeconds (10f);

		/// Grabbing stuff
		yield return DialogMaster.StartNew ("Tutorial/3");
		ps.ForEach (p => p.RemoveCC ("Interaction"));
		/// Wait until someone grabs something
		while (!ps.Any (p=> p.grab)) yield return null;
		/// Then wait some secs
		yield return new WaitForSeconds (10f);

		/// Casting spells
		yield return DialogMaster.StartNew ("Tutorial/4");
		ps.ForEach (p => p.RemoveCC ("Spells"));
		/// Then wait some secs
		yield return new WaitForSeconds (10f);

		var hey = GameObject.Find ("Hey").GetComponent<Image> ();
		/// Time to GO!
		hey.CrossFadeAlphaFixed (1f, 1.5f, true);
		yield return DialogMaster.StartNew ("Tutorial/5");
		yield return SceneManager.LoadSceneAsync ("Waterloo");
		hey.CrossFadeAlpha (0f, 2f, true);

		yield return DialogMaster.StartNew ("Tutorial/6");
	}

	private void Awake () 
	{
		DontDestroyOnLoad (gameObject);
		paused = true;
	}
}
