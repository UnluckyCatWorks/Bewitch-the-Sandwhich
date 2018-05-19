using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoPoint : MonoBehaviour
{
	#region DATA
	public Characters observedCharacter;
	public Tutorial.Phases phase;
	internal Marker marker;

	// Some kind of green for when players enter the check-point
	public static Color validColor = new Color32 (009, 242, 195, 255); 
	#endregion

	#region CALLBACKS
	private void OnTriggerEnter (Collider other)
	{
		if (other.name != observedCharacter.ToString ()) return;

		if (Tutorial.SetCheckFor (observedCharacter, phase, true))
			GetComponent<Marker> ().On (validColor);
	}
	private void OnTriggerExit (Collider other) 
	{
		if (other.name != observedCharacter.ToString ()) return;

		if (Tutorial.SetCheckFor (observedCharacter, phase, false))
			GetComponent<Marker> ().Off ();
	}

	private void Awake () 
	{
		marker = GetComponent<Marker> ();
	}
	#endregion
}
