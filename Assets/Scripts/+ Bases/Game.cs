using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Game : MonoBehaviour
{
	#region DATA
	public static Game manager;			// Self-reference
	public static bool paused;          // Whether the game paused
	#endregion

	#region CALLBACKS
	protected virtual void Awake ()
	{
		// Self reference
		manager = this;
	}

	protected virtual void Start ()
	{
		// Initialize game 
		StartCoroutine (Logic ());
	}

	protected abstract IEnumerator Logic (); 
	#endregion

	public enum Modes 
	{
		UNESPECIFIED, 
		Tutorial, 

		// Real Game Modes
		TohLoQuePuedas,
		AtrapaElCaldero,
		CaenDelSielo,

		// Special
		Count = CaenDelSielo - 2
	}
}
