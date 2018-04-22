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

	internal Animator anim;
	#endregion

	public IEnumerator Display (params Speech.Dialog[] dialogs) 
	{
		/// Turn dialog UI on
		bg.CrossShowAlpha (1f, 0.3f, true);
		border.CrossShowAlpha (1f, 0.3f, true);
		arrow.CrossShowAlpha (1f, 0.15f, true);
		yield return new WaitForSecondsRealtime (0.4f);

		// Go through the script
		foreach (var d in dialogs)
		{
            var fill = 0f;
			var speed = d.speed * 2f;
			while (fill <= d.message.Length + 1)
			{
				/// Get cursor position
				var cursor = Mathf.FloorToInt (fill);

				/// Fill text
				content.text = d.message + "</color>";
				content.text = content.text.Insert (cursor, "<color=#0000>");

				yield return null;
				fill += Time.unscaledDeltaTime * speed;

				/// Increase speed if pressing skip
				if (Input.GetButtonDown ("Skip"))
					speed = Mathf.Pow (speed, 2);
			}
			content.text = d.message;

			/// Wait until dialog is skipped
			while (!Input.GetButtonDown ("Skip")) yield return null;

            /// Play Animator trigger if given
            if (!string.IsNullOrEmpty (d.trigger) && anim != null)
                anim.SetTrigger (d.trigger);
        }

		/// Turn off dialog UI
		bg.CrossFadeAlpha (0, 0.2f, true);
		border.CrossFadeAlpha (0, 0.2f, true);
		arrow.CrossFadeAlpha (0, 0.15f, true);
		content.CrossFadeAlpha (0, 0.2f, true);
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
	public static Coroutine StartNew (string path, Animator anim=null)
	{
		var speech = Resources.Load<Speech> ("Dialogs/" + path);
		if (speech)
		{
			var ui = GameObject.Find ("UI_Master");

			var prefab = Resources.Load<Dialog> ("Prefabs/Dialog");
			var dialog = Instantiate (prefab, ui.transform);
			dialog.anim = anim;

			return dialog.StartCoroutine (dialog.Display (speech.dialog));
		}
		else throw new UnityException ("Can't find dialog asset!");
	}
	#endregion
}
