using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGame : Game 
{
	#region UTILS
	public void Play () 
	{
		enabled = true;
	}

	public void Exit () 
	{
		Application.Quit ();
	}
	#endregion

	protected override IEnumerator Logic () 
	{
		print ("hey");
		yield return null;
	}
}
