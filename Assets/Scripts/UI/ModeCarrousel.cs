using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModeCarrousel : MonoBehaviour
{
	#region DATA
	public Game.Modes selectedMode;
	public int roundAmount;
	[Range (0f, selectionMax)]
	public int selected;
	public GameObject blocker;

	[Header ("UI")]
	public Text slider;
	public Button sliderLeft;
	public Button sliderRight;

	public static ModeCarrousel menu;
	public const float Speed = 7f;
	public const float FadeDuration = 1f;
	public const int selectionMax = (int) Game.Modes.Count + 1;

	internal SmartAnimator anim;

	private bool closeEnough;
	private Dictionary<Graphic, float> graphics;
	#endregion

	#region UI UTILS
	public void SetRounds (int delta) 
	{
		roundAmount = Mathf.Clamp (roundAmount + delta, 1, 99);
		if (roundAmount == 99) sliderRight.interactable = false;
		else
		if (roundAmount == 01) sliderLeft.interactable = false;
		else
		{
			sliderLeft.interactable = true;
			sliderRight.interactable = true;
		}
		slider.text = roundAmount.ToString ();
	}

	public void MoveMode (int dir) 
	{
		if (!closeEnough) return;
		selected += dir;

		 // Correct out of bounds selection
		if (selected == 0 - 1) 
		{
			float lastModeValue = (float) Game.Modes.Count / selectionMax;
			anim.SetFloat ("Blend", lastModeValue);
			selected = selectionMax - 2;
		}
		else
		if (selected == selectionMax + 1)
		{
			anim.SetFloat ("Blend", (float) 1 / selectionMax);
			selected = 2;
		}

		// Correct selected mode (for helpers)
		if (selected == 0)					selectedMode = Game.Modes.Count;
		else if (selected == selectionMax)	selectedMode = Game.Modes.Tutorial;
		else								selectedMode = (Game.Modes) selected;

		// Disable slider if selecting tutorial
		if (selectedMode == Game.Modes.Tutorial)
			slider.transform.parent.gameObject.SetActive (false);
		else
			slider.transform.parent.gameObject.SetActive (true);
	}

	public void StartGame () 
	{
		Game.mode = selectedMode;
		Game.rounds = roundAmount;

		if (Game.mode == Game.Modes.Tutorial)
			Tutorial.manager.StartTutorial ();
		else
			UIMaster.LoadScene (Game.mode.ToString ());
	}
	#endregion

	#region UTILS
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
	private void Update () 
	{
		float iValue = anim.GetFloat ("Blend");
		float tValue = (float) selected / selectionMax;
		// Move animator towards selected value
		anim.SetFloat ("Blend", Mathf.Lerp (iValue, tValue, Time.smoothDeltaTime * Speed));

		// Check if close enough to selected to keep moving
		closeEnough = Mathf.Abs (tValue - iValue) <= 0.2f;
	}

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
