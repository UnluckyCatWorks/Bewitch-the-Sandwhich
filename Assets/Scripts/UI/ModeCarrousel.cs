using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
	public const float selectionMax = (float) Game.Modes.Count;

	internal SmartAnimator anim;

	private bool active;
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

		 // Below lower bound
		if (selected == -1) 
		{
			float lastModeValue = (selectionMax - 1f) / selectionMax;
			anim.SetFloat ("Blend", lastModeValue);
			selected = (int) selectionMax - 2;
		}
		else
		// Above higher bound
		if (selected == selectionMax + 1)
		{
			anim.SetFloat ("Blend", 1f / selectionMax);
			selected = 2;
		}

		// Correct selected mode (for helpers)
		if (selected == 0)					selectedMode = Game.Modes.WizardWeather;
		else if (selected == selectionMax)	selectedMode = Game.Modes.Lobby;
		else								selectedMode = (Game.Modes) selected;

		// Disable slider if selecting tutorial
		if (selectedMode == Game.Modes.Lobby)
			slider.transform.parent.gameObject.SetActive (false);
		else
			slider.transform.parent.gameObject.SetActive (true);
	}

	public void StartGame () 
	{
		Game.mode = selectedMode;
		Game.rounds = roundAmount;

		if (Game.mode == Game.Modes.Lobby)
			Tutorial.manager.StartTutorial ();
		else
			UIMaster.LoadScene (Game.mode);
	}

	public void BackToSelection () 
	{
		GameObject.Find ("Camera_Rig").GetComponent<Animator> ().SetTrigger ("ToCharSelect");
//		StartCoroutine (Extensions.FadeAmbient (0.7f, 2f, 0.8f));

		// Wait until players are reselected
		Lobby.charactersSelected = false;
		StartCoroutine (WaitUntilReselection ());

		// Enable Selectors
		FindObjectsOfType<Selector> ().ToList ().ForEach (s => s.SwitchState (state: true));
		Switch (state: false);
	}

	IEnumerator WaitUntilReselection () 
	{
		yield return new WaitUntil (()=> Lobby.charactersSelected);
		GameObject.Find ("Camera_Rig").GetComponent<Animator> ().SetTrigger ("ToModeSelect");
		Switch (state: true);
	}
	#endregion

	#region UTILS
	public static void Switch (bool state) 
	{
		if (!state)
		{
			menu.blocker.SetActive (true);
			menu.active = false;
		}
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
		if (state)
		{
			blocker.SetActive (false);
			active = true;
		}
	}
	#endregion

	#region CALLBACKS
	private void Update () 
	{
		float iValue = anim.GetFloat ("Blend");
		float tValue = selected / selectionMax;
		// Move animator towards selected value
		anim.SetFloat ("Blend", Mathf.Lerp (iValue, tValue, Time.smoothDeltaTime * Speed));

		// Check if close enough to selected to keep moving
		closeEnough = Mathf.Abs (tValue - iValue) <= 0.05f;

		if (!active) return;

		#region INPUT HANLDING
		float input1 = Player.Get (1).GetAxis ("Horizontal", true);
		float input2 = Player.Get (2).GetAxis ("Horizontal", true);

		if (input1 != 0) MoveMode ((int)input1);
		else
		if (input2 != 0) MoveMode ((int)input2);
		#endregion
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