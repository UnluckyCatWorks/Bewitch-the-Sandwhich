using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorParam<T> where T : struct
{
	private Animator animator;
	private T cache;

	public AnimatorParam (Animator animator)
	{
		// Check that type is valid
		if (!typeof (T).Equals (typeof (float))
		||  !typeof (T).Equals (typeof (bool))
		||  !typeof (T).Equals (typeof (int)))
			throw new Exception ( "Only valid types for parameters are float, int and bool" );


	}

	public void Set (T value) 
	{
		// :(
	}
}
