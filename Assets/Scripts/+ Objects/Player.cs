using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

[CreateAssetMenu]
public class Player : ScriptableObject 
{
	#region DATA
	public ControllerType controller;
	public PlayerData data;

	private List<string> consumedInputs;
	#endregion

	#region SPECIAL INPUT HELPERS
	public float GetAxis (string axis) 
	{

	}

	public bool GetButtonDown (string button, bool consume = true) 
	{
		var input = GetInputName (button);
		// If not consumed, return input value
		if (!consumedInputs.Contains (input))
		{
			var pressed = Input.GetButtonDown (input);
			if (pressed && consume) consumedInputs.Add (input);
			return pressed;
		}
		else return false;
	}

	// Generates correct name
	private string GetInputName (string input) 
	{
		KeyCode.
		var prefix = owner.controller.ToString ();
		return prefix + "_" + input;
	}

	public void ResetInputs () { consumedInputs.Clear (); }
	#endregion
}
