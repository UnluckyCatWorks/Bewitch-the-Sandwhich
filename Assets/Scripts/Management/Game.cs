using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
	#region DATA
	public static bool paused;		// Whether the game paused
	public static Game manager;		// Self-reference
	public static int[] scores;		// Score of both players

	public static RectTransform ui; // Canvas (parent of all UI)
	#endregion

	private void Update () 
	{
		OrderMaster.Update ();
	}

	private void Awake () 
	{
		// Initialize game
		OrderMaster.Initialize ();
		DialogMaster.Initialize ();
		ui = GameObject.Find ("UI").transform as RectTransform;
		scores = new int[3];
		manager = this;
	}
}
