using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WetDeath : Game
{
	#region DATA
	[Header ("Settings")]
	public float startRotation;
	public float rotationIncrement;

	[Header ("References")]
	public Transform isleRotator;

	public static float rotatorSpeed;
	#endregion

	#region CALLBACKS
	protected override IEnumerator Logic () 
	{
		rotatorSpeed = startRotation;

		while (true) 
		{
			// Rotate isle
			isleRotator.Rotate (Vector3.up, rotatorSpeed * Time.deltaTime);

			yield return null;
		}
	}

	public override IEnumerator ResetStage ()
	{
		throw new System.NotImplementedException ();
	} 
	#endregion
}
