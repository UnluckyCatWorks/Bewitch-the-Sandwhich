using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
	#region DATA
	public static Game manager;		// Self-reference
	public static bool paused;      // Whether the game paused
	public static RectTransform ui;

	public Color[] teamColors;
	#endregion

	private void Update () 
	{
		OrderMaster.Update ();
	}

	private void Awake () 
	{
		// Initialize game
		Marker.Initialize ();
		OrderMaster.Initialize ();
		DialogMaster.Initialize ();
		manager = this;
	}
}
