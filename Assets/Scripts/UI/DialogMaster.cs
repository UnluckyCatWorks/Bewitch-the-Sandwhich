using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class DialogMaster
{
	private static bool initialized; // ?

	public static Image speaker;
	public static Image textBG;
	public static Text message;


	public static Coroutine StartNew (string path) 
	{
		var talk = Resources.Load<Talk> ("Dialogs/" + path);

		if (talk) return Game.manager.StartCoroutine (DisplayDialog (talk.dialog));
		else throw new UnityException ("Can't find dialog asset");
	}

	static IEnumerator DisplayDialog (Dialog[] dialogs) 
	{
		// Turn dialog UI
		if (!Game.paused) Time.timeScale = 0f;
		speaker.CrossFadeAlphaFixed (0.1f, 0.3f, true);
		message.CrossFadeAlphaFixed (1f, 0.3f, true);
		textBG.CrossFadeAlphaFixed  (1f, 0.3f, true);

		// Go through the script
		foreach (var d in dialogs)
		{
			var fill = 0f;
			var speed = d.speed * 2f;
			while (fill <= d.message.Length + 1)
			{
				// Get cursor position
				var cursor = Mathf.FloorToInt (fill);

				// Fill text
				message.text = d.message + "</color>";
				message.text = message.text.Insert (cursor, "<color=#0000>");

				yield return null;
				fill += Time.unscaledDeltaTime * speed;
				// Increase speed if pressing skip
				if (Input.GetButtonDown ("Skip")) speed = Mathf.Pow (speed, 2);
			}
			message.text = d.message;

			// Wait until dialog is skipped
			while (!Input.GetButtonDown ("Skip"))
				yield return null;
		}

		// Turn off dialog UI
		speaker.CrossFadeAlpha (0, 0.2f, true);
		message.CrossFadeAlpha (0, 0.2f, true);
		textBG.CrossFadeAlpha  (0, 0.2f, true);
		if (!Game.paused) Time.timeScale = 1f;
	}

	public static void Initialize () 
	{
		speaker = GameObject.Find ("Speaker").GetComponent<Image> ();
		message = GameObject.Find ("Message").GetComponent<Text> ();
		textBG = GameObject.Find ("Text_BG").GetComponent<Image> ();
		initialized = true;
	}
}
