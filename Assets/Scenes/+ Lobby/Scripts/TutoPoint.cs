using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoPoint : MonoBehaviour
{
	public Characters observedCharacter;
	public TutorialGame.TutorialPhases phase;

	// Some kind of green for when players enter the check-point
	private readonly Color validColor = new Color32 (009, 242, 195, 255);

	private void OnTriggerEnter (Collider other) 
	{
		if (other.name != observedCharacter.ToString ()) return;
		if (!TutorialGame.IsChecking (phase)) return;

		GetComponent<Marker> ().On (validColor);
		TutorialGame.Checks[phase].Set (observedCharacter, true);
	}
	private void OnTriggerExit (Collider other) 
	{
		if (other.name != observedCharacter.ToString ()) return;
		if (!TutorialGame.IsChecking (phase)) return;

		GetComponent<Marker> ().Off ();
		TutorialGame.Checks[phase].Set (observedCharacter, false);
	}
}
