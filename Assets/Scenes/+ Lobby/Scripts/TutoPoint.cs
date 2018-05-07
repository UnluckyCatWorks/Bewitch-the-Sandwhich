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

		GetComponent<Marker> ().On (validColor);
		Tutorial.SetCheckFor (observedCharacter, phase, true);
	}
	private void OnTriggerExit (Collider other) 
	{
		if (other.name != observedCharacter.ToString ()) return;

		GetComponent<Marker> ().Off ();
		Tutorial.SetCheckFor (observedCharacter, phase, false);
	}

	private void Awake () 
	{
		marker = GetComponent<Marker> ();
	}
	#endregion
}
