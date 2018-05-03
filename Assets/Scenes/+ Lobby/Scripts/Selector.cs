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
	public Transform canvas;
	public Marker marker;

	private SmartAnimator anim;
	private Transform cam;
	private List<Graphic> icons;
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
	}

	private IEnumerator Switch (float target) 
	{
		float factor = 0f;
		float duration = 0.5f;
		while (factor < 1.1f) 
		{
			float value = Mathf.Lerp (1-target, target, factor);

			light.intensity = lightIntensity * value;
			icons.ForEach (i => i.SetAlpha (value));

			yield return null;
			factor += Time.deltaTime / duration;
		}
	}

	private void UpdateControllerInfo () 
	{
		for (int i=1; i!=canvas.childCount; i++) 
		{
			bool active = (i == (int)Owner.scheme.type);
			canvas.GetChild (i).gameObject.SetActive (active);
		}
	}
	#endregion

	#region CALLBACKS
	private void Update () 
	{
		// Move with horizontal axis
		if (active && Owner != null)
			Move ();

		// Move towards selected character
		var tPos = showcase[selected].transform.position;
		transform.position = Vector3.Lerp (transform.position, tPos, Time.deltaTime * Speed);
		// Check if close enough to selected to keep moving
		closeEnough = Vector3.Distance (tPos, transform.position) <= 0.4f;

		// Move animator towards selected value
		float iValue = anim.GetFloat ("Blend");
		float tValue = selected / 3f;
		anim.SetFloat ("Blend", Mathf.Lerp (iValue, tValue, Time.deltaTime * Speed));

		// Make info face camera
		canvas.LookAt (cam);
	}

	private void Start () 
	{
		// Get references
		cam = Camera.main.transform;
		other = FindObjectsOfType<Selector> ().First (s=> s != this);
		icons = canvas.GetComponentsInChildren<Graphic> ().ToList ();
		lightIntensity = light.intensity;
		// Initialize animator
		anim = new SmartAnimator ( canvas.GetComponent<Animator> () );

		// Turn off by default
		SwitchState (state: false);

		// Update UI
		UpdateControllerInfo ();
	}
	#endregion
}
