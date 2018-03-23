using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
	public void dw ()
	{
		Application.Quit ();
	}

	public void VAMONOS ()
	{
		Cursor.visible = false;
		Game.paused = true;
		StartCoroutine (VENGAAAAAAAAAA ());
	}

	private IEnumerator VENGAAAAAAAAAA () 
	{
		// CC both players
		var ps = FindObjectsOfType<Character> ();
		foreach (var p in ps) p.AddCC ("Tuto", Locks.Spells | Locks.Dash);

		// Delay
		yield return new WaitForSecondsRealtime (0.5f);

		// Fade background
		// Move menu up
		var stamp = Time.unscaledTime;
		var t = transform.GetChild (0);
		while (Time.unscaledTime <= stamp + 1.5f)
		{
			t.Translate (0, Time.unscaledDeltaTime * 500f, 0);
			yield return null;
		}
		GetComponent<Image> ().CrossFadeAlpha (0, 1f, true);
		// Delay and start dialog
		yield return new WaitForSecondsRealtime (2f);
		yield return DialogMaster.StartNew ("TutoTest/1");
		yield return new WaitForSecondsRealtime (2.5f);
		yield return DialogMaster.StartNew ("TutoTest/2");
		Game.paused = false;
		Time.timeScale = 1f;
		yield return new WaitForSeconds (8f);
		yield return DialogMaster.StartNew ("TutoTest/3");
		// activate supplies
		var ss = FindObjectsOfType<Supply> ();
		foreach (var s in ss) s.GetComponent<BoxCollider> ().enabled = true;
		while (!ps.All ( x => (x.grab != null) )) yield return null;
		yield return new WaitForSeconds (4f);
		yield return DialogMaster.StartNew ("TutoTest/4");
		var cs = FindObjectsOfType<Cauldron> ();
		foreach (var c in cs) c.GetComponent<Rigidbody> ().isKinematic = false;
		yield return new WaitForSeconds (1.11f);
		foreach (var c in cs) c.GetComponent<Rigidbody> ().isKinematic = true;
		while (!cs.All (x => (x.clock != 0f))) yield return null;
		yield return new WaitForSeconds (2.5f);
		yield return DialogMaster.StartNew ("TutoTest/5");
		GetComponent<Image> ().CrossFadeAlphaFixed (0.7f, 1f, true);
		yield return new WaitForSeconds (1.5f);
		OrderMaster.SpawnOrder (1);
		yield return new WaitForSeconds (0.5f);
		OrderMaster.SpawnOrder (2);
		yield return new WaitForSeconds (1f);
		yield return DialogMaster.StartNew ("TutoTest/6");
		GetComponent<Image> ().CrossFadeAlphaFixed (1f, 1f, true);
		yield return new WaitForSeconds (1.1f);
		SceneManager.LoadScene (1);
		yield return new WaitForSeconds (2f);

		// CC both players
		var ps2 = FindObjectsOfType<Character> ();
		foreach (var p in ps2) p.AddCC ("Tuto", Locks.Spells | Locks.Dash);

		GetComponent<Image> ().CrossFadeAlpha (0f, 2f, true);
		yield return new WaitForSeconds (3f);
		yield return DialogMaster.StartNew ("TutoTest/7");
		yield return new WaitForSeconds (5f);
		yield return DialogMaster.StartNew ("TutoTest/8");
		FindObjectOfType<BridgeController> ().GetComponent<Animation> ().Play ();
	}

	private void Awake () 
	{
		Time.timeScale = 0f;
	}
}
