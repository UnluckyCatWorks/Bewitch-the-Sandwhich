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

	private Selector other;
	private Graphic[] icons;
	private new Light light;
	private bool closeEnough;

	private const float Speed = 7f;
	#endregion

	#region UTILS
	private void Move () 
	{
		// Only move if already on target slot
		if (closeEnough)
		{
			// Move selector with input axis
			int delta = Mathf.CeilToInt (Owner.GetAxis ("Horizontal"));
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
	}

	// Turn on / off this 
	public void SwitchState (bool value) 
	{

	}
	#endregion

	#region CALLBACKS
	private void Update () 
	{
		Move ();

		// Move towards selected character
		var tPos = showcase[selected].transform.position;
		transform.position = Vector3.Lerp (transform.position, tPos, Time.deltaTime * Speed);
		closeEnough = Vector3.Distance (tPos, transform.position) <= 0.2f;
	}

	private void Awake () 
	{
		// Get references
		other = FindObjectsOfType<Selector> ().First (s=> s != this);
		light = GetComponentInChildren<Light> ();
		icons = GetComponentsInChildren<Graphic> ();
	}
	#endregion
}
