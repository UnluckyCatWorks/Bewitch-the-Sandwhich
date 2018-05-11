using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeCarrousel : MonoBehaviour
{
	#region DATA
	public static ModeCarrousel menu;
	public const float FadeDuration = 1f;

	public GameObject blocker;

	internal SmartAnimator anim;

	private Dictionary<Graphic, float> graphics;
	#endregion

	#region UTILS
	// Turns it on/off
	public static void Switch (bool state) 
	{
		if (!state) menu.blocker.SetActive (true);
		menu.StartCoroutine (menu.FadeAll (state));
	}

	private IEnumerator FadeAll (bool state) 
	{
		float target = state? 1f : 0f;

		float factor = 0f;
		while (factor <= 1.1f)
		{
			// Compute fade factors
			float powerFactor = Mathf.Pow (factor, state? 4f : 0.1f);
			float value = Mathf.Lerp (1-target, target, powerFactor);

			// Fade graphics
			foreach (var g in graphics)
				g.Key.SetAlpha (g.Value * value);

			yield return null;
			factor += Time.deltaTime / FadeDuration;
		}
		if (state) blocker.SetActive (false);
	}
	#endregion

	#region CALLBACKS
	private void Awake () 
	{
		menu = this;
		anim = new SmartAnimator (GetComponent<Animator> ());

		// Initialize graphics-alpha_values dictionary
		graphics = new Dictionary<Graphic, float> ();
		foreach (var g in GetComponentsInChildren<Graphic> (true))
			graphics.Add (g, g.color.a);
	}
	#endregion
}
