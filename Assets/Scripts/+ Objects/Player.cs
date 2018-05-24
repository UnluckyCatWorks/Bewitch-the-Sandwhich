using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player 
{
	#region DATA
	private static List<Player> all;

	public string name;
	public GameStats ranking;
	public ControlScheme scheme;

	public Character character 
	{
		get { return Character.Get (playingAs); }
	}
	public Characters playingAs;

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

		all = new List<Player> (2)
		{
			new Player (MasterControls.controllers[0]),
			new Player (MasterControls.controllers[1])
		};

		// Default player characters
		all[0].name = "Ciruelo";
		all[0].playingAs = Characters.Milton;
		all[1].name = "Lentejo";
		all[1].playingAs = Characters.Lilith;
	} 
	#endregion

	#region HELPERS
	public static Player Get (int player) 
	{
		int id = player - 1;
		return all[id];
	}
	public static Player GetOther (int player) 
	{
		int id = (player==1? 2:1) - 1;
		return all[id];
	}

	public float GetAxis (string axis, bool raw = false) 
	{
		axis = scheme.GetAxisName (axis);

		if (raw)	return Input.GetAxisRaw (axis);
		else		return Input.GetAxis (axis);
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
