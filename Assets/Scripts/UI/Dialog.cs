using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dialog : MonoBehaviour 
{
	#region DATA
	public Image bg;
	public Image border;
	public Image arrow;
	public Text content;
	#endregion

	public IEnumerator Display (params Speech.Dialog[] dialog) 
	{
		/// Turn dialog UI on
		speaker.CrossFadeAlphaFixed (0.1f, 0.3f, true);
		message.CrossFadeAlphaFixed (1f, 0.3f, true);
		textBG.CrossFadeAlphaFixed (1f, 0.3f, true);

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
		textBG.CrossFadeAlpha (0, 0.2f, true);

	}

	#region CALLBACKS
	private void Awake () 
	{
		bg.SetAlpha (0f);
		border.SetAlpha (0f);
		arrow.SetAlpha (1f);
		content.text = string.Empty;
	}
	#endregion

	#region HELPERS
	public static Coroutine StartNew (string path)
	{
		var speech = Resources.Load<Speech> ("Dialogs/" + path);
		if (speech)
		{
			var ui = GameObject.Find ("UI_Master");

			var prefab = Resources.Load<Dialog> ("Prefabs/Dialog");
			var dialog = Instantiate (prefab, ui.transform);

			return dialog.StartCoroutine (dialog.Display (speech.dialog));
		}
		else throw new UnityException ("Can't find dialog asset!");
	}
	#endregion
}
