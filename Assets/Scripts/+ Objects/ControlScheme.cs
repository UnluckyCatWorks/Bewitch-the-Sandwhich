using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ControlScheme : ScriptableObject
{
	#region DATA
	public ControllerType type;

	public string action;
	public string spell;
	public string dash;
	#endregion

	#region ACCESSING
	public string GetButtonName ( string button ) 
	{
		if (type == ControllerType.UNESPECIFIED)
			throw new Exception ("Controller type not setup!");

		string name = string.Empty;
		if (button == "Action") name = action;
		else
		if (button == "Spell") name = spell;
		else
		if (button == "Dash") name = dash;

		if (type == ControllerType.Gamepad_1) name = name.Insert (0, "joystick 1 ");
		if (type == ControllerType.Gamepad_2) name = name.Insert (0, "joystick 2 ");

		return name;
	}

	public string GetAxisName (string input) 
	{
		string prefix = type.ToString ();
		return prefix + "_" + input;
	}
	#endregion
}
