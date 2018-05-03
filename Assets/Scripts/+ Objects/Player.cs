using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public class Player 
{
	#region DATA
	public static Player[] all;

	public ControlScheme scheme;
	public PlayerData data;

	private List<string> consumedInputs;
	#endregion

	#region CTOR
	public Player (ControllerType controller) 
	{
		// Initialize player for specific control scheme
		scheme = Resources.LoadAll<ControlScheme> ("Control-Schemes").First (c => (c.type == controller));
		consumedInputs = new List<string> (3);
	}

	[RuntimeInitializeOnLoadMethod
	(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void CreatePlayers () 
	{
		// Control configuration is loaded and then players are created
		MasterControls.LoadControllers ();

		all = new Player[2];
		all[0] = new Player (MasterControls.controllers[0]);
		all[1] = new Player (MasterControls.controllers[1]);
	} 
	#endregion

	#region HELPERS
	public float GetAxis (string axis) 
	{
		axis = scheme.GetAxisName (axis);
		return Input.GetAxis (axis);
	}

	public bool GetButton (string button, bool consume = true) 
	{
		string input = scheme.GetButtonName (button);
		// If not consumed, return input value
		if (!consumedInputs.Contains (button))
		{
			bool pressed = Input.GetKeyDown (input);
			if (pressed && consume) consumedInputs.Add (button);
			return pressed;
		}
		else return false;
	}

	public void ResetInputs () { consumedInputs.Clear (); }
	#endregion
}
