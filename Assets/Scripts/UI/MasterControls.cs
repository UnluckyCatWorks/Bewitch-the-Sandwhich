using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterControls : MonoBehaviour
{
	#region DATA
	public static ControllerType[] controllers;
	public static int connectedJoysticks;
	#endregion

	#region HELPERS
	public static void LoadControllers () 
	{
		// Read controllers from player prefs
		#warning CHANGE THIS TO CHANGE DEFAULT CONTROLLERS
		var player1 = (ControllerType) PlayerPrefs.GetInt ("Player_1_Controller", (int) ControllerType.Keyboard_Left);
		var player2 = (ControllerType) PlayerPrefs.GetInt ("Player_1_Controller", (int) ControllerType.Keyboard_Right);

		// Create array
		controllers = new ControllerType[] 
		{
			player1,
			player2
		};
	} 
	#endregion
}
