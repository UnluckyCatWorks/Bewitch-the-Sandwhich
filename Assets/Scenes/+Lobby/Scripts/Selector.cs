using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Selector : Pawn
{
	#region DATA
	[Header ("Selector settings")]
	public Character[] showcase;
	[Range (0, 3)] public int selected;
	[ColorUsage(true, true, 0, 5, 0, 5)] public Color selectionColor;
	[ColorUsage(true, true, 0, 5, 0, 5)] public Color highlightColor;
	public new Light light;
	public Marker marker;
	public Transform canvas;
	public InputField userName;
	public Transform controllersParent;

	private SmartAnimator anim;
	private Transform cam;
	private Dictionary<Graphic, float> icons;
	private float lightIntensity;
	
	private Selector other;
	private bool closeEnough;
	private bool active = true;

	private const float Speed = 7f;
	#endregion

	#region UTILS
	private void Move () 
	{
		// Cache last selected
		int lastSelected = selected;
		int delta = 0;

		// Only move if already on target slot
		if (closeEnough)
		{
			// Move selector with input axis
			delta = Mathf.CeilToInt (Owner.GetAxis ("Horizontal", raw: true));
			selected += delta;

			// Don't fall into an occupied character slot
			if (other.selected == selected)
			{
				// If selected is on limit, just stop
				if (other.selected == 3 || other.selected == 0)
					selected -= delta;

				// Otherwise, pass over
				else selected += delta;
			}
			// Clamp value
			selected = Mathf.Clamp (selected, 0, 3);
		}

		// If value has changed
		if (lastSelected != selected) 
		{
			// Set as not-ready
			anim.SetBool ("Ready", false);
			// Switch crystal states
			showcase[selected].SwitchCrystal (value: true);
			showcase[lastSelected].SwitchCrystal (value: false);
		}
	}

	public void SwitchState (bool state) 
	{
		active = state;
		showcase[selected].SwitchCrystal (value: state);
		StartCoroutine (Switch (state? 1f : 0f));

		if (state) anim.SetBool ("Ready", false);
	}

	private IEnumerator Switch (float target) 
	{
		float factor = 0f;
		float duration = 0.5f;
		while (factor < 1.1f) 
		{
			float value = Mathf.Lerp (1-target, target, factor);

			light.intensity = lightIntensity * value;
			foreach (var i in icons) i.Key.SetAlpha (i.Value * value);

			yield return null;
			factor += Time.deltaTime / duration;
		}
	}

	private void UpdateControllerInfo () 
	{
		for (int i=1; i!=controllersParent.childCount; i++) 
		{
			bool active = (i == (int) Owner.scheme.type);
			controllersParent.GetChild (i-1).gameObject.SetActive (active);
		}
	}
	#endregion

	#region ANIMATOR UTILS
	private void EndedEdit (string name) 
	{
		// Can only get ready if a name was provided
		bool isEmpty = string.IsNullOrEmpty (name);
		anim.SetBool ("NameSet", !isEmpty);
		if (isEmpty) anim.SetBool ("Ready", false);

		// De-frozen all
		SetAllFrozen (frozen: false);
	}

	public static void SetAllFrozen (bool frozen) 
	{
		FindObjectsOfType<Selector> ().ToList () 
			.ForEach (s =>
			{
				s.active = !frozen;
				s.anim.SetBool ("Mask", frozen);
			});
	} 
	#endregion

	#region CALLBACKS
	private void Update () 
	{
		#region INPUT HANDLING
		if (active)
		{
			// Move with horizontal axis
			if (Owner != null) Move ();

			// Get ready
			if (closeEnough && anim.GetBool ("NameSet")) 
			{
				if (Owner.GetButton ("Dash", consume: false))
				{
					anim.SetBool ("Ready", true);

					// If both are ready
					if (other.anim.GetBool ("Ready"))
					{
						// Save names
						Owner.name = userName.text;
						other.Owner.name = other.userName.text;

						// Save player selection into their owners
						Owner.playingAs = showcase[selected].ID;
						other.Owner.playingAs = other.showcase[other.selected].ID;

						// Notify that players are ready
						Lobby.charactersSelected = true;

						// De-activate both selectors
						SwitchState (state: false);
						other.SwitchState (state: false);
					}
				}
			}
		} 
		#endregion

		#region MOVING TO POSITION
		// Move towards selected character
		var tPos = showcase[selected].transform.position;
		transform.position = Vector3.Lerp (transform.position, tPos, Time.deltaTime * Speed);
		// Check if close enough to selected to keep moving
		closeEnough = Vector3.Distance (tPos, transform.position) <= 0.4f;

		// Move animator towards selected value
		float iValue = anim.GetFloat ("Blend");
		float tValue = selected / 3f;
		anim.SetFloat ("Blend", Mathf.Lerp (iValue, tValue, Time.deltaTime * Speed)); 
		#endregion

		// Make info face camera
		canvas.LookAt (cam);
	}

	private void Start ()  
	{
		// Get references
		cam = Camera.main.transform;
		other = FindObjectsOfType<Selector> ().First (s=> s != this);
		lightIntensity = light.intensity;

		icons = new Dictionary<Graphic, float> ();
		foreach (var g in canvas.GetComponentsInChildren<Graphic> ())
			icons.Add (g, g.color.a);

		// Initialize animator
		anim = new SmartAnimator ( canvas.GetComponent<Animator> () );

		// Turn off by default
		SwitchState (state: false);

		// Update UI
		UpdateControllerInfo ();
		userName.onEndEdit.AddListener (str=> EndedEdit (str));

		// Turn off all crystals
		showcase.ToList ().ForEach (c => c.SwitchCrystal (value: false));
	}
	#endregion
}
