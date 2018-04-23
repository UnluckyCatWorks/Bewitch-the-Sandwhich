using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Game : MonoBehaviour
{
	#region DATA
	public static Game manager;			/// Self-reference
	public static bool paused;			/// Whether the game paused
	public static Color[] teamColors =	/// The representative color of each player
	{
		new Color32 (000, 000, 000, 000),	/// Nothing, just pure black
		new Color32 (247, 133, 019, 200),	/// Orange (Wall Guy)
		new Color32 (059, 129, 249, 230),	/// Blue (Ice Guy)
		new Color32 (248, 071, 255, 190),	/// Purple? (Both)

		new Color32 (034, 255, 148, 255)	/// Green (when on valid tuto point)
	};
	#endregion

	protected virtual void Start () 
	{
		/// Initialize game 
		manager = this;
		StartCoroutine (Logic ());
	}
	protected abstract IEnumerator Logic ();
}
